using Microsoft.EntityFrameworkCore;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Identity;
using Sienar.Media;

namespace TestProject.Data;

public class AppDbContext : DbContext, ISienarDbContext
{
	public DbSet<SienarUser> Users { get; set; }
	public DbSet<SienarRole> Roles { get; set; }
	public DbSet<LockoutReason> LockoutReasons { get; set; }
	public DbSet<Upload> Files { get; set; }

	public AppDbContext(DbContextOptions options)
		: base(options) {}

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.AddSienarCmsModels();
	}
}