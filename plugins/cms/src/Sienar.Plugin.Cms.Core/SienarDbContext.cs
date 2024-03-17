#nullable disable
// ReSharper disable NotNullOrRequiredMemberIsNotInitialized

using Microsoft.EntityFrameworkCore;
using Sienar.Identity;
using Sienar.Media;

namespace Sienar;

public class SienarDbContext : DbContext, ISienarDbContext
{
	/// <inheritdoc />
	public SienarDbContext() {}

	/// <inheritdoc />
	public SienarDbContext(DbContextOptions options) : base(options) {}

	/// <inheritdoc />
	public DbSet<SienarUser> Users { get; set; }

	/// <inheritdoc />
	public DbSet<SienarRole> Roles { get; set; }

	/// <inheritdoc />
	public DbSet<LockoutReason> LockoutReasons { get; set; }

	/// <inheritdoc />
	public DbSet<Upload> Files { get; set; }
}