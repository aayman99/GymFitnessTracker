namespace GymFitnessTracker.Models.Domain
{
    public class PrimaryMuscle
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Title_ar { get; set; }
        //public ICollection<Exercise> Exercises { get; set; }
    }
}
