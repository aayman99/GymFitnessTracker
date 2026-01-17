namespace GymFitnessTracker.Models.Domain
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Description_ar { get; set; }
        public string? VideoUrl { get; set; }
        public string? FemaleVideoUrl { get; set; }
        public string? PicturePath { get; set; }
        public Guid PrimaryMuscleId { get; set; }
        public Guid CategoryId { get; set; }
        public PrimaryMuscle PrimaryMuscle { get; set; }
        public Category Category { get; set; }

        /* public string PrimaryMuscle { get; set; }
         public string Category { get; set; }*/
    }
}
