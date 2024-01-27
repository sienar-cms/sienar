using System.Collections.Generic;
using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public interface IRoleService
{
	/// <summary>
	/// Gets a list of all roles in the app
	/// </summary>
	/// <returns>the roles</returns>
	Task<IEnumerable<SienarRole>> Get();
}