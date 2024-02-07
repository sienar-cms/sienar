using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarServerPlugin : ISienarPlugin
{
	/// <summary>
	/// Performs operations against the application's <see cref="WebApplicationBuilder"/>
	/// </summary>
	/// <param name="builder">the application's underlying <see cref="WebApplicationBuilder"/></param>
	void SetupDependencies(WebApplicationBuilder builder);

	/// <summary>
	/// Determines whether the plugin should execute for the current request
	/// </summary>
	/// <param name="context">the <see cref="HttpContext"/> of the current request</param>
	/// <returns></returns>
	bool PluginShouldExecute(HttpContext context);

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplication"/>
	/// </summary>
	/// <param name="app">the application's underlying <see cref="WebApplication"/></param>
	void SetupApp(WebApplication app);
}