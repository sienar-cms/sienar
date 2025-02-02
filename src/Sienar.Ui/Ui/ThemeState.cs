using Sienar.Infrastructure;

namespace Sienar.Ui;

/// <summary>
/// Contains the state required to render the current theme
/// </summary>
public class ThemeState : StateProviderBase
{
	private bool _isDarkMode = true;
	private SienarTheme _theme = new();

	/// <summary>
	/// Whether the current theme should render in dark mode
	/// </summary>
	public bool IsDarkMode
	{
		get => _isDarkMode;
		set
		{
			if (_isDarkMode == value)
			{
				return;
			}

			_isDarkMode = value;
			NotifyStateChanged();
		}
	}

	/// <summary>
	/// The current theme object
	/// </summary>
	public SienarTheme Theme
	{
		get => _theme;
		set
		{
			if (_theme == value)
			{
				return;
			}

			_theme = value;
			NotifyStateChanged();
		}
	}
}