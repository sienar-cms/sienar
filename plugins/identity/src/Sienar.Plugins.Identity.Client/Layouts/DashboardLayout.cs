using Sienar.Infrastructure;

namespace Sienar.Layouts;

public class DashboardLayout : DashboardLayoutBase
{
	public DashboardLayout()
	{
		MenuNames = [
			SienarMenus.Main,
			SienarMenus.Info
		];
		LayoutType = typeof(DashboardLayout);
	}
}