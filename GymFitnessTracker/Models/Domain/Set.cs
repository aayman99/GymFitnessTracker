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
        public Guid? TimeUnitId { get; set; }
        public Guid? WeightUnitId { get; set; }
        public TimeUnit? TimeUnit { get; set; }
        public WeightUnit? WeightUnit { get; set; }
        [JsonIgnore]
        public WorkoutExercise WorkoutExercise { get; set; }
    }
}
