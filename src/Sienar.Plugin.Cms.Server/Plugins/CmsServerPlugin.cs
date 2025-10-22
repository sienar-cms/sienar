#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Extensions;
using Sienar.Hooks;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Media;
using Sienar.Media.Hooks;
using Sienar.Security;

namespace Sienar.Plugins;

/// <exclude />
public class CmsServerPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;
	private readonly PluginDataProvider _pluginDataProvider;

	public CmsServerPlugin(
		WebApplicationBuilder builder,
		PluginDataProvider pluginDataProvider)
	{
		_builder = builder;
		_pluginDataProvider = pluginDataProvider;
	}

	public void Configure()
	{
		_pluginDataProvider.Add(new PluginData
		{
			Name = "Sienar CMS - REST API",
			Description = "Configures Sienar as a collection of REST API endpoints that can be used as a backend for desktop applications, mobile apps, or JavaScript/WebAssembly SPAs.",
			Author = "Christian LeVesque",
			AuthorUrl = "https://levesque.dev",
			Homepage = "https://sienar.io",
			Version = Version.Parse("0.1.1")
		});

		SienarUtils.SetupBaseDirectory();

		var services = _builder.Services;
		var config = _builder.Configuration;

		services.AddHttpContextAccessor();

		services.TryAddScoped<IPasswordHasher<SienarUser>, PasswordHasher<SienarUser>>();
		services.TryAddScoped<IPasswordManager, PasswordManager>();
		services.TryAddScoped<IUserClaimsFactory, UserClaimsFactory>();
		services.TryAddScoped<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();

		services.TryAddScoped<IEmailSender, DefaultEmailSender>();
		services.TryAddScoped<IBotDetector, DefaultBotDetector>();


		/************
		 * Identity *
		 ***********/

		services.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		services.TryAddScoped<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		services.TryAddScoped<IAccountEmailManager, AccountEmailManager>();
		services.TryAddScoped<IAccountUrlProvider, AccountUrlProvider>();

		// CRUD
		services
			.AddAccessValidator<SienarUser, UserIsAdminAccessValidator<SienarUser>>()
			.AddBeforeActionHook<SienarUser, UserMapNormalizedFieldsHook>()
			.AddBeforeActionHook<SienarUser, UserPasswordUpdateHook>()
			.AddStateValidator<SienarUser, EnsureAccountInfoUniqueValidator>()
			.AddBeforeActionHook<SienarUser, RemoveUserRelatedEntitiesHook>()
			.AddBeforeActionHook<LockoutReason, LockoutReasonMapNormalizedFieldsHook>()

		// Security
			.AddProcessor<LoginRequest, LoginResult, LoginProcessor>()
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
			.AddStatusProcessor<ChangePasswordRequest, ChangePasswordProcessor>()
			.AddStatusProcessor<ForgotPasswordRequest, ForgotPasswordProcessor>()
			.AddStatusProcessor<ResetPasswordRequest, ResetPasswordProcessor>()
			.AddResultProcessor<AccountDataResult, GetAccountDataProcessor>()
			.AddStatusProcessor<AccessTokenRequest, AccessTokenProcessor>()
			.AddProcessor<AccountLockoutRequest, AccountLockoutResult, GetLockoutReasonsProcessor>()

		// Registration
			.AddStateValidator<RegisterRequest, RegistrationOpenValidator>()
			.AddStateValidator<RegisterRequest, AcceptTosValidator>()
			.AddStateValidator<RegisterRequest, EnsureAccountInfoUniqueValidator>()
			.AddStatusProcessor<RegisterRequest, RegisterProcessor>()

		// Email
			.AddStatusProcessor<ConfirmAccountRequest, ConfirmAccountProcessor>()
			.AddStatusProcessor<InitiateEmailChangeRequest, InitiateEmailChangeProcessor>()
			.AddStatusProcessor<PerformEmailChangeRequest, PerformEmailChangeProcessor>()

		// Personal data
			.AddBeforeActionHook<DeleteAccountRequest, RemoveUserRelatedEntitiesHook>()
			.AddStatusProcessor<DeleteAccountRequest, DeleteAccountProcessor>();


		/********
		 * Auth *
		 *******/

		services.TryAddScoped<ISignInManager, CookieSignInManager>();


		/*********
		 * Media *
		 ********/

		services.TryAddScoped<IMediaDirectoryMapper, MediaDirectoryMapper>();
		services.TryAddScoped<IMediaManager, MediaManager>();

		services
			.AddAccessValidator<Upload, VerifyUserCanReadFileHook>()
			.AddAccessValidator<Upload, VerifyUserCanModifyFileHook>()
			.AddAccessValidator<Upload, VerifyUserCanModifyFileHook>()
			.AddBeforeActionHook<Upload, AssignMediaFieldsHook>()
			.AddBeforeActionHook<Upload, UploadFileHook>();


		/***********
		 * Options *
		 **********/

		services
			.ApplyDefaultConfiguration<SienarOptions>(config.GetSection("Sienar:Core"))
			.ApplyDefaultConfiguration<EmailSenderOptions>(config.GetSection("Sienar:Email:Sender"))
			.ApplyDefaultConfiguration<IdentityEmailSubjectOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.ApplyDefaultConfiguration<LoginOptions>(config.GetSection("Sienar:Login"));
	}

	/// <summary>
	/// Configures the <see cref="SienarAppBuilder"/> with configurers and dependent plugins
	/// </summary>
	/// <param name="builder">The <see cref="SienarAppBuilder"/></param>
	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder.AddPlugin<MvcPlugin>();

		builder.AddStartupServices(sp =>
		{
			sp
				.TryAddConfigurer<DefaultAuthorizationConfigurer>()
				.TryAddConfigurer<DefaultAuthenticationConfigurer>()
				.TryAddConfigurer<DefaultAuthenticationBuilderConfigurer>()
				.TryAddConfigurer<DefaultMvcConfigurer>()
				.TryAddConfigurer<DefaultMvcBuilderConfigurer>()
				.TryAddConfigurer<DefaultAntiforgeryConfigurer>();
		});
	}
}
