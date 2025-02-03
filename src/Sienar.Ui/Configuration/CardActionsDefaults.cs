using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for card actions
/// </summary>
public static class CardActionsDefaults
{
	/// <summary>
	/// The default card actions HTML tag
	/// </summary>
	public static string Tag { get; set; } = "footer";

	/// <summary>
	/// The defaut card action button size
	/// </summary>
	public static Size Size { get; set; } = Size.Normal;
}
