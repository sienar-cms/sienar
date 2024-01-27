using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCode_Users_AppUserId",
                table: "VerificationCode");

            migrationBuilder.DropTable(
                name: "AppUserSienarRole");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "VerificationCode",
                newName: "SienarUserId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCode_AppUserId",
                table: "VerificationCode",
                newName: "IX_VerificationCode_SienarUserId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Files",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Files",
                type: "TEXT",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SienarRoleSienarUser",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SienarUserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SienarRoleSienarUser", x => new { x.RolesId, x.SienarUserId });
                    table.ForeignKey(
                        name: "FK_SienarRoleSienarUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SienarRoleSienarUser_Users_SienarUserId",
                        column: x => x.SienarUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SienarRoleSienarUser_SienarUserId",
                table: "SienarRoleSienarUser",
                column: "SienarUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCode_Users_SienarUserId",
                table: "VerificationCode",
                column: "SienarUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCode_Users_SienarUserId",
                table: "VerificationCode");

            migrationBuilder.DropTable(
                name: "SienarRoleSienarUser");

            migrationBuilder.RenameColumn(
                name: "SienarUserId",
                table: "VerificationCode",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCode_SienarUserId",
                table: "VerificationCode",
                newName: "IX_VerificationCode_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Files",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Files",
                type: "TEXT",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 300);

            migrationBuilder.CreateTable(
                name: "AppUserSienarRole",
                columns: table => new
                {
                    AppUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RolesId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserSienarRole", x => new { x.AppUserId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_AppUserSienarRole_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserSienarRole_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserSienarRole_RolesId",
                table: "AppUserSienarRole",
                column: "RolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCode_Users_AppUserId",
                table: "VerificationCode",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
