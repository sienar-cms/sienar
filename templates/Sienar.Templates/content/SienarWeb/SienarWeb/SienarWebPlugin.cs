using System;
using Sienar;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace SienarWeb;

public class SienarWebPlugin : ISienarPlugin
{
	private readonly IStyleProvider _styleProvider;
	private readonly IComponentProvider _componentProvider;
	private readonly IMenuProvider _menuProvider;
	private readonly IRoutableAssemblyProvider _routableAssemblyProvider;
	private readonly IPluginExecutionTracker _executionTracker;

	public SienarWebPlugin(
		IStyleProvider styleProvider,
		IComponentProvider componentProvider,
		IMenuProvider menuProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IPluginExecutionTracker executionTracker)
	{
		_styleProvider = styleProvider;
		_componentProvider = componentProvider;
		_menuProvider = menuProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_executionTracker = executionTracker;
	}

	/// <inheritdoc />
	public static Type StartupPlugin => typeof(SienarWebStartupPlugin);

	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "App plugin",
		Author = string.Empty,
		AuthorUrl = string.Empty,
		Version = Version.Parse("1.0.0"),
		Description = string.Empty
	};

	/// <inheritdoc />
	public void Execute()
	{
		SetupStyles();
		SetupComponents();
		SetupMenu();
		SetupRoutableAssemblies();
	}

	/// <inheritdoc />
	public bool ShouldExecute()
	{
		if (!_executionTracker.SubAppHasExecuted)
		{
			_executionTracker.ClaimExecution();
			return true;
		}

		return false;
	}

	private void SetupStyles()
		=> _styleProvider.Add("/css/site.css");

	private void SetupComponents()
	{
		_componentProvider.DefaultLayout = typeof(SienarWebDefaultLayout);
	}

	private void SetupMenu()
	{
		_menuProvider
			.Access(SienarWebMenuNames.MainMenu)
			.AddLink(
				new()
				{
					Text = "Home",
					Url = "/"
				})
			.AddLink(
				new()
				{
					Text = "Register",
					Url = DashboardUrls.Account.Register.Index,
					RequireLoggedOut = true
				},
				MenuPriority.Lowest)
			.AddLink(
				new()
				{
					Text = "Log in",
					Url = DashboardUrls.Account.Login,
					RequireLoggedOut = true
				},
				MenuPriority.Lowest)
			.AddLink(
				new()
				{
					Text = "Log out",
					Url = DashboardUrls.Account.Logout,
					RequireLoggedIn = true
				});
	}

	private void SetupRoutableAssemblies()
		=> _routableAssemblyProvider.Add(typeof(SienarWebPlugin).Assembly);
}