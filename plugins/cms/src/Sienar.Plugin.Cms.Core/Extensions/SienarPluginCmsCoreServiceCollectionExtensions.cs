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
		self
			.ReplaceService(
				typeof(IStatusService<>),
				typeof(StatusService<>),
				typeof(SienarStatusService<>))
			.ReplaceService(
				typeof(IService<,>),
				typeof(Service<,>),
				typeof(SienarService<,>))
			.AddHttpContextAccessor();

		self.TryAddScoped<IBotDetector, BotDetector>();
		self.TryAddScoped<IEmailSender, DefaultEmailSender>();
		self.TryAddScoped<IPasswordHasher<SienarUser>, PasswordHasher<SienarUser>>();
		self.TryAddScoped<IUserClaimsFactory, UserClaimsFactory>();
		self.TryAddScoped<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();
		self.TryAddScoped<IVerificationCodeManager, VerificationCodeManager>();
		self.TryAddScoped<IUserManager, UserManager>();


		/************
		 * Identity *
		 ***********/

		self.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		self.TryAddScoped<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		self.TryAddScoped<IAccountEmailManager, AccountEmailManager>();
		self.TryAddScoped<IAccountUrlProvider, AccountUrlProvider>();

		// CRUD
		self
			.AddAccessValidator<SienarUser, UserIsAdminAccessValidator<SienarUser>>()
			.AddBeforeHook<SienarUser, UserPasswordUpdateHook>()
			.AddStateValidator<SienarUser, EnsureAccountInfoUniqueValidator>()
			.AddBeforeHook<SienarUser, RemoveUserRelatedEntitiesHook>()
			.AddEntityFrameworkEntity<SienarUser, SienarUserFilterProcessor>()
			.AddEntityFrameworkEntity<SienarRole, SienarRoleFilterProcessor>()
			.AddEntityFrameworkEntity<LockoutReason, LockoutReasonFilterProcessor>()

		// Security
			.AddProcessor<LoginRequest, Guid, LoginProcessor>()
			.AddStatusProcessor<PerformLoginRequest, PerformLoginProcessor>()
			.AddStatusProcessor<LogoutRequest, LogoutProcessor>()
			.AddResultProcessor<PersonalDataResult, PersonalDataProcessor>()
			.AddStatusProcessor<AddUserToRoleRequest, UserRoleChangeProcessor>()
			.AddAccessValidator<AddUserToRoleRequest, UserIsAdminAccessValidator<AddUserToRoleRequest>>()
			.AddStatusProcessor<RemoveUserFromRoleRequest, UserRoleChangeProcessor>()
			.AddAccessValidator<RemoveUserFromRoleRequest, UserIsAdminAccessValidator<RemoveUserFromRoleRequest>>()
			.AddStatusProcessor<LockUserAccountRequest, LockUserAccountProcessor>()
			.AddAccessValidator<LockUserAccountRequest, UserIsAdminAccessValidator<LockUserAccountRequest>>()
			.AddStatusProcessor<UnlockUserAccountRequest, UnlockUserAccountProcessor>()
			.AddAccessValidator<UnlockUserAccountRequest, UserIsAdminAccessValidator<UnlockUserAccountRequest>>()
			.AddStatusProcessor<ManuallyConfirmUserAccountRequest, ManuallyConfirmUserAccountProcessor>()
			.AddAccessValidator<ManuallyConfirmUserAccountRequest, UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>>()
			.AddSingleton<LoginTokenCache>()

		// Registration
			.AddStateValidator<RegisterRequest, RegistrationOpenValidator>()
			.AddStateValidator<RegisterRequest, AcceptTosValidator>()
			.AddStateValidator<RegisterRequest, EnsureAccountInfoUniqueValidator>()
			.AddStatusProcessor<RegisterRequest, RegisterProcessor>()

		// Email
			.AddStatusProcessor<ConfirmAccountRequest, ConfirmAccountProcessor>()
			.AddStatusProcessor<InitiateEmailChangeRequest, InitiateEmailChangeProcessor>()
			.AddStatusProcessor<PerformEmailChangeRequest, PerformEmailChangeProcessor>()

		// Password
			.AddStatusProcessor<ChangePasswordRequest, ChangePasswordProcessor>()
			.AddStatusProcessor<ForgotPasswordRequest, ForgotPasswordProcessor>()
			.AddStatusProcessor<ResetPasswordRequest, ResetPasswordProcessor>()

		// Personal data
			.AddBeforeHook<DeleteAccountRequest, RemoveUserRelatedEntitiesHook>()
			.AddStatusProcessor<DeleteAccountRequest, DeleteAccountProcessor>();


		/********
		 * Auth *
		 *******/

		self.TryAddScoped<ISignInManager, CookieSignInManager>();

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

		self.TryAddScoped<IMediaDirectoryMapper, MediaDirectoryMapper>();
		self.TryAddScoped<IMediaManager, MediaManager>();

		self
			.AddAccessValidator<Upload, VerifyUserCanReadFileHook>()
			.AddAccessValidator<Upload, VerifyUserCanModifyFileHook>()
			.AddAccessValidator<Upload, VerifyUserCanModifyFileHook>()
			.AddBeforeHook<Upload, AssignMediaFieldsHook>()
			.AddBeforeHook<Upload, UploadFileHook>()
			.AddEntityFrameworkEntity<Upload, UploadFilterProcessor>();


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