using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor;
using MudBlazor.Services;
using Sienar.Configuration;
using Sienar.Infrastructure;
using Sienar.Email;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;
using Sienar.Infrastructure.Processors;
using Sienar.Media;
using Sienar.Media.Hooks;
using Sienar.Media.Processors;

namespace Sienar;

public static class SienarBlazorExtensions
{
	public static IServiceCollection AddSienarUtilities(this IServiceCollection self)
	{
		self.RemoveService(typeof(IService<>));

		self.TryAddTransient<IBotDetector, BotDetector>();
		self.TryAddTransient<IEmailSender, DefaultEmailSender>();

		return self
			.AddTransient(typeof(IService<>), typeof(SienarService<>))
			.AddTransient(typeof(IEntityStateValidator<>), typeof(ConcurrencyStampValidatorHook<>))
			.AddTransient(typeof(IBeforeUpsert<>), typeof(ConcurrencyStampUpdateHook<>));
	}

	public static IServiceCollection AddSienarIdentity(this IServiceCollection self)
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

		/*********
		 * Other *
		 ********/

		self.TryAddTransient<IPasswordHasher<SienarUser>, PasswordHasher<SienarUser>>();
		self.TryAddTransient<IUserClaimsFactory, UserClaimsFactory>();
		self.TryAddTransient<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();
		self.TryAddTransient<IVerificationCodeManager, VerificationCodeManager>();
		self.TryAddTransient<IUserManager, UserManager>();

		self.TryAddScoped<IUserAccessor, BlazorUserAccessor>();
		self.TryAddTransient<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		self.TryAddTransient<IAccountEmailManager, AccountEmailManager>();
		self.TryAddTransient<IAccountUrlProvider, AccountUrlProvider>();

		return self;
	}

	public static IServiceCollection AddSienarMedia(this IServiceCollection self)
	{
		self.TryAddTransient<IFilterProcessor<Upload>, UploadFilterProcessor>();
		self.TryAddTransient<IMediaDirectoryMapper, MediaDirectoryMapper>();
		self.TryAddTransient<IMediaManager, MediaManager>();

		return self
			.AddTransient<IAfterRead<Upload>, VerifyUserCanReadFileHook>()
			.AddTransient<IBeforeUpsert<Upload>, AssignMediaFieldsHook>()
			.AddTransient<IBeforeUpsert<Upload>, UploadFileHook>()
			.AddTransient<IBeforeUpsert<Upload>, VerifyUserCanModifyFileHook>()
			.AddTransient<IBeforeDelete<Upload>, VerifyUserCanModifyFileHook>();
	}

	public static IServiceCollection ConfigureSienarOptions(
		this IServiceCollection self,
		IConfiguration config)
	{
		self
			.Configure<SienarOptions>(config.GetSection("Sienar:Core"))
			.Configure<EmailOptions>(config.GetSection("Sienar:Email:Core"))
			.Configure<IdentityEmailOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.Configure<LoginOptions>(config.GetSection("Sienar:Login"))
			.Configure<SiteOptions>(config.GetSection("Sienar:Site"));

		return self;
	}

	public static IServiceCollection ConfigureSienarBlazor(this IServiceCollection self)
	{
		self.TryAddSingleton<MudTheme, SienarTheme>();

		var razorPagesConfigurer = self.GetAndRemoveService<Action<RazorPagesOptions>>();
		var circuitOptions = self.GetAndRemoveService<Action<CircuitOptions>>();
		var mudblazorConfigurer = self.GetAndRemoveService<Action<MudServicesConfiguration>>();

		self.AddRazorPages(razorPagesConfigurer ?? delegate {});
		self.AddServerSideBlazor(circuitOptions ?? delegate {});
		return self.AddMudServices(mudblazorConfigurer ?? delegate {});
	}

	public static IServiceCollection ConfigureSienarBlazorAuth(this IServiceCollection self)
	{
		self.TryAddTransient<ISignInManager, BlazorServerSignInManager>();
		self.TryAddTransient<IBlazorServerSignInManager>(
			sp => (IBlazorServerSignInManager)sp.GetRequiredService<ISignInManager>());
		self.TryAddSingleton<IForcedLogoutNotifier, BlazorForcedLogoutNotifier>();
		self.TryAddTransient<IBlazorLoginDataManager, BlazorLoginDataManager>();

		self.RemoveService<AuthenticationStateProvider>();

		self.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
		self.AddScoped<AuthStateProvider>(
			sp => (AuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
		self.AddScoped<AccountStateProvider>();

		var authorizationConfigurer = self.GetAndRemoveService<Action<AuthorizationOptions>>();
		var authenticationConfigurer = self.GetAndRemoveService<Action<AuthenticationOptions>>();

		self
			.AddAuthorization(authorizationConfigurer ?? delegate {})
			.AddAuthentication(authenticationConfigurer ?? delegate {});

		return self;
	}
}