using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Plugins;
using TestProject.Configuration;
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

		if (builder.Builder.Environment.IsDevelopment())
		{
			builder.StartupServices.TryAddConfigurer<DevelopmentCorsConfigurer>();
		}
	}
}
