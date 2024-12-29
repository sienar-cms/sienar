using Microsoft.EntityFrameworkCore;
using Sienar.Data;

namespace TestProject.Data;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}
}