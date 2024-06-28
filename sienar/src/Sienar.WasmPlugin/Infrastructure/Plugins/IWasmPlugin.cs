using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Sienar.Infrastructure.Plugins;

/// <summary>
/// A plugin for a Sienar WebAssembly client
/// </summary>
public interface IWasmPlugin
{
	/// <summary>
	/// Plugin data for the current plugin
	/// </summary>
	PluginData PluginData { get; }

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplicationBuilder"/>
	/// </summary>
	/// <param name="builder">the application's underlying <see cref="WebAssemblyHostBuilder"/></param>
	void SetupDependencies(WebAssemblyHostBuilder builder) {}

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplication"/>
	/// </summary>
	/// <param name="app">the application's underlying <see cref="WebAssemblyHost"/></param>
	void SetupApp(WebAssemblyHost app) {}
}