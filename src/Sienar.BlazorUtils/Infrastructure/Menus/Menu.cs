using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public class Menu : Dictionary<MenuPriority, List<MenuLink>>
{
	/// <summary>
	/// Adds a navigation link to the active menu at the given priority
	/// </summary>
	/// <param name="menuLink">The nav link to add to the menu</param>
	/// <param name="priority">The priority at which to add the nav link to the menu</param>
	public Menu AddMenuLink(
		MenuLink menuLink,
		MenuPriority priority = MenuPriority.Mid)
	{
		if (!TryGetValue(priority, out var menuItems))
		{
			menuItems = [];
			this[priority] = menuItems;
		}

		menuItems.Add(menuLink);

		return this;
	}
}