namespace Sienar.Layouts;

public enum DashboardLayoutSections
{
	/// <summary>
	/// Content to render on the main dashboard page
	/// </summary>
	Dashboard,

	/// <summary>
	/// Content to render inside the appbar (on mobile). Pinned to the left side of the appbar
	/// </summary>
	AppbarLeft,

	/// <summary>
	/// Content to render inside the appbar (on mobile). Pinned to the right side of the appbar
	/// </summary>
	AppbarRight,

	/// <summary>
	/// Content to render inside the application sidebar, above the menu content 
	/// </summary>
	SidebarHeader,

	/// <summary>
	/// Content to render inside the application sidebar, below the menu content. This content is pinned to the bottom of the sidebar
	/// </summary>
	SidebarFooter
}