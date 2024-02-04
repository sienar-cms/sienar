using Sienar.Docs;
using Sienar.Infrastructure.Menus;

namespace Sienar.Extensions;

public static class MenuProviderExtensions
{
	public static IMenuProvider AddDocsMenu(this IMenuProvider self)
	{
		self
			.AccessMenu(Constants.MenuNames.Docs)
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
							Text = "Creating an application",
							Url = Urls.Introduction.CreatingAnApplication
						}
					]
				});

		return self;
	}
}