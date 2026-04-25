using AutoMapper;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;
using GymFitnessTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymFitnessTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {

        private readonly IWorkoutRepository _workoutRepository;
        private readonly IPlanRepository _planRepository;
        private readonly ICustomExerciseRepository _customExerciseRepository;
        private readonly IMapper _mapper;
        public WorkoutsController(IWorkoutRepository workoutRepository, IMapper mapper, IPlanRepository planRepository, ICustomExerciseRepository customExerciseRepository)
        {
            _workoutRepository = workoutRepository;
            _mapper = mapper;
            _planRepository = planRepository;
            _customExerciseRepository = customExerciseRepository;
        }

        private Guid GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "UserId");
            return Guid.Parse(userId.Value);
        }

        [HttpGet("{planId}")]
        public async Task<IActionResult> GetAllWorkouts(Guid planId)
        {
            var userId = GetUserId();
            var domainWorkouts = await _workoutRepository.GetAllWorkoutsAsync(userId, planId);
            var workoutDto = _mapper.Map<List<WorkoutDto>>(domainWorkouts);
            return Ok(workoutDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkout([FromBody] AddWorkoutRequestDto workoutRequestDto)
        {
            var userId = GetUserId();
            var plan = await _planRepository.GetPlanByIdAsync(workoutRequestDto.PlanId);
            if (plan == null)
            {
                return Forbid();
            }

            // Check if plan is static
            if (plan.IsStatic)
            {
                // Only admins can create workouts in static plans
                var userRoles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value);
                if (!userRoles.Contains("Admin"))
                {
                    return BadRequest(new { message = "Only admins can create workouts in static plans." });
                }
            }
            else
            {
                // For non-static plans, check if user owns the plan
                if (plan.UserId != userId)
                {
                    return Forbid();
                }
            }

            //var workoutDomain = _mapper.Map<Workout>(workoutRequestDto);
            var workoutDomain = new Workout
            {
                Id = Guid.NewGuid(),
                PlanId = workoutRequestDto.PlanId,
                UserId = userId,
                Title = workoutRequestDto.Title,
                Date = DateTime.UtcNow,
                Note = workoutRequestDto.Note
            };
            workoutDomain = await _workoutRepository.CreateWorkoutAsync(workoutDomain);

            var workoutDto = _mapper.Map<WorkoutDto>(workoutDomain);
            return Ok(workoutDto);
        }

        [HttpPut("UpdateWorkoutInfo/{id}")]
        public async Task<IActionResult> UpdateWorkoutInfo([FromRoute] Guid id, [FromBody] UpdateWorkoutInfoDto request)
        {
            var userId = GetUserId();
            
            // Check if workout belongs to a static plan
            var workout = await _workoutRepository.GetWorkoutById(id);
            if (workout != null)
            {
                var plan = await _planRepository.GetPlanByIdAsync(workout.PlanId);
                if (plan != null && plan.IsStatic)
                {
                    // Only admins can edit workouts in static plans
                    var userRoles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value);
                    if (!userRoles.Contains("Admin"))
                    {
                        return BadRequest(new { message = "Only admins can edit workouts in static plans." });
                    }
                }
                else if (plan != null && !plan.IsStatic && workout.UserId != userId)
                {
                    return Forbid();
                }
            }

            var updatedWorkout = await _workoutRepository.UpdateWorkoutTitleAsync(userId, id, request.Title, request.Note);

            if(updatedWorkout == null)
            {
                return NotFound(new {message = "No workout found"});
            }

            return Ok(new { message = "Workout information updated successfully" });
        }

        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddExerciseToWorkout([FromRoute] Guid workoutId,[FromBody]AddWorkoutExerciseRequestDto addWorkoutExerciseRequestDto)
        {
            var userId = GetUserId();

            // Validate Workout Ownership
            var workout = await _workoutRepository.GetWorkoutById(workoutId);
            if (workout == null)
            {
                return Forbid();
            }

            // Check if workout belongs to a static plan
            var plan = await _planRepository.GetPlanByIdAsync(workout.PlanId);
            if (plan != null && plan.IsStatic)
            {
                // Only admins can add exercises to workouts in static plans
                var userRoles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value);
                if (!userRoles.Contains("Admin"))
                {
                    return BadRequest(new { message = "Only admins can add exercises to workouts in static plans." });
                }
            }
            else if (workout.UserId != userId)
            {
                return Forbid();
            }

            /*var workoutExercise = await _workoutRepository.AddExerciseToWorkoutAsync(
                workoutId, addWorkoutExerciseRequestDto.ExerciseId);*/
            var addedWorkoutExercises = new List<WorkoutExercise>();
            foreach(var exerciseId in addWorkoutExerciseRequestDto.ExerciseId)
            {
                var workoutExercise = await _workoutRepository.AddGeneralExerciseToWorkoutAsync(
                workoutId, exerciseId);
                
                if (workoutExercise != null)
                {
                    //return NotFound(new { message = "Workout or Exercise not found" });
                    addedWorkoutExercises.Add(workoutExercise);
                }
            }

            foreach (var exerciseId in addWorkoutExerciseRequestDto.CustomExerciseId)
            {

                var custom = await _customExerciseRepository.GetCustomExerciseByIdAsync(exerciseId);
                if(custom == null || custom.UserId != userId)
                {
                    continue;
                }

                var workoutExercise = await _workoutRepository.AddCustomExerciseToWorkoutAsync(workoutId, exerciseId);
                if (workoutExercise != null)
                {
                    //return NotFound(new { message = "Workout or Exercise not found" });
                    addedWorkoutExercises.Add(workoutExercise);
                }
            }

            /*if (workoutExercise == null)
            {
                return NotFound(new {message = "Workout or Exercise not found"});
            }*/

            //var workoutExerciseDto = _mapper.Map<WorkoutExerciseDto>(workoutExercise);
            var workoutExerciseDto = _mapper.Map<List<WorkoutExerciseDto>>(addedWorkoutExercises);

            return Ok(workoutExerciseDto);
        }

        [HttpPost("exercise/{workoutExerciseId}/sets")]
        public async Task<IActionResult> AddSetsToWorkoutExercise([FromRoute] Guid workoutExerciseId, [FromBody] AddSetRequestDto addSetRequestDto)
        {
            var userId = GetUserId();

            // Validate WorkoutExercise exists
            var workoutExercise = await _workoutRepository.GetWorkoutExerciseByIdAsync(workoutExerciseId);
            if (workoutExercise == null)
            {
                return Forbid();
            }

            // Check if workout belongs to a static plan
            var plan = await _planRepository.GetPlanByIdAsync(workoutExercise.Workout.PlanId);
            if (plan != null && plan.IsStatic)
            {
                // For static plans, users can add sets to any exercise
                // No additional ownership check needed - users can add sets to admin-created exercises
            }
            else
            {
                // For non-static plans, check if user owns the workout
                if (workoutExercise.Workout.UserId != userId)
                {
                    return Forbid();
                }
            }

            // Note: Users can add sets to exercises even in static plans
            // This is the only thing they can control in static plans
            /*else if (workoutExercise.Workout.UserId != userId)
            {
                return BadRequest(new { message = "user id is not equal workout user id" });
            }*/
            /*var set = await _workoutRepository.AddSetToWorkoutExerciseAsync(
                workoutExerciseId, addSetRequestDto);*/
            var set = await _workoutRepository.AddSetToWorkoutExerciseAsync(
               workoutExerciseId, 
               addSetRequestDto.Repetitions, addSetRequestDto.Note,addSetRequestDto.Weight, addSetRequestDto.Duration, 
               addSetRequestDto.RestTime, addSetRequestDto.RestTimeUnitId, addSetRequestDto.DurationTimeUnitId, addSetRequestDto.WeightUnitId, userId);

            if (set == null)
            {
                return NotFound(new {message = "Workout Exercise not found"});
            }

            var setDto = _mapper.Map<SetDto>(set);
            return Ok(setDto);
        }

        [HttpPut("set/{setId}")]
        public async Task<IActionResult> UpdateSet([FromRoute] Guid setId, [FromBody] UpdateSetRequestDto updateSetRequestDto)
        {
            var userId = GetUserId();

            // Validate Set exists
            var set = await _workoutRepository.GetSetByIdAsync(setId);
            if (set == null)
            {
                return Forbid();
            }

            // Check if workout belongs to a static plan
            var plan = await _planRepository.GetPlanByIdAsync(set.WorkoutExercise.Workout.PlanId);
            if (plan != null && plan.IsStatic)
            {
                // For static plans, users can only update their own sets
                if (set.UserId != userId)
                {
                    return Forbid();
                }
            }
            else
            {
                // For non-static plans, check if user owns the workout
                if (set.WorkoutExercise.Workout.UserId != userId)
                {
                    return Forbid();
                }
            }

            // Note: Users can update their own sets even in static plans
            // This is the only thing they can control in static plans
            var updatedSet = await _workoutRepository.UpdateSetAsync(
                setId, 
                updateSetRequestDto.Repetitions, updateSetRequestDto.Note ,updateSetRequestDto.Weight, updateSetRequestDto.Duration, 
                updateSetRequestDto.RestTime, updateSetRequestDto.RestTimeUnitId, updateSetRequestDto.DurationTimeUnitId, updateSetRequestDto.WeightUnitId
                );

            if (updatedSet == null)
            {
                return NotFound(new { message = "Set not found"});
            }

            var setDto = _mapper.Map<SetDto>(updatedSet);
            return Ok(setDto);
        }

        [HttpDelete("set/{setId}")]
        public async Task<IActionResult> DeleteSet([FromRoute] Guid setId)
        {
            var userId = GetUserId();

            var set = await _workoutRepository.GetSetByIdAsync(setId);
            if (set == null)
            {
                return Forbid();
            }

            // Check if workout belongs to a static plan
            var plan = await _planRepository.GetPlanByIdAsync(set.WorkoutExercise.Workout.PlanId);
            if (plan != null && plan.IsStatic)
            {
                // For static plans, users can only delete their own sets
                if (set.UserId != userId)
                {
                    return Forbid();
                }
            }
            else
            {
                // For non-static plans, check if user owns the workout
                if (set.WorkoutExercise.Workout.UserId != userId)
                {
                    return Forbid();
                }
            }

            // Note: Users can delete their own sets even in static plans
            // This is the only thing they can control in static plans
            var deleted = await _workoutRepository.DeleteSetAsync(setId);
            if (!deleted)
            {
                return NotFound(new { message = "Set not found" });
            }
            return Ok(new {message = "Set deleted successfully"});
        }
       
        [HttpDelete("exercise/{workoutExerciseId}")]
        public async Task<IActionResult> DeleteWorkoutExercise([FromRoute] Guid workoutExerciseId)
        {
            var userId = GetUserId();

            var workoutExercise = await _workoutRepository.GetWorkoutExerciseByIdAsync(workoutExerciseId);
            if (workoutExercise == null)
            {
                return Forbid();
            }

            // Check if workout exercise belongs to a static plan
            var plan = await _planRepository.GetPlanByIdAsync(workoutExercise.Workout.PlanId);
            if (plan != null && plan.IsStatic)
            {
                // Only admins can delete exercises from workouts in static plans
                var userRoles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value);
                if (!userRoles.Contains("Admin"))
                {
                    return BadRequest(new { message = "Only admins can delete exercises from workouts in static plans." });
                }
            }
            else if (workoutExercise.Workout.UserId != userId)
            {
                return Forbid();
            }

            var deleted = await _workoutRepository.DeleteWorkoutExerciseAsync(workoutExerciseId);
            if(!deleted)
            {
                return NotFound(new { message = "Workout Exercise not found" });
            }

            return Ok(new {message = "Workout Exercise deleted successfully"});
        }

        [HttpDelete("{workoutId}")]
        public async Task<IActionResult> DeleteWorkout(Guid workoutId)
        {
            var userId = GetUserId();

            var workout = await _workoutRepository.GetWorkoutById(workoutId);
            if (workout == null)
            {
                return Forbid();
            }

            // Check if workout belongs to a static plan
            var plan = await _planRepository.GetPlanByIdAsync(workout.PlanId);
            if (plan != null && plan.IsStatic)
            {
                // Only admins can delete workouts in static plans
                var userRoles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value);
                if (!userRoles.Contains("Admin"))
                {
                    return BadRequest(new { message = "Only admins can delete workouts in static plans." });
                }
            }
            else if (workout.UserId != userId)
            {
                return Forbid();
            }

            var deleted = await _workoutRepository.DeleteWorkoutAsync(workoutId);
            if (!deleted)
            {
                return NotFound(new { message = "No workout found." });
            }

            return Ok(new {message = "Workout deleted successfully"});
        }

        [HttpGet("GetTimeUnit")]
        public async Task<IActionResult> GetTimeUnit()
        {
            var timeUnits = await _workoutRepository.GetTimeUnit();
            return Ok(timeUnits);
        }
        [HttpGet("GetWeightUnit")]
        public async Task<IActionResult> GetWeightUnit()
        {
            var weightUnits = await _workoutRepository.GetWeightUnit();
            return Ok(weightUnits);
        }

        [HttpPut("ReorderExercises")]
        public async Task<IActionResult> ReorderExercises([FromBody] ReorderExercisesRequestDto request)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _workoutRepository.ReorderExercisesAsync(userId, request.WorkoutId, request.ExerciseOrders);
            
            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }
            
            return Ok(new { message = "Exercises reordered successfully" });
        }

        [HttpPut("ReorderWorkouts")]
        public async Task<IActionResult> ReorderWorkouts([FromBody] ReorderWorkoutsRequestDto request)
        {
            var userId = GetUserId();
            var isAdmin = User.Claims
                .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .Select(c => c.Value)
                .Contains("Admin");

            var (success, errorMessage) = await _workoutRepository.ReorderWorkoutsAsync(userId, request.PlanId, request.WorkoutOrders, isAdmin);

            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Workouts reordered successfully" });
        }
    }
}
