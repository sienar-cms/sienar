using System.Collections.Generic;
using System.Reflection;

namespace Sienar.Infrastructure;

/// <summary>
/// A container for assemblies containing routable Blazor components
/// </summary>
public class RoutableAssemblyProvider : List<Assembly>;
