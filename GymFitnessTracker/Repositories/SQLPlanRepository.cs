using GymFitnessTracker.Data;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.DTO;
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
            var existingPlans = plan.IsStatic
                ? await _context.Plans.Where(p => p.IsStatic).ToListAsync()
                : await _context.Plans.Where(p => !p.IsStatic && p.UserId == plan.UserId).ToListAsync();

            plan.Order = existingPlans.Any()
                ? existingPlans.Max(p => p.Order) + 1
                : 0;

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
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<List<Plan>> GetAllUserPlansAsync(Guid userId)
        {
            return await _context.Plans
                .Where(u => u.UserId == userId && u.IsStatic == false)
                .Include(w => w.Workouts)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Title)
                .ToListAsync();
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

        public async Task<(bool Success, string ErrorMessage)> ReorderPlansAsync(Guid userId, List<PlanOrderDto> planOrders, bool isAdmin, bool reorderStatic)
        {
            if (reorderStatic && !isAdmin)
                return (false, "Access denied: Only admins can reorder static plans");

            foreach (var planOrder in planOrders)
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == planOrder.PlanId);

                if (plan == null)
                    return (false, $"Plan with ID {planOrder.PlanId} not found");

                if (reorderStatic)
                {
                    if (!plan.IsStatic)
                        return (false, $"Plan with ID {planOrder.PlanId} is not a static plan");
                }
                else
                {
                    if (plan.IsStatic)
                        return (false, $"Plan with ID {planOrder.PlanId} is a static plan and cannot be reordered here");

                    if (plan.UserId != userId)
                        return (false, $"Access denied: You don't have permission to modify plan {planOrder.PlanId}");
                }

                plan.Order = planOrder.Order;
            }

            await _context.SaveChangesAsync();
            return (true, "Success");
        }

    }
}
