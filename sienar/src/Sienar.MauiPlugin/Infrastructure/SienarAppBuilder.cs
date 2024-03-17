using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Infrastructure;

public sealed class SienarAppBuilder
{
	private bool _hasRootContext;

	public Assembly AppAssembly = default!;
	public readonly MauiAppBuilder Builder;
	public readonly List<Action<MauiApp>> Middlewares = [];
	public readonly Dictionary<string, object> CustomItems = new();
	public readonly IPluginDataProvider PluginDataProvider = new PluginDataProvider();

	private SienarAppBuilder(MauiAppBuilder builder)
	{
		Builder = builder;
	}

	/// <summary>
	/// Creates a new <see cref="SienarAppBuilder"/> and registers core Sienar services on its service collection
	/// </summary>
	/// <param name="usesDefaults">whether to create the <see cref="MauiAppBuilder"/> with common defaults</param>
	/// <param name="addDebugServices">whether to register debugging services and logging</param>
	/// <returns>the new <see cref="SienarAppBuilder"/></returns>
	public static SienarAppBuilder Create(
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

		var sienarAppBuilder = new SienarAppBuilder(builder);
		sienarAppBuilder.Builder.Services.AddSingleton(sienarAppBuilder.PluginDataProvider);

		return sienarAppBuilder;
	}

	/// <summary>
	/// Registers a primary <see cref="DbContext"/> using the provided options
	/// </summary>
	/// <param name="dbContextOptionsConfigurer">an action to figure the <see cref="DbContextOptionsBuilder{TContext}"/></param>
	/// <param name="dbContextLifetime">the service lifetime of the context</param>
	/// <param name="dbContextOptionsLifetime">the service lifetime of the <see cref="DbContextOptions{TContext}"/></param>
	/// <typeparam name="TContext">the type of the <see cref="DbContext"/></typeparam>
	/// <returns>the <see cref="SienarAppBuilder"/></returns>
	public SienarAppBuilder AddRootDbContext<TContext>(
		Action<DbContextOptionsBuilder>? dbContextOptionsConfigurer = null,
		ServiceLifetime dbContextLifetime = ServiceLifetime.Scoped,
		ServiceLifetime dbContextOptionsLifetime = ServiceLifetime.Scoped)
		where TContext : DbContext
	{
		if (_hasRootContext)
		{
			throw new InvalidOperationException("You can only have one root DbContext");
		}

		_hasRootContext = true;

		Builder.Services.AddDbContext<TContext>(
			dbContextOptionsConfigurer,
			dbContextLifetime,
			dbContextOptionsLifetime);

		var baseContextDefinition = new ServiceDescriptor(
			typeof(DbContext),
			sp => sp.GetRequiredService<TContext>(),
			dbContextLifetime);

		Builder.Services.Add(baseContextDefinition);

		return this;
	}

	/// <summary>
	/// Adds an <see cref="ISienarPlugin"/> to the Sienar app
	/// </summary>
	/// <typeparam name="TPlugin">the type of the plugin to add</typeparam>
	/// <returns>the Sienar app builder</returns>
	public SienarAppBuilder AddPlugin<TPlugin>()
		where TPlugin : ISienarPlugin, new()
		=> AddPlugin(new TPlugin());

	/// <summary>
	/// Adds an instance of <see cref="ISienarPlugin"/> to the Sienar app
	/// </summary>
	/// <param name="plugin">an instance of the plugin to add</param>
	/// <returns>the Sienar app builder</returns>
	public SienarAppBuilder AddPlugin(ISienarPlugin plugin)
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
	public SienarAppBuilder SetupDependencies(Action<MauiAppBuilder> configurer)
	{
		configurer(Builder);
		return this;
	}

	/// <summary>
	/// Performs operations against the application's <see cref="MauiApp"/>
	/// </summary>
	/// <param name="configurer">an <see cref="Action{MauiApp}"/> that accepts the <see cref="MauiApp"/> as its only argument</param>
	/// <returns></returns>
	public SienarAppBuilder SetupApp(Action<MauiApp> configurer)
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
		app.ConfigureRoutableAssemblies(r => r.Add(AppAssembly));

		foreach (var middleware in Middlewares)
		{
			middleware(app);
		}

		return app;
	}
}