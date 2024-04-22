#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Infrastructure.Menus;

/// <exclude />
public class DashboardGenerator : AuthorizedLinkAggregator<DashboardLink>, IDashboardGenerator
{
	public DashboardGenerator(
		IUserAccessor userAccessor,
		IDashboardProvider dashboardProvider)
		: base(userAccessor, dashboardProvider) {}
}