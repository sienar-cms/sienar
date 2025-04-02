﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Menus;
using Sienar.Plugins;

namespace Sienar.Infrastructure;

/// <summary>
/// The Sienar app builder, which is used to create Sienar applications
/// </summary>
public sealed class SienarAppBuilder
{
	private readonly List<Type> _plugins = [];
	private readonly List<Action<IServiceCollection>> _serviceRegistrars = [];

	/// <summary>
	/// Services that are only used at application startup for configuration by Sienar
	/// </summary>
	public readonly IServiceCollection StartupServices;

	/// <summary>
	/// The startup args from the command line, if applicable
	/// </summary>
	public readonly string[] StartupArgs;

	/// <summary>
	/// The underlying <see cref="WebApplicationBuilder"/>
	/// </summary>
	public readonly WebApplicationBuilder Builder;

	/// <summary>
	/// Creates a new <see cref="SienarAppBuilder"/> and registers core Sienar services on its startup service collection
	/// </summary>
	/// <param name="args">The runtime arguments supplied to <c>Program.Main()</c></param>
	private SienarAppBuilder(string[]? args = null)
	{
		StartupServices = new ServiceCollection();
		StartupArgs = args ?? Environment.GetCommandLineArgs();
		Builder = WebApplication.CreateBuilder(StartupArgs);

		StartupServices
			.AddSingleton(Builder)
			.AddSingleton(Builder.Environment)
			.AddSingleton<IConfiguration>(Builder.Configuration)
			.AddSingleton<IMenuProvider, MenuProvider>()
			.AddSingleton<IPluginDataProvider, PluginDataProvider>()
			.AddSingleton<IScriptProvider, ScriptProvider>()
			.AddSingleton<IStyleProvider, StyleProvider>()
			.AddSingleton<MiddlewareProvider>();

		AddPlugin<WebArchitecturePlugin>();
	}

	/// <summary>
	/// Creates a new <c>SienarAppBuilder</c>
	/// </summary>
	/// <param name="args">The runtime arguments supplied to <c>Program.Main()</c></param>
	/// <returns>The Sienar app builder</returns>
	public static SienarAppBuilder Create(string[]? args = null)
	{
		return new SienarAppBuilder(args);
	} 

	/// <summary>
	/// Adds an <see cref="IPlugin"/> to the Sienar app
	/// </summary>
	/// <typeparam name="TPlugin">The type of the plugin to add</typeparam>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder AddPlugin<TPlugin>()
		where TPlugin : class, IPlugin
	{
		var type = typeof(TPlugin);

		if (!_plugins.Contains(type))
		{
			// Call startup configurer, if defined
			var startupConfigurer = type
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.FirstOrDefault(
					m => m.GetCustomAttribute<AppConfigurerAttribute>() is not null);
			startupConfigurer?.Invoke(null, [this]);

			// Register plugin
			_plugins.Add(type);
			StartupServices.AddSingleton(type);
		}

		return this;
	}

	/// <summary>
	/// Adds services to the underlying application
	/// </summary>
	/// <param name="configurer">The <see cref="Action"/> to call against the app builder's <see cref="IServiceCollection"/></param>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder AddServices(Action<IServiceCollection> configurer)
	{
		_serviceRegistrars.Add(configurer);
		return this;
	}

	/// <summary>
	/// Adds services to the startup DI container
	/// </summary>
	/// <param name="configurer">The <see cref="Action"/> to call against the app's startup <see cref="IServiceCollection"/></param>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder AddStartupServices(Action<IServiceCollection> configurer)
	{
		configurer(StartupServices);
		return this;
	}

	/// <summary>
	/// Builds the final application and returns it
	/// </summary>
	/// <returns>The new application</returns>
	public WebApplication Build()
	{
		Builder.Services.AddSienarCoreUtilities();

		foreach (var registrar in _serviceRegistrars)
		{
			registrar(Builder.Services);
		}

		var container = StartupServices.BuildServiceProvider();
		using var scope = container.CreateScope();
		var sp = scope.ServiceProvider;

		foreach (var pluginType in _plugins)
		{
			var plugin = (IPlugin)sp.GetRequiredService(pluginType);
			plugin.Configure();
		}

		Builder.Services
			.AddSingleton(
				sp.GetRequiredService<IMenuProvider>())
			.AddSingleton(
				sp.GetRequiredService<IPluginDataProvider>())
			.AddSingleton(
				sp.GetRequiredService<IScriptProvider>())
			.AddSingleton(
				sp.GetRequiredService<IStyleProvider>());

		var app = Builder.Build();

		var middlewareProvider = sp.GetRequiredService<MiddlewareProvider>();

		foreach (var middleware in middlewareProvider.AggregatePrioritized())
		{
			middleware(app);
		}

		return app;
	}
}
