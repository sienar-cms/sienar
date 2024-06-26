using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Infrastructure;

public sealed class SienarWebAppBuilder
{
	private bool _hasRootContext;

	public readonly WebApplicationBuilder Builder;
	public readonly Assembly? AppAssembly;
	public readonly List<Action<WebApplication>> Middlewares = [];
	public readonly Dictionary<string, object> CustomItems = new();
	public readonly IPluginDataProvider PluginDataProvider;
	public string[] StartupArgs = Array.Empty<string>();

	private SienarWebAppBuilder(
		WebApplicationBuilder builder,
		Assembly? appAssembly)
	{
		Builder = builder;
		AppAssembly = appAssembly;
		PluginDataProvider = new PluginDataProvider();
		Builder.Services.AddSingleton(PluginDataProvider);
	}

	/// <summary>
	/// Creates a new <see cref="SienarWebAppBuilder"/> and registers core Sienar services on its service collection
	/// </summary>
	/// <param name="args">the runtime arguments supplied to <c>Program.Main()</c></param>
	/// <param name="appAssembly">the <see cref="Assembly"/> of the main program, used to map its routable components at startup</param>
	/// <returns>the new <see cref="SienarWebAppBuilder"/></returns>
	public static SienarWebAppBuilder Create(
		string[] args,
		Assembly? appAssembly = null)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddSienarCoreUtilities();

		return new SienarWebAppBuilder(builder, appAssembly) { StartupArgs = args };
	}

	/// <summary>
	/// Adds an <see cref="IWebPlugin"/> to the Sienar app
	/// </summary>
	/// <typeparam name="TPlugin">the type of the plugin to add</typeparam>
	/// <returns>the Sienar app builder</returns>
	public SienarWebAppBuilder AddPlugin<TPlugin>()
		where TPlugin : IWebPlugin, new()
		=> AddPlugin(new TPlugin());

	/// <summary>
	/// Adds an instance of <see cref="IWebPlugin"/> to the Sienar app
	/// </summary>
	/// <param name="plugin">an instance of the plugin to add</param>
	/// <returns>the Sienar app builder</returns>
	public SienarWebAppBuilder AddPlugin(IWebPlugin plugin)
	{
		plugin.SetupDependencies(Builder);
		Middlewares.Add(plugin.SetupApp);
		PluginDataProvider.Add(plugin.PluginData);
		return this;
	}

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplicationBuilder"/>
	/// </summary>
	/// <param name="configurer">an <see cref="Action{WebApplicationBuilder}"/> that accepts the <see cref="WebApplicationBuilder"/> as its only argument</param>
	/// <returns>the Sienar app builder</returns>
	public SienarWebAppBuilder SetupDependencies(Action<WebApplicationBuilder> configurer)
	{
		configurer(Builder);
		return this;
	}

	/// <summary>
	/// Performs operations against the application's <see cref="WebApplication"/>
	/// </summary>
	/// <param name="configurer">an <see cref="Action{WebApplication}"/> that accepts the <see cref="WebApplication"/> as its only argument</param>
	/// <returns>the Sienar app builder</returns>
	public SienarWebAppBuilder SetupApp(Action<WebApplication> configurer)
	{
		Middlewares.Add(configurer);
		return this;
	}

	/// <summary>
	/// Builds the final <see cref="WebApplication"/> and returns it
	/// </summary>
	/// <returns>the new <see cref="WebApplication"/></returns>
	public WebApplication Build()
	{
		var app = Builder.Build();

		if (!app.Environment.IsDevelopment())
		{
			app
				.UseExceptionHandler("/Error")
				.UseHsts();
		}

		app
			.UseStaticFiles()
			.UseAuthentication()
			.UseAuthorization();

		if (AppAssembly is not null)
		{
			app.ConfigureRoutableAssemblies(p => p.Add(AppAssembly));
		}

		foreach (var middleware in Middlewares)
		{
			middleware(app);
		}

		return app;
	}
}