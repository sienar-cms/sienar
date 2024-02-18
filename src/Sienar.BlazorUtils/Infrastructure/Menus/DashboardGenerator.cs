namespace Sienar.Infrastructure.Menus;

public class DashboardGenerator : AuthorizedLinkAggregator<DashboardLink>, IDashboardGenerator
{
	public DashboardGenerator(
		IUserAccessor userAccessor,
		IDashboardProvider dashboardProvider)
		: base(userAccessor, dashboardProvider) {}
}