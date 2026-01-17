using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddOrderColumnToReorderWorkoutExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "WorkoutExercises");
        }
    }
}
