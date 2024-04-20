using System.Collections.Generic;
using System.Reflection;

namespace Sienar.Infrastructure.Plugins;

/// <summary>
/// A container for assemblies containing routable Blazor components
/// </summary>
public interface IRoutableAssemblyProvider : IList<Assembly>;