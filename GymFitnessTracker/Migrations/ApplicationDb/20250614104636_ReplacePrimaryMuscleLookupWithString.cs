using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class ReplacePrimaryMuscleLookupWithString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomExercises_Categories_CategoryId",
                table: "CustomExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomExercises_PrimaryMuscles_PrimaryMuscleId",
                table: "CustomExercises");

            migrationBuilder.DropIndex(
                name: "IX_CustomExercises_CategoryId",
                table: "CustomExercises");

            migrationBuilder.DropIndex(
                name: "IX_CustomExercises_PrimaryMuscleId",
                table: "CustomExercises");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CustomExercises");

            migrationBuilder.DropColumn(
                name: "PrimaryMuscleId",
                table: "CustomExercises");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryMuscle",
                table: "CustomExercises",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryMuscle",
                table: "CustomExercises");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "CustomExercises",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryMuscleId",
                table: "CustomExercises",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CustomExercises_CategoryId",
                table: "CustomExercises",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomExercises_PrimaryMuscleId",
                table: "CustomExercises",
                column: "PrimaryMuscleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomExercises_Categories_CategoryId",
                table: "CustomExercises",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomExercises_PrimaryMuscles_PrimaryMuscleId",
                table: "CustomExercises",
                column: "PrimaryMuscleId",
                principalTable: "PrimaryMuscles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
