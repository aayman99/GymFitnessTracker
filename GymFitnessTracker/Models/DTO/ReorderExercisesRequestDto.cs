namespace GymFitnessTracker.Models.DTO
{
    public class ReorderExercisesRequestDto
    {
        public Guid WorkoutId { get; set; }
        public List<ExerciseOrderDto> ExerciseOrders { get; set; }
    }
}


