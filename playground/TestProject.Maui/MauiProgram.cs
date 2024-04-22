using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

namespace TestProject.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		return SienarDesktopAppBuilder
#if DEBUG
			.Create(addDebugServices: true)
#else
			.Create()
#endif
			.UseSienarApplication(typeof(MauiProgram).Assembly)
			.AddPlugin<SienarMauiBlazorPlugin>()
			.ConfigureTheme<MudTheme>()
			.Build();
	}
}