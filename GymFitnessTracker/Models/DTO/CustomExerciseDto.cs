namespace GymFitnessTracker.Models.DTO
{
    public class CustomExerciseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? VideoUrl { get; set; }
        public string? PrimaryMuscle { get; set; }

        /*public Guid PrimaryMuscleId { get; set; }
        public Guid CategoryId { get; set; }
        public string PrimaryMuscle { get; set; }
        public string Category { get; set; }*/
    }
}
