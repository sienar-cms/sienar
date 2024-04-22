using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;
using Sienar.Media;
using Sienar.Media.Hooks;
using Sienar.Media.Processors;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Plugin.Cms.Core</c> assembly
/// </summary>
public static class SienarPluginCmsCoreServiceCollectionExtensions
{
	/// <summary>
	/// Adds Sienar's core CMS services
	/// </summary>
	/// <param name="self">the <see cref="IServiceCollection"/></param>
	/// <param name="config">the <see cref="IConfiguration"/></param>
	/// <returns>the <see cref="IServiceCollection"/></returns>
	public static IServiceCollection AddSienarCmsCore(
		this IServiceCollection self,
		IConfiguration config)
	{
		self.RemoveService(typeof(IStatusService<>));
		self.RemoveService(typeof(IService<,>));

		self.TryAddTransient<IBotDetector, BotDetector>();
		self.TryAddTransient<IEmailSender, DefaultEmailSender>();

		self
			.AddHttpContextAccessor()
			.AddTransient(typeof(IStatusService<>), typeof(SienarStatusService<>))
			.AddScoped(typeof(IService<,>), typeof(SienarService<,>))
			.AddTransient(typeof(IStateValidator<>), typeof(ConcurrencyStampValidator<>))
			.AddTransient(typeof(IBeforeProcess<>), typeof(ConcurrencyStampUpdateHook<>));

		self.TryAddTransient<IPasswordHasher<SienarUser>, PasswordHasher<SienarUser>>();
		self.TryAddTransient<IUserClaimsFactory, UserClaimsFactory>();
		self.TryAddTransient<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();
		self.TryAddTransient<IVerificationCodeManager, VerificationCodeManager>();
		self.TryAddTransient<IUserManager, UserManager>();

		
		/************
		 * Identity *
		 ***********/

		self.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		self.TryAddTransient<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		self.TryAddTransient<IAccountEmailManager, AccountEmailManager>();
		self.TryAddTransient<IAccountUrlProvider, AccountUrlProvider>();

		// CRUD
		self
			// .AddTransient<IBeforeRead<SienarUser>, IncludeRolesInFilterHook>()
			.AddTransient<IAccessValidator<SienarUser>, UserIsAdminAccessValidator<SienarUser>>()
			.AddTransient<IBeforeProcess<SienarUser>, UserPasswordUpdateHook>()
			.AddTransient<IStateValidator<SienarUser>, EnsureAccountInfoUniqueValidator>()
			.AddTransient<IBeforeProcess<SienarUser>, RemoveUserRelatedEntitiesHook>();

		self.TryAddTransient<IFilterProcessor<SienarUser>, SienarUserFilterProcessor>();
		self.TryAddTransient<IFilterProcessor<SienarRole>, SienarRoleFilterProcessor>();
		self.TryAddTransient<IFilterProcessor<LockoutReason>, LockoutReasonFilterProcessor>();

		// Security
		self.TryAddTransient<IProcessor<LoginRequest, Guid>, LoginProcessor>();
		self.TryAddTransient<IProcessor<PerformLoginRequest, bool>, PerformLoginProcessor>();
		self.TryAddTransient<IProcessor<LogoutRequest, bool>, LogoutProcessor>();
		self.TryAddTransient<IProcessor<PersonalDataResult>, PersonalDataProcessor>();
		self.TryAddTransient<IProcessor<AddUserToRoleRequest, bool>, UserRoleChangeProcessor>();
		self.TryAddTransient<IAccessValidator<AddUserToRoleRequest>, UserIsAdminAccessValidator<AddUserToRoleRequest>>();
		self.TryAddTransient<IProcessor<RemoveUserFromRoleRequest, bool>, UserRoleChangeProcessor>();
		self.TryAddTransient<IAccessValidator<RemoveUserFromRoleRequest>, UserIsAdminAccessValidator<RemoveUserFromRoleRequest>>();
		self.TryAddTransient<IProcessor<LockUserAccountRequest, bool>, LockUserAccountProcessor>();
		self.TryAddTransient<IAccessValidator<LockUserAccountRequest>, UserIsAdminAccessValidator<LockUserAccountRequest>>();
		self.TryAddTransient<IProcessor<UnlockUserAccountRequest, bool>, UnlockUserAccountProcessor>();
		self.TryAddTransient<IAccessValidator<UnlockUserAccountRequest>, UserIsAdminAccessValidator<UnlockUserAccountRequest>>();
		self.TryAddTransient<IProcessor<ManuallyConfirmUserAccountRequest, bool>, ManuallyConfirmUserAccountProcessor>();
		self.TryAddTransient<IAccessValidator<ManuallyConfirmUserAccountRequest>, UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>>();

		// Registration
		self.TryAddTransient<IStateValidator<RegisterRequest>, RegistrationOpenValidator>();
		self.TryAddTransient<IStateValidator<RegisterRequest>, AcceptTosValidator>();
		self.TryAddTransient<IStateValidator<RegisterRequest>, EnsureAccountInfoUniqueValidator>();
		self.TryAddTransient<IProcessor<RegisterRequest, bool>, RegisterProcessor>();

		// Email
		self.TryAddTransient<IProcessor<ConfirmAccountRequest, bool>, ConfirmAccountProcessor>();
		self.TryAddTransient<IProcessor<InitiateEmailChangeRequest, bool>, InitiateEmailChangeProcessor>();
		self.TryAddTransient<IProcessor<PerformEmailChangeRequest, bool>, PerformEmailChangeProcessor>();

		// Password
		self.TryAddTransient<IProcessor<ChangePasswordRequest, bool>, ChangePasswordProcessor>();
		self.TryAddTransient<IProcessor<ForgotPasswordRequest, bool>, ForgotPasswordProcessor>();
		self.TryAddTransient<IProcessor<ResetPasswordRequest, bool>, ResetPasswordProcessor>();

		// Personal data
		self.TryAddTransient<IBeforeProcess<DeleteAccountRequest>, RemoveUserRelatedEntitiesHook>();
		self.TryAddTransient<IProcessor<DeleteAccountRequest, bool>, DeleteAccountProcessor>();

		self.AddSingleton<LoginTokenCache>();


		/********
		 * Auth *
		 *******/

		self.TryAddTransient<ISignInManager, CookieSignInManager>();

		var authorizationConfigurer = self.GetAndRemoveService<Action<AuthorizationOptions>>();
		var authenticationConfigurer = self.GetAndRemoveService<Action<AuthenticationOptions>>();
		var cookieAuthenticationConfigurer = self.GetAndRemoveService<Action<CookieAuthenticationOptions>>();

		self
			.AddAuthorization(authorizationConfigurer ?? delegate {})
			.AddAuthentication(authenticationConfigurer ?? delegate {})
			.AddCookie(
				CookieAuthenticationDefaults.AuthenticationScheme,
				o => ConfigureCookie(o, cookieAuthenticationConfigurer));


		/*********
		 * Media *
		 ********/

		self.TryAddTransient<IMediaDirectoryMapper, MediaDirectoryMapper>();
		self.TryAddTransient<IMediaManager, MediaManager>();

		self.TryAddTransient<IFilterProcessor<Upload>, UploadFilterProcessor>();

		self.TryAddTransient<IAccessValidator<Upload>, VerifyUserCanReadFileHook>();
		self.TryAddTransient<IAccessValidator<Upload>, VerifyUserCanModifyFileHook>();
		self.TryAddTransient<IAccessValidator<Upload>, VerifyUserCanModifyFileHook>();
		self.TryAddTransient<IBeforeProcess<Upload>, AssignMediaFieldsHook>();
		self.TryAddTransient<IBeforeProcess<Upload>, UploadFileHook>();


		/***********
		 * Options *
		 **********/

		self
			.ApplyDefaultConfiguration<SienarOptions>(config.GetSection("Sienar:Core"))
			.ApplyDefaultConfiguration<EmailSenderOptions>(config.GetSection("Sienar:Email:Sender"))
			.ApplyDefaultConfiguration<IdentityEmailSubjectOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.ApplyDefaultConfiguration<LoginOptions>(config.GetSection("Sienar:Login"));

		return self;
	}

	private static void ConfigureCookie(
		CookieAuthenticationOptions o,
		Action<CookieAuthenticationOptions>? configurer)
	{
		configurer?.Invoke(o);
		o.LoginPath = "/dashboard/account/login";
		o.LogoutPath = "/dashboard/account/logout";
		o.AccessDeniedPath = "/dashboard/account/forbidden";
	}
}