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
using Sienar.Identity.Data;
using Sienar.Identity.Hooks;
using Sienar.Identity.Processors;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Security;

namespace Sienar.Plugins;

/// <exclude />
[AppConfigurer(typeof(IdentityServerAppConfigurer))]
public class IdentityServerPlugin<TContext> : IPlugin
	where TContext : DbContext
{
	private readonly WebApplicationBuilder _builder;
	private readonly PluginDataProvider _pluginDataProvider;

	public IdentityServerPlugin(
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
			Name = "Sienar Core - REST API",
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
		services.TryAddScoped<IVerificationCodeManager, VerificationCodeManager<TContext>>();
		services.TryAddScoped<IUserRepository, UserRepository<TContext>>();

		services.TryAddScoped<IEmailSender, DefaultEmailSender>();


		/************
		 * Identity *
		 ***********/

		services.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		services.TryAddScoped<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		services.TryAddScoped<IAccountEmailManager, AccountEmailManager>();
		services.TryAddScoped<IAccountUrlProvider, AccountUrlProvider>();

		// CRUD
		services
			.AddEfEntity<SienarUser, SienarUserFilterProcessor, TContext>()
			.AddAccessValidator<UserIsAdminAccessValidator<SienarUser>, SienarUser>()
			.AddBeforeActionHook<FetchNotUpdatedUserPropertiesHook<TContext>, SienarUser>()
			.AddBeforeActionHook<UserMapNormalizedFieldsHook, SienarUser>()
			.AddBeforeActionHook<UserPasswordUpdateHook, SienarUser>()
			.AddBeforeActionHook<RemoveUserRelatedEntitiesHook<TContext>, SienarUser>()
			.AddStateValidator<EnsureAccountInfoUniqueValidator<TContext>, SienarUser>()
			.AddEfEntity<LockoutReason, LockoutReasonFilterProcessor, TContext>()
			.AddBeforeActionHook<LockoutReasonMapNormalizedFieldsHook, LockoutReason>().AddEfEntity<SienarRole, SienarRoleFilterProcessor, TContext>()

		// Security
			.AddProcessor<LoginProcessor<TContext>, LoginRequest, LoginResult>()
			.AddStatusProcessor<LogoutProcessor, LogoutRequest>()
			.AddResultProcessor<PersonalDataProcessor<TContext>, PersonalDataResult>()
			.AddStatusProcessor<UserRoleChangeProcessor<TContext>, AddUserToRoleRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<AddUserToRoleRequest>, AddUserToRoleRequest>()
			.AddStatusProcessor<UserRoleChangeProcessor<TContext>, RemoveUserFromRoleRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<RemoveUserFromRoleRequest>, RemoveUserFromRoleRequest>()
			.AddStatusProcessor<LockUserAccountProcessor<TContext>, LockUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<LockUserAccountRequest>, LockUserAccountRequest>()
			.AddStatusProcessor<UnlockUserAccountProcessor<TContext>, UnlockUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<UnlockUserAccountRequest>, UnlockUserAccountRequest>()
			.AddStatusProcessor<ManuallyConfirmUserAccountProcessor<TContext>, ManuallyConfirmUserAccountRequest>()
			.AddAccessValidator<UserIsAdminAccessValidator<ManuallyConfirmUserAccountRequest>, ManuallyConfirmUserAccountRequest>()
			.AddStatusProcessor<ChangePasswordProcessor<TContext>, ChangePasswordRequest>()
			.AddStatusProcessor<ForgotPasswordProcessor<TContext>, ForgotPasswordRequest>()
			.AddStatusProcessor<ResetPasswordProcessor, ResetPasswordRequest>()
			.AddResultProcessor<GetAccountDataProcessor, AccountDataResult>()
			.AddProcessor<GetLockoutReasonsProcessor, AccountLockoutRequest, AccountLockoutResult>()

		// Registration
			.AddStateValidator<RegistrationOpenValidator, RegisterRequest>()
			.AddStateValidator<AcceptTosValidator, RegisterRequest>()
			.AddStateValidator<EnsureAccountInfoUniqueValidator<TContext>, RegisterRequest>()
			.AddStatusProcessor<RegisterProcessor, RegisterRequest>()

		// Email
			.AddStatusProcessor<ConfirmAccountProcessor, ConfirmAccountRequest>()
			.AddStatusProcessor<InitiateEmailChangeProcessor, InitiateEmailChangeRequest>()
			.AddStatusProcessor<PerformEmailChangeProcessor, PerformEmailChangeRequest>()

		// Personal data
			.AddBeforeActionHook<RemoveUserRelatedEntitiesHook<TContext>, DeleteAccountRequest>()
			.AddStatusProcessor<DeleteAccountProcessor, DeleteAccountRequest>();


		/********
		 * Auth *
		 *******/

		services.TryAddScoped<ISignInManager, CookieSignInManager>();


		/***********
		 * Options *
		 **********/

		services
			.ApplyDefaultConfiguration<SienarOptions>(config.GetSection("Sienar:Core"))
			.ApplyDefaultConfiguration<EmailSenderOptions>(config.GetSection("Sienar:Email:Sender"))
			.ApplyDefaultConfiguration<IdentityEmailSubjectOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.ApplyDefaultConfiguration<LoginOptions>(config.GetSection("Sienar:Login"));
	}
}
