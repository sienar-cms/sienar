using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure.Plugins;

public class Wasm : IWasmPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Blazor WASM Plugin",
		Description = "A Sienar client packaged as a Blazor WASM plugin",
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Homepage = "https://sienar.levesque.dev",
		Version = Version.Parse("0.1.0")
	};

	/// <inheritdoc />
	public void SetupDependencies(WebAssemblyHostBuilder builder)
	{
		var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
		builder.Services.AddScoped(_ => new HttpClient { BaseAddress = baseAddress });
	}
}