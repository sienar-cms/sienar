using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Infrastructure;

namespace Sienar.Plugins;

/// <summary>
/// Configures the Sienar app to run as a web application with auth, CORS, and other core web-based services
/// </summary>
[AppConfigurer(typeof(SienarAppConfigurer))]
public class CoreServerPlugin : IPlugin
{
	/// <inheritdoc />
	public void Configure() {}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder
				.AddPlugin<CoreSecurityPlugin>()
				.AddPlugin<CoreMvcPlugin>()
				.AddPlugin<CoreBlazorPlugin>()
				.AddStartupServices(sp => sp.AddSingleton<MiddlewareProvider>());
		}
	}
}
