// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// Represents a tab that can be rendered by a <see cref="TabGroup"/>
/// </summary>
public interface ITab
{
	/// <summary>
	/// The tab title to display
	/// </summary>
	string? Title { get; }

	/// <summary>
	/// The tab icon to display
	/// </summary>
	string? Icon { get; }

	/// <summary>
	/// Informs a tab that it is currently active and should render itself
	/// </summary>
	void SetActive();

	/// <summary>
	/// Informs a tab that it is currently inactive and should not render itself
	/// </summary>
	void SetInactive();
}