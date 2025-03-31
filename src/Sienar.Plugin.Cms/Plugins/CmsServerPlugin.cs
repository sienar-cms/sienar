#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
using Sienar.Media.Processors;

namespace Sienar.Plugins;

/// <exclude />
public class CmsServerPlugin<TContext> : IPlugin
	where TContext : DbContext
{
	private readonly WebApplicationBuilder _builder;
	private readonly IPluginDataProvider _pluginDataProvider;

	public CmsServerPlugin(
		WebApplicationBuilder builder,
		IPluginDataProvider pluginDataProvider)
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
		services.TryAddScoped<IPasswordManager, PasswordManager<TContext>>();
		services.TryAddScoped<IUserClaimsFactory, UserClaimsFactory>();
		services.TryAddScoped<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();


		/************
		 * Identity *
		 ***********/

		services.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		services.TryAddScoped<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		services.TryAddScoped<IAccountEmailManager, AccountEmailManager>();
		services.TryAddScoped<IAccountUrlProvider, AccountUrlProvider>();
		services.TryAddScoped<IVerificationCodeManager, VerificationCodeManager<TContext>>();

		// CRUD
		services
			.AddEntity<SienarUser, SienarUserFilterProcessor, TContext>()
			.AddAccessValidator<SienarUser, UserIsAdminAccessValidator<SienarUser>>()
			.AddBeforeActionHook<SienarUser, UserMapNormalizedFieldsHook>()
			.AddBeforeActionHook<SienarUser, UserPasswordUpdateHook>()
			.AddBeforeActionHook<SienarUser, FetchNotUpdatedUserPropertiesHook<TContext>>()
			.AddStateValidator<SienarUser, EnsureAccountInfoUniqueValidator<TContext>>()
			.AddBeforeActionHook<SienarUser, RemoveUserRelatedEntitiesHook<TContext>>()

		// Security
			.AddEntity<SienarRole, SienarRoleFilterProcessor, TContext>()
			.AddEntity<LockoutReason, LockoutReasonFilterProcessor, TContext>()
			.AddBeforeActionHook<LockoutReason, LockoutReasonMapNormalizedFieldsHook>()

			.AddProcessor<LoginRequest, LoginResult, LoginProcessor<TContext>>()
			.AddStatusProcessor<LogoutRequest, LogoutProcessor>()
			.AddResultProcessor<PersonalDataResult, PersonalDataProcessor<TContext>>()
			.AddStatusProcessor<AddUserToRoleRequest, UserRoleChangeProcessor<TContext>>()
			.AddAccessValidator<AddUserToRoleRequest, UserIsAdminAccessValidator<AddUserToRoleRequest>>()
			.AddStatusProcessor<RemoveUserFromRoleRequest, UserRoleChangeProcessor<TContext>>()
			.AddAccessValidator<RemoveUserFromRoleRequest, UserIsAdminAccessValidator<RemoveUserFromRoleRequest>>()
			.AddStatusProcessor<LockUserAccountRequest, LockUserAccountProcessor<TContext>>()
			.AddAccessValidator<LockUserAccountRequest, UserIsAdminAccessValidator<LockUserAccountRequest>>()
			.AddStatusProcessor<UnlockUserAccountRequest, UnlockUserAccountProcessor<TContext>>()
			.AddAccessValidator<UnlockUserAccountRequest, UserIsAdminAccessValidator<UnlockUserAccountRequest>>()
			.AddStatusProcessor<ManuallyConfirmUserAccountRequest, ManuallyConfirmUserAccountProcessor<TContext>>()
			.AddAccessValidator<ManuallyConfirmUserAccountRequest, UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>>()
			.AddStatusProcessor<ChangePasswordRequest, ChangePasswordProcessor<TContext>>()
			.AddStatusProcessor<ForgotPasswordRequest, ForgotPasswordProcessor<TContext>>()
			.AddStatusProcessor<ResetPasswordRequest, ResetPasswordProcessor<TContext>>()
			.AddResultProcessor<AccountDataResult, GetAccountDataProcessor>()
			.AddStatusProcessor<AccessTokenRequest, AccessTokenProcessor>()
			.AddProcessor<AccountLockoutRequest, AccountLockoutResult, GetLockoutReasonsProcessor<TContext>>()

		// Registration
			.AddStateValidator<RegisterRequest, RegistrationOpenValidator>()
			.AddStateValidator<RegisterRequest, AcceptTosValidator>()
			.AddStateValidator<RegisterRequest, EnsureAccountInfoUniqueValidator<TContext>>()
			.AddStatusProcessor<RegisterRequest, RegisterProcessor<TContext>>()

		// Email
			.AddStatusProcessor<ConfirmAccountRequest, ConfirmAccountProcessor<TContext>>()
			.AddStatusProcessor<InitiateEmailChangeRequest, InitiateEmailChangeProcessor<TContext>>()
			.AddStatusProcessor<PerformEmailChangeRequest, PerformEmailChangeProcessor<TContext>>()

		// Personal data
			.AddBeforeActionHook<DeleteAccountRequest, RemoveUserRelatedEntitiesHook<TContext>>()
			.AddStatusProcessor<DeleteAccountRequest, DeleteAccountProcessor<TContext>>();


		/********
		 * Auth *
		 *******/

		services.TryAddScoped<ISignInManager, CookieSignInManager>();


		/*********
		 * Media *
		 ********/

		services.TryAddScoped<IMediaDirectoryMapper, MediaDirectoryMapper>();
		services.TryAddScoped<IMediaManager, MediaManager>();

		services.AddEntity<Upload, UploadFilterProcessor, TContext>();

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
		builder.StartupServices
			.TryAddConfigurer<DefaultAuthorizationConfigurer>()
			.TryAddConfigurer<DefaultAuthenticationConfigurer>()
			.TryAddConfigurer<DefaultAuthenticationBuilderConfigurer>()
			.TryAddConfigurer<DefaultMvcConfigurer>()
			.TryAddConfigurer<DefaultMvcBuilderConfigurer>()
			.TryAddConfigurer<DefaultAntiforgeryConfigurer>();
	}
}
