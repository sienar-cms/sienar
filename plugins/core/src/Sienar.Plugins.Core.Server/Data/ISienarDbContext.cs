using Microsoft.EntityFrameworkCore;
using Sienar.Identity;

namespace Sienar.Data;

public interface ISienarDbContext
{
	DbSet<SienarUser> Users { get; set; }
	DbSet<SienarRole> Roles { get; set; }
	DbSet<LockoutReason> LockoutReasons { get; set; }
}