using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Sienar.Infrastructure.Menus;
using Sienar.UI;
using Sienar.UI.Views;

namespace Sienar.Infrastructure.Plugins;

public class SienarBlazorPlugin : ISienarServerPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Blazor",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar Blazor provides all of the main services and configuration required to operate the Sienar CMS. Sienar cannot serve a Blazor app without this plugin.",
		Homepage = "https://sienar.siteveyor.com"
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		SienarUtils.SetupBaseDirectory();

		var services = builder.Services;
		var config = builder.Configuration;

		services
			.AddSienarUtilities()
			.AddSienarIdentity()
			.AddSienarMedia()
			.ConfigureSienarOptions(config)
			.ConfigureSienarBlazor()
			.ConfigureSienarBlazorAuth();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			app
				.UseExceptionHandler("/Error")
				.UseHsts();
		}

		app
			.UseStaticFiles()
			.UseRouting()
			.UseAuthorization();
		app.MapBlazorHub();
	}

	public bool PluginShouldExecute(
		HttpContext context,
		IPluginExecutionTracker executionTracker) => true;

	public void SetupMenu(IMenuProvider menuProvider) {}

	/// <inheritdoc /> 
	public void SetupDashboard(IMenuProvider dashboardProvider) {}

	public void SetupStyles(IStyleProvider styleProvider)
	{
		styleProvider
			.Add("/_content/MudBlazor/MudBlazor.min.css")
			.Add("/_content/Sienar.Blazor/sienar.css")
			.Add("/_content/Sienar.BlazorUtils/Sienar.BlazorUtils.bundle.scp.css");
	}

	public void SetupScripts(IScriptProvider scriptProvider)
	{
		scriptProvider
			.Add("/_framework/blazor.server.js")
			.Add("/_content/MudBlazor/MudBlazor.min.js")
			.Add("/_content/Sienar.Blazor/sienar.js");
	}

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.AppComponent = typeof(SienarBlazorServerApp);
		componentProvider.TopLevelComponents.Add(typeof(AuthStateRefresher));
		componentProvider.AuthorizingView = typeof(Authorizing);
		componentProvider.NotAuthorizedView = typeof(UnauthorizedRedirect);
	}

	/// <inheritdoc />
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider) {}
}