using System;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Layouts;
using Sienar.Ui;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar application to run the WASM CMS client
/// </summary>
public class CmsClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly IComponentProvider _componentProvider;
	private readonly IMenuProvider _menuProvider;
	private readonly IPluginDataProvider _pluginDataProvider;
	private readonly IRoutableAssemblyProvider _routableAssemblyProvider;
	private readonly IScriptProvider _scriptProvider;
	private readonly IStyleProvider _styleProvider;

	/// <summary>
	/// Creates a new instance of <c>CmsClientPlugin</c>
	/// </summary>
	public CmsClientPlugin(
		IApplicationAdapter adapter,
		IComponentProvider componentProvider,
		IMenuProvider menuProvider,
		IPluginDataProvider pluginDataProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IScriptProvider scriptProvider,
		IStyleProvider styleProvider)
	{
		_adapter = adapter;
		_componentProvider = componentProvider;
		_menuProvider = menuProvider;
		_pluginDataProvider = pluginDataProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_scriptProvider = scriptProvider;
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_adapter.AddServices(s => s.AddRestClient());

		SetupComponents();
		SetupMenu();
		SetupPluginData();
		SetupRoutableAssemblies();
		SetupScripts();
		SetupStyles();
	}

	private void SetupComponents()
	{
		_componentProvider.DefaultLayout ??= typeof(DashboardLayout);
		_componentProvider.SidebarHeader ??= typeof(DrawerHeader);
	}

	private void SetupMenu()
	{
		_menuProvider
			.CreateMainMenu()
			.CreateInfoMenu()
			.CreateUserManagementMenu();
	}

	private void SetupPluginData()
	{
		_pluginDataProvider.Add(new PluginData
		{
			Name = "Sienar CMS Client",
			Version = Version.Parse("0.1.0"),
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Description = "The Sienar CMS Client plugin provides all of the main services and configuration required to render the Sienar CMS user interface.",
			Homepage = "https://sienar.io"
		});
	}

	private void SetupRoutableAssemblies()
	{
		_routableAssemblyProvider.Add(typeof(CmsClientPlugin).Assembly);
	}

	private void SetupScripts()
	{
		_scriptProvider.Add("/_content/Sienar.Plugin.Cms.Client/sienar-cms.js");
	}

	private void SetupStyles()
	{
		_styleProvider.Add("/_content/Sienar.UI/sienar.css");
		_styleProvider.Add("/_content/Sienar.Ui.MudBlazor/Sienar.UI.bundle.scp.css");
	}

	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder.AddPlugin<MudBlazorPlugin>();
	}
}