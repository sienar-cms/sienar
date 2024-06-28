using System;
using Microsoft.AspNetCore.Builder;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.Layouts;
using Sienar.UI;

namespace Sienar.Infrastructure.Plugins;

/// <summary>
/// Adds Sienar as a Blazor United application
/// </summary>
public class SienarCmsBlazorPlugin : IWebPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar CMS",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar CMS provides all of the main services and configuration required to operate the Sienar CMS. Sienar cannot function without this plugin.",
		Homepage = "https://sienar.levesque.dev"
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		SienarUtils.SetupBaseDirectory();

		builder.Services
			.AddSienarCmsCore(builder.Configuration)
			.AddSienarCmsBlazor();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.Services
			.ConfigureMenu(SetupMenu)
			.ConfigureDashboard(SetupDashboard)
			.ConfigureComponents(SetupComponents)
			.ConfigureStyles(SetupStyles)
			.ConfigureScripts(SetupScripts)
			.ConfigureRoutableAssemblies(SetupRoutableAssemblies);
	}

	private static void SetupMenu(IMenuProvider p)
		=> p
			.CreateMainMenu()
			.CreateInfoMenu();

	private static void SetupDashboard(IDashboardProvider p)
		=> p.CreateUserManagementDashboard();

	private static void SetupComponents(IComponentProvider p)
	{
		p.DefaultLayout ??= typeof(DashboardLayout);
		p.SidebarHeader ??= typeof(DrawerHeader);
	}

	private static void SetupStyles(IStyleProvider p)
	{
		p.Add("/_content/MudBlazor/MudBlazor.min.css");
		p.Add("/_content/Sienar.UI/sienar.css");
		p.Add("/_content/Sienar.UI/Sienar.UI.bundle.scp.css");
	}

	private static void SetupScripts(IScriptProvider p)
	{
		p.Add("/_content/MudBlazor/MudBlazor.min.js");
		p.Add("/_content/Sienar.Plugin.Cms.Blazor/sienar-cms.js");
	}

	private static void SetupRoutableAssemblies(IRoutableAssemblyProvider p)
	{
		p.Add(typeof(SienarCmsBlazorPlugin).Assembly);
	}
}