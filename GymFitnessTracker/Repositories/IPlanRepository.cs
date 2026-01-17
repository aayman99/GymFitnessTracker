using GymFitnessTracker.Models.Domain;

namespace GymFitnessTracker.Repositories
{
    public interface IPlanRepository
    {
        Task<Plan> CreatePlanAsync(Plan plan);
        Task<Plan?> UpdatePlanTitleAsync(Guid userId, Guid planId, string? newTitle, string? newNote);
        Task<List<Plan>> GetAllPlansAsync(Guid userId);
        Task<List<Plan>> GetAllStaticPlansAsync();
        Task<List<Plan>> GetAllUserPlansAsync(Guid userId);
        Task<Plan?> GetPlanByIdAsync(Guid id);
        Task<bool> DeletePlanAsync(Guid id);
        Task<bool> IsPlanStaticAsync(Guid planId);
    }
}
