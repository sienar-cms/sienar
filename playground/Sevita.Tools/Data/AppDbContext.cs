using Microsoft.EntityFrameworkCore;
using Sienar;

namespace Sevita.Tools.Data;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}
}