using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Plugins;
using TestProject.Data;

namespace TestProject;

[AppConfigurer(typeof(SienarAppConfigurer))]
public class TestProjectServerPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;

	public TestProjectServerPlugin(WebApplicationBuilder builder)
	{
		_builder = builder;
	}

	public void Configure()
	{
		_builder.Services.AddDbContextForSienar<AppDbContext>(o => o.UseSienarDb());
	}

	private class SienarAppConfigurer : IConfigurer<SienarAppBuilder>
	{
		public void Configure(SienarAppBuilder builder)
		{
			builder.AddPlugin<IdentityServerPlugin<AppDbContext>>();
		}
	}
}
