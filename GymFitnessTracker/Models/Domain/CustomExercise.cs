namespace GymFitnessTracker.Models.Domain
{
    public class CustomExercise
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? VideoUrl { get; set; }
        public string? PrimaryMuscle { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
        /*public Guid PrimaryMuscleId { get; set; }
        public Guid CategoryId { get; set; }
        public PrimaryMuscle PrimaryMuscle { get; set; }
        public Category Category { get; set; }*/


    }
}
