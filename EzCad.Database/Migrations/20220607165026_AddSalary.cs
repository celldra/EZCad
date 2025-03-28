using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzCad.Database.Migrations
{
    public partial class AddSalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Identities_FromIdentityId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "FromIdentityId",
                table: "Transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBenefitCollection",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Identities_FromIdentityId",
                table: "Transactions",
                column: "FromIdentityId",
                principalTable: "Identities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Identities_FromIdentityId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LastBenefitCollection",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "FromIdentityId",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Identities_FromIdentityId",
                table: "Transactions",
                column: "FromIdentityId",
                principalTable: "Identities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
