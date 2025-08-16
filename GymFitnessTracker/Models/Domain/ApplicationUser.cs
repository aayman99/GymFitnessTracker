using Microsoft.AspNetCore.Identity;

namespace GymFitnessTracker.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? VerificationPin { get; set; }
        public DateTime? VerificationPinExpiry { get; set; }
        public string? PasswordResetPin { get; set; }
        public DateTime? PasswordResetPinExpiry { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? InAppName { get; set; }
        public string Gender { get; set; }
    }
}
