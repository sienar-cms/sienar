using Sienar.Layouts;

namespace TestProject.Web.Layouts;

public class MainAppLayout : TopNavLayout
{
	/// <inheritdoc />
	public MainAppLayout()
	{
		MenuNames = [Constants.MenuNames.MainMenu];
	}
}