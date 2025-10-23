using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Configuration;
using Sienar.Infrastructure;
using Sienar.Plugins;
using TestProject.Data;

namespace TestProject;

public class TestProjectServerPlugin : IPlugin
{
	private readonly WebApplicationBuilder _builder;

	public TestProjectServerPlugin(WebApplicationBuilder builder)
	{
		_builder = builder;
	}

	public void Configure()
	{
		_builder.Services.AddDbContext<AppDbContext>(o => o.UseSienarDb());
	}

	[AppConfigurer]
	public static void ConfigureApp(SienarAppBuilder builder)
	{
		builder.AddPlugin<CmsServerPlugin<AppDbContext>>();
	}
}
