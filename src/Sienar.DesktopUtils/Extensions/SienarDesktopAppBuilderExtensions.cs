using System;
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Sienar.Exceptions;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

public static class SienarDesktopAppBuilderExtensions
{
	public const string ElectronConfigurerKey = "ElectronConfigurer";

	public static SienarServerAppBuilder AddElectron(this SienarServerAppBuilder self)
	{
		self.Builder.WebHost.UseElectron(self.StartupArgs);
		return self;
	}

	public static SienarServerAppBuilder ConfigureElectron(
		this SienarServerAppBuilder self,
		Action configurer)
	{
		self.CustomItems[ElectronConfigurerKey] = configurer;
		return self;
	}

	public static WebApplication BuildElectron(this SienarServerAppBuilder self)
	{
		var app = self.Build();

		if (self.CustomItems[ElectronConfigurerKey] is not Action electronConfigurer)
		{
			throw new SienarException(
				$"You must pass a delegate to the  {nameof(SienarDesktopAppBuilderExtensions)}.{nameof(ConfigureElectron)} method prior to building a desktop app.");
		}

		if (HybridSupport.IsElectronActive) electronConfigurer.Invoke();
		return app;
	}
}