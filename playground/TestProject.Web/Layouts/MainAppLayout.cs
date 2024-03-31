using Sienar.Layouts;

namespace TestProject.Web.Layouts;

public class MainAppLayout : TopNavLayoutBase
{
	/// <inheritdoc />
	public MainAppLayout()
	{
		MenuNames = [Constants.MenuNames.MainMenu];
	}
}