using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure.Processors;
using Sienar.Media;
using Sienar.Media.Hooks;
using Sienar.Media.Processors;

namespace Sienar;

public static class SienarCmsExtensions
{
	public static IServiceCollection AddSienarCmsUtilities(this IServiceCollection self)
	{
		self.TryAddScoped<IComponentProvider>(
			sp => new ComponentProvider(typeof(DashboardLayout)));
		return self;
	}

	public static IServiceCollection AddIdentityHooks(this IServiceCollection self)
	{
		/*********
		 * Hooks *
		 ********/

		// CRUD
		self
			.AddTransient<IBeforeRead<SienarUser>, IncludeRolesInFilterHook>()
			.AddTransient<IBeforeUpsert<SienarUser>, UserPasswordUpdateHook>()
			.AddTransient<IEntityStateValidator<SienarUser>, EnsureAccountInfoUniqueHook>()
			.AddTransient<IBeforeDelete<SienarUser>, RemoveUserRelatedEntitiesHook>()
			.AddTransient<IAfterDelete<SienarUser>, ForceDeletedAccountLogoutHook>();

		self.TryAddTransient<IFilterProcessor<SienarUser>, SienarUserFilterProcessor>();
		self.TryAddTransient<IFilterProcessor<SienarRole>, SienarRoleFilterProcessor>();
		self.TryAddTransient<IFilterProcessor<LockoutReason>, LockoutReasonFilterProcessor>();

		// Security
		self
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
		self
			.AddTransient<IBeforeProcess<RegisterRequest>, RegistrationOpenHook>()
			.AddTransient<IBeforeProcess<RegisterRequest>, AcceptTosHook>()
			.AddTransient<IBeforeProcess<RegisterRequest>, EnsureAccountInfoUniqueHook>()
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

		return self;
	}

	public static IServiceCollection AddMediaHooks(this IServiceCollection self)
	{
		self.TryAddTransient<IFilterProcessor<Upload>, UploadFilterProcessor>();

		return self
			.AddTransient<IAfterRead<Upload>, VerifyUserCanReadFileHook>()
			.AddTransient<IBeforeUpsert<Upload>, AssignMediaFieldsHook>()
			.AddTransient<IBeforeUpsert<Upload>, UploadFileHook>()
			.AddTransient<IBeforeUpsert<Upload>, VerifyUserCanModifyFileHook>()
			.AddTransient<IBeforeDelete<Upload>, VerifyUserCanModifyFileHook>();
	}
}