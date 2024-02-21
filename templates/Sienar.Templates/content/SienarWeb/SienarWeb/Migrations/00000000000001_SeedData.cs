using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SienarWeb.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
	{
		private const string AdministratorId = "B8889B3C-1A61-4EE1-89CD-40B6CDEEFCAB";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"INSERT INTO \"Roles\" VALUES (\"{AdministratorId}\", \"Administrator\", \"355F2719-420E-4CCF-94FA-6E01EAA1F1C8\")");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"DELETE FROM \"Roles\" WHERE \"Id\"=\"{AdministratorId}\"");
		}
    }
}
