using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Media;
using Sienar.Media.Hooks;
using Sienar.Media.Processors;

namespace Sienar.Infrastructure.Plugins;

internal class SienarCmsStartupPlugin : ISienarServerStartupPlugin
{
	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		var services = builder.Services;

		/************
		 * Identity *
		 ***********/

		// CRUD
		services
			// .AddTransient<IBeforeRead<SienarUser>, IncludeRolesInFilterHook>()
			.AddTransient<IAccessValidator<SienarUser>, UserIsAdminAccessValidator<SienarUser>>()
			.AddTransient<IBeforeProcess<SienarUser>, UserPasswordUpdateHook>()
			.AddTransient<IStateValidator<SienarUser>, EnsureAccountInfoUniqueValidator>()
			.AddTransient<IBeforeProcess<SienarUser>, RemoveUserRelatedEntitiesHook>()
			.AddTransient<IAfterProcess<SienarUser>, ForceDeletedAccountLogoutHook>();

		services.TryAddTransient<IFilterProcessor<SienarUser>, SienarUserFilterProcessor>();
		services.TryAddTransient<IFilterProcessor<SienarRole>, SienarRoleFilterProcessor>();
		services.TryAddTransient<IFilterProcessor<LockoutReason>, LockoutReasonFilterProcessor>();

		// Security
		services
			.AddTransient<IProcessor<LoginRequest>, LoginProcessor>()
			.AddTransient<IProcessor<LogoutRequest>, LogoutProcessor>()
			.AddTransient<IResultProcessor<PersonalDataResult>, PersonalDataProcessor>()
			.AddTransient<IProcessor<AddUserToRoleRequest>, UserRoleChangeProcessor>()
			.AddTransient<IAccessValidator<AddUserToRoleRequest>, UserIsAdminAccessValidator<AddUserToRoleRequest>>()
			.AddTransient<IProcessor<RemoveUserFromRoleRequest>, UserRoleChangeProcessor>()
			.AddTransient<IAccessValidator<RemoveUserFromRoleRequest>, UserIsAdminAccessValidator<RemoveUserFromRoleRequest>>()
			.AddTransient<IProcessor<LockUserAccountRequest>, LockUserAccountProcessor>()
			.AddTransient<IAccessValidator<LockUserAccountRequest>, UserIsAdminAccessValidator<LockUserAccountRequest>>()
			.AddTransient<IProcessor<UnlockUserAccountRequest>, UnlockUserAccountProcessor>()
			.AddTransient<IAccessValidator<UnlockUserAccountRequest>, UserIsAdminAccessValidator<UnlockUserAccountRequest>>()
			.AddTransient<IProcessor<ManuallyConfirmUserAccountRequest>, ManuallyConfirmUserAccountProcessor>()
			.AddTransient<IAccessValidator<ManuallyConfirmUserAccountRequest>, UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>>();

		// Registration
		services
			.AddTransient<IStateValidator<RegisterRequest>, RegistrationOpenValidator>()
			.AddTransient<IStateValidator<RegisterRequest>, AcceptTosValidator>()
			.AddTransient<IStateValidator<RegisterRequest>, EnsureAccountInfoUniqueValidator>()
			.AddTransient<IProcessor<RegisterRequest>, RegisterProcessor>()

			// Email
			.AddTransient<IProcessor<ConfirmAccountRequest>, ConfirmAccountProcessor>()
			.AddTransient<IProcessor<InitiateEmailChangeRequest>, InitiateEmailChangeProcessor>()
			.AddTransient<IProcessor<PerformEmailChangeRequest>, PerformEmailChangeProcessor>()

			// Password
			.AddTransient<IProcessor<ChangePasswordRequest>, ChangePasswordProcessor>()
			.AddTransient<IProcessor<ForgotPasswordRequest>, ForgotPasswordProcessor>()
			.AddTransient<IProcessor<ResetPasswordRequest>, ResetPasswordProcessor>()

			// Personal data
			.AddTransient<IBeforeProcess<DeleteAccountRequest>, RemoveUserRelatedEntitiesHook>()
			.AddTransient<IProcessor<DeleteAccountRequest>, DeleteAccountProcessor>();

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
		app.MapFallbackToPage("/dashboard/{**segment}", "/_Host");
	}
}