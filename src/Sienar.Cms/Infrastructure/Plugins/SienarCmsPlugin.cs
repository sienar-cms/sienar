using System;
using Microsoft.AspNetCore.Http;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.UI;

namespace Sienar.Infrastructure.Plugins;

public class SienarCmsPlugin : ISienarPlugin
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IMenuProvider _menuProvider;
	private readonly IDashboardProvider _dashboardProvider;
	private readonly IComponentProvider _componentProvider;
	private readonly IRoutableAssemblyProvider _routableAssemblyProvider;
	private readonly IPluginExecutionTracker _executionTracker;

	public SienarCmsPlugin(
		IHttpContextAccessor contextAccessor,
		IMenuProvider menuProvider,
		IDashboardProvider dashboardProvider,
		IComponentProvider componentProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IPluginExecutionTracker executionTracker)
	{
		_contextAccessor = contextAccessor;
		_menuProvider = menuProvider;
		_dashboardProvider = dashboardProvider;
		_componentProvider = componentProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
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
	public void Execute()
	{
		SetupMenu();
		SetupDashboard();
		SetupComponents();
		SetupRoutableAssemblies();
	}

	/// <inheritdoc />
	public bool ShouldExecute()
		=> _executionTracker.ExecuteAsSubApp(
			_contextAccessor.HttpContext!,
			"/dashboard");

	private void SetupMenu()
		=> _menuProvider
			.CreateMainMenu()
			.CreateInfoMenu();

	private void SetupDashboard()
		=> _dashboardProvider.CreateUserManagementDashboard();

	private void SetupComponents()
	{
		_componentProvider.DefaultLayout = typeof(DashboardLayout);
		_componentProvider.SidebarHeader = typeof(DrawerHeader);
	}

	private void SetupRoutableAssemblies()
		=> _routableAssemblyProvider.Add(typeof(SienarCmsPlugin).Assembly);
}