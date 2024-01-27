#nullable disable

using Microsoft.EntityFrameworkCore;
using Sienar.Identity;
using Sienar.Infrastructure;

namespace Sienar;

public interface ISienarDbContext
{
	DbSet<SienarUser> Users { get; set; }
	DbSet<SienarRole> Roles { get; set; }
	DbSet<Medium> Files { get; set; }
}