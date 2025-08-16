using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Repositories
{
    public interface ICustomExerciseRepository
    {
        Task<CustomExercise> CreateCustomExerciseAsync(CustomExercise customExercise);
        Task<List<CustomExercise>> GetAllCustomExerciseByUserIdAsync(Guid userId, string? title = null);
        Task<CustomExercise?> GetCustomExerciseByIdAsync(Guid id);
        Task<CustomExercise?> DeleteCustomExerciseAsync(Guid userId, Guid id);
        Task<CustomExercise?> UpdateCustomExerciseAsync(Guid id, CustomExercise customExercise);

    }
}
