using AutoMapper;
using GymFitnessTracker.Models.DTO;
using GymFitnessTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymFitnessTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IMapper _mapper;

        public RecommendationController(IRecommendationService recommendationService, IMapper mapper)
        {
            _recommendationService = recommendationService;
            _mapper = mapper;
        }

        private Guid GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return Guid.Parse(userId.Value);
        }

        [HttpGet("templates")]
        public IActionResult GetAllTemplates()
        {
            var templates = _recommendationService.GetAllTemplates();
            var templatesDto = _mapper.Map<List<PlanTemplateDto>>(templates);
            return Ok(templatesDto);
        }

        [HttpGet("templates/{id}")]
        public IActionResult GetTemplateById(string id)
        {
            var template = _recommendationService.GetTemplateById(id);
            if (template == null)
                return NotFound(new { message = $"Plan template '{id}' not found." });

            var templateDto = _mapper.Map<PlanTemplateDto>(template);
            return Ok(templateDto);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GeneratePlan([FromBody] GeneratePlanRequestDto request)
        {
            var template = _recommendationService.GetTemplateById(request.TemplateId);
            if (template == null)
                return NotFound(new { message = $"Plan template '{request.TemplateId}' not found." });

            var userId = GetUserId();
            var plan = await _recommendationService.GeneratePlanAsync(userId, request.TemplateId);

            if (plan == null)
                return BadRequest(new { message = "Failed to generate plan." });

            var planDto = _mapper.Map<PlanDto>(plan);
            return CreatedAtAction(nameof(GetTemplateById), new { id = request.TemplateId }, planDto);
        }
    }
}
