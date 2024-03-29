﻿using Microsoft.EntityFrameworkCore;
using Sienar.Identity;
using Sienar.Infrastructure;
using Sienar.Media;

namespace Sienar;

public interface ISienarDbContext
{
	DbSet<SienarUser> Users { get; set; }
	DbSet<SienarRole> Roles { get; set; }
	DbSet<LockoutReason> LockoutReasons { get; set; }
	DbSet<Upload> Files { get; set; }
}