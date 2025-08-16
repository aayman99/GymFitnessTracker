namespace GymFitnessTracker.Models.DTO
{
    public class AddWorkoutRequestDto
    {
        public string Title { get; set; }
        public string? Note { get; set; }
        public Guid PlanId { get; set; }
    }
}
