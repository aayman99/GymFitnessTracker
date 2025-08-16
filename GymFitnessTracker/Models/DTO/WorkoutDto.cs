using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Models.DTO
{
    public class WorkoutDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid PlanId { get; set; }
        public Guid UserId { get; set; }
        public ICollection<WorkoutExerciseDto> WorkoutExercises { get; set; }
    }
}
