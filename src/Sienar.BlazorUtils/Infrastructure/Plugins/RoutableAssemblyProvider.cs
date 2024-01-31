using System.Collections.Generic;
using System.Reflection;

namespace Sienar.Infrastructure.Plugins;

public class RoutableAssemblyProvider : IRoutableAssemblyProvider
{
	private readonly List<Assembly> _assemblies = [];

	/// <inheritdoc />
	public IRoutableAssemblyProvider Add(Assembly assembly)
	{
		_assemblies.Add(assembly);
		return this;
	}

	/// <inheritdoc />
	public List<Assembly> GetAssemblies() => _assemblies;
}