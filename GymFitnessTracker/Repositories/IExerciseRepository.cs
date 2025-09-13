using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;

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
        Task<List<string>> GetExerciseTitles(List<Guid> id);
        //Task<List<string>> CheckBrokenVideoAsync();
        //Task<List<string>> CheckBrokenVideosAsync(CancellationToken ct = default);
        Task<List<(Guid Id, string Title)>> GetExercisesWithBrokenYoutubeAsync(
       int maxConcurrency = 10,
       CancellationToken ct = default);

    }
}
