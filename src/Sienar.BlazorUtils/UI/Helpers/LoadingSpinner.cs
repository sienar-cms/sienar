using MudBlazor;

namespace Sienar.UI.Helpers;

public class LoadingSpinner : MudProgressCircular
{
	public LoadingSpinner()
	{
		Color = Color.Primary;
		Indeterminate = true;
	}
}