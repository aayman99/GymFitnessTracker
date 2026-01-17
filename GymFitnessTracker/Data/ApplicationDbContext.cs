using GymFitnessTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GymFitnessTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<CustomExercise> CustomExercises { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PrimaryMuscle> PrimaryMuscles { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<WeightUnit> WeightUnits { get; set; }
        public DbSet<TimeUnit> TimeUnits { get; set; }
        public DbSet<Plan> Plans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Set entity with two relationships to TimeUnit
            modelBuilder.Entity<Set>()
                .HasOne(s => s.RestTimeUnit)
                .WithMany()
                .HasForeignKey(s => s.RestTimeUnitId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Set>()
                .HasOne(s => s.DurationTimeUnit)
                .WithMany()
                .HasForeignKey(s => s.DurationTimeUnitId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
