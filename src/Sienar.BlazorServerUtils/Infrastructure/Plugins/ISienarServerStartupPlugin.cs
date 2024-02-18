using Microsoft.AspNetCore.Builder;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarServerStartupPlugin
{
	/// <summary>
	/// Optionally adds plugin data to the Sienar app
	/// </summary>
	/// <remarks>
	/// This property is only used when directly adding a startup plugin to the app builder. When adding a plugin using the <c>SienarServerAppBuilder.AddPlugin()</c> method, the <see cref="ISienarPlugin"/>'s plugin data is used instead. 
	/// </remarks>
	PluginData? PluginData => null;

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplicationBuilder"/>
	/// </summary>
	/// <param name="builder">the application's underlying <see cref="WebApplicationBuilder"/></param>
	void SetupDependencies(WebApplicationBuilder builder) {}

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplication"/>
	/// </summary>
	/// <param name="app">the application's underlying <see cref="WebApplication"/></param>
	void SetupApp(WebApplication app) {}
}