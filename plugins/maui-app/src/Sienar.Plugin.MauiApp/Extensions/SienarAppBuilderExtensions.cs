using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using MudBlazor;
using Sienar.Infrastructure;
using Sienar.State;

namespace Sienar.Extensions;

public static class SienarAppBuilderExtensions
{
	public static SienarAppBuilder SetApplication<TApp>(this SienarAppBuilder self)
		where TApp : class, IApplication
	{
		self.Builder.UseMauiApp<TApp>();
		self.AppAssembly = typeof(TApp).Assembly;
		return self;
	}

	public static SienarAppBuilder UseSienarApplication(
		this SienarAppBuilder self,
		Assembly appAssembly)
	{
		self.Builder.UseMauiApp<SienarMauiApp>();
		self.AppAssembly = appAssembly;
		return self;
	}

	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <typeparam name="TTheme">the type of the theme to register</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static SienarAppBuilder ConfigureTheme<TTheme>(
		this SienarAppBuilder self,
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
	public static SienarAppBuilder ConfigureTheme(
		this SienarAppBuilder self,
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