using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Repositories
{
    public interface IExerciseRepository
    {
        Task<List<Exercise>> GetAllExercisesAsync(string? filterOn = null,string? filterQuery = null);
        Task<Exercise?> GetExerciseAsync(string title);
        Task<Exercise?> CreateExerciseAsync(Exercise exercise);
        Task<Exercise?> DeleteExerciseAsync(Guid id);
        Task<Exercise?> UpdateExerciseAsync(Guid id, Exercise exercise);
        Task<List<Category>> GetAllCategories();
        Task<List<PrimaryMuscle>> GetAllMuscles();

    }
}
