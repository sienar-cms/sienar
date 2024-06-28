using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Infrastructure;

public sealed class SienarDesktopAppBuilder
{
	public Assembly AppAssembly = default!;
	public readonly MauiAppBuilder Builder;
	public readonly List<Action<MauiApp>> Middlewares = [];
	public readonly Dictionary<string, object> CustomItems = new();
	public readonly IPluginDataProvider PluginDataProvider = new PluginDataProvider();

	private SienarDesktopAppBuilder(MauiAppBuilder builder)
	{
		Builder = builder;
	}

	/// <summary>
	/// Creates a new <see cref="SienarDesktopAppBuilder"/> and registers core Sienar services on its service collection
	/// </summary>
	/// <param name="usesDefaults">whether to create the <see cref="MauiAppBuilder"/> with common defaults</param>
	/// <param name="addDebugServices">whether to register debugging services and logging</param>
	/// <returns>the new <see cref="SienarDesktopAppBuilder"/></returns>
	public static SienarDesktopAppBuilder Create(
		bool usesDefaults = true,
		bool addDebugServices = false)
	{
		var builder = MauiApp.CreateBuilder(usesDefaults);
		builder.Services.AddMauiBlazorWebView();

		if (addDebugServices)
		{
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
		}

		var sienarAppBuilder = new SienarDesktopAppBuilder(builder);
		sienarAppBuilder.Builder.Services.AddSingleton(sienarAppBuilder.PluginDataProvider);

		return sienarAppBuilder;
	}

	/// <summary>
	/// Adds an <see cref="IDesktopPlugin"/> to the Sienar app
	/// </summary>
	/// <typeparam name="TPlugin">the type of the plugin to add</typeparam>
	/// <returns>the Sienar app builder</returns>
	public SienarDesktopAppBuilder AddPlugin<TPlugin>()
		where TPlugin : IDesktopPlugin, new()
		=> AddPlugin(new TPlugin());

	/// <summary>
	/// Adds an instance of <see cref="IDesktopPlugin"/> to the Sienar app
	/// </summary>
	/// <param name="plugin">an instance of the plugin to add</param>
	/// <returns>the Sienar app builder</returns>
	public SienarDesktopAppBuilder AddPlugin(IDesktopPlugin plugin)
	{
		plugin.SetupDependencies(Builder);
		Middlewares.Add(plugin.SetupApp);
		PluginDataProvider.Add(plugin.PluginData);
		return this;
	}

	/// <summary>
	/// Performs operations against the application's <see cref="MauiAppBuilder"/>
	/// </summary>
	/// <param name="configurer">an <see cref="Action{MauiAppBuilder}"/> that accepts the <see cref="MauiAppBuilder"/> as its only argument</param>
	/// <returns>the Sienar app builder</returns>
	public SienarDesktopAppBuilder SetupDependencies(Action<MauiAppBuilder> configurer)
	{
		configurer(Builder);
		return this;
	}

	/// <summary>
	/// Performs operations against the application's <see cref="MauiApp"/>
	/// </summary>
	/// <param name="configurer">an <see cref="Action{MauiApp}"/> that accepts the <see cref="MauiApp"/> as its only argument</param>
	/// <returns></returns>
	public SienarDesktopAppBuilder SetupApp(Action<MauiApp> configurer)
	{
		Middlewares.Add(configurer);
		return this;
	}

	/// <summary>
	/// Builds the final <see cref="MauiApp"/> and returns it
	/// </summary>
	/// <returns>the new <see cref="MauiApp"/></returns>
	public MauiApp Build()
	{
		var app = Builder.Build();
		app.Services.ConfigureRoutableAssemblies(r => r.Add(AppAssembly));

		foreach (var middleware in Middlewares)
		{
			middleware(app);
		}

		return app;
	}
}