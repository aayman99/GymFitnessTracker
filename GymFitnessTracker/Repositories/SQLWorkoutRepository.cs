using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Repositories
{
    public class SQLWorkoutRepository : IWorkoutRepository
    {
        private readonly ApplicationDbContext _context;

        public SQLWorkoutRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Workout> CreateWorkoutAsync(Workout workout)
        {
            var existingWorkouts = await _context.Workouts
                .Where(w => w.PlanId == workout.PlanId)
                .ToListAsync();

            workout.Order = existingWorkouts.Any()
                ? existingWorkouts.Max(w => w.Order) + 1
                : 0;

            await _context.AddAsync(workout);
            await _context.SaveChangesAsync();
            return workout;
        }
        public async Task<Workout?> UpdateWorkoutTitleAsync(Guid userId, Guid workoutId, string? newTitle, string? newNote)
        {
            var workout = await _context.Workouts
                                        .FirstOrDefaultAsync
                                        (w => w.UserId == userId && w.Id == workoutId);

            if(workout == null)
            {
                return null;
            }

            workout.Title = newTitle ?? workout.Title ;
            workout.Note = newNote ?? workout.Note ;
            await _context.SaveChangesAsync();
            return workout;

        }
        public async Task<List<Workout>> GetAllWorkoutsAsync(Guid userId, Guid planId)
        {
            // First check if the plan is static
            var plan = await _context.Plans.FindAsync(planId);
            if (plan != null && plan.IsStatic)
            {
                // For static plans, return all workouts in the plan (created by admin)
                // But only include sets that belong to the current user
                var workouts = await _context.Workouts
                    .Where(u => u.PlanId == planId)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.Exercise)
                            .ThenInclude(p => p.PrimaryMuscle)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.Exercise)
                            .ThenInclude(c => c.Category)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.CustomExercise)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets.Where(set => set.UserId == userId))
                            .ThenInclude(s => s.RestTimeUnit)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets.Where(set => set.UserId == userId))
                            .ThenInclude(s => s.DurationTimeUnit)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets.Where(set => set.UserId == userId))
                            .ThenInclude(wt => wt.WeightUnit)
                    .ToListAsync();

            foreach(var workout in workouts)
            {
                workout.WorkoutExercises = workout.WorkoutExercises.OrderBy(we => we.Order).ThenBy(we => we.TimeCreated).ToList();
            }
            return workouts.OrderBy(w => w.Order).ThenBy(w => w.Date).ToList();
            }
            else
            {
                // For non-static plans, return workouts that belong to the user
                var workouts = await _context.Workouts
                    .Where(u => u.UserId == userId && u.PlanId == planId)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.Exercise)
                            .ThenInclude(p => p.PrimaryMuscle)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.Exercise)
                            .ThenInclude(c => c.Category)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.CustomExercise)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets)
                            .ThenInclude(s => s.RestTimeUnit)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets)
                            .ThenInclude(s => s.DurationTimeUnit)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets)
                            .ThenInclude(wt => wt.WeightUnit)
                    .ToListAsync();

                foreach(var workout in workouts)
                {
                    workout.WorkoutExercises = workout.WorkoutExercises.OrderBy(we => we.Order).ThenBy(we => we.TimeCreated).ToList();
                }
                return workouts.OrderBy(w => w.Order).ThenBy(w => w.Date).ToList();
            }
        }
        public async Task<Workout?> GetWorkoutById(Guid id)
        {
            return await _context.Workouts
                    .Include(w => w.WorkoutExercises)
                    .ThenInclude(w => w.Exercise)
                    .Include(w => w.WorkoutExercises)
                    .ThenInclude(s => s.Sets)
                    .FirstOrDefaultAsync(w => w.Id ==  id);
        }
        public async Task<WorkoutExercise?> GetWorkoutExerciseByIdAsync(Guid workoutExerciseId)
        {
            return await _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.Id == workoutExerciseId);
        }
        public async Task<WorkoutExercise?> AddGeneralExerciseToWorkoutAsync(Guid workoutId, Guid exerciseId)
        {
            var workout = await _context.Workouts
                                        .Include(w => w.WorkoutExercises)
                                            .ThenInclude(we => we.Exercise)
                                                .ThenInclude(e => e.PrimaryMuscle)
                                        .Include(w => w.WorkoutExercises)
                                            .ThenInclude(we => we.Exercise)
                                                .ThenInclude(e => e.Category)
                                        .FirstOrDefaultAsync(x => x.Id == workoutId);

            var exercise = await _context.Exercises
                                         .Include(e => e.PrimaryMuscle)
                                         .Include(e => e.Category)
                                         .FirstOrDefaultAsync(x => x.Id == exerciseId);

            if (workout == null || exercise == null) 
            {
                return null;
            }

            // Calculate the next order number to add the exercise at the bottom
            int nextOrder = workout.WorkoutExercises.Any() 
                ? workout.WorkoutExercises.Max(we => we.Order) + 1 
                : 0;

            var workoutExercise = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                ExerciseId = exerciseId,
                Exercise = exercise,
                Order = nextOrder,
                //TimeCreated = DateTime.UtcNow,
                Sets = new List<Set>()
            };

            await _context.WorkoutExercises.AddAsync(workoutExercise);
            await _context.SaveChangesAsync();
            return workoutExercise;
        }
        public async Task<WorkoutExercise?> AddCustomExerciseToWorkoutAsync(Guid workoutId, Guid customexerciseId)
        {
            var workout = await _context.Workouts
                                        .Include(w => w.WorkoutExercises)
                                            /*.ThenInclude(we => we.CustomExercise)
                                                .ThenInclude(e => e.PrimaryMuscle)
                                        .Include(w => w.WorkoutExercises)
                                            .ThenInclude(we => we.CustomExercise)
                                                .ThenInclude(e => e.Category)*/
                                        .FirstOrDefaultAsync(x => x.Id == workoutId);
            var exercise = await _context.CustomExercises.FirstOrDefaultAsync(x => x.Id == customexerciseId);

            if (workout == null || exercise == null)
            {
                return null;
            }

            // Calculate the next order number to add the exercise at the bottom
            int nextOrder = workout.WorkoutExercises.Any() 
                ? workout.WorkoutExercises.Max(we => we.Order) + 1 
                : 0;

            var workoutExercise = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                CustomExerciseId = customexerciseId,
                Order = nextOrder,
                //TimeCreated = DateTime.UtcNow,
                Sets = new List<Set>()
            };

            await _context.WorkoutExercises.AddAsync(workoutExercise);
            await _context.SaveChangesAsync();
            return workoutExercise;
        }
        //public async Task<Set?> AddSetToWorkoutExerciseAsync(Guid workoutExerciseId, int? repetitions, float? weight, float? duration, float? rest, )

        public async Task<Set?> AddSetToWorkoutExerciseAsync(
            Guid workoutExerciseId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? restTimeUnitId, Guid? durationTimeUnitId, Guid? weightUnitId, Guid? userId = null)
        {
            var workoutExercise = await _context.WorkoutExercises.FirstOrDefaultAsync(x=>x.Id == workoutExerciseId);
            if (workoutExercise == null)
            {
                return null;
            }

            var set = new Set
            {
                Id = Guid.NewGuid(),
                WorkoutExerciseId = workoutExerciseId,
                Repetitions = repetitions,
                Note = note,
                Weight = weight,
                Duration = duration,
                RestTime = rest,
                RestTimeUnitId = restTimeUnitId,
                DurationTimeUnitId = durationTimeUnitId,
                WeightUnitId = weightUnitId,
                UserId = userId
                
            };


            await _context.Sets.AddAsync(set);
            await _context.SaveChangesAsync();

            var setWithUnits = await _context.Sets
                                             .Include(s => s.RestTimeUnit)
                                             .Include(s => s.DurationTimeUnit)
                                             .Include(s => s.WeightUnit)
                                             .FirstOrDefaultAsync(s => s.Id == set.Id);
            return setWithUnits;
        }
        public async Task<Set?> UpdateSetAsync(
            Guid setId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? restTimeUnitId, Guid? durationTimeUnitId, Guid? weightUnitId)
        {
            var set = await _context.Sets.FirstOrDefaultAsync(x => x.Id == setId);
            if (set == null)
            {
                return null;
            }
            set.Repetitions = repetitions;
            set.Note = note;
            set.Weight = weight;
            set.Duration = duration;
            set.RestTime = rest;
            set.RestTimeUnitId = restTimeUnitId;
            set.DurationTimeUnitId = durationTimeUnitId;
            set.WeightUnitId = weightUnitId;
            
            await _context.SaveChangesAsync();
            return set;
        }
        /*public async Task<Set?> UpdateSetAsync(Guid setId, Set oldSet)
        {
            var set = await _context.Sets.FirstOrDefaultAsync(x => x.Id == setId);
            if (set == null)
            {
                return null;
            }
            set.Repetitions = oldSet.Repetitions;
            set.Weight = oldSet.Weight;
            set.Duration = oldSet.Duration;
            set.RestTime = oldSet.RestTime;
            set.TimeUnit = oldSet.TimeUnit;
            set.TimeUnitId = oldSet.TimeUnitId;
            set.WeightUnit = oldSet.WeightUnit;
            set.WeightUnitId = oldSet.WeightUnitId;
            
            await _context.SaveChangesAsync();
            return set;
        }*/
        public async Task<Set?> GetSetByIdAsync(Guid setId)
        {
            return await _context.Sets
                .Include(s => s.WorkoutExercise)
                    .ThenInclude(we => we.Workout)
                .Include(s => s.RestTimeUnit)
                .Include(s => s.DurationTimeUnit)
                .Include(s => s.WeightUnit)
                .FirstOrDefaultAsync(s => s.Id == setId);
        }
        public async Task<bool> DeleteSetAsync(Guid setId)
        {
            var set = _context.Sets.FirstOrDefault(x => x.Id == setId);
            if(set == null)
            {
                return false;
            }    
            _context.Sets.Remove(set);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteWorkoutExerciseAsync(Guid workoutExerciseId)
        {
            var workoutExercise = await _context.WorkoutExercises
                .Include(s =>s.Sets)
                .FirstOrDefaultAsync(x => x.Id==workoutExerciseId);

            if(workoutExercise == null)
            {
                return false;
            }

            _context.Sets.RemoveRange(workoutExercise.Sets);
            _context.WorkoutExercises.Remove(workoutExercise);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteWorkoutAsync(Guid workoutId)
        {
            var workout = await _context.Workouts.FindAsync(workoutId);
            if(workout == null)
            {
                return false;
            }

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<TimeUnit>> GetTimeUnit()
        {
            var timeUnits = await _context.TimeUnits.ToListAsync();
            return timeUnits;
        }
        public async Task<List<WeightUnit>> GetWeightUnit()
        {
            var weightUnits = await _context.WeightUnits.ToListAsync();
            return weightUnits;
        }

        public async Task<(bool Success, string ErrorMessage)> ReorderExercisesAsync(Guid userId, Guid workoutId, List<ExerciseOrderDto> exerciseOrders)
        {
            var workout = await _context.Workouts.FindAsync(workoutId);
            if (workout == null) 
                return (false, "Workout not found");

            // Check permissions
            var plan = await _context.Plans.FindAsync(workout.PlanId);
            if (plan != null && !plan.IsStatic && workout.UserId != userId)
                return (false, "Access denied: You don't have permission to modify this workout");

            // Update orders
            foreach (var exerciseOrder in exerciseOrders)
            {
                var workoutExercise = await _context.WorkoutExercises
                    .FirstOrDefaultAsync(we => we.Id == exerciseOrder.WorkoutExerciseId && we.WorkoutId == workoutId);

                if (workoutExercise == null)
                    return (false, $"Exercise with ID {exerciseOrder.WorkoutExerciseId} not found in workout");
                
                workoutExercise.Order = exerciseOrder.Order;
            }

            await _context.SaveChangesAsync();
            return (true, "Success");
        }

        public async Task<(bool Success, string ErrorMessage)> ReorderWorkoutsAsync(Guid userId, Guid planId, List<WorkoutOrderDto> workoutOrders, bool isAdmin)
        {
            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null)
                return (false, "Plan not found");

            if (plan.IsStatic)
            {
                if (!isAdmin)
                    return (false, "Access denied: Only admins can reorder workouts in static plans");
            }
            else
            {
                if (plan.UserId != userId)
                    return (false, "Access denied: You don't have permission to modify this plan");
            }

            foreach (var workoutOrder in workoutOrders)
            {
                var workout = await _context.Workouts
                    .FirstOrDefaultAsync(w => w.Id == workoutOrder.WorkoutId && w.PlanId == planId);

                if (workout == null)
                    return (false, $"Workout with ID {workoutOrder.WorkoutId} not found in plan");

                workout.Order = workoutOrder.Order;
            }

            await _context.SaveChangesAsync();
            return (true, "Success");
        }

    }
}
