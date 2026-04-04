namespace GymFitnessTracker.Models.DTO
{
    public class PlanTemplateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int DaysPerWeek { get; set; }
        public List<WorkoutTemplateDto> Workouts { get; set; }
    }

    public class WorkoutTemplateDto
    {
        public string Title { get; set; }
        public int DayNumber { get; set; }
        public List<MuscleTargetDto> MuscleTargets { get; set; }
    }

    public class MuscleTargetDto
    {
        public string MuscleName { get; set; }
        public int ExerciseCount { get; set; }
    }
}
