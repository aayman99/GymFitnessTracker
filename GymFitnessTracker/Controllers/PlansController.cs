using AutoMapper;
using Azure.Core;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;
using GymFitnessTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace GymFitnessTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IPlanRepository _planRepository;
        private readonly IMapper _mapper;
        public PlansController(IPlanRepository planRepository, IMapper mapper)
        {
            _planRepository = planRepository;
            _mapper = mapper;
        }

        private Guid GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return Guid.Parse(userId.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            var userId = GetUserId();
            var plans = await _planRepository.GetAllPlansAsync(userId);
            var plansDto = _mapper.Map<List<PlanDto>>(plans);
            return Ok(plansDto);
        }

        [HttpGet("static")]
        public async Task<IActionResult> GetAllStaticPlans()
        {
            var staticPlans = await _planRepository.GetAllStaticPlansAsync();
            var plansDto = _mapper.Map<List<PlanDto>>(staticPlans);
            return Ok(plansDto);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllUserPlans()
        {
            var userId = GetUserId();
            var userPlans = await _planRepository.GetAllUserPlansAsync(userId);
            var plansDto = _mapper.Map<List<PlanDto>>(userPlans);
            return Ok(plansDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(Guid id)
        {
            var userId = GetUserId();
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if(plan == null || plan.UserId != userId)
            {
                return NotFound(new {message = "No plan found."});
            }

            var planDto = _mapper.Map<PlanDto>(plan);
            return Ok(planDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] AddPlanRequestDto planRequestDto)
        {
            var userId = GetUserId();   
            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = planRequestDto.Title,
                Notes = planRequestDto.Notes
            };

            var createdPlan = await _planRepository.CreatePlanAsync(plan);
            var planDto = _mapper.Map<PlanDto>(createdPlan);
            return Ok(planDto);
        }

        [HttpPut("UpdatePlanInfo/{id}")]
        public async Task<IActionResult> UpdatePlanInfo([FromRoute] Guid id, [FromBody] UpdatePlanInfoDto request)
        {
            var userId = GetUserId();
            var updatedPlan = await _planRepository.UpdatePlanTitleAsync(userId, id, request.Title, request.Notes);

            if (updatedPlan == null)
            {
                return NotFound(new { message = "No plan found" });
            }

            return Ok(new { message = "Plan information updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(Guid id)
        {
            var userId = GetUserId();
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if(plan == null || plan.UserId != userId)
            {
                return NotFound(new {message = "No plan found."});
            }
            var deleted = await _planRepository.DeletePlanAsync(id);
            if(!deleted)
            {
                return NotFound(new { message = "No plan found." });
            }

            return Ok(new {message = "Plan deleted successfully."});
        }

        // Admin-only endpoints for static plans
        [Authorize(Roles = "Admin")]
        [HttpPost("static")]
        public async Task<IActionResult> CreateStaticPlan([FromBody] AddStaticPlanRequestDto planRequestDto)
        {
            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Empty, // Static plans don't belong to a specific user
                Title = planRequestDto.Title,
                Notes = planRequestDto.Notes,
                IsStatic = true
            };

            var createdPlan = await _planRepository.CreatePlanAsync(plan);
            var planDto = _mapper.Map<PlanDto>(createdPlan);
            return Ok(planDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("static/{id}")]
        public async Task<IActionResult> UpdateStaticPlan([FromRoute] Guid id, [FromBody] UpdatePlanInfoDto request)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if (plan == null || !plan.IsStatic)
            {
                return NotFound(new { message = "Static plan not found" });
            }

            var updatedPlan = await _planRepository.UpdatePlanTitleAsync(Guid.Empty, id, request.Title, request.Notes);
            if (updatedPlan == null)
            {
                return NotFound(new { message = "Static plan not found" });
            }

            return Ok(new { message = "Static plan updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("static/{id}")]
        public async Task<IActionResult> DeleteStaticPlan(Guid id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if (plan == null || !plan.IsStatic)
            {
                return NotFound(new { message = "Static plan not found." });
            }

            var deleted = await _planRepository.DeletePlanAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Static plan not found." });
            }

            return Ok(new { message = "Static plan deleted successfully." });
        }
    }
}
