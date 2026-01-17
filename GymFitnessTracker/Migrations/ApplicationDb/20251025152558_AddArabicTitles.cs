using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddArabicTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title_ar",
                table: "PrimaryMuscles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_ar",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title_ar",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title_ar",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title_ar",
                table: "PrimaryMuscles");

            migrationBuilder.DropColumn(
                name: "Description_ar",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "Title_ar",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "Title_ar",
                table: "Categories");
        }
    }
}
