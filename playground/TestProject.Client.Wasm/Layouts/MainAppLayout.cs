using Sienar.Layouts;

namespace TestProject.Client.Layouts;

public class MainAppLayout : DashboardLayoutBase
{
	/// <inheritdoc />
	public MainAppLayout()
	{
		MenuNames = [Menus.Main];
	}
}