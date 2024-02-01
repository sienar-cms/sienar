using Sienar.Infrastructure;
using Sienar.Layouts;

namespace Sienar;

public class DashboardLayout : SienarLayoutBase
{
	public DashboardLayout()
	{
		MenuNames = [
			DashboardMenuNames.MainMenu,
			DashboardMenuNames.InfoMenu
		];
	}
}