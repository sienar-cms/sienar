using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Sienar.Infrastructure.Menus;

namespace Sienar.Infrastructure.Plugins;

public class SienarBlazorPlugin : ISienarPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Blazor",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar Blazor provides all of the main services and configuration required to operate the Sienar CMS. Sienar cannot serve a Blazor app without this plugin.",
		Homepage = "https://sienar.siteveyor.com"
	};

	/// <inheritdoc />
	public PluginSettings PluginSettings { get; } = new()
	{
		ModifiesStyles = true,
		ModifiesScripts = true
	};

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

	public bool PluginShouldExecute(HttpContext context) => true;

	public void SetupMenu(IMenuProvider menuProvider) {}

	public void SetupStyles(IStyleProvider styleProvider)
	{
		styleProvider
			.Enqueue("/_content/MudBlazor/MudBlazor.min.css")
			.Enqueue("/_content/Sienar.Blazor/sienar.css");
	}

	public void SetupScripts(IScriptProvider scriptProvider)
	{
		scriptProvider
			.Enqueue("/_framework/blazor.server.js")
			.Enqueue("/_content/MudBlazor/MudBlazor.min.js")
			.Enqueue("/_content/Sienar.Blazor/sienar.js");
	}

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider) {}
}