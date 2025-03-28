﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzCad.Database.Migrations
{
    public partial class AddEmergencyReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmergencyReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Area = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PostCode = table.Column<int>(type: "integer", nullable: false),
                    ReportingIdentityId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyReports_Identities_ReportingIdentityId",
                        column: x => x.ReportingIdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyReports_ReportingIdentityId",
                table: "EmergencyReports",
                column: "ReportingIdentityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmergencyReports");
        }
    }
}
