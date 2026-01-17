using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Repositories
{
    public class SQLPlanRepository : IPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public SQLPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Plan> CreatePlanAsync(Plan plan)
        {
            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return plan;
        }
        public async Task<Plan?> UpdatePlanTitleAsync(Guid userId, Guid planId, string? newTitle, string? newNote)
        {
            var plan = await _context.Plans
                                        .FirstOrDefaultAsync
                                        (w => w.UserId == userId && w.Id == planId);

            if (plan == null)
            {
                return null;
            }

            plan.Title = newTitle ?? plan.Title;
            plan.Notes = newNote ?? plan.Notes;
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<List<Plan>> GetAllPlansAsync(Guid userId)
        {
            // Get both user plans and static plans
            var userPlans = await GetAllUserPlansAsync(userId);
            var staticPlans = await GetAllStaticPlansAsync();
            
            return userPlans.Concat(staticPlans).ToList();
        }

        public async Task<List<Plan>> GetAllStaticPlansAsync()
        {
            return await _context.Plans
                .Where(p => p.IsStatic == true)
                .Include(w => w.Workouts)
                .ThenInclude(we => we.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .Include(w => w.Workouts)
                .ThenInclude(we => we.WorkoutExercises)
                .ThenInclude(we => we.CustomExercise)
                .ToListAsync();
        }

        public async Task<List<Plan>> GetAllUserPlansAsync(Guid userId)
        {
            return await _context.Plans
                .Where(u => u.UserId == userId && u.IsStatic == false)
                .Include(w => w.Workouts).ToListAsync();
        }

        public async Task<Plan?> GetPlanByIdAsync(Guid id)
        {
            return await _context.Plans
                .Include(w => w.Workouts)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> DeletePlanAsync(Guid id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return false;
            }
            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsPlanStaticAsync(Guid planId)
        {
            var plan = await _context.Plans.FindAsync(planId);
            return plan?.IsStatic ?? false;
        }

    }
}
