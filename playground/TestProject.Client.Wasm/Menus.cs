using System.ComponentModel;

namespace TestProject.Client;

/// <summary>
/// The test app's supported menus
/// </summary>
public enum Menus
{
	/// <summary>
	/// The main menu
	/// </summary>
	Main,

	/// <summary>
	/// The menu containing social media links
	/// </summary>
	Social,

	/// <summary>
	/// The menu containing links to common hobbies
	/// </summary>
	Hobbies,

	/// <summary>
	/// The menu containing links to sports networks
	/// </summary>
	Sports,

	/// <summary>
	/// The menu containing links to operating systems
	/// </summary>
	[Description("Operating systems")]
	OperatingSystems
}