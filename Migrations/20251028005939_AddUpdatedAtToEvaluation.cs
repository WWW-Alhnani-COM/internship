using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternshipManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_CreatedAt",
                table: "ActivityLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Evaluations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Evaluations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_UserId",
                table: "Evaluations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Users_UserId",
                table: "Evaluations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Users_UserId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_UserId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Evaluations");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_CreatedAt",
                table: "ActivityLogs",
                column: "CreatedAt");
        }
    }
}
