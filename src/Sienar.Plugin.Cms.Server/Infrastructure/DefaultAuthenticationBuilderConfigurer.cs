﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

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
				o.LoginPath = "/Dashboard/Account/Login";
				o.LogoutPath = "/Dashboard/Account/Logout";
				o.AccessDeniedPath = "/Dashboard/Account/Forbidden";
			});
	}
}