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
							Text = "ISienarServerPlugin",
							Url = Urls.Api.ISienarServerPlugin
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