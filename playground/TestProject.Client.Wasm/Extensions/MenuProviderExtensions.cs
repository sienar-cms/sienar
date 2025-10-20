using MudBlazor;
using Sienar.Menus;

namespace TestProject.Client.Extensions;

public static class MenuProviderExtensions
{
	public static void AddMenu(this MenuProvider self)
	{
		self
			.Access(Menus.Main)
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
					Icon = Icons.Material.Filled.Apps,
					ChildMenu = Menus.Social
				},
				new()
				{
					Url = "https://google.com",
					Icon = Icons.Material.Filled.LocalActivity,
					ChildMenu = Menus.Hobbies
				}
			);

		self
			.Access(Menus.Social)
			.AddWithNormalPriority(
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
				});

		self
			.Access(Menus.Hobbies)
			.AddWithNormalPriority(
				new()
				{
					Url = "https://google.com",
					Icon = Icons.Material.Filled.Sports,
					ChildMenu = Menus.Sports
				},
				new()
				{
					Url = "https://google.com",
					Icon = Icons.Material.Filled.Computer,
					ChildMenu = Menus.OperatingSystems
				});

		self
			.Access(Menus.Sports)
			.AddWithNormalPriority(
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
				});

		self
			.Access(Menus.OperatingSystems)
			.AddWithNormalPriority(
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
				});
	}
}