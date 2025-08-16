using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Models.DTO
{
    public class AddSetRequestDto
    {
        public float? Weight { get; set; }
        public string? Note { get; set; }
        public int? Repetitions { get; set; }
        public float? Duration { get; set; }
        public float? RestTime { get; set; }
        public Guid? TimeUnitId { get; set; }
        public Guid? WeightUnitId { get; set; }

    }
}
