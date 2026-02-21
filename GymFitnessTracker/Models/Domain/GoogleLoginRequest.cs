using System.ComponentModel.DataAnnotations;

namespace GymFitnessTracker.Models.Domain
{
    public class GoogleLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string GoogleUserId { get; set; }
        
        // Optional - required only for new users (validated in controller)
        public string? InAppName { get; set; }
        
        // Optional - required only for new users (validated in controller)
        public string? Gender { get; set; }
        
        public string? ProfilePictureUrl { get; set; }
    }
}
