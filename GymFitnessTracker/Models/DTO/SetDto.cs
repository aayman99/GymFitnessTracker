using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Models.DTO
{
    public class SetDto
    {
        public Guid Id { get; set; }
        public string? Note { get; set; }
        public float? Weight { get; set; }
        public int? Repetitions { get; set; }
        public float? Duration { get; set; }
        public float? RestTime { get; set; }
        public Guid? RestTimeUnitId { get; set; }
        public Guid? DurationTimeUnitId { get; set; }
        public Guid? WeightUnitId { get; set; }
        public Guid? UserId { get; set; }
        public string? RestTimeUnit { get; set; }
        public string? DurationTimeUnit { get; set; }
        public string? WeightUnit { get; set; }

    }
}
