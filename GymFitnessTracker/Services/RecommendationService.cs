using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.Recommendation;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _dbContext;

        public RecommendationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PlanTemplate> GetAllTemplates() => PlanTemplateStore.Templates;

        public PlanTemplate? GetTemplateById(string templateId) => PlanTemplateStore.GetById(templateId);

        public async Task<Plan?> GeneratePlanAsync(Guid userId, string templateId)
        {
            var template = PlanTemplateStore.GetById(templateId);
            if (template == null)
                return null;

            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = template.Name,
                Workouts = new List<Workout>()
            };

            var allMuscleNames = template.Workouts
                .SelectMany(w => w.MuscleTargets)
                .Select(mt => mt.MuscleName.ToLower())
                .Distinct()
                .ToList();

            var exercisesByMuscle = await _dbContext.Exercises
                .Include(e => e.PrimaryMuscle)
                .Include(e => e.Category)
                .Where(e => allMuscleNames.Contains(e.PrimaryMuscle.Title.ToLower()))
                .ToListAsync();

            var muscleExerciseMap = exercisesByMuscle
                .GroupBy(e => e.PrimaryMuscle.Title.ToLower())
                .ToDictionary(g => g.Key, g => g.ToList());

            var random = new Random();

            foreach (var workoutTemplate in template.Workouts)
            {
                var workout = new Workout
                {
                    Id = Guid.NewGuid(),
                    Title = workoutTemplate.Title,
                    Date = DateTime.UtcNow,
                    PlanId = plan.Id,
                    UserId = userId,
                    WorkoutExercises = new List<WorkoutExercise>()
                };

                int order = 0;

                foreach (var muscleTarget in workoutTemplate.MuscleTargets)
                {
                    var key = muscleTarget.MuscleName.ToLower();
                    if (!muscleExerciseMap.TryGetValue(key, out var available))
                        continue;

                    var selected = available
                        .OrderBy(_ => random.Next())
                        .Take(muscleTarget.ExerciseCount)
                        .ToList();

                    foreach (var exercise in selected)
                    {
                        workout.WorkoutExercises.Add(new WorkoutExercise
                        {
                            Id = Guid.NewGuid(),
                            WorkoutId = workout.Id,
                            ExerciseId = exercise.Id,
                            Order = order++,
                            TimeCreated = DateTime.UtcNow,
                            Sets = new List<Set>()
                        });
                    }
                }

                plan.Workouts.Add(workout);
            }

            _dbContext.Plans.Add(plan);
            await _dbContext.SaveChangesAsync();

            var savedPlan = await _dbContext.Plans
                .Include(p => p.Workouts)
                    .ThenInclude(w => w.WorkoutExercises)
                        .ThenInclude(we => we.Exercise)
                            .ThenInclude(e => e.PrimaryMuscle)
                .Include(p => p.Workouts)
                    .ThenInclude(w => w.WorkoutExercises)
                        .ThenInclude(we => we.Exercise)
                            .ThenInclude(e => e.Category)
                .Include(p => p.Workouts)
                    .ThenInclude(w => w.WorkoutExercises)
                        .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(p => p.Id == plan.Id);

            return savedPlan;
        }
    }
}
