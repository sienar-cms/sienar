using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Services;
using Sienar.Configuration;
using Sienar.Email;
using Sienar.Identity;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;
using Sienar.Media;

namespace Sienar.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSienarUtilities(this IServiceCollection self)
	{
		self.RemoveService(typeof(IStatusService<>));
		self.RemoveService(typeof(IService<,>));

		self.TryAddTransient<IBotDetector, BotDetector>();
		self.TryAddTransient<IEmailSender, DefaultEmailSender>();

		return self
			.AddHttpContextAccessor()
			.AddTransient<INotificationService, NotificationService>()
			.AddTransient(typeof(IStatusService<>), typeof(SienarStatusService<>))
			.AddScoped(typeof(IService<,>), typeof(SienarService<,>))
			.AddTransient(typeof(IStateValidator<>), typeof(ConcurrencyStampValidator<>))
			.AddTransient(typeof(IBeforeProcess<>), typeof(ConcurrencyStampUpdateHook<>));
	}

	public static IServiceCollection AddSienarIdentity(this IServiceCollection self)
	{
		/*********
		 * Other *
		 ********/

		self.TryAddTransient<IPasswordHasher<SienarUser>, PasswordHasher<SienarUser>>();
		self.TryAddTransient<IUserClaimsFactory, UserClaimsFactory>();
		self.TryAddTransient<IUserClaimsPrincipalFactory<SienarUser>, UserClaimsPrincipalFactory>();
		self.TryAddTransient<IVerificationCodeManager, VerificationCodeManager>();
		self.TryAddTransient<IUserManager, UserManager>();

		self.TryAddScoped<IUserAccessor, HttpContextUserAccessor>();
		self.TryAddTransient<IAccountEmailMessageFactory, AccountEmailMessageFactory>();
		self.TryAddTransient<IAccountEmailManager, AccountEmailManager>();
		self.TryAddTransient<IAccountUrlProvider, AccountUrlProvider>();

		return self;
	}

	public static IServiceCollection AddSienarMedia(this IServiceCollection self)
	{
		self.TryAddTransient<IMediaDirectoryMapper, MediaDirectoryMapper>();
		self.TryAddTransient<IMediaManager, MediaManager>();

		return self;
	}

	public static IServiceCollection ConfigureSienarOptions(
		this IServiceCollection self,
		IConfiguration config)
	{
		self
			.ApplyDefaultConfiguration<SienarOptions>(config.GetSection("Sienar:Core"))
			.ApplyDefaultConfiguration<EmailSenderOptions>(config.GetSection("Sienar:Email:Sender"))
			.ApplyDefaultConfiguration<IdentityEmailSubjectOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.ApplyDefaultConfiguration<LoginOptions>(config.GetSection("Sienar:Login"));

		return self;
	}

	public static IServiceCollection ConfigureSienarBlazor(this IServiceCollection self)
	{
		var mudblazorConfigurer = self.GetAndRemoveService<Action<MudServicesConfiguration>>();

		self
			.AddRazorComponents()
			.AddInteractiveServerComponents();

		self.AddCascadingAuthenticationState();

		return self.AddMudServices(mudblazorConfigurer ?? delegate {});
	}

	public static IServiceCollection ConfigureSienarAuth(this IServiceCollection self)
	{
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