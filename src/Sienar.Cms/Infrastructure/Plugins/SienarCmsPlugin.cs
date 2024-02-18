using System;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.UI;

namespace Sienar.Infrastructure.Plugins;

public class SienarCmsPlugin : ISienarPlugin
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IPluginExecutionTracker _executionTracker;

	public SienarCmsPlugin(
		IHttpContextAccessor contextAccessor,
		IPluginExecutionTracker executionTracker)
	{
		_contextAccessor = contextAccessor;
		_executionTracker = executionTracker;
	}

	/// <inheritdoc />
	public static Type StartupPlugin => typeof(SienarCmsStartupPlugin);

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
	public bool ShouldExecute()
		=> _executionTracker.ExecuteAsSubApp(
			_contextAccessor.HttpContext!,
			"/dashboard");

	/// <inheritdoc />
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
			.Access(DashboardMenuNames.Dashboards.UserManagement)
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

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.DefaultLayout = typeof(DashboardLayout);
		componentProvider.SidebarHeader = typeof(DrawerHeader);
	}

	/// <inheritdoc />
	public void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider)
	{
		routableAssemblyProvider.Add(typeof(SienarCmsPlugin).Assembly);
	}
}