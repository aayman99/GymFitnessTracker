using AutoMapper;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;
using GymFitnessTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.AccessControl;

namespace GymFitnessTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IMapper _mapper;


        public ExercisesController(IExerciseRepository exerciseRepository, IMapper mapper, IConfiguration config)
        {
            _exerciseRepository = exerciseRepository;
            _mapper = mapper;
        }

       


        [HttpGet("GetAllExercises")]
        public async Task<IActionResult> GetAll([FromQuery] string role, [FromQuery] string gender,
                                                [FromQuery] string? titleFilter, [FromQuery] string? categoryFilter, [FromQuery] string? muscleFilter,
                                                [FromQuery] string language = "EN")
        {
            

            var exercisesDomain = await _exerciseRepository.GetAllExercisesAsync(titleFilter, categoryFilter, muscleFilter, language);
            //var exercisesDto = _mapper.Map<List<ExerciseDto>>(exercisesDomain);

            if (role.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                var exercisesDto = _mapper.Map<List<ExerciseDto>>(exercisesDomain);
                return Ok(exercisesDto);
            }

            if (string.IsNullOrWhiteSpace(gender) == false)
            {
                if (gender.Equals("male", StringComparison.OrdinalIgnoreCase))
                {
                    var exercisesDto = _mapper.Map<List<MaleExerciseDto>>(exercisesDomain);
                    return Ok(exercisesDto);
                }
                else if (gender.Equals("female", StringComparison.OrdinalIgnoreCase))
                {
                    var exercisesDto = _mapper.Map<List<FemaleExerciseDto>>(exercisesDomain);
                    return Ok(exercisesDto);
                }
                
            }
            return BadRequest(new {message = "Gender is null"});
            //return Ok(exercisesDto);
        }

        [HttpGet("GetExercise/{name}")]
        public async Task<IActionResult> GetExerciseByName([FromRoute]string title)
        {
            var exerciseDomain = await _exerciseRepository.GetExerciseAsync(title);

            if(exerciseDomain == null)
            {
                return NotFound(new {message = "No exercise with this title found" });
            }
            var exerciseDto = _mapper.Map<ExerciseDto>(exerciseDomain);

            return Ok(exerciseDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateExercise")]
        public async Task<IActionResult> CreateExercise([FromBody] AddExerciseRequestDto exerciseRequestDto)
        {
            var exerciseDomain = _mapper.Map<Exercise>(exerciseRequestDto);

            exerciseDomain = await _exerciseRepository.CreateExerciseAsync(exerciseDomain);
            if(exerciseDomain == null)
            {
                return BadRequest(new { message = "An exercise with the same title already exists." });
            }

            var exerciseDto = _mapper.Map<ExerciseDto>(exerciseDomain);

            return Ok(exerciseDto);
            //return CreatedAtAction(nameof(GetExerciseById), new {id = exerciseDto.Id}, exerciseDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateExercise/{id}")]
        public async Task<IActionResult> UpdateExercise([FromRoute] Guid id,[FromBody] UpdateExerciseRequestDto updateExerciseRequestDto)
        {
            var exerciseDomain = _mapper.Map<Exercise>(updateExerciseRequestDto);

            exerciseDomain = await _exerciseRepository.UpdateExerciseAsync(id, exerciseDomain);
            if(exerciseDomain == null)
            {
                return NotFound(new { message = "not found" });
            }
            var exerciseDto = _mapper.Map<ExerciseDto>(exerciseDomain);
            return Ok(exerciseDto);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteExercise/{id}")]
        public async Task<IActionResult> DeleteExercise([FromRoute] Guid id)
        {
            var exerciseDomain = await _exerciseRepository.DeleteExerciseAsync(id);
            if(exerciseDomain == null)
            {
                return NotFound(new { message = "not found" });
            }

            var exerciseDto = _mapper.Map<ExerciseDto>(exerciseDomain);
            return Ok(new { message = "deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("GetTitles")]
        public async Task<IActionResult> GetExerciseTitles([FromBody] List<Guid> ids)
        {
            if(ids == null || !ids.Any())
            {
                return BadRequest("No Exercises Found");
            }

            var titles = await _exerciseRepository.GetExerciseTitles(ids);
            return Ok(titles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("CheckIsVideoAvailable")]
        public async Task<ActionResult<IEnumerable<string>>> CheckIsVideoAvailable(CancellationToken ct)
        {
            var brokenExercises = await _exerciseRepository.GetExercisesWithBrokenYoutubeAsync(ct: ct);
            var result = brokenExercises.Select(x => new { id = x.Id, title = x.Title });
            return Ok(result);
        }

        //[Authorize(Roles ="Admin")]
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetCategories([FromQuery] string language = "EN")
        {
            var categories = await _exerciseRepository.GetAllCategories(language);
            return Ok(categories);
        }
        
        //[Authorize(Roles ="Admin")]
        [HttpGet("GetAllMuscles")]
        public async Task<IActionResult> GetMuscles([FromQuery] string language = "EN")
        {
            var muscles = await _exerciseRepository.GetAllMuscles(language);
            return Ok(muscles);
        }


    }
}
