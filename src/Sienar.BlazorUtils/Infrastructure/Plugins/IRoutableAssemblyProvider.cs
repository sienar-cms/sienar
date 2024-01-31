using System.Collections.Generic;
using System.Reflection;

namespace Sienar.Infrastructure.Plugins;

public interface IRoutableAssemblyProvider
{
	/// <summary>
	/// Adds an assembly to the list of assemblies with routable components
	/// </summary>
	/// <param name="assembly">The assembly to add</param>
	/// <returns>this</returns>
	IRoutableAssemblyProvider Add(Assembly assembly);

	/// <summary>
	/// Gets all plugins in the plugin provider
	/// </summary>
	/// <returns>the list of plugins</returns>
	List<Assembly> GetAssemblies();
}