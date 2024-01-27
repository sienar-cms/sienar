using Microsoft.EntityFrameworkCore;
using Sienar;

namespace Project.Data;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}
}