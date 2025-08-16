using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Repositories
{
    public class SQLCustomExerciseRepository : ICustomExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public SQLCustomExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CustomExercise> CreateCustomExerciseAsync(CustomExercise customExercise)
        {
            await _context.CustomExercises.AddAsync(customExercise);
            await _context.SaveChangesAsync();
            return customExercise;
        }

        public async Task<List<CustomExercise>> GetAllCustomExerciseByUserIdAsync(Guid userId, string? title = null)
        {
            /*return await _context.CustomExercises
                        .Where(c => c.UserId == userId)
                        .Include(c => c.PrimaryMuscle)
                        .Include(c => c.Category)
                        .ToListAsync();*/
            var customExercises = _context.CustomExercises
                                          .Where(ce => ce.UserId == userId)
                                          .AsQueryable();
            if (string.IsNullOrWhiteSpace(title) == false)
            {
                customExercises = customExercises.Where(ce => ce.Title.Contains(title));
            }
            return await customExercises.ToListAsync();


        }

        public async Task<CustomExercise?> GetCustomExerciseByIdAsync(Guid id)
        {
            return await _context.CustomExercises
                        /*.Include(c => c.PrimaryMuscle)
                        .Include (c => c.Category)*/
                        .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CustomExercise?> DeleteCustomExerciseAsync(Guid userId, Guid id)
        {
            var customExercise = await _context.CustomExercises
                                               .Include(ce => ce.WorkoutExercises)
                                               .FirstOrDefaultAsync(ce => ce.UserId == userId && ce.Id == id);
            if (customExercise == null)
            {
                return null;
            }

            // remove related workout exercises
            if(customExercise.WorkoutExercises != null && customExercise.WorkoutExercises.Any())
            {
                _context.WorkoutExercises.RemoveRange(customExercise.WorkoutExercises);
            }

            _context.CustomExercises.Remove(customExercise);
            await _context.SaveChangesAsync();
            return customExercise;

        }

        public async Task<CustomExercise?> UpdateCustomExerciseAsync(Guid id, CustomExercise customExercise)
        {
            var existingCustomExercise = await _context.CustomExercises.FirstOrDefaultAsync(c => c.Id == id);
            if (existingCustomExercise == null)
            {
                return null;
            }

            existingCustomExercise.Title = customExercise.Title;
            existingCustomExercise.Description = customExercise.Description;
            existingCustomExercise.VideoUrl = customExercise.VideoUrl;
            existingCustomExercise.PrimaryMuscle = customExercise.PrimaryMuscle;

            await _context.SaveChangesAsync();
            return customExercise;
        }
    }
}
