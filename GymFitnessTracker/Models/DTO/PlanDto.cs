using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Models.DTO
{
    public class PlanDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string? Notes { get; set; }
        public bool IsStatic { get; set; }
        public ICollection<WorkoutDto> Workouts { get; set; }
    }
}
