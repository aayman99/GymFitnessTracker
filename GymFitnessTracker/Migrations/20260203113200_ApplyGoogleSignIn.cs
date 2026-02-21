using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class ApplyGoogleSignIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalProviderUserId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginProvider",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalProviderUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LoginProvider",
                table: "AspNetUsers");
        }
    }
}
