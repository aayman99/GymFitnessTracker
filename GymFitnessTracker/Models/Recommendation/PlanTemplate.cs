namespace GymFitnessTracker.Models.Recommendation
{
    public class PlanTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int DaysPerWeek { get; set; }
        public List<WorkoutTemplate> Workouts { get; set; }
    }

    public class WorkoutTemplate
    {
        public string Title { get; set; }
        public int DayNumber { get; set; }
        public List<MuscleTarget> MuscleTargets { get; set; }
    }

    public class MuscleTarget
    {
        public string MuscleName { get; set; }
        public int ExerciseCount { get; set; }
    }
}
