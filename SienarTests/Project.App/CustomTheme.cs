using MudBlazor;

namespace Project.App;

public class CustomTheme : MudTheme
{
	/// <inheritdoc />
	public CustomTheme()
	{
		Palette.AppbarBackground = Colors.Orange.Accent1;
	}
}