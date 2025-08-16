namespace GymFitnessTracker.Models.DTO
{
    public class UpdateCustomExerciseRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? VideoUrl { get; set; }
        public string? PrimaryMuscle { get; set; }
    }
}
