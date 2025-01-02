using MudBlazor;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IMenuProvider"/> extension methods used by the <c>Sienar.Plugin.Cms.Server</c> assembly
/// </summary>
public static class MenuProviderExtensions
{
	/// <summary>
	/// Registers the main menu with the menu provider
	/// </summary>
	/// <param name="self">the menu provider</param>
	/// <returns>the menu provider</returns>
	public static IMenuProvider CreateMainMenu(this IMenuProvider self)
	{
		self
			.Access(DashboardMenuNames.Main)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "Dashboard",
					Icon = Icons.Material.Filled.Dashboard,
					Url = DashboardUrls.Index,
					RequireLoggedIn = true
				},
				new MenuLink
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
				})
			.AddWithPriority(
				Priority.Lowest,
				new MenuLink
				{
					Text = "Log out",
					Icon = Icons.Material.Filled.Logout,
					Url = DashboardUrls.Account.Logout,
					RequireLoggedIn = true
				},
				new MenuLink
				{
					Text = "Register",
					Icon = Icons.Material.Filled.Assignment,
					Url = DashboardUrls.Account.Register.Index,
					RequireLoggedOut = true
				},
				new MenuLink
				{
					Text = "Log in",
					Icon = Icons.Material.Filled.Login,
					Url = DashboardUrls.Account.Login,
					RequireLoggedOut = true
				});

		return self;
	}

	/// <summary>
	/// Registers the info menu with the menu provider
	/// </summary>
	/// <param name="self">the menu provider</param>
	/// <returns>the menu provider</returns>
	public static IMenuProvider CreateInfoMenu(this IMenuProvider self)
	{
		self
			.Access(DashboardMenuNames.Info)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "About",
					Icon = Icons.Material.Outlined.Info,
					Url = DashboardUrls.About
				});

		return self;
	}
}