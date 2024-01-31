using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Project.Data;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Project.App;

public class AppPlugin : ISienarPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "App plugin",
		Author = string.Empty,
		AuthorUrl = string.Empty,
		Version = Version.Parse("1.0.0"),
		Description = string.Empty
	};

	/// <inheritdoc />
	public PluginSettings PluginSettings { get; } = new()
	{
		ModifiesStyles = true,
		HasRoutableComponents = true
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder) {}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.Services.MigrateDb<AppDbContext>(SienarDataExtensions.GetSienarDbPath());
	}

	public bool PluginShouldExecute(HttpContext context) => true;

	/// <inheritdoc />
	public void SetupStyles(IStyleProvider styleProvider)
	{
		styleProvider.Enqueue("/css/site.css");
	}

	/// <inheritdoc />
	public void SetupScripts(IScriptProvider scriptProvider) {}

	/// <inheritdoc />
	public void SetupMenu(IMenuProvider menuProvider) {}

	/// <inheritdoc />
	public void SetupComponents(IComponentProvider componentProvider) {}
}