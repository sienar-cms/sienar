using Microsoft.EntityFrameworkCore;
using Sienar;

namespace TestProject.Data;

public static class SienarDataExtensions
{
	public static string GetSienarDbPath()
		=> FileUtils.GetBaseRelativePath("sienar.db");

	public static void UseSienarDb(this DbContextOptionsBuilder self)
		=> self.UseSqlite($"Data Source={GetSienarDbPath()}");
}