using System;
using System.Collections.Generic;
using System.Linq;
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
	public readonly List<Action<IApplicationBuilder>> MiddlewareSetupFuncs = [];
	public MudTheme? Theme;
	public bool IsDarkMode;

	protected SienarServerAppBuilder(WebApplicationBuilder builder)
	{
		Builder = builder;
	}

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

		var contextServiceDefinition = builder.Builder.Services.First(
			s => s.ImplementationType == typeof(TContext));

		var baseContextDefinition = new ServiceDescriptor(
			typeof(DbContext),
			sp => sp.GetRequiredService<TContext>(),
			contextServiceDefinition.Lifetime);

		builder.Builder.Services.Add(baseContextDefinition);

		return builder;
	}

	public static SienarServerAppBuilder Create(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services
			.AddSienarBlazorUtilities()
			.AddSienarBlazorServerUtilities();

		return new SienarServerAppBuilder(builder);
	}

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

		foreach (var func in MiddlewareSetupFuncs) func(app);

		return app;
	}
}