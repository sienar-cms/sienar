using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Html;
using Sienar.Identity;
using Sienar.Identity.Data;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Layouts;
using Sienar.Menus;
using Sienar.Ui;
using Sienar.Ui.Views;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar application to run the WASM client
/// </summary>
[AppConfigurer(typeof(SienarAppConfigurer))]
public class IdentityClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;
	private readonly IConfiguration _configuration;
	private readonly ComponentProvider _componentProvider;
	private readonly GlobalComponentProvider _globalComponentProvider;
	private readonly MenuProvider _menuProvider;
	private readonly PluginDataProvider _pluginDataProvider;
	private readonly RoutableAssemblyProvider _routableAssemblyProvider;
	private readonly StyleProvider _styleProvider;

	/// <summary>
	/// Creates a new instance of <c>CoreClientPlugin</c>
	/// </summary>
	public IdentityClientPlugin(
		IApplicationAdapter adapter,
		IConfiguration configuration,
		ComponentProvider componentProvider,
		GlobalComponentProvider globalComponentProvider,
		MenuProvider menuProvider,
		PluginDataProvider pluginDataProvider,
		RoutableAssemblyProvider routableAssemblyProvider,
		StyleProvider styleProvider)
	{
		_adapter = adapter;
		_configuration = configuration;
		_componentProvider = componentProvider;
		_globalComponentProvider = globalComponentProvider;
		_menuProvider = menuProvider;
		_pluginDataProvider = pluginDataProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		SetupComponents();
		SetupMenu();
		SetupPluginData();
		SetupRoutableAssemblies();
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
			Name = "Sienar Core Client",
			Version = Version.Parse("0.1.0"),
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Description = "The Sienar Core Client plugin provides all of the main services and configuration required to render the Sienar Core user interface.",
			Homepage = "https://sienar.io"
		});
	}

	private void SetupRoutableAssemblies()
	{
		_routableAssemblyProvider.Add(typeof(IdentityClientPlugin).Assembly);
	}

	private void SetupStyles()
	{
		_styleProvider.Add("/_content/Sienar.Ui/sienar.css");
		_styleProvider.Add("/_content/Sienar.Ui/Sienar.Ui.bundle.scp.css");
	}

	private void SetupServices()
	{
		if (_adapter.ApplicationType is not ApplicationType.Client) return;

		_adapter.AddServices(s =>
		{
			// Infrastructure
			s
				.AddCookieRestClient()
				.AddBeforeTaskHook<AddCsrfTokenToHttpRequestHook, RestClientRequest<CookieRestClient>>()
				.AddStartupTask<InitializeCsrfTokenOnAppStartHook>()
				.AddStartupTask<LoadUserDataProcessor>();

			s.TryAddScoped<INotifier, DefaultNotifier>();
			s.TryAddScoped<IUserClaimsFactory, UserClaimsFactory>();

			s
				// Account
				.TryAddProcessor<ClientLoginProcessor, LoginRequest, LoginResult>()
				.TryAddProcessor<ClientAccountLockoutProcessor, AccountLockoutRequest, AccountLockoutResult>()
				.TryAddStatusProcessor<ClientLogoutProcessor, LogoutRequest>()
				.TryAddStatusProcessor<ClientRegisterProcessor, RegisterRequest>()
				.TryAddStatusProcessor<ClientConfirmAccountProcessor, ConfirmAccountRequest>()
				.TryAddStatusProcessor<ClientInitiateEmailChangeProcessor, InitiateEmailChangeRequest>()
				.TryAddStatusProcessor<ClientPerformEmailChangeProcessor, PerformEmailChangeRequest>()
				.TryAddStatusProcessor<ClientChangePasswordProcessor, ChangePasswordRequest>()
				.TryAddStatusProcessor<ClientForgotPasswordProcessor, ForgotPasswordRequest>()
				.TryAddStatusProcessor<ClientResetPasswordProcessor, ResetPasswordRequest>()
				.TryAddStatusProcessor<ClientDeleteAccountProcessor, DeleteAccountRequest>()
				.TryAddResultProcessor<LoadUserDataProcessor, AccountDataResult>()

				// Users
				.TryAddStatusProcessor<ClientLockUserAccountProcessor, LockUserAccountRequest>()
				.TryAddStatusProcessor<ClientUnlockUserAccountProcessor, UnlockUserAccountRequest>()
				.TryAddStatusProcessor<ClientManuallyConfirmUserAccountProcessor, ManuallyConfirmUserAccountRequest>()
				.TryAddStatusProcessor<ClientAddUsertoRoleProcessor, AddUserToRoleRequest>()
				.TryAddStatusProcessor<ClientRemoveUserFromRoleProcessor, RemoveUserFromRoleRequest>()
				.AddRestfulEntity<SienarUser, UsersUrlProvider>()
				.AddRestfulEntity<SienarRole, RolesUrlProvider>()

				// Lockout reasons
				.AddRestfulEntity<LockoutReason, LockoutReasonsUrlProvider>();

			s.ApplyDefaultConfiguration<SienarOptions>(
				_configuration.GetSection("Sienar:Core"));
		});
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder
				.AddPlugin<MudBlazorPlugin>()
				.AddPlugin<CoreClientPlugin>();
		}
	}
}