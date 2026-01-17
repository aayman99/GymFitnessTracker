using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class RemoveArabicExerciseTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title_ar",
                table: "Exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title_ar",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
