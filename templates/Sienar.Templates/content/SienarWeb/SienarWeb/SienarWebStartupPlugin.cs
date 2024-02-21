using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure.Plugins;
using SienarWeb.Data;

namespace SienarWeb;

public class SienarWebStartupPlugin : ISienarServerStartupPlugin
{
	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		// Delete the next line to disable startup database migration
		app.Services.MigrateDb<AppDbContext>(DataUtils.GetDbPath());
	}
}