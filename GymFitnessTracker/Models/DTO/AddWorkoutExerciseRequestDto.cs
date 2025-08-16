namespace GymFitnessTracker.Models.DTO
{
    public class AddWorkoutExerciseRequestDto
    {
        //public Guid ExerciseId { get; set; }
        public List<Guid> ExerciseId { get; set; }
        public List<Guid> CustomExerciseId { get; set; }
    }
}
