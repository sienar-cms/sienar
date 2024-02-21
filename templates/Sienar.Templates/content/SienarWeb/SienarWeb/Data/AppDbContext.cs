using Microsoft.EntityFrameworkCore;
using Sienar;

namespace SienarWeb.Data;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}
}