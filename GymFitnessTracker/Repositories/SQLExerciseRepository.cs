using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Repositories
{
    public class SQLExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public SQLExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
