using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Models.Recommendation;

namespace GymFitnessTracker.Services
{
    public interface IRecommendationService
    {
        List<PlanTemplate> GetAllTemplates();
        PlanTemplate? GetTemplateById(string templateId);
        Task<Plan?> GeneratePlanAsync(Guid userId, string templateId);
    }
}
