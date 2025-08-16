using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class ApplyCustomExerciseFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExerciseId",
                table: "WorkoutExercises",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomExerciseId",
                table: "WorkoutExercises",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryMuscleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomExercises_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomExercises_PrimaryMuscles_PrimaryMuscleId",
                        column: x => x.PrimaryMuscleId,
                        principalTable: "PrimaryMuscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_CustomExerciseId",
                table: "WorkoutExercises",
                column: "CustomExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomExercises_CategoryId",
                table: "CustomExercises",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomExercises_PrimaryMuscleId",
                table: "CustomExercises",
                column: "PrimaryMuscleId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_CustomExercises_CustomExerciseId",
                table: "WorkoutExercises",
                column: "CustomExerciseId",
                principalTable: "CustomExercises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_CustomExercises_CustomExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "CustomExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_CustomExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "CustomExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExerciseId",
                table: "WorkoutExercises",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
