using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzCad.Database.Migrations
{
    public partial class FixTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Identities_ToIdentityId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "ToIdentityId",
                table: "Transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Identities_ToIdentityId",
                table: "Transactions",
                column: "ToIdentityId",
                principalTable: "Identities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Identities_ToIdentityId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "ToIdentityId",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Identities_ToIdentityId",
                table: "Transactions",
                column: "ToIdentityId",
                principalTable: "Identities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
