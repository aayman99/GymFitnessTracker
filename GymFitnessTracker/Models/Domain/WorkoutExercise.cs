using System.Text.Json.Serialization;

namespace GymFitnessTracker.Models.Domain
{
    public class WorkoutExercise
    {
        public Guid Id { get; set; }
        public Guid WorkoutId { get; set; }
        [JsonIgnore]
        public Workout Workout { get; set; }
        public Guid? ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
        public Guid? CustomExerciseId { get; set; }
        public CustomExercise? CustomExercise { get; set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
        public int Order { get; set; } = 0;
        public ICollection<Set> Sets { get; set; }
    }
}
