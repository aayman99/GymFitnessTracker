using GymFitnessTracker.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Data
{
    public class ApplicationAuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminId = "1";
            var userId = "2";
            var coachId = "3";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = adminId,
                    ConcurrencyStamp = adminId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper(),
                },
                new IdentityRole
                {
                    Id = userId,
                    ConcurrencyStamp = userId,
                    Name = "User",
                    NormalizedName = "User".ToUpper(),
                },
                new IdentityRole
                {
                    Id = coachId,
                    ConcurrencyStamp = coachId,
                    Name = "Coach",
                    NormalizedName = "Coach".ToUpper(),
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }

    }
}
