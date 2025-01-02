using Sienar.Infrastructure;

namespace Sienar.Layouts;

public class DashboardLayout : DashboardLayoutBase
{
	public DashboardLayout()
	{
		MenuNames = [
			DashboardMenuNames.Main,
			DashboardMenuNames.Info
		];
	}
}