using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;
using GymFitnessTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymFitnessTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CoachesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoachesController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Returns all users who have the Coach role, with optional search by InAppName and pagination.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCoaches(
            [FromQuery] string? searchQuery,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var coaches = await _userManager.GetUsersInRoleAsync("Coach");

            IEnumerable<ApplicationUser> filtered = coaches;
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                filtered = filtered.Where(u =>
                    u.InAppName != null &&
                    u.InAppName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = filtered.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paged = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CoachDto
                {
                    Id = u.Id,
                    InAppName = u.InAppName,
                    UserName = u.UserName,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Bio = u.Bio,
                    Experience = u.Experience
                }).ToList();

            return Ok(new
            {
                data = paged,
                totalCount,
                pageNumber,
                pageSize,
                totalPages
            });
        }

        /// <summary>
        /// Activates the Coach role for the current authenticated user. Idempotent if already a coach.
        /// Optional body can include Bio and Experience for the coach profile.
        /// </summary>
        [HttpPost("BecomeCoach")]
        public async Task<IActionResult> RequestBecomeCoach([FromForm] RequestBecomeCoachDto? request = null)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Coach"))
            {
                if (request != null)
                {
                    if (request.Bio != null) user.Bio = request.Bio;
                    if (request.Experience != null) user.Experience = request.Experience;
                    await SaveIdDocumentAsync(request, user);
                    await _userManager.UpdateAsync(user);
                }
                var token = _tokenRepository.CreateJWTToken(user, roles.ToList());
                return Ok(new { message = "Already a coach.", token });
            }

            var addResult = await _userManager.AddToRoleAsync(user, "Coach");
            if (!addResult.Succeeded)
            {
                return BadRequest(new { message = "Could not add Coach role.", errors = addResult.Errors.Select(e => e.Description) });
            }

            if (request != null)
            {
                if (request.Bio != null) user.Bio = request.Bio;
                if (request.Experience != null) user.Experience = request.Experience;
                await SaveIdDocumentAsync(request, user);
                await _userManager.UpdateAsync(user);
            }

            roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

            return Ok(new { message = "You are now a coach.", token = jwtToken });
        }

        private async Task SaveIdDocumentAsync(RequestBecomeCoachDto request, ApplicationUser user)
        {
            if (request.IdDocument == null || request.IdDocument.Length == 0)
                return;

            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath ?? Directory.GetCurrentDirectory(), "uploads", "coach-id-documents");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.IdDocument.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.IdDocument.CopyToAsync(stream);
            }

            user.IdDocumentUrl = $"/uploads/coach-id-documents/{fileName}";
        }
    }
}
