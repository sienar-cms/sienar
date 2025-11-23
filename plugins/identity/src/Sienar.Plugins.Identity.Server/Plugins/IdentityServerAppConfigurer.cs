using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Infrastructure;

namespace Sienar.Plugins;

/// <exclude />
public class IdentityServerAppConfigurer : IConfigurer<SienarAppBuilder>
{
	/// <exclude />
	public void Configure(SienarAppBuilder builder)
	{
		builder.AddPlugin<CoreServerPlugin>();

		builder.AddStartupServices(sp =>
		{
			sp
				.TryAddConfigurer<DefaultAuthorizationConfigurer, AuthorizationOptions>()
				.TryAddConfigurer<DefaultAuthenticationConfigurer, AuthenticationOptions>()
				.TryAddConfigurer<DefaultAuthenticationBuilderConfigurer, AuthenticationBuilder>()
				.TryAddConfigurer<DefaultMvcConfigurer, MvcOptions>()
				.TryAddConfigurer<DefaultMvcBuilderConfigurer, IMvcBuilder>()
				.TryAddConfigurer<DefaultAntiforgeryConfigurer, AntiforgeryOptions>();
		});
	}
}
