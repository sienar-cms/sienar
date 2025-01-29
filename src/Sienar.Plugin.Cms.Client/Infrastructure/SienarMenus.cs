namespace Sienar.Infrastructure;

/// <summary>
/// The menus supported by Sienar CMS out of the box
/// </summary>
public enum SienarMenus
{
	/// <summary>
	/// The main dashboard menu, rendered in the side menu
	/// </summary>
	Main,

	/// <summary>
	/// The user settings menu, rendered by the user badge in the footer of the side menu
	/// </summary>
	UserSettings,

	/// <summary>
	/// The user logout menu, rendered by the user badge in the footer of the side menu
	/// </summary>
	UserLogout,

	/// <summary>
	/// The info menu, rendered in the side menu
	/// </summary>
	Info,

	/// <summary>
	/// The user management menu, rendered on the default dashboard homepage
	/// </summary>
	UserManagement
}