using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.UI;

namespace Sienar.Infrastructure.Plugins;

public class SienarCmsPlugin : ISienarServerPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar CMS",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar CMS provides pages and hooks that allow users to manage their account and media. Sienar itself can operate without this plugin, but core functionality like logging in won't work.",
		Homepage = "https://sienar.siteveyor.com"
	};

	/// <inheritdoc />
	public PluginSettings PluginSettings { get; } = new()
	{
		HasRoutableComponents = true,
		UsesProviders = true
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		builder.Services
			.AddSienarCmsUtilities()
			.AddIdentityHooks()
			.AddMediaHooks();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.MapFallbackToPage("/dashboard/{**segment}", "/_Host");
	}

	public bool PluginShouldExecute(HttpContext context)
		=> context.Request.Path.StartsWithSegments("/dashboard");

	public void SetupMenu(IMenuProvider menuProvider)
	{
		menuProvider
			.CreateMainMenu()
			.CreateInfoMenu();
	}

	/// <inheritdoc />
	public void SetupDashboard(IMenuProvider dashboardProvider)
	{
		dashboardProvider
			.Access(DashboardSectionNames.UserManagement)
			.AddMenuLink(
				new()
				{
					Text = "Users",
					Icon = Icons.Material.Filled.SupervisorAccount,
					Url = DashboardUrls.Users.Index,
					Roles = [Roles.Admin]
				})
			.AddMenuLink(new()
				{
					Text = "Lockout reasons",
					Icon = Icons.Material.Filled.Lock,
					Url = DashboardUrls.LockoutReasons.Index,
					Roles = [Roles.Admin]
				});
	}

	public void SetupStyles(IStyleProvider styleProvider) {}

	public void SetupScripts(IScriptProvider scriptProvider) {}

	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.DefaultLayout = typeof(DashboardLayout);
		componentProvider.SidebarHeader = typeof(DrawerHeader);
	}
}