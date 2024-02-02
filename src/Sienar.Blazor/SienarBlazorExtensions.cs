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
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Services;
using Sienar.Media;

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

		self.TryAddScoped<IUserAccessor, BlazorUserAccessor>();
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
			.Configure<SienarOptions>(config.GetSection("Sienar:Core"))
			.Configure<EmailOptions>(config.GetSection("Sienar:Email:Core"))
			.Configure<IdentityEmailOptions>(config.GetSection("Sienar:Email:IdentityEmailSubjects"))
			.Configure<LoginOptions>(config.GetSection("Sienar:Login"));

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