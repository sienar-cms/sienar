using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SiteveyorSqliteUtils
{
	public static void MigrateDb<TContext>(
		this IServiceProvider self,
		string dbPath)
		where TContext : DbContext
	{
		var backupPath = $"{dbPath}.backup";
		var enableBackup = File.Exists(dbPath);

		// Make backup of existing database
		if (enableBackup)
		{
			File.Copy(dbPath, backupPath, true);
		}

		// Perform migration
		try
		{
			using var scope = self.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
			var migrator = dbContext.Database.GetService<IMigrator>();
			migrator.Migrate();
		}
		catch (Exception e)
		{
			if (enableBackup)
			{
				File.Copy(backupPath, dbPath, true);
				File.Delete(backupPath);
			}

			throw new Exception($"Database {dbPath} failed to update", e);
		}

		// Migration was successful, so delete backup
		if (enableBackup)
		{
			File.Delete(backupPath);
		}
	}

	public static void UseSqliteDb(
		this DbContextOptionsBuilder self,
		string source)
	{
		self.UseSqlite($"Data Source={source}");
	}
}