using System.Collections.Generic;
using System.Threading.Tasks;
using Sienar.Identity;

namespace Sienar.Infrastructure.Hooks;

public interface IUserPersonalDataRetriever
{
	/// <summary>
	/// Retrieves personal data 
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	Task<Dictionary<string, string>> GetUserData(SienarUser user);
}