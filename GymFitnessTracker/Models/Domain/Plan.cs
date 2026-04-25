namespace GymFitnessTracker.Models.Domain
{
    public class Plan
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string? Notes { get; set; }
        public bool IsStatic { get; set; } = false;
        public int Order { get; set; } = 0;
        public ICollection<Workout> Workouts { get; set; }
    }
}
