namespace Sienar.Infrastructure.Menus;

public interface IMenu
{
	/// <summary>
	/// Adds a navigation link to the active menu at the given priority
	/// </summary>
	/// <param name="menuLink">The nav link to add to the menu</param>
	/// <param name="priority">The priority at which to add the nav link to the menu</param>
	IMenu AddMenuLink(
		MenuLink menuLink,
		MenuPriority priority = MenuPriority.Mid);
}