using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using MudBlazor;
using Sienar.Infrastructure;
using Sienar.State;

namespace Sienar.Extensions;

public static class SienarDesktopAppBuilderExtensions
{
	public static SienarDesktopAppBuilder SetApplication<TApp>(this SienarDesktopAppBuilder self)
		where TApp : class, IApplication
	{
		self.Builder.UseMauiApp<TApp>();
		self.AppAssembly = typeof(TApp).Assembly;
		return self;
	}

	public static SienarDesktopAppBuilder UseSienarApplication(
		this SienarDesktopAppBuilder self,
		Assembly appAssembly)
	{
		self.Builder.UseMauiApp<SienarMauiApp>();
		self.AppAssembly = appAssembly;
		return self;
	}

	/// <summary>
	/// Registers a primary <see cref="DbContext"/> using the provided options
	/// </summary>
	/// <param name="self">the desktop app builder</param>
	/// <param name="dbContextOptionsConfigurer">an action to figure the <see cref="DbContextOptionsBuilder{TContext}"/></param>
	/// <param name="dbContextLifetime">the service lifetime of the context</param>
	/// <param name="dbContextOptionsLifetime">the service lifetime of the <see cref="DbContextOptions{TContext}"/></param>
	/// <typeparam name="TContext">the type of the <see cref="DbContext"/></typeparam>
	/// <returns>the <see cref="SienarDesktopAppBuilder"/></returns>
	public static SienarDesktopAppBuilder AddRootDbContext<TContext>(
		this SienarDesktopAppBuilder self,
		Action<DbContextOptionsBuilder>? dbContextOptionsConfigurer = null,
		ServiceLifetime dbContextLifetime = ServiceLifetime.Scoped,
		ServiceLifetime dbContextOptionsLifetime = ServiceLifetime.Scoped)
		where TContext : DbContext
	{
		const string hasRootContextKey = "Sienar.Plugin.MauiApp.HasRootContext";
		if (self.CustomItems.TryGetValue(hasRootContextKey, out var hasRootContext)
			&& (bool)hasRootContext)
		{
			throw new InvalidOperationException("You can only have one root DbContext");
		}

		self.CustomItems[hasRootContextKey] = true;

		self.Builder.Services.AddDbContext<TContext>(
			dbContextOptionsConfigurer,
			dbContextLifetime,
			dbContextOptionsLifetime);

		var baseContextDefinition = new ServiceDescriptor(
			typeof(DbContext),
			sp => sp.GetRequiredService<TContext>(),
			dbContextLifetime);

		self.Builder.Services.Add(baseContextDefinition);

		return self;
	}

	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <typeparam name="TTheme">the type of the theme to register</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static SienarDesktopAppBuilder ConfigureTheme<TTheme>(
		this SienarDesktopAppBuilder self,
		bool isDarkMode = false)
		where TTheme : MudTheme, new()
		=> ConfigureTheme(
			self,
			new TTheme(),
			isDarkMode);

	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="theme">the <see cref="MudTheme"/> to use</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <returns>the Sienar app builder</returns>
	public static SienarDesktopAppBuilder ConfigureTheme(
		this SienarDesktopAppBuilder self,
		MudTheme theme,
		bool isDarkMode = false)
	{
		var themeState = new ThemeState
		{
			Theme = theme,
			IsDarkMode = isDarkMode
		};
		self.Builder.Services.AddScoped(_ => themeState);
		return self;
	}
}