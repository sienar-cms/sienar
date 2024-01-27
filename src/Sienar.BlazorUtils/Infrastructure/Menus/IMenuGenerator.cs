using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public interface IMenuGenerator
{
	/// <summary>
	/// Creates a list of <see cref="MenuLink"/> to be rendered in the dashboard menu
	/// </summary>
	/// <param name="menuName">The name of the menu to create</param>
	/// <returns>the list of <see cref="MenuLink"/> to render</returns>
	List<MenuLink> CreateMenu(string menuName);
}