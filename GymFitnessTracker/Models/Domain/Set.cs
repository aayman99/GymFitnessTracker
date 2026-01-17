using System.Text.Json.Serialization;

namespace GymFitnessTracker.Models.Domain
{
    public class Set
    {
        public Guid Id { get; set; }
        public string? Note { get; set; }
        public float? Weight { get; set; }
        public int? Repetitions { get; set; }
        public float? Duration { get; set; }
        public float? RestTime { get; set; }
        public Guid WorkoutExerciseId { get; set; }
        public Guid? RestTimeUnitId { get; set; }
        public Guid? DurationTimeUnitId { get; set; }
        public Guid? WeightUnitId { get; set; }
        public Guid? UserId { get; set; } // For tracking user ownership of sets in static plans
        public TimeUnit? RestTimeUnit { get; set; }
        public TimeUnit? DurationTimeUnit { get; set; }
        public WeightUnit? WeightUnit { get; set; }
        [JsonIgnore]
        public WorkoutExercise WorkoutExercise { get; set; }
    }
}
