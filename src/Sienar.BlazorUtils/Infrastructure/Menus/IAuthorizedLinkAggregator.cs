using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public interface IAuthorizedLinkAggregator<TLink>
	where TLink : DashboardLink
{
	/// <summary>
	/// Creates a list of authorized <see cref="TLink"/> to be rendered
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	List<TLink> Create(string name);
}