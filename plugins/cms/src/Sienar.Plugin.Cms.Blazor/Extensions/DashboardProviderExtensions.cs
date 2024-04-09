using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Menus;

namespace Sienar.Extensions;

public static class DashboardProviderExtensions
{
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