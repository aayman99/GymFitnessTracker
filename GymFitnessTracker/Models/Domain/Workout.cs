namespace GymFitnessTracker.Models.Domain
{
    public class Workout
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid PlanId { get; set; }
        public Plan Plan { get; set; }
        public Guid UserId { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}
