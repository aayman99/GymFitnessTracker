using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;

namespace GymFitnessTracker.Repositories
{
    public interface IWorkoutRepository
    {
        Task<Workout> CreateWorkoutAsync(Workout workout);
        Task<Workout?> UpdateWorkoutTitleAsync(Guid userId, Guid workoutId, string? newTitle, string? newNote);
        Task<Workout?> GetWorkoutById(Guid id);
        Task<List<Workout>> GetAllWorkoutsAsync(Guid userId, Guid planId);
        Task<WorkoutExercise?> GetWorkoutExerciseByIdAsync(Guid workoutExerciseId);
        Task<WorkoutExercise?> AddGeneralExerciseToWorkoutAsync(Guid workoutId, Guid exerciseId);
        Task<WorkoutExercise?> AddCustomExerciseToWorkoutAsync(Guid workoutId, Guid customExerciseId);
        Task<Set?> AddSetToWorkoutExerciseAsync(
            Guid workoutExerciseId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? restTimeUnitId, Guid? durationTimeUnitId, Guid? weightUnitId, Guid? userId = null);
        //Task<Set?> AddSetToWorkoutExerciseAsync(Guid workoutExerciseId, Set set);
        //Task<Set?> UpdateSetAsync(Guid setId, Set oldSet);
        Task<Set?> UpdateSetAsync(
            Guid setId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? restTimeUnitId, Guid? durationTimeUnitId, Guid? weightUnitId);
        Task<Set?> GetSetByIdAsync(Guid setId);
        Task <bool> DeleteSetAsync(Guid setId);
        Task<bool> DeleteWorkoutExerciseAsync(Guid workoutExerciseId);
        Task<bool> DeleteWorkoutAsync(Guid workoutId);
        Task<List<TimeUnit>> GetTimeUnit();
        Task<List<WeightUnit>> GetWeightUnit();
        Task<(bool Success, string ErrorMessage)> ReorderExercisesAsync(Guid userId, Guid workoutId, List<ExerciseOrderDto> exerciseOrders);
        Task<(bool Success, string ErrorMessage)> ReorderWorkoutsAsync(Guid userId, Guid planId, List<WorkoutOrderDto> workoutOrders, bool isAdmin);

    }
}
