using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Html;
using Sienar.Menus;
using Sienar.Plugins;

namespace Sienar.Infrastructure;

/// <summary>
/// The Sienar app builder, which is used to create Sienar applications
/// </summary>
public sealed class SienarAppBuilder
{
	private readonly List<Type> _plugins = [];
	private readonly IServiceCollection _startupServices;
	private IApplicationAdapter? _adapter;
	private readonly string[] _startupArgs;

	/// <summary>
	/// Creates a new <see cref="SienarAppBuilder"/> and registers core Sienar services on its startup service collection
	/// </summary>
	/// <param name="args">The runtime arguments supplied to <c>Program.Main()</c></param>
	private SienarAppBuilder(string[]? args = null)
	{
		_startupServices = new ServiceCollection();
		_startupArgs = args ?? Environment.GetCommandLineArgs();

		_startupServices
			.AddSingleton<MenuProvider>()
			.AddSingleton<PluginDataProvider>()
			.AddSingleton<ScriptProvider>()
			.AddSingleton<StyleProvider>();
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
			var startupConfigurers = type.GetCustomAttributes<AppConfigurerAttribute>();

			// Call startup configurers, if defined
			foreach (var c in startupConfigurers)
			{
				var configurer = (IConfigurer<SienarAppBuilder>)Activator.CreateInstance(c.Configurer)!;
				configurer.Configure(this);
			}

			// Register plugin
			_plugins.Add(type);
			_startupServices.AddSingleton(type);
		}

		return this;
	}

	/// <summary>
	/// Adds services to the startup DI container
	/// </summary>
	/// <param name="configurer">The <see cref="Action"/> to call against the app's startup <see cref="IServiceCollection"/></param>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder AddStartupServices(Action<IServiceCollection> configurer)
	{
		configurer(_startupServices);
		return this;
	}

	/// <summary>
	/// Sets the application adapter
	/// </summary>
	/// <param name="adapter">The application adapter</param>
	/// <returns>The Sienar app builder</returns>
	public SienarAppBuilder SetApplicationAdapter(IApplicationAdapter adapter)
	{
		_adapter = adapter;
		return this;
	}

	/// <summary>
	/// Builds the final application and returns it
	/// </summary>
	/// <returns>The new application</returns>
	public TApp Build<TApp>()
	{
		if (_adapter is null)
		{
			throw new InvalidOperationException($"You must register an {nameof(IApplicationAdapter)} before calling {nameof(Build)}.");
		}

		_adapter.Create(_startupArgs, _startupServices);
		_adapter.AddServices(s => s.AddSienarCoreUtilities());

		var container = _startupServices.BuildServiceProvider();
		using var scope = container.CreateScope();
		var sp = scope.ServiceProvider;

		foreach (var pluginType in _plugins)
		{
			var plugin = (IPlugin)sp.GetRequiredService(pluginType);
			plugin.Configure();
		}

		_adapter.AddServices(services =>
		{
			services
				.AddSingleton(sp.GetRequiredService<MenuProvider>())
				.AddSingleton(sp.GetRequiredService<PluginDataProvider>())
				.AddSingleton(sp.GetRequiredService<ScriptProvider>())
				.AddSingleton(sp.GetRequiredService<StyleProvider>());
		});

		return (TApp)_adapter.Build(sp);
	}
}
