using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sienar;

namespace Sevita.Tools.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	/// <inheritdoc />
	public AppDbContext CreateDbContext(string[] args)
	{
		SienarUtils.SetupBaseDirectory();

		var builder = new DbContextOptionsBuilder<AppDbContext>();

		builder.UseSevitaDb();

		return new AppDbContext(builder.Options);
	}
}