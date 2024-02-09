using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Infrastructure;

public class SienarServerAppBuilder
{
	public readonly WebApplicationBuilder Builder;
	public readonly List<ISienarServerPlugin> Plugins = [];
	public MudTheme? Theme;
	public bool IsDarkMode;

	protected SienarServerAppBuilder(WebApplicationBuilder builder)
	{
		Builder = builder;
	}

	/// <summary>
	/// Creates a new <see cref="SienarServerAppBuilder"/> and registers a <see cref="TContext"/> using the provided options
	/// </summary>
	/// <param name="args">the runtime arguments supplied to <c>Program.Main()</c></param>
	/// <param name="dbContextOptionsConfigurer">an action to figure the <see cref="DbContextOptionsBuilder{TContext}"/></param>
	/// <param name="dbContextLifetime">the service lifetime of the <see cref="TContext"/></param>
	/// <param name="dbContextOptionsLifetime">the service lifetime of the <see cref="DbContextOptions{TContext}"/></param>
	/// <typeparam name="TContext">the type of the <see cref="DbContext"/></typeparam>
	/// <returns>the new <see cref="SienarServerAppBuilder"/></returns>
	public static SienarServerAppBuilder Create<TContext>(
		string[] args,
		Action<DbContextOptionsBuilder>? dbContextOptionsConfigurer = null,
		ServiceLifetime dbContextLifetime = ServiceLifetime.Scoped,
		ServiceLifetime dbContextOptionsLifetime = ServiceLifetime.Scoped)
		where TContext : DbContext
	{
		var builder = Create(args);

		builder.Builder.Services.AddDbContext<TContext>(
			dbContextOptionsConfigurer,
			dbContextLifetime,
			dbContextOptionsLifetime);

		var baseContextDefinition = new ServiceDescriptor(
			typeof(DbContext),
			sp => sp.GetRequiredService<TContext>(),
			dbContextLifetime);

		builder.Builder.Services.Add(baseContextDefinition);

		return builder;
	}

	/// <summary>
	/// Creates a new <see cref="SienarServerAppBuilder"/> and registers core Sienar services on its service collection
	/// </summary>
	/// <param name="args">the runtime arguments supplied to <c>Program.Main()</c></param>
	/// <returns>the new <see cref="SienarServerAppBuilder"/></returns>
	public static SienarServerAppBuilder Create(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services
			.AddSienarBlazorUtilities()
			.AddSienarBlazorServerUtilities();

		return new SienarServerAppBuilder(builder);
	}

	/// <summary>
	/// Builds the final <see cref="WebApplication"/> and returns it
	/// </summary>
	/// <returns>the new <see cref="WebApplication"/></returns>
	public virtual WebApplication Build()
	{
		// Set up remaining services on the IServiceCollection
		Theme ??= new MudTheme();
		var themeState = new ThemeState
		{
			Theme = Theme,
			IsDarkMode = IsDarkMode
		};
		Builder.Services.AddScoped(_ => themeState);

		var app = Builder.Build();

		foreach (var plugin in Plugins)
		{
			plugin.SetupApp(app);
		}

		app.UseMiddleware<SienarPluginMiddleware>();

		return app;
	}
}