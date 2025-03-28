using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzCad.Database.Migrations
{
    public partial class AddDiscordLinking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscordId",
                table: "AspNetUsers",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LicenseId_DiscordId_Salt",
                table: "AspNetUsers",
                columns: new[] { "LicenseId", "DiscordId", "Salt" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LicenseId_DiscordId_Salt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DiscordId",
                table: "AspNetUsers");
        }
    }
}
