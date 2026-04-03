namespace GymFitnessTracker.Models.DTO
{
    /// <summary>
    /// Optional body for coach role activation. The current authenticated user becomes a coach.
    /// Send as multipart/form-data to include an ID document file.
    /// </summary>
    public class RequestBecomeCoachDto
    {
        public string? Bio { get; set; }
        public string? Experience { get; set; }
        public IFormFile? IdDocument { get; set; }
    }
}
