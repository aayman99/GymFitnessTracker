using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddMeasuringUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "RestTime",
                table: "Sets",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TimeUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WeightUnitId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TimeUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeightUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightUnits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sets_TimeUnitId",
                table: "Sets",
                column: "TimeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_WeightUnitId",
                table: "Sets",
                column: "WeightUnitId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_TimeUnits_TimeUnitId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_WeightUnits_WeightUnitId",
                table: "Sets");

            migrationBuilder.DropTable(
                name: "TimeUnits");

            migrationBuilder.DropTable(
                name: "WeightUnits");

            migrationBuilder.DropIndex(
                name: "IX_Sets_TimeUnitId",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Sets_WeightUnitId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "RestTime",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "TimeUnitId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "WeightUnitId",
                table: "Sets");
        }
    }
}
