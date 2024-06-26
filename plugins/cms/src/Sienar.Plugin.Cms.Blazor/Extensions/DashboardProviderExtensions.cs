using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Menus;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IDashboardProvider"/> extension methods used by the <c>Sienar.Plugin.Cms.Blazor</c> assembly
/// </summary>
public static class DashboardProviderExtensions
{
	/// <summary>
	/// Registers dashboards for use by the default Sienar dashboard landing page
	/// </summary>
	/// <param name="self">the dashboard provider</param>
	/// <returns>the dashboard provider</returns>
	public static IDashboardProvider CreateUserManagementDashboard(
		this IDashboardProvider self)
	{
		self
			.Access(DashboardMenuNames.Dashboards.UserManagement)
			.AddLink(
				new()
				{
					Text = "Users",
					Icon = Icons.Material.Filled.SupervisorAccount,
					Url = DashboardUrls.Users.Index,
					Roles = [Roles.Admin]
				})
			.AddLink(
				new()
				{
					Text = "Lockout reasons",
					Icon = Icons.Material.Filled.Lock,
					Url = DashboardUrls.LockoutReasons.Index,
					Roles = [Roles.Admin]
				});

		return self;
	}
}