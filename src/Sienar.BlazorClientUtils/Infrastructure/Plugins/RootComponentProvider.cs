using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public class RootComponentProvider : IRootComponentProvider
{
	/// <inheritdoc />
	public Dictionary<string, Type> RootComponents { get; } = new();

	/// <inheritdoc />
	public IRootComponentProvider AddRootComponent(string selector, Type componentType)
	{
		RootComponents[selector] = componentType;
		return this;
	}
}