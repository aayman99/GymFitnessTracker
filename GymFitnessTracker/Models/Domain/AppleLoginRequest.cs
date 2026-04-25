using System.ComponentModel.DataAnnotations;

namespace GymFitnessTracker.Models.Domain
{
    public class AppleLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string AppleUserId { get; set; }

        // Apple only provides name on the first sign-in
        public string? InAppName { get; set; }
        
        // Optional - required only for new users (validated in controller)
        public string? Gender { get; set; }
        
        public string? ProfilePictureUrl { get; set; }
    }
}
