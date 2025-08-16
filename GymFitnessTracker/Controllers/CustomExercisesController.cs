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
    public class CustomExercisesController : ControllerBase
    {
        private readonly ICustomExerciseRepository _customExerciseRepository;
        private readonly IMapper _mapper;

        public CustomExercisesController(ICustomExerciseRepository customExerciseRepository, IMapper mapper)
        {
            _customExerciseRepository = customExerciseRepository;
            _mapper = mapper;
        }

        private Guid GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "UserId");
            return Guid.Parse(userId.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomExercise([FromBody] AddCustomExerciseRequestDto requestDto)
        {
            var userId = GetUserId();
            var customExercise = new CustomExercise
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = requestDto.Title,
                Description = requestDto.Description,
                VideoUrl = requestDto.VideoUrl,
                PrimaryMuscle = requestDto.PrimaryMuscle
                /*PrimaryMuscleId = requestDto.PrimaryMuscleId,
                CategoryId = requestDto.CategoryId*/
            };

            await _customExerciseRepository.CreateCustomExerciseAsync(customExercise);

            var customExerciseDto = _mapper.Map<CustomExerciseDto>(customExercise);
            return Ok(customExerciseDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomExercises([FromQuery] string? title)
        {
            var userId = GetUserId();
            var customExercises = await _customExerciseRepository.GetAllCustomExerciseByUserIdAsync(userId, title);

            var customExercisesDto = _mapper.Map<List<CustomExerciseDto>>(customExercises);
            return Ok(customExercisesDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomExercise([FromRoute] Guid id)
        {
            var userId = GetUserId();
            var exerciseDomain = await _customExerciseRepository.DeleteCustomExerciseAsync(userId,id);
            if(exerciseDomain == null)
            {
                return NotFound(new {message = "No custom exercise found"});
            }

            var exerciseDto = _mapper.Map<CustomExerciseDto>(exerciseDomain);
            return Ok(new {message = "Custom exercise deleted successfully"});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomExercise([FromRoute] Guid id, [FromBody] UpdateCustomExerciseRequestDto request)
        {
            var domainCustomExercise = _mapper.Map<CustomExercise>(request);

            domainCustomExercise = await _customExerciseRepository.UpdateCustomExerciseAsync(id, domainCustomExercise);

            if (domainCustomExercise == null)
            {
                return NotFound(new { message = "not found" });
            }

            var customExerciseDto = _mapper.Map<UpdateCustomExerciseRequestDto>(domainCustomExercise);

            return Ok(customExerciseDto);
        }
    }
}
