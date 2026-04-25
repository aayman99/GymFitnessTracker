namespace GymFitnessTracker.Models.DTO
{
    public class ReorderWorkoutsRequestDto
    {
        public Guid PlanId { get; set; }
        public List<WorkoutOrderDto> WorkoutOrders { get; set; }
    }
}
