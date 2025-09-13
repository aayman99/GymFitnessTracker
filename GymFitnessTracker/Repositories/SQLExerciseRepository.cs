using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace GymFitnessTracker.Repositories
{
    public class SQLExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClient;


        

        public SQLExerciseRepository(ApplicationDbContext context, IHttpClientFactory httpClient)
        {
            _context = context;
            _httpClient = httpClient;

        }

        public async Task<Exercise?> CreateExerciseAsync(Exercise exercise)
        {
            // check if the name of exercise is duplicate or not
            var exerciseExist = await _context.Exercises.FirstOrDefaultAsync(x => x.Title == exercise.Title);
            if(exerciseExist != null)
            {
                return null;
            }
            await _context.Exercises.AddAsync(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise?> DeleteExerciseAsync(Guid id)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);
            if (exercise == null)
            {
                return null;
            }
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }

        public async Task<List<Exercise>> GetAllExercisesAsync(string? filterOn = null, string? filterQuery = null)
        {
            var exercises = _context.Exercises.Include(x=>x.PrimaryMuscle)
                                              .Include(x=>x.Category)
                                              .AsQueryable()
                                              .OrderBy(x => x.Title);
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    exercises = exercises.Where(x => x.Title.Contains(filterQuery)).OrderBy(x => x.Title);
                }
                else if(filterOn.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    exercises = exercises.Where(x => x.Category.Title.Contains(filterQuery)).OrderBy(x => x.Title);
                    //exercises = exercises.Where(x => x.Category.Contains(filterQuery));
                }
                else if (filterOn.Equals("Muscle", StringComparison.OrdinalIgnoreCase))
                {
                    exercises = exercises.Where(x => x.PrimaryMuscle.Title.Contains(filterQuery)).OrderBy(x => x.Title);
                    //exercises = exercises.Where(x => x.PrimaryMuscle.Contains(filterQuery));
                }
            }
            return await exercises.ToListAsync();
            //return await _context.Exercises.ToListAsync();
        }

        public async Task<Exercise?> GetExerciseAsync(string title)
        {
            return await _context.Exercises
                                 .Include(x=>x.PrimaryMuscle)
                                 .Include(x=>x.Category)
                                 .FirstOrDefaultAsync(x => x.Title.Contains(title));
        }

        public async Task<Exercise?> UpdateExerciseAsync(Guid id, Exercise exercise)
        {
            var existingExercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);

            if (existingExercise == null)
            {
                return null;
            }
            existingExercise.Title = exercise.Title;
            existingExercise.PrimaryMuscleId = exercise.PrimaryMuscleId;
            existingExercise.VideoUrl = exercise.VideoUrl;
            existingExercise.FemaleVideoUrl = exercise.FemaleVideoUrl;
            existingExercise.CategoryId = exercise.CategoryId;
            existingExercise.Description = exercise.Description;
            existingExercise.PicturePath = exercise.PicturePath;

            await _context.SaveChangesAsync();

            var updatedExercise = await _context.Exercises
                    .Include(e => e.PrimaryMuscle)
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(e => e.Id == existingExercise.Id);

            return updatedExercise;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories;
            /*var categories = await _context.Exercises.Select(x => x.Category).Distinct().ToListAsync();
            return categories;*/
        }
        public async Task<List<PrimaryMuscle>> GetAllMuscles()
        {
            var muscles = await _context.PrimaryMuscles.ToListAsync();
            return muscles;

            /*var muscles = await _context.Exercises.Select(x => x.PrimaryMuscle).Distinct().ToListAsync();
            var filteredMuscles = new List<string>();
            foreach (var muscle in muscles)
            {
                if(!muscle.Contains("/"))
                    filteredMuscles.Add(muscle);
            }
            return filteredMuscles;*/
        }

        public async Task<List<string>> GetExerciseTitles(List<Guid> ids)
        {
            return await _context.Exercises
                        .Where(e => ids.Contains(e.Id))
                        .Select(e => e.Title)
                        .ToListAsync();
        }
        public async Task<List<(Guid Id, string Title)>> GetExercisesWithBrokenYoutubeAsync(
        int maxConcurrency = 10,
        CancellationToken ct = default)
        {
            // pull minimal columns
            var exercises = await _context.Exercises
                .AsNoTracking()
                .Select(e => new { e.Id, e.Title, e.VideoUrl, e.FemaleVideoUrl })
                .ToListAsync(ct);

            var http = _httpClient.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(8);

            // keep concurrency but no concurrent collections
            var sem = new SemaphoreSlim(maxConcurrency);
            var tasks = new List<Task>();

            // use a normal set+lock to collect unique titles
            //var brokenTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var brokenTitles = new Dictionary<Guid, string>();
            var gate = new object();

            foreach (var ex in exercises)
            {
                tasks.Add(CheckOne(ex.Id, ex.Title, ex.VideoUrl));
                tasks.Add(CheckOne(ex.Id, ex.Title, ex.FemaleVideoUrl));
            }

            await Task.WhenAll(tasks);

            // return as ordered list
            //return brokenTitles.OrderBy(t => t).ToList();
            return brokenTitles.Select(x => (x.Key, x.Value))
                               .OrderBy(x => x.Key)
                               .ToList();

            async Task CheckOne(Guid id, string title, string? url)
            {
                // treat null/empty as broken
                
                if (string.IsNullOrWhiteSpace(url))
                {
                    lock (gate) brokenTitles[id] = title;
                    return;
                }

                var vid = TryGetVideoId(url);
                if (vid is null)
                {
                    lock (gate) brokenTitles[id] = title;
                    return;
                }

                await sem.WaitAsync(ct);
                try
                {
                    var (ok, _, _) = await IsAvailableViaOEmbedAsync(http, vid, ct);
                    if (!ok)
                    {
                        lock (gate) brokenTitles[id] = title;
                    }
                }
                finally
                {
                    sem.Release();
                }
            }
        }
        public static string? TryGetVideoId(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var patterns = new[]
            {
                @"(?:v=)([A-Za-z0-9_\-]{6,})",
                @"youtu\.be/([A-Za-z0-9_\-]{6,})",
                @"youtube\.com/embed/([A-Za-z0-9_\-]{6,})",
                @"youtube\.com/shorts/([A-Za-z0-9_\-]{6,})"
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(url, p, RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups[1].Value;
            }
            return null;
        }
        public static async Task<(bool ok, HttpStatusCode? status, string? reason)> IsAvailableViaOEmbedAsync(
            HttpClient http, string videoId, CancellationToken ct = default)
        {
            var watchUrl = $"https://www.youtube.com/watch?v={videoId}";
            var requestUrl = $"https://www.youtube.com/oembed?url={Uri.EscapeDataString(watchUrl)}&format=json";

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                using var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

                if (resp.IsSuccessStatusCode)
                    return (true, resp.StatusCode, null);

                return (false, resp.StatusCode, resp.ReasonPhrase);
            }
            catch (TaskCanceledException)
            {
                return (false, null, "timeout");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
        
    }
}
