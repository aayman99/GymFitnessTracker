using GymFitnessTracker.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymFitnessTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private Guid GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return Guid.Parse(userId.Value);
        }

        [HttpPost("UploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No File Uploaded" });
            }

            var userId = GetUserId().ToString();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new {message = "No User Found"});
            }

            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-pictures");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // save url
            user.ProfilePictureUrl = $"/uploads/profile-pictures/{fileName}";
            await _userManager.UpdateAsync(user);

            /*return Ok(new
            {
                message = "Profile picture uploaded.",
                url = user.ProfilePictureUrl
            });*/

            return Ok(user.ProfilePictureUrl);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId().ToString();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.UserName,
                user.Email,
                user.ProfilePictureUrl,
                user.InAppName
            });
        }

        

    }
}
