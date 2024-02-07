using System;

namespace Sienar.Infrastructure;

public static class SienarDesktopAppBuilderExtensions
{
	public static TBuilder ConfigureElectron<TBuilder>(
		this TBuilder self,
		Action configurer)
		where TBuilder : SienarDesktopAppBuilder
	{
		self.ElectronConfigurer = configurer;
		return self;
	}
}