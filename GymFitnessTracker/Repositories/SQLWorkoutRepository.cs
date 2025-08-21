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
           
            var workouts =  await _context.Workouts
                    .Where(u => u.UserId == userId && u.PlanId == planId)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.Exercise)
                            .ThenInclude(p => p.PrimaryMuscle)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.Exercise)
                            .ThenInclude(c => c.Category)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.CustomExercise)
                            /*.ThenInclude(pce => pce.PrimaryMuscle)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(e => e.CustomExercise)
                            .ThenInclude(cce => cce.Category)*/
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets)
                            .ThenInclude(tu => tu.TimeUnit)
                    .Include(w => w.WorkoutExercises)
                        .ThenInclude(s => s.Sets)
                            .ThenInclude(wt => wt.WeightUnit)
                    .ToListAsync();

            foreach(var workout in workouts)
            {
                workout.WorkoutExercises = workout.WorkoutExercises.OrderBy(we => we.TimeCreated).ToList();
            }
            return workouts;
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

            var workoutExercise = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                ExerciseId = exerciseId,
                Exercise = exercise,
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

            var workoutExercise = new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workoutId,
                CustomExerciseId = customexerciseId,
                //TimeCreated = DateTime.UtcNow,
                Sets = new List<Set>()
            };

            await _context.WorkoutExercises.AddAsync(workoutExercise);
            await _context.SaveChangesAsync();
            return workoutExercise;
        }
        //public async Task<Set?> AddSetToWorkoutExerciseAsync(Guid workoutExerciseId, int? repetitions, float? weight, float? duration, float? rest, )

        public async Task<Set?> AddSetToWorkoutExerciseAsync(
            Guid workoutExerciseId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? timeUnitId,  Guid? weightUnitId)
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
                TimeUnitId = timeUnitId,
                WeightUnitId = weightUnitId
                
            };


            await _context.Sets.AddAsync(set);
            await _context.SaveChangesAsync();

            var setWithUnits = await _context.Sets
                                             .Include(s => s.TimeUnit)
                                             .Include(s => s.WeightUnit)
                                             .FirstOrDefaultAsync(s => s.Id == set.Id);
            return setWithUnits;
        }
        public async Task<Set?> UpdateSetAsync(
            Guid setId, int? repetitions, string? note, float? weight, float? duration, float? rest, Guid? timeUnitId, Guid? weightUnitId)
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
            set.TimeUnitId = timeUnitId;
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
                .Include(tu => tu.TimeUnit)
                .Include(wu => wu.WeightUnit)
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

    }
}
