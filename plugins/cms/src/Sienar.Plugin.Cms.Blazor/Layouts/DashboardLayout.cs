using Sienar.Infrastructure;

namespace Sienar.Layouts;

public class DashboardLayout : CmsLayoutBase
{
	public DashboardLayout()
	{
		MenuNames = [
			DashboardMenuNames.MainMenu,
			DashboardMenuNames.InfoMenu
		];
	}
}