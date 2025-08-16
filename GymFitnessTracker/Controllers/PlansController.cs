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
    }
}
