using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Menus;

namespace Sienar.Extensions;

public static class MenuProviderExtensions
{
	public static IMenuProvider CreateMainMenu(this IMenuProvider self)
	{
		var accountLink = new MenuLink
		{
			Text = "My account",
			Icon = Icons.Material.Filled.AccountCircle,
			RequireLoggedIn = true,
			Sublinks =
			[
				new()
				{
					Text = "Email",
					Icon = Icons.Material.Filled.Email,
					Url = DashboardUrls.Account.EmailChange.Index
				},
				new()
				{
					Text = "Password",
					Icon = Icons.Material.Filled.Lock,
					Url = DashboardUrls.Account.PasswordChange.Index
				},
				new()
				{
					Text = "Personal data",
					Icon = Icons.Material.Filled.Archive,
					Url = DashboardUrls.Account.PersonalData
				}
			]
		};

		self
			.Access(DashboardMenuNames.MainMenu)
			.AddMenuLink(
				new()
				{
					Text = "Dashboard",
					Icon = Icons.Material.Filled.Dashboard,
					Url = DashboardUrls.Index,
					RequireLoggedIn = true
				})
			.AddMenuLink(accountLink)
			.AddMenuLink(
				new()
				{
					Text = "Log out",
					Icon = Icons.Material.Filled.Logout,
					Url = DashboardUrls.Account.Logout,
					RequireLoggedIn = true
				})
			.AddMenuLink(
				new()
				{
					Text = "Register",
					Icon = Icons.Material.Filled.Assignment,
					Url = DashboardUrls.Account.Register.Index,
					RequireLoggedOut = true
				})
			.AddMenuLink(
				new()
				{
					Text = "Log in",
					Icon = Icons.Material.Filled.Login,
					Url = DashboardUrls.Account.Login,
					RequireLoggedOut = true
				});

		return self;
	}

	public static IMenuProvider CreateInfoMenu(this IMenuProvider self)
	{
		self
			.Access(DashboardMenuNames.InfoMenu)
			.AddMenuLink(
				new()
				{
					Text = "About",
					Icon = Icons.Material.Outlined.Info,
					Url = DashboardUrls.About
				});

		return self;
	}
}