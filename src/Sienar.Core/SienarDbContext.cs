#nullable disable

using Microsoft.EntityFrameworkCore;
using Sienar.Identity;
using Sienar.Infrastructure;

namespace Sienar;

public class SienarDbContext : DbContext, ISienarDbContext
{
	/// <inheritdoc />
	public SienarDbContext(DbContextOptions options) : base(options) {}

	/// <inheritdoc />
	public DbSet<SienarUser> Users { get; set; }

	/// <inheritdoc />
	public DbSet<SienarRole> Roles { get; set; }

	/// <inheritdoc />
	public DbSet<Medium> Files { get; set; }

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new SienarUserEntityTypeConfiguration());
	}
}