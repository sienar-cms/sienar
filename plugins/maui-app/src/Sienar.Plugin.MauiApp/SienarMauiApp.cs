using Microsoft.Maui.Controls;

namespace Sienar;

public class SienarMauiApp : Application
{
	public SienarMauiApp()
	{
		MainPage = new SienarMauiBlazorMainPage();
	}
}