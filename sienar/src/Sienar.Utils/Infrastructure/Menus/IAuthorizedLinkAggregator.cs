using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sienar.Infrastructure.Menus;

public interface IAuthorizedLinkAggregator<TLink>
	where TLink : DashboardLink
{
	/// <summary>
	/// Creates a list of authorized <see cref="TLink"/> to be rendered
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	Task<List<TLink>> Create(string name);
}