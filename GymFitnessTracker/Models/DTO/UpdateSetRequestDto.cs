using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Models.DTO
{
    public class UpdateSetRequestDto
    {
        public float? Weight { get; set; }
        public string? Note { get; set; }
        public int? Repetitions { get; set; }
        public float? Duration { get; set; }
        public float? RestTime { get; set; }
        public Guid? RestTimeUnitId { get; set; }
        public Guid? DurationTimeUnitId { get; set; }
        public Guid? WeightUnitId { get; set; }
    }
}
