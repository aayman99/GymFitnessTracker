using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Models.DTO
{
    public class WorkoutExerciseDto
    {
        public Guid Id { get; set; }
        public Guid? ExerciseId { get; set; }
        public Guid? CustomExerciseId { get; set; }
        public string ExerciseTitle { get; set; }
        public ExerciseDto Exercise { get; set; }
        public CustomExerciseDto CustomExercise { get; set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
        //public int SetCount { get; set; }
        public ICollection<SetDto> Sets { get; set; }
    }
}
