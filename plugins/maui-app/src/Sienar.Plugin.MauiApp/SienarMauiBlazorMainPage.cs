using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Controls;
using Sienar.UI;

namespace Sienar;

public class SienarMauiBlazorMainPage : ContentPage
{
	public SienarMauiBlazorMainPage()
	{
		var view = new BlazorWebView { HostPage = "wwwroot/sienar.html" };
		view.RootComponents.Add(
			new()
			{
				Selector = "head::after",
				ComponentType = typeof(SienarHead)
			});
		view.RootComponents.Add(
			new()
			{
				Selector = "#app",
				ComponentType = typeof(SienarRoutes)
			});
		Content = view;
	}
}