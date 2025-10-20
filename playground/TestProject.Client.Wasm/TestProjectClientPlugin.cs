using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Layouts;
using Sienar.Plugins;
using TestProject.Client.Extensions;
using TestProject.Client.Layouts;
using TestProject.Client.UI;

namespace TestProject.Client;

public class TestProjectClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly IRoutableAssemblyProvider _routableAssemblyProvider;
	private readonly IComponentProvider _componentProvider;
	private readonly IGlobalComponentProvider _globalComponentProvider;
	private readonly IMenuProvider _menuProvider;
	private readonly StyleProvider _styleProvider;

	public TestProjectClientPlugin(
		IApplicationAdapter adapter,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IComponentProvider componentProvider,
		IGlobalComponentProvider globalComponentProvider,
		IMenuProvider menuProvider,
		StyleProvider styleProvider)
	{
		_adapter = adapter;
		_routableAssemblyProvider = routableAssemblyProvider;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_menuProvider = menuProvider;
		_styleProvider = styleProvider;
	}

	public void Configure()
	{
		_adapter.AddServices(sp => sp.AddDefaultTheme());

		_routableAssemblyProvider.Add(typeof(TestProjectClientPlugin).Assembly);
		_menuProvider.AddMenu();

		ConfigureStyles();
		ConfigureComponents();
	}

	private void ConfigureComponents()
	{
		_globalComponentProvider.DefaultLayout = typeof(MainAppLayout);
		var mainAppComponents = _componentProvider.Access(typeof(MainAppLayout));
		mainAppComponents[DashboardLayoutSections.AppbarLeft] = typeof(Branding);
	}

	private void ConfigureStyles()
	{
		_styleProvider.Add("/styles.css");
		_styleProvider.Add("/TestProject.Client.Wasm.styles.css");
	}

	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder.AddPlugin<CmsClientPlugin>();
	}
}
