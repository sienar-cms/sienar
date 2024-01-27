using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "States");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Abbreviation = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_States_Abbreviation",
                table: "States",
                column: "Abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_Name",
                table: "States",
                column: "Name",
                unique: true);
        }
    }
}
