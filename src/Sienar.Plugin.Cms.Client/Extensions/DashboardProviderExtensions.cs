using MudBlazor;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IDashboardProvider"/> extension methods used by the <c>Sienar.Plugin.Cms.Server</c> assembly
/// </summary>
public static class DashboardProviderExtensions
{
	/// <summary>
	/// Registers menu links for use by the default Sienar dashboard landing page
	/// </summary>
	/// <param name="self">The menu provider</param>
	/// <returns>The menu provider</returns>
	public static IMenuProvider CreateUserManagementMenu(
		this IMenuProvider self)
	{
		self
			.Access(DashboardMenuNames.UserManagement)
			.AddWithNormalPriority(
				new MenuLink
				{
					Text = "Users",
					Icon = Icons.Material.Filled.SupervisorAccount,
					Url = DashboardUrls.Users.Index,
					Roles = [Roles.Admin]
				},
				new MenuLink
				{
					Text = "Lockout reasons",
					Icon = Icons.Material.Filled.Lock,
					Url = DashboardUrls.LockoutReasons.Index,
					Roles = [Roles.Admin]
				});

		return self;
	}
}