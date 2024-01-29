using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedLockoutReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LockoutReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockoutReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LockoutReasonSienarUser",
                columns: table => new
                {
                    LockoutReasonsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockoutReasonSienarUser", x => new { x.LockoutReasonsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_LockoutReasonSienarUser_LockoutReasons_LockoutReasonsId",
                        column: x => x.LockoutReasonsId,
                        principalTable: "LockoutReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LockoutReasonSienarUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LockoutReasons_Reason",
                table: "LockoutReasons",
                column: "Reason",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LockoutReasonSienarUser_UsersId",
                table: "LockoutReasonSienarUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LockoutReasonSienarUser");

            migrationBuilder.DropTable(
                name: "LockoutReasons");
        }
    }
}
