using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzCad.Database.Migrations
{
    public partial class AddMotAndInsurance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsuranceState",
                table: "Vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MotState",
                table: "Vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceState",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "MotState",
                table: "Vehicles");
        }
    }
}
