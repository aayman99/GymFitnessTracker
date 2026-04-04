namespace GymFitnessTracker.Models.Recommendation
{
    public static class PlanTemplateStore
    {
        private static readonly List<MuscleTarget> PushMuscles = new()
        {
            new() { MuscleName = "Chest", ExerciseCount = 3 },
            new() { MuscleName = "Shoulders", ExerciseCount = 2 },
            new() { MuscleName = "Triceps", ExerciseCount = 2 }
        };

        private static readonly List<MuscleTarget> PullMuscles = new()
        {
            new() { MuscleName = "Back", ExerciseCount = 3 },
            new() { MuscleName = "Biceps", ExerciseCount = 2 },
            new() { MuscleName = "Traps", ExerciseCount = 2 }
        };

        private static readonly List<MuscleTarget> LegMuscles = new()
        {
            new() { MuscleName = "Legs", ExerciseCount = 2 },
            new() { MuscleName = "Quads", ExerciseCount = 2 },
            new() { MuscleName = "Hamstrings", ExerciseCount = 2 },
            new() { MuscleName = "Calves", ExerciseCount = 1 }
        };

        private static readonly List<MuscleTarget> UpperMuscles = new()
        {
            new() { MuscleName = "Chest", ExerciseCount = 2 },
            new() { MuscleName = "Back", ExerciseCount = 2 },
            new() { MuscleName = "Shoulders", ExerciseCount = 2 },
            new() { MuscleName = "Biceps", ExerciseCount = 1 },
            new() { MuscleName = "Triceps", ExerciseCount = 1 }
        };

        private static readonly List<MuscleTarget> LowerMuscles = new()
        {
            new() { MuscleName = "Legs", ExerciseCount = 2 },
            new() { MuscleName = "Quads", ExerciseCount = 2 },
            new() { MuscleName = "Hamstrings", ExerciseCount = 1 },
            new() { MuscleName = "Glutes", ExerciseCount = 1 },
            new() { MuscleName = "Calves", ExerciseCount = 1 }
        };

        private static readonly List<MuscleTarget> FullBodyMuscles = new()
        {
            new() { MuscleName = "Chest", ExerciseCount = 1 },
            new() { MuscleName = "Back", ExerciseCount = 1 },
            new() { MuscleName = "Shoulders", ExerciseCount = 1 },
            new() { MuscleName = "Legs", ExerciseCount = 1 },
            new() { MuscleName = "Hamstrings", ExerciseCount = 1 },
            new() { MuscleName = "Biceps", ExerciseCount = 1 },
            new() { MuscleName = "Triceps", ExerciseCount = 1 }
        };

        public static List<PlanTemplate> Templates { get; } = new()
        {
            new PlanTemplate
            {
                Id = "ppl-3day",
                Name = "Push Pull Legs",
                DaysPerWeek = 3,
                Workouts = new()
                {
                    new() { Title = "Push Day", DayNumber = 1, MuscleTargets = PushMuscles },
                    new() { Title = "Pull Day", DayNumber = 2, MuscleTargets = PullMuscles },
                    new() { Title = "Legs Day", DayNumber = 3, MuscleTargets = LegMuscles }
                }
            },
            new PlanTemplate
            {
                Id = "ppl-6day",
                Name = "Push Pull Legs (6 Days)",
                DaysPerWeek = 6,
                Workouts = new()
                {
                    new() { Title = "Push Day", DayNumber = 1, MuscleTargets = PushMuscles },
                    new() { Title = "Pull Day", DayNumber = 2, MuscleTargets = PullMuscles },
                    new() { Title = "Legs Day", DayNumber = 3, MuscleTargets = LegMuscles },
                    new() { Title = "Push Day", DayNumber = 4, MuscleTargets = PushMuscles },
                    new() { Title = "Pull Day", DayNumber = 5, MuscleTargets = PullMuscles },
                    new() { Title = "Legs Day", DayNumber = 6, MuscleTargets = LegMuscles }
                }
            },
            new PlanTemplate
            {
                Id = "upper-lower-2day",
                Name = "Upper Lower",
                DaysPerWeek = 2,
                Workouts = new()
                {
                    new() { Title = "Upper Day", DayNumber = 1, MuscleTargets = UpperMuscles },
                    new() { Title = "Lower Day", DayNumber = 2, MuscleTargets = LowerMuscles }
                }
            },
            new PlanTemplate
            {
                Id = "upper-lower-4day",
                Name = "Upper Lower (4 Days)",
                DaysPerWeek = 4,
                Workouts = new()
                {
                    new() { Title = "Upper Day", DayNumber = 1, MuscleTargets = UpperMuscles },
                    new() { Title = "Lower Day", DayNumber = 2, MuscleTargets = LowerMuscles },
                    new() { Title = "Upper Day", DayNumber = 3, MuscleTargets = UpperMuscles },
                    new() { Title = "Lower Day", DayNumber = 4, MuscleTargets = LowerMuscles }
                }
            },
            new PlanTemplate
            {
                Id = "fullbody-3day",
                Name = "Full Body",
                DaysPerWeek = 3,
                Workouts = new()
                {
                    new() { Title = "Full Body Day", DayNumber = 1, MuscleTargets = FullBodyMuscles },
                    new() { Title = "Full Body Day", DayNumber = 2, MuscleTargets = FullBodyMuscles },
                    new() { Title = "Full Body Day", DayNumber = 3, MuscleTargets = FullBodyMuscles }
                }
            },
            new PlanTemplate
            {
                Id = "bro-split-5day",
                Name = "Bro Split",
                DaysPerWeek = 5,
                Workouts = new()
                {
                    new() { Title = "Chest Day", DayNumber = 1, MuscleTargets = new()
                    {
                        new() { MuscleName = "Chest", ExerciseCount = 5 }
                    }},
                    new() { Title = "Back Day", DayNumber = 2, MuscleTargets = new()
                    {
                        new() { MuscleName = "Back", ExerciseCount = 4 },
                        new() { MuscleName = "Traps", ExerciseCount = 1 }
                    }},
                    new() { Title = "Shoulders Day", DayNumber = 3, MuscleTargets = new()
                    {
                        new() { MuscleName = "Shoulders", ExerciseCount = 4 },
                        new() { MuscleName = "Traps", ExerciseCount = 1 }
                    }},
                    new() { Title = "Arms Day", DayNumber = 4, MuscleTargets = new()
                    {
                        new() { MuscleName = "Biceps", ExerciseCount = 3 },
                        new() { MuscleName = "Triceps", ExerciseCount = 3 }
                    }},
                    new() { Title = "Legs Day", DayNumber = 5, MuscleTargets = new()
                    {
                        new() { MuscleName = "Legs", ExerciseCount = 2 },
                        new() { MuscleName = "Quads", ExerciseCount = 2 },
                        new() { MuscleName = "Hamstrings", ExerciseCount = 1 },
                        new() { MuscleName = "Glutes", ExerciseCount = 1 },
                        new() { MuscleName = "Calves", ExerciseCount = 1 }
                    }}
                }
            }
        };

        public static PlanTemplate? GetById(string id) =>
            Templates.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }
}
