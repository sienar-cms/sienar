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
using Sienar.Infrastructure.Data;
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
		self.TryAddScoped<IBotDetector, BotDetector>();
		self.TryAddScoped<IEmailSender, DefaultEmailSender>();
		self.ReplaceService(
			typeof(IStatusService<>),
			typeof(StatusService<>),
			typeof(SienarStatusService<>));
		self.ReplaceService(
			typeof(IService<,>),
			typeof(Service<,>),
			typeof(SienarService<,>));

		self
			.AddHttpContextAccessor()
			.AddScoped(typeof(IRepository<>), typeof(EntityFrameworkRepository<>))
			.AddScoped(typeof(IStateValidator<>), typeof(ConcurrencyStampValidator<>))
			.AddScoped(typeof(IBeforeProcess<>), typeof(ConcurrencyStampUpdateHook<>));

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
			// .AddTransient<IBeforeRead<SienarUser>, IncludeRolesInFilterHook>()
			.AddScoped<IAccessValidator<SienarUser>, UserIsAdminAccessValidator<SienarUser>>()
			.AddScoped<IBeforeProcess<SienarUser>, UserPasswordUpdateHook>()
			.AddScoped<IStateValidator<SienarUser>, EnsureAccountInfoUniqueValidator>()
			.AddScoped<IBeforeProcess<SienarUser>, RemoveUserRelatedEntitiesHook>();

		self.TryAddScoped<IEntityFrameworkFilterProcessor<SienarUser>, SienarUserFilterProcessor>();
		self.TryAddScoped<IEntityFrameworkFilterProcessor<SienarRole>, SienarRoleFilterProcessor>();
		self.TryAddScoped<IEntityFrameworkFilterProcessor<LockoutReason>, LockoutReasonFilterProcessor>();

		// Security
		self.TryAddScoped<IProcessor<LoginRequest, Guid>, LoginProcessor>();
		self.TryAddScoped<IProcessor<PerformLoginRequest, bool>, PerformLoginProcessor>();
		self.TryAddScoped<IProcessor<LogoutRequest, bool>, LogoutProcessor>();
		self.TryAddScoped<IProcessor<PersonalDataResult>, PersonalDataProcessor>();
		self.TryAddScoped<IProcessor<AddUserToRoleRequest, bool>, UserRoleChangeProcessor>();
		self.AddScoped<IAccessValidator<AddUserToRoleRequest>, UserIsAdminAccessValidator<AddUserToRoleRequest>>();
		self.TryAddScoped<IProcessor<RemoveUserFromRoleRequest, bool>, UserRoleChangeProcessor>();
		self.AddScoped<IAccessValidator<RemoveUserFromRoleRequest>, UserIsAdminAccessValidator<RemoveUserFromRoleRequest>>();
		self.TryAddScoped<IProcessor<LockUserAccountRequest, bool>, LockUserAccountProcessor>();
		self.AddScoped<IAccessValidator<LockUserAccountRequest>, UserIsAdminAccessValidator<LockUserAccountRequest>>();
		self.TryAddScoped<IProcessor<UnlockUserAccountRequest, bool>, UnlockUserAccountProcessor>();
		self.AddScoped<IAccessValidator<UnlockUserAccountRequest>, UserIsAdminAccessValidator<UnlockUserAccountRequest>>();
		self.TryAddScoped<IProcessor<ManuallyConfirmUserAccountRequest, bool>, ManuallyConfirmUserAccountProcessor>();
		self.AddScoped<IAccessValidator<ManuallyConfirmUserAccountRequest>, UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>>();

		// Registration
		self.AddScoped<IStateValidator<RegisterRequest>, RegistrationOpenValidator>();
		self.AddScoped<IStateValidator<RegisterRequest>, AcceptTosValidator>();
		self.AddScoped<IStateValidator<RegisterRequest>, EnsureAccountInfoUniqueValidator>();
		self.TryAddScoped<IProcessor<RegisterRequest, bool>, RegisterProcessor>();

		// Email
		self.TryAddScoped<IProcessor<ConfirmAccountRequest, bool>, ConfirmAccountProcessor>();
		self.TryAddScoped<IProcessor<InitiateEmailChangeRequest, bool>, InitiateEmailChangeProcessor>();
		self.TryAddScoped<IProcessor<PerformEmailChangeRequest, bool>, PerformEmailChangeProcessor>();

		// Password
		self.TryAddScoped<IProcessor<ChangePasswordRequest, bool>, ChangePasswordProcessor>();
		self.TryAddScoped<IProcessor<ForgotPasswordRequest, bool>, ForgotPasswordProcessor>();
		self.TryAddScoped<IProcessor<ResetPasswordRequest, bool>, ResetPasswordProcessor>();

		// Personal data
		self.AddScoped<IBeforeProcess<DeleteAccountRequest>, RemoveUserRelatedEntitiesHook>();
		self.TryAddScoped<IProcessor<DeleteAccountRequest, bool>, DeleteAccountProcessor>();

		self.AddSingleton<LoginTokenCache>();


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

		self.TryAddScoped<IEntityFrameworkFilterProcessor<Upload>, UploadFilterProcessor>();

		self.AddScoped<IAccessValidator<Upload>, VerifyUserCanReadFileHook>();
		self.AddScoped<IAccessValidator<Upload>, VerifyUserCanModifyFileHook>();
		self.AddScoped<IAccessValidator<Upload>, VerifyUserCanModifyFileHook>();
		self.AddScoped<IBeforeProcess<Upload>, AssignMediaFieldsHook>();
		self.AddScoped<IBeforeProcess<Upload>, UploadFileHook>();


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