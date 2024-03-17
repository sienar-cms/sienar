using Microsoft.EntityFrameworkCore;
using Sienar;

namespace TestProject.Web.Data;

public class AppDbContext : SienarDbContext
{
	/// <inheritdoc />
	public AppDbContext(DbContextOptions options)
		: base(options) {}
}