using Sienar.Infrastructure;

namespace TestProject.Client.Extensions;

public static class MenuProviderExtensions
{
	public static void AddMenu(this IMenuProvider self)
	{
		self
			.Access(Menus.Main)
			.AddWithNormalPriority(
				new()
				{
					Url = "/",
					Text = "Home",
					Icon = "Icons.Material.Filled.Home" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "/dashboard",
					Text = "Dashboard",
					Icon = "Icons.Material.Filled.Dashboard" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "https://google.com",
					Icon = "Icons.Material.Filled.Apps", // TODO: Replace with FontAweomse icon
					ChildMenu = Menus.Social
				},
				new()
				{
					Url = "https://google.com",
					Icon = "Icons.Material.Filled.LocalActivity", // TODO: Replace with FontAweomse icon
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
					Icon = "Icons.Custom.Brands.Facebook" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "https://twitter.com",
					Text = "Twitter",
					Icon = "Icons.Custom.Brands.Twitter" // TODO: Replace with FontAweomse icon
				});

		self
			.Access(Menus.Hobbies)
			.AddWithNormalPriority(
				new()
				{
					Url = "https://google.com",
					Icon = "Icons.Material.Filled.Sports", // TODO: Replace with FontAweomse icon
					ChildMenu = Menus.Sports
				},
				new()
				{
					Url = "https://google.com",
					Icon = "Icons.Material.Filled.Computer", // TODO: Replace with FontAweomse icon
					ChildMenu = Menus.OperatingSystems
				});

		self
			.Access(Menus.Sports)
			.AddWithNormalPriority(
				new()
				{
					Url = "https://nba.com",
					Text = "Basketball",
					Icon = "Icons.Material.Filled.SportsBasketball" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "https://nfl.com",
					Text = "Football",
					Icon = "Icons.Material.Filled.SportsFootball" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "https://mlb.com",
					Text = "Baseball",
					Icon = "Icons.Material.Filled.SportsBaseball" // TODO: Replace with FontAweomse icon
				});

		self
			.Access(Menus.OperatingSystems)
			.AddWithNormalPriority(
				new()
				{
					Url = "https://chrome.com",
					Text = "Microsoft Windows",
					Icon = "Icons.Custom.Brands.MicrosoftWindows" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "https://apple.com",
					Text = "Apple macOS",
					Icon = "Icons.Custom.Brands.Apple" // TODO: Replace with FontAweomse icon
				},
				new()
				{
					Url = "https://linux.org",
					Text = "Linux",
					Icon = "Icons.Custom.Brands.Linux" // TODO: Replace with FontAweomse icon
				});
	}
}