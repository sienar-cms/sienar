using Sienar.Infrastructure;

namespace Sienar.Layouts;

public class DashboardLayout : MauiLayoutBase
{
	public DashboardLayout()
	{
		MenuNames = [
			DashboardMenuNames.MainMenu,
			DashboardMenuNames.InfoMenu
		];
	}
}