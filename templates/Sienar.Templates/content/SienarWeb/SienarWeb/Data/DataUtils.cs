using Microsoft.EntityFrameworkCore;
using Sienar;

namespace SienarWeb.Data;

public static class DataUtils
{
	public static string GetDbPath() => FileUtils.GetBaseRelativePath("SienarWeb.db");

	public static void UseDb(this DbContextOptionsBuilder self)
	{
		self.UseSqlite($"Data Source={GetDbPath()}");
	}
}