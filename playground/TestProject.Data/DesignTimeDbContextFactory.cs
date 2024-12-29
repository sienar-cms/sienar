using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sienar;

namespace TestProject.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	/// <inheritdoc />
	public AppDbContext CreateDbContext(string[] args)
	{
		SienarUtils.SetupBaseDirectory();

		var builder = new DbContextOptionsBuilder<AppDbContext>();

		builder.UseSienarDb();

		return new AppDbContext(builder.Options);
	}
}