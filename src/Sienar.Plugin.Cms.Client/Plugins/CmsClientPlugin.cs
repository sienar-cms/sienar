using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Html;
using Sienar.Identity;
using Sienar.Identity.Data;
using Sienar.Identity.Processors;
using Sienar.Infrastructure;
using Sienar.Layouts;
using Sienar.Ui;
using Sienar.Ui.Views;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar application to run the WASM CMS client
/// </summary>
public class CmsClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly IConfiguration _configuration;
	private readonly ComponentProvider _componentProvider;
	private readonly GlobalComponentProvider _globalComponentProvider;
	private readonly MenuProvider _menuProvider;
	private readonly PluginDataProvider _pluginDataProvider;
	private readonly RoutableAssemblyProvider _routableAssemblyProvider;
	private readonly ScriptProvider _scriptProvider;
	private readonly StyleProvider _styleProvider;

	/// <summary>
	/// Creates a new instance of <c>CmsClientPlugin</c>
	/// </summary>
	public CmsClientPlugin(
		IApplicationAdapter adapter,
		IConfiguration configuration,
		ComponentProvider componentProvider,
		GlobalComponentProvider globalComponentProvider,
		MenuProvider menuProvider,
		PluginDataProvider pluginDataProvider,
		RoutableAssemblyProvider routableAssemblyProvider,
		ScriptProvider scriptProvider,
		StyleProvider styleProvider)
	{
		_adapter = adapter;
		_configuration = configuration;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_menuProvider = menuProvider;
		_pluginDataProvider = pluginDataProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_scriptProvider = scriptProvider;
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		SetupComponents();
		SetupMenu();
		SetupPluginData();
		SetupRoutableAssemblies();
		SetupScripts();
		SetupStyles();
		SetupServices();
	}

	private void SetupComponents()
	{
		_componentProvider
			.Access(typeof(DashboardLayout))
			.TryAddComponent<DrawerHeader>(DashboardLayoutSections.SidebarHeader)
			.TryAddComponent<DrawerFooter>(DashboardLayoutSections.SidebarFooter);

		_globalComponentProvider.DefaultLayout ??= typeof(DashboardLayout);
		_globalComponentProvider.NotFoundView ??= typeof(NotFound);
		_globalComponentProvider.UnauthorizedView ??= typeof(Unauthorized);
	}

	private void SetupMenu()
	{
		_menuProvider
			.CreateMainMenu()
			.CreateUserSettingsMenu()
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
		_styleProvider.Add("/_content/Sienar.Ui/Sienar.UI.bundle.scp.css");
	}

	private void SetupServices()
	{
		if (_adapter.ApplicationType is not ApplicationType.Client) return;

		_adapter.AddServices(s =>
		{
			// Infrastructure
			s
				.AddCookieRestClient()
				.AddBeforeTaskHook<AddCsrfTokenToHttpRequestHook>()
				.AddStartupTask<InitializeCsrfTokenOnAppStartHook>()
				.AddStartupTask<LoadUserDataProcessor>();

			s.TryAddScoped<INotifier, DefaultNotifier>();
			s.TryAddScoped<IUserClaimsFactory, UserClaimsFactory>();

			s
				// Account
				.TryAddProcessor<ClientLoginProcessor>()
				.TryAddProcessor<ClientAccountLockoutProcessor>()
				.TryAddStatusProcessor<ClientLogoutProcessor>()
				.TryAddStatusProcessor<ClientRegisterProcessor>()
				.TryAddStatusProcessor<ClientConfirmAccountProcessor>()
				.TryAddStatusProcessor<ClientInitiateEmailChangeProcessor>()
				.TryAddStatusProcessor<ClientPerformEmailChangeProcessor>()
				.TryAddStatusProcessor<ClientChangePasswordProcessor>()
				.TryAddStatusProcessor<ClientForgotPasswordProcessor>()
				.TryAddStatusProcessor<ClientResetPasswordProcessor>()
				.TryAddStatusProcessor<ClientDeleteAccountProcessor>()
				.TryAddResultProcessor<LoadUserDataProcessor>()

				// Users
				.TryAddStatusProcessor<ClientLockUserAccountProcessor>()
				.TryAddStatusProcessor<ClientUnlockUserAccountProcessor>()
				.TryAddStatusProcessor<ClientManuallyConfirmUserAccountProcessor>()
				.TryAddStatusProcessor<ClientAddUsertoRoleProcessor>()
				.TryAddStatusProcessor<ClientRemoveUserFromRoleProcessor>()
				.AddRestfulEntity<SienarUser, UsersUrlProvider>()
				.AddRestfulEntity<SienarRole, RolesUrlProvider>()

				// Lockout reasons
				.AddRestfulEntity<LockoutReason, LockoutReasonsUrlProvider>();

			s.ApplyDefaultConfiguration<SienarOptions>(
				_configuration.GetSection("Sienar:Core"));
		});
	}

	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder
			.AddPlugin<MudBlazorPlugin>()
			.AddPlugin<WasmPlugin>();
	}
}