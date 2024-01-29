#nullable disable

using Microsoft.EntityFrameworkCore;
using Sienar.Identity;
using Sienar.Infrastructure;
using Sienar.Media;

namespace Sienar;

public interface ISienarDbContext
{
	DbSet<SienarUser> Users { get; set; }
	DbSet<SienarRole> Roles { get; set; }
	DbSet<Upload> Files { get; set; }
}