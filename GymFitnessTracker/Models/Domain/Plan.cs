namespace GymFitnessTracker.Models.Domain
{
    public class Plan
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string? Notes { get; set; }
        public ICollection<Workout> Workouts { get; set; }
    }
}
