using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_LockoutReasons_Reason",
                table: "LockoutReasons");

			// Add "Normalized____" columns
            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedPendingEmail",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUsername",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedReason",
                table: "LockoutReasons",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

			// Map values
			migrationBuilder.Sql(@"
				UPDATE Users
				SET NormalizedEmail = upper(Email)
			");

			migrationBuilder.Sql(@"
				UPDATE Users
				SET NormalizedPendingEmail = upper(PendingEmail)
				WHERE PendingEmail IS NOT NULL
			");

			migrationBuilder.Sql(@"
				UPDATE Users
				SET NormalizedUsername = upper(Username)
			");

			migrationBuilder.Sql(@"
				UPDATE Roles
				Set NormalizedName = upper(Name)
			");

			migrationBuilder.Sql(@"
				UPDATE LockoutReasons
				SET NormalizedReason = upper(Reason)
			");

			// Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUsername",
                table: "Users",
                column: "NormalizedUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LockoutReasons_NormalizedReason",
                table: "LockoutReasons",
                column: "NormalizedReason",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_NormalizedUsername",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_LockoutReasons_NormalizedReason",
                table: "LockoutReasons");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedPendingEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "NormalizedReason",
                table: "LockoutReasons");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LockoutReasons_Reason",
                table: "LockoutReasons",
                column: "Reason",
                unique: true);
        }
    }
}
