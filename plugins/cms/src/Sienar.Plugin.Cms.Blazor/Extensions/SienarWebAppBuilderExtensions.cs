using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="SienarWebAppBuilder"/> extension methods used by the <c>Sienar.Plugin.Cms.Blazor</c> assembly
/// </summary>
public static class SienarWebAppBuilderExtensions
{
	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <typeparam name="TTheme">the type of the theme to register</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static SienarWebAppBuilder ConfigureTheme<TTheme>(
		this SienarWebAppBuilder self,
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
	public static SienarWebAppBuilder ConfigureTheme(
		this SienarWebAppBuilder self,
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

	/// <summary>
	/// Builds the <see cref="SienarWebAppBuilder"/> as a Blazor United application
	/// </summary>
	/// <param name="self">the <see cref="SienarWebAppBuilder"/></param>
	/// <returns>the built <see cref="WebApplication"/></returns>
	public static WebApplication BuildBlazor(this SienarWebAppBuilder self)
	{
		var app = self.Build();

		var routableAssemblyProvider = app.Services.GetRequiredService<IRoutableAssemblyProvider>();
		app
			.MapRazorComponents<SienarApp>()
			.AddInteractiveServerRenderMode()
			.AddAdditionalAssemblies(routableAssemblyProvider.ToArray());

		return app;
	}
}