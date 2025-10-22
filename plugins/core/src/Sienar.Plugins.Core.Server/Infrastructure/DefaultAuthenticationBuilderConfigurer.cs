#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;

namespace Sienar.Infrastructure;

/// <exclude />
public class DefaultAuthenticationBuilderConfigurer
	: IConfigurer<AuthenticationBuilder>
{
	public void Configure(AuthenticationBuilder options)
	{
		options.AddCookie(
			CookieAuthenticationDefaults.AuthenticationScheme,
			o =>
			{
				o.LoginPath = DashboardUrls.Account.Login;
				o.AccessDeniedPath = DashboardUrls.Account.Forbidden;
			});
	}
}