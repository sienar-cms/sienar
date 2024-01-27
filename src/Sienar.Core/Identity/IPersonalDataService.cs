using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public interface IPersonalDataService
{
	/// <summary>
	/// Returns the personal data of the currently logged in user, in binary format
	/// </summary>
	/// <returns>a file containing the user's personal data</returns>
	Task<FileDto?> GetPersonalData();
}