using Microsoft.AspNetCore.Components;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Services;

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
			.Access(SienarMenus.Main)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "Dashboard",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					Url = DashboardUrls.Index,
					RequireLoggedIn = true
				});

		return self;
	}

	/// <summary>
	/// Registers the user settings menu with the menu provider
	/// </summary>
	/// <param name="self">the menu provider</param>
	/// <returns>the menu provider</returns>
	public static IMenuProvider CreateUserSettingsMenu(this IMenuProvider self)
	{
		self
			.Access(SienarMenus.UserSettings)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "Email",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					RequireLoggedIn = true,
					Url = DashboardUrls.Account.EmailChange.Index
				},
				new MenuLink
				{
					Text = "Password",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					RequireLoggedIn = true,
					Url = DashboardUrls.Account.PasswordChange.Index
				},
				new MenuLink
				{
					Text = "Personal data",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					RequireLoggedIn = true,
					Url = DashboardUrls.Account.PersonalData
				},
				new MenuLink
				{
					Text = "Delete account",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					RequireLoggedIn = true,
					Url = DashboardUrls.Account.Delete
				});

		self
			.Access(SienarMenus.UserLogout)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "Log out",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					RequireLoggedIn = true,
					OnClick = async (
						IStatusService<LogoutRequest> service,
						NavigationManager navManager) =>
					{
						var result = await service.Execute(new LogoutRequest());

						if (result.Status == OperationStatus.Success)
						{
							navManager.NavigateTo(DashboardUrls.Account.Login);
						}
					}
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
			.Access(SienarMenus.Info)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "About",
					Icon = string.Empty, // TODO: replace with FontAwesome icon
					Url = DashboardUrls.About
				});

		return self;
	}
}