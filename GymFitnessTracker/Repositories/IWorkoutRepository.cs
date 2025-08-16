using GymFitnessTracker.Models.Domain;

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
            Guid workoutExerciseId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? timeUnitId, Guid? weightUnitId);
        //Task<Set?> AddSetToWorkoutExerciseAsync(Guid workoutExerciseId, Set set);
        //Task<Set?> UpdateSetAsync(Guid setId, Set oldSet);
        Task<Set?> UpdateSetAsync(
            Guid setId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? timeUnitId, Guid? weightUnitId);
        Task<Set?> GetSetByIdAsync(Guid setId);
        Task <bool> DeleteSetAsync(Guid setId);
        Task<bool> DeleteWorkoutExerciseAsync(Guid workoutExerciseId);
        Task<bool> DeleteWorkoutAsync(Guid workoutId);
        Task<List<TimeUnit>> GetTimeUnit();
        Task<List<WeightUnit>> GetWeightUnit();

    }
}
