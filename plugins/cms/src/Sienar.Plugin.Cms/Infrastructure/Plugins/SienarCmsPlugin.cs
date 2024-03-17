using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Extensions;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Processors;
using Sienar.Layouts;
using Sienar.Media;
using Sienar.Media.Hooks;
using Sienar.Media.Processors;
using Sienar.UI;

namespace Sienar.Infrastructure.Plugins;

public class SienarCmsPlugin : ISienarPlugin
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
		var services = builder.Services;
		var config = builder.Configuration;

		/********
		 * Core *
		 *******/

		services
			.AddRequestConfigurer<SienarCmsRequestConfigurer>()
			.AddSienarUtilities()
			.AddSienarIdentity()
			.AddSienarMedia()
			.ConfigureSienarOptions(config)
			.ConfigureSienarBlazor()
			.ConfigureSienarAuth();

		/************
		 * Identity *
		 ***********/

		// CRUD
		services
			// .AddTransient<IBeforeRead<SienarUser>, IncludeRolesInFilterHook>()
			.AddTransient<IAccessValidator<SienarUser>, UserIsAdminAccessValidator<SienarUser>>()
			.AddTransient<IBeforeProcess<SienarUser>, UserPasswordUpdateHook>()
			.AddTransient<IStateValidator<SienarUser>, EnsureAccountInfoUniqueValidator>()
			.AddTransient<IBeforeProcess<SienarUser>, RemoveUserRelatedEntitiesHook>();

		services.TryAddTransient<IFilterProcessor<SienarUser>, SienarUserFilterProcessor>();
		services.TryAddTransient<IFilterProcessor<SienarRole>, SienarRoleFilterProcessor>();
		services.TryAddTransient<IFilterProcessor<LockoutReason>, LockoutReasonFilterProcessor>();

		// Security
		services
			.AddTransient<IProcessor<LoginRequest, Guid>, LoginProcessor>()
			.AddTransient<IProcessor<PerformLoginRequest, bool>, PerformLoginProcessor>()
			.AddTransient<IProcessor<LogoutRequest, bool>, LogoutProcessor>()
			.AddTransient<IProcessor<PersonalDataResult>, PersonalDataProcessor>()
			.AddTransient<IProcessor<AddUserToRoleRequest, bool>, UserRoleChangeProcessor>()
			.AddTransient<IAccessValidator<AddUserToRoleRequest>, UserIsAdminAccessValidator<AddUserToRoleRequest>>()
			.AddTransient<IProcessor<RemoveUserFromRoleRequest, bool>, UserRoleChangeProcessor>()
			.AddTransient<IAccessValidator<RemoveUserFromRoleRequest>, UserIsAdminAccessValidator<RemoveUserFromRoleRequest>>()
			.AddTransient<IProcessor<LockUserAccountRequest, bool>, LockUserAccountProcessor>()
			.AddTransient<IAccessValidator<LockUserAccountRequest>, UserIsAdminAccessValidator<LockUserAccountRequest>>()
			.AddTransient<IProcessor<UnlockUserAccountRequest, bool>, UnlockUserAccountProcessor>()
			.AddTransient<IAccessValidator<UnlockUserAccountRequest>, UserIsAdminAccessValidator<UnlockUserAccountRequest>>()
			.AddTransient<IProcessor<ManuallyConfirmUserAccountRequest, bool>, ManuallyConfirmUserAccountProcessor>()
			.AddTransient<IAccessValidator<ManuallyConfirmUserAccountRequest>, UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>>();

		// Registration
		services
			.AddTransient<IStateValidator<RegisterRequest>, RegistrationOpenValidator>()
			.AddTransient<IStateValidator<RegisterRequest>, AcceptTosValidator>()
			.AddTransient<IStateValidator<RegisterRequest>, EnsureAccountInfoUniqueValidator>()
			.AddTransient<IProcessor<RegisterRequest, bool>, RegisterProcessor>()

			// Email
			.AddTransient<IProcessor<ConfirmAccountRequest, bool>, ConfirmAccountProcessor>()
			.AddTransient<IProcessor<InitiateEmailChangeRequest, bool>, InitiateEmailChangeProcessor>()
			.AddTransient<IProcessor<PerformEmailChangeRequest, bool>, PerformEmailChangeProcessor>()

			// Password
			.AddTransient<IProcessor<ChangePasswordRequest, bool>, ChangePasswordProcessor>()
			.AddTransient<IProcessor<ForgotPasswordRequest, bool>, ForgotPasswordProcessor>()
			.AddTransient<IProcessor<ResetPasswordRequest, bool>, ResetPasswordProcessor>()

			// Personal data
			.AddTransient<IBeforeProcess<DeleteAccountRequest>, RemoveUserRelatedEntitiesHook>()
			.AddTransient<IProcessor<DeleteAccountRequest, bool>, DeleteAccountProcessor>();

		services.AddSingleton<LoginTokenCache>();

		/*********
		 * Media *
		 ********/

		services.TryAddTransient<IFilterProcessor<Upload>, UploadFilterProcessor>();

		services
			.AddTransient<IAccessValidator<Upload>, VerifyUserCanReadFileHook>()
			.AddTransient<IAccessValidator<Upload>, VerifyUserCanModifyFileHook>()
			.AddTransient<IAccessValidator<Upload>, VerifyUserCanModifyFileHook>()
			.AddTransient<IBeforeProcess<Upload>, AssignMediaFieldsHook>()
			.AddTransient<IBeforeProcess<Upload>, UploadFileHook>();
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
		componentProvider.DefaultLayout = typeof(DashboardLayout);
		componentProvider.SidebarHeader = typeof(DrawerHeader);
	}
}