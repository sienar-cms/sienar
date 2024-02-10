using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Project.Data;
using Sienar.Infrastructure.Plugins;

namespace Project.App;

internal class AppStartupPlugin : ISienarServerStartupPlugin
{
	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.Services.MigrateDb<AppDbContext>(SienarDataExtensions.GetSienarDbPath());
	}
}