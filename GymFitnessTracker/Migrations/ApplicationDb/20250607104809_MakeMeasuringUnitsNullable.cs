using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class MakeMeasuringUnitsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_WeightUnits_WeightUnitId",
                table: "Sets");

            migrationBuilder.AlterColumn<Guid>(
                name: "WeightUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets",
                column: "TimeUnitId",
                principalTable: "TimeUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_WeightUnits_WeightUnitId",
                table: "Sets",
                column: "WeightUnitId",
                principalTable: "WeightUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_WeightUnits_WeightUnitId",
                table: "Sets");

            migrationBuilder.AlterColumn<Guid>(
                name: "WeightUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TimeUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets",
                column: "TimeUnitId",
                principalTable: "TimeUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_WeightUnits_WeightUnitId",
                table: "Sets",
                column: "WeightUnitId",
                principalTable: "WeightUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
