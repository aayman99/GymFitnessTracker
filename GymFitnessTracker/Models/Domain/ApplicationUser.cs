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
        
        // Google OAuth fields
        public string? LoginProvider { get; set; } // "Email", "Google", etc.
        public string? ExternalProviderUserId { get; set; } // Google's user ID

        // Optional coach-only fields (shown when user has Coach role)
        public string? Bio { get; set; }
        public string? Experience { get; set; }
        public string? IdDocumentUrl { get; set; }
    }
}
