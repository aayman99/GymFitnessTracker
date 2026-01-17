using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddSeperateTimeUnitKeyForDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets");

            migrationBuilder.RenameColumn(
                name: "TimeUnitId",
                table: "Sets",
                newName: "RestTimeUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Sets_TimeUnitId",
                table: "Sets",
                newName: "IX_Sets_RestTimeUnitId");

            migrationBuilder.AddColumn<Guid>(
                name: "DurationTimeUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sets_DurationTimeUnitId",
                table: "Sets",
                column: "DurationTimeUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TimeUnits_DurationTimeUnitId",
                table: "Sets",
                column: "DurationTimeUnitId",
                principalTable: "TimeUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TimeUnits_RestTimeUnitId",
                table: "Sets",
                column: "RestTimeUnitId",
                principalTable: "TimeUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TimeUnits_DurationTimeUnitId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TimeUnits_RestTimeUnitId",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Sets_DurationTimeUnitId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "DurationTimeUnitId",
                table: "Sets");

            migrationBuilder.RenameColumn(
                name: "RestTimeUnitId",
                table: "Sets",
                newName: "TimeUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Sets_RestTimeUnitId",
                table: "Sets",
                newName: "IX_Sets_TimeUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets",
                column: "TimeUnitId",
                principalTable: "TimeUnits",
                principalColumn: "Id");
        }
    }
}
