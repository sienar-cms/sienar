using MudBlazor;

namespace Project.App.Blazor;

public class CustomTheme : MudTheme
{
	/// <inheritdoc />
	public CustomTheme()
	{
		Palette.AppbarBackground = Colors.Orange.Accent1;
	}
}