using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Infrastructure;

public class SienarWebAppBuilder
{
	public readonly WebApplicationBuilder Builder;
	public readonly List<ISienarPlugin> Plugins = [];
	public MudTheme? Theme;
	public bool IsDarkMode;

	protected SienarWebAppBuilder(WebApplicationBuilder builder)
	{
		Builder = builder;
	}

	public static SienarWebAppBuilder Create<TContext>(
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

	public static SienarWebAppBuilder Create(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		return new SienarWebAppBuilder(builder);
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
		Builder.Services.AddSingleton(themeState);

		var app = Builder.Build();

		foreach (var plugin in Plugins)
		{
			plugin.SetupApp(app);
		}

		return app;
	}
}