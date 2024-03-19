using System;
using Microsoft.AspNetCore.Builder;
using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.Plugin.Cms.DevMode;

// ReSharper disable once CheckNamespace
namespace Sienar.Infrastructure.Plugins;

public class DevmodePlugin : ISienarPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Dev Mode Plugin",
		Description = "A plugin that allows developers to add roles and users to the app without needing to log in.",
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Homepage = "https://sienar.levesque.dev",
		Version = Version.Parse("0.1.0")
	};

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.ConfigureRoutableAssemblies(p => p.Add(typeof(DevmodePlugin).Assembly));
		app.ConfigureMenu(p =>
		{
			p
				.Access(DashboardMenuNames.MainMenu)
				.AddLink(
					new()
					{
						Url = DevUrls.DevConfiguration,
						Text = "Dev configuration",
						Icon = Icons.Material.Filled.DeveloperMode
					},
					MenuPriority.High);
		});
	}
}