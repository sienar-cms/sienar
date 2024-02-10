using Sienar.Docs;
using Sienar.Infrastructure.Menus;

namespace Sienar.Extensions;

public static class MenuProviderExtensions
{
	public static IMenuProvider AddDocsMenu(this IMenuProvider self)
	{
		self
			.Access(Constants.MenuNames.Docs)
			.AddMenuLink(
				new()
				{
					Text = "Overview",
					Url = Urls.Overview
				})
			.AddMenuLink(
				new()
				{
					Text = "Introduction",
					Sublinks =
					[
						new()
						{
							Text = "Getting started",
							Url = Urls.Introduction.GettingStarted
						},
						new()
						{
							Text = "Basics",
							Url = Urls.Introduction.Basics
						},
						new()
						{
							Text = "Plugins",
							Url = Urls.Introduction.Plugins
						},
						new()
						{
							Text = "Hooks",
							Url = Urls.Introduction.Hooks
						}
					]
				})
			.AddMenuLink(
				new()
				{
					Text = "Guides",
					Sublinks =
					[
						new()
						{
							Text = "Sub-apps",
							Url = Urls.Guides.SubApps
						}
					]
				})
			.AddMenuLink(
				new()
				{
					Text = "API",
					Sublinks =
					[
						new()
						{
							Text = "ISienarPlugin",
							Url = Urls.Api.ISienarPlugin
						},
						new()
						{
							Text = "ISienarServerStartupPlugin",
							Url = Urls.Api.ISienarServerStartupPlugin
						},
						new()
						{
							Text = "SienarServerAppBuilder",
							Url = Urls.Api.SienarServerAppBuilder
						}
					]
				});

		return self;
	}
}