using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public interface IRootComponentProvider
{
	Dictionary<string, Type> RootComponents { get; }

	IRootComponentProvider AddRootComponent(string selector, Type componentType);
}