using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Infrastructure;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to run as a web application with auth, CORS, and other core web-based services
/// </summary>
public class WebArchitecturePlugin : IPlugin
{
	/// <inheritdoc />
	public void Configure() {}

	/// <summary>
	/// Sets up the plugins that all Sienar web applications depend on
	/// </summary>
	/// <param name="builder">The <see cref="SienarAppBuilder"/></param>
	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder
			.AddPlugin<SecurityPlugin>()
			.AddPlugin<MvcPlugin>()
			.AddPlugin<BlazorPlugin>();
	}
}
