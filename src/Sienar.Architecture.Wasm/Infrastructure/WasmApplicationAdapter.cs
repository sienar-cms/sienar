using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Html;
using Sienar.Plugins;

namespace Sienar.Infrastructure;

/// <summary>
/// Maps Sienar application method calls to underlying <see cref="WebAssemblyHostBuilder"/> method calls
/// </summary>
public class WasmApplicationAdapter : IApplicationAdapter<WebAssemblyHostBuilder>
{
	/// <inheritdoc />
	public ApplicationType ApplicationType => ApplicationType.Client;

	/// <inheritdoc />
	public WebAssemblyHostBuilder Builder { get; private set; } = null!;

	/// <inheritdoc />
	public void Create(
		string[] args,
		IServiceCollection startupServices)
	{
		Builder = WebAssemblyHostBuilder.CreateDefault(args);

		startupServices
			.AddSingleton(Builder)
			.AddSingleton(Builder.HostEnvironment)
			.AddSingleton<IConfiguration>(Builder.Configuration)
			.AddSingleton<IApplicationAdapter>(this)
			.AddSingleton<IGlobalComponentProvider, GlobalComponentProvider>()
			.AddSingleton<ComponentProvider>()
			.AddSingleton<RoutableAssemblyProvider>();
	}

	/// <inheritdoc />
	public object Build(IServiceProvider sp)
	{
		Builder.Services
			.AddSingleton(sp.GetRequiredService<ComponentProvider>())
			.AddSingleton(sp.GetRequiredService<IGlobalComponentProvider>())
			.AddSingleton(sp.GetRequiredService<MenuProvider>())
			.AddSingleton(sp.GetRequiredService<PluginDataProvider>())
			.AddSingleton(sp.GetRequiredService<RoutableAssemblyProvider>())
			.AddSingleton(sp.GetRequiredService<ScriptProvider>())
			.AddSingleton(sp.GetRequiredService<StyleProvider>());

		return Builder.Build();
	}

	/// <inheritdoc />
	public void AddServices(Action<IServiceCollection> configurer)
	{
		configurer(Builder.Services);
	}
}
