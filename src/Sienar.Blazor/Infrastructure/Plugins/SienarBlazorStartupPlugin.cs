using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Sienar.Infrastructure.Plugins;

internal class SienarBlazorStartupPlugin : ISienarServerStartupPlugin
{
	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		SienarUtils.SetupBaseDirectory();

		var services = builder.Services;
		var config = builder.Configuration;

		services
			.AddSienarUtilities()
			.AddSienarIdentity()
			.AddSienarMedia()
			.ConfigureSienarOptions(config)
			.ConfigureSienarBlazor()
			.ConfigureSienarBlazorAuth();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			app
				.UseExceptionHandler("/Error")
				.UseHsts();
		}

		app
			.UseStaticFiles()
			.UseRouting()
			.UseAuthorization();
		app.MapBlazorHub();
	}
}