using Microsoft.Maui.Hosting;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarPlugin
{
	/// <summary>
	/// Plugin data for the current plugin
	/// </summary>
	PluginData PluginData { get; }

	/// <summary>
	/// Performs operations against the application's <see cref="MauiAppBuilder"/>
	/// </summary>
	/// <param name="builder">the application's underlying <see cref="MauiAppBuilder"/></param>
	void SetupDependencies(MauiAppBuilder builder) {}

	/// <summary>
	/// Performs operations against the application's <see cref="MauiApp"/>
	/// </summary>
	/// <param name="app">the application's underlying <see cref="MauiApp"/></param>
	void SetupApp(MauiApp app) {}
}