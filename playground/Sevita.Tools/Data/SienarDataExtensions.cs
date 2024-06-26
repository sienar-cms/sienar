using Microsoft.EntityFrameworkCore;
using Sienar;

namespace Sevita.Tools.Data;

public static class SienarDataExtensions
{
	public static string GetSevitaDbPath()
		=> FileUtils.GetBaseRelativePath("sevita.db");

	public static void UseSevitaDb(this DbContextOptionsBuilder self)
		=> self.UseSqlite($"Data Source={GetSevitaDbPath()}");
}