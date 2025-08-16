using GymFitnessTracker.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace GymFitnessTracker.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser/*IdentityUser*/ user, List<string> roles);
    }
}
