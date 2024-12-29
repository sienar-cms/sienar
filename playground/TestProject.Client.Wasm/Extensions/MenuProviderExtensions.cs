using MudBlazor;
using Sienar;
using Sienar.Infrastructure;

namespace TestProject.Client.Extensions;

public static class MenuProviderExtensions
{
	public static void AddMenu(this IMenuProvider self)
	{
		self
			.Access(Constants.MenuNames.MainMenu)
			.AddWithNormalPriority(
				new()
				{
					Url = "/",
					Text = "Home",
					Icon = Icons.Material.Filled.Home
				},
				new()
				{
					Url = "/dashboard",
					Text = "Dashboard",
					Icon = Icons.Material.Filled.Dashboard
				},
				new()
				{
					Url = "https://google.com",
					Text = "Social",
					Icon = Icons.Material.Filled.Apps,
					Sublinks =
					[
						new()
						{
							Url = "https://facebook.com",
							Text = "Facebook",
							Icon = Icons.Custom.Brands.Facebook
						},
						new()
						{
							Url = "https://twitter.com",
							Text = "Twitter",
							Icon = Icons.Custom.Brands.Twitter
						}
					]
				},
				new()
				{
					Url = "https://google.com",
					Text = "Hobbies",
					Icon = Icons.Material.Filled.LocalActivity,
					Sublinks = [
						new()
						{
							Url = "https://google.com",
							Text = "Sports",
							Icon = Icons.Material.Filled.Sports,
							Sublinks = [
								new()
								{
									Url = "https://nba.com",
									Text = "Basketball",
									Icon = Icons.Material.Filled.SportsBasketball
								},
								new()
								{
									Url = "https://nfl.com",
									Text = "Football",
									Icon = Icons.Material.Filled.SportsFootball
								},
								new()
								{
									Url = "https://mlb.com",
									Text = "Baseball",
									Icon = Icons.Material.Filled.SportsBaseball
								}
							]
						},
						new()
						{
							Url = "https://google.com",
							Text = "Operating systems",
							Icon = Icons.Material.Filled.Computer,
							Sublinks = [
								new()
								{
									Url = "https://chrome.com",
									Text = "Microsoft Windows",
									Icon = Icons.Custom.Brands.MicrosoftWindows
								},
								new()
								{
									Url = "https://apple.com",
									Text = "Apple macOS",
									Icon = Icons.Custom.Brands.Apple
								},
								new()
								{
									Url = "https://linux.org",
									Text = "Linux",
									Icon = Icons.Custom.Brands.Linux
								}
							]
						}
					]
				}
			);
	}
}