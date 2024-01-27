using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Project.Data;
using Sienar.Infrastructure.Plugins;

namespace Project.App.Blazor;

public class AppPlugin : ISienarPlugin
{
	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder) {}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.Services.MigrateDb<AppDbContext>(SienarDataExtensions.GetSienarDbPath());
	}

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
	public PluginSettings PluginSettings { get; } = new();
}