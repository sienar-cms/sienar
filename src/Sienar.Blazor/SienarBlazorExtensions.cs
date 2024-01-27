using System;
using System.Linq;
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
using Sienar.UI.Drawers;
using Sienar.Infrastructure;
using Sienar.Email;
using Sienar.Identity;
using Sienar.Identity.Hooks;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;
using Sienar.Infrastructure.States;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Menus;

namespace Sienar;

public static class SienarBlazorExtensions
{
	public static IServiceCollection AddSienarUtilities(this IServiceCollection self)
	{
		self.TryAddTransient<IBotDetector, BotDetector>();
		self.TryAddTransient<IEmailSender, DefaultEmailSender>();
		self.TryAddScoped<IMenuGenerator, MenuGenerator>();
		self.TryAddSingleton<IMenuProvider, MenuProvider>();
		self.TryAddTransient<INotificationService, NotificationService>();
		self.TryAddScoped(typeof(IDbContextAccessor<>), typeof(DbContextAccessor<>));
		self.TryAddTransient(typeof(IHookableService<>), typeof(SienarHookableService<>));
		self
			.AddTransient(typeof(IEntityReader<>), typeof(EntityReader<>))
			.AddTransient(typeof(IEntityWriter<>), typeof(EntityWriter<>))
			.AddTransient(typeof(IEntityDeleter<>), typeof(EntityDeleter<>))
			.AddTransient(typeof(IEntityStateValidator<>), typeof(ConcurrencyStampValidatorHook<>))
			.AddTransient(typeof(IBeforeUpsert<>), typeof(ConcurrencyStampUpdateHook<>));

		return self;
	}

	public static IServiceCollection AddSienarIdentity(this IServiceCollection self)
	{
		self.TryAddTransient<IRoleService, RoleService>();
		self.TryAddTransient<IPersonalDataService, PersonalDataService>();
		self.TryAddTransient<IUserRoleService, UserRoleService>();

		// Hooks
		self
			// CRUD
			.AddTransient<IBeforeRead<SienarUser>, IncludeRolesInFilterHook>()
			.AddTransient<IBeforeUpsert<SienarUser>, UserPasswordUpdateHook>()
			.AddTransient<IEntityStateValidator<SienarUser>, EnsureAccountInfoUniqueHook>()
			.AddTransient<IAfterDelete<SienarUser>, DeleteOwnAccountLogoutHook>()

			// Login
			.AddTransient<IProcessor<LoginRequest>, LoginHook>()
			.AddTransient<IProcessor<LogoutRequest>, LogoutHook>()

			// Registration
			.AddTransient<IBeforeProcess<RegisterRequest>, RegistrationOpenHook>()
			.AddTransient<IBeforeProcess<RegisterRequest>, AcceptTosHook>()
			.AddTransient<IBeforeProcess<RegisterRequest>, EnsureAccountInfoUniqueHook>()
			.AddTransient<IProcessor<RegisterRequest>, RegisterHook>()

			// Email
			.AddTransient<IProcessor<ConfirmAccountRequest>, ConfirmAccountHook>()
			.AddTransient<IProcessor<InitiateEmailChangeRequest>, InitiateEmailChangeHook>()
			.AddTransient<IProcessor<PerformEmailChangeRequest>, PerformEmailChangeHook>()

			// Password
			.AddTransient<IProcessor<ChangePasswordRequest>, ChangePasswordHook>()
			.AddTransient<IProcessor<ForgotPasswordRequest>, ForgotPasswordHook>()
			.AddTransient<IProcessor<ResetPasswordRequest>, ResetPasswordHook>()

			// Personal data
			.AddTransient<IProcessor<DeleteAccountRequest>, DeleteAccountHook>();

		self.TryAddTransient<IFilterProcessor<SienarUser>, SienarUserFilterProcessor>();
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
		self.TryAddTransient<IFilterProcessor<Medium>, MediumFilterProcessor>();
		self.TryAddTransient<IMediaDirectoryMapper, MediaDirectoryMapper>();
		self.TryAddTransient<IMediaManager, MediaManager>();

		return self
			.AddTransient<IAfterRead<Medium>, VerifyUserCanReadFileHook>()
			.AddTransient<IBeforeUpsert<Medium>, AssignMediaFieldsHook>()
			.AddTransient<IBeforeUpsert<Medium>, UploadFileHook>()
			.AddTransient<IBeforeUpsert<Medium>, VerifyUserCanModifyFileHook>()
			.AddTransient<IBeforeDelete<Medium>, VerifyUserCanModifyFileHook>();
	}

	public static IServiceCollection AddSienarStates(this IServiceCollection self)
	{
		self.TryAddTransient<IFilterProcessor<Infrastructure.State>, StateFilterProcessor>();
		self.TryAddTransient<IEntityStateValidator<Infrastructure.State>, EnsureStateNameAbbreviationUniqueHook>();

		return self;
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
		CustomizableComponents.SidebarHeaderContent = typeof(DrawerHeader);
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

		var defaultAuthStateProvider = self.FirstOrDefault(s => s.ImplementationType == typeof(AuthenticationStateProvider));
		if (defaultAuthStateProvider is not null)
		{
			self.Remove(defaultAuthStateProvider);
		}

		self.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
		self.AddScoped<AuthStateProvider>(
			sp => (AuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
		self.AddScoped<AccountStateProvider>();

		var authorizationConfigurer = self.GetAndRemoveService<Action<AuthorizationOptions>>();
		var authenticationConfigurer = self.GetAndRemoveService<Action<AuthenticationOptions>>();

		self
			.AddAuthorization(
				o =>
				{
					authorizationConfigurer?.Invoke(o);
				})
			.AddAuthentication(
				o =>
				{
					authenticationConfigurer?.Invoke(o);
				});

		return self;
	}
}