#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Reflection;

namespace Sienar.Infrastructure.Plugins;

/// <exclude />
public class RoutableAssemblyProvider : ListProvider<Assembly>, IRoutableAssemblyProvider;