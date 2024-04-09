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
			.AddRequestConfigurer<SienarCmsRequestConfigurer>()
			.AddSienarCmsCore(builder.Configuration)
			.AddSienarCmsBlazor();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app
			.ConfigureMenu(SetupMenu)
			.ConfigureDashboard(SetupDashboard)
			.ConfigureComponents(SetupComponents);
	}

	private static void SetupMenu(IMenuProvider menuProvider)
		=> menuProvider
			.CreateMainMenu()
			.CreateInfoMenu();

	private static void SetupDashboard(IDashboardProvider dashboardProvider)
		=> dashboardProvider.CreateUserManagementDashboard();

	private static void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.DefaultLayout ??= typeof(DashboardLayout);
		componentProvider.SidebarHeader ??= typeof(DrawerHeader);
	}
}