using System;
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Exceptions;

namespace Sienar.Infrastructure;

public class SienarDesktopAppBuilder : SienarWebAppBuilder
{
	public Action? ElectronConfigurer;

	protected SienarDesktopAppBuilder(SienarWebAppBuilder builder)
		: base(builder.Builder) {}

	public new static SienarDesktopAppBuilder Create<TContext>(
		string[] args,
		Action<DbContextOptionsBuilder>? dbContextOptionsConfigurer = null,
		ServiceLifetime dbContextLifetime = ServiceLifetime.Scoped,
		ServiceLifetime dbContextOptionsLifetime = ServiceLifetime.Scoped)
		where TContext : DbContext
	{
		var builder = SienarWebAppBuilder.Create<TContext>(
			args,
			dbContextOptionsConfigurer,
			dbContextLifetime,
			dbContextOptionsLifetime);

		builder.Builder.WebHost.UseElectron(args);
		return new SienarDesktopAppBuilder(builder);
	}

	public new static SienarDesktopAppBuilder Create(string[] args)
	{
		var builder = SienarWebAppBuilder.Create(args);
		builder.Builder.WebHost.UseElectron(args);
		return new SienarDesktopAppBuilder(builder);
	}

	public override WebApplication Build()
	{
		// Build
		var app = base.Build();

		// Run Electron
		if (ElectronConfigurer is null)
		{
			throw new SienarException($"You must pass a delegate to the {nameof(SienarDesktopAppBuilderExtensions)}.{nameof(SienarDesktopAppBuilderExtensions.ConfigureElectron)} method when configuring the desktop app using the {nameof(SienarDesktopAppBuilder)} class. Alternatively, you can set the {nameof(SienarDesktopAppBuilder)}.{nameof(ElectronConfigurer)} field directly.");
		}

		if (HybridSupport.IsElectronActive)
		{
			ElectronConfigurer.Invoke();
		}

		// Done
		return app;
	}
}