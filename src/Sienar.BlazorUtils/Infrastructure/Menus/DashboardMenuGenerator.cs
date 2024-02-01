using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure.Menus;

public class DashboardMenuGenerator : MenuGenerator
{
	public DashboardMenuGenerator(
		IUserAccessor userAccessor,
		[FromKeyedServices(SienarBlazorUtilsServiceKeys.DashboardProvider)] IMenuProvider menuProvider)
		: base(userAccessor, menuProvider) {}
}