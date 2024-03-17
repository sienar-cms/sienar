using MudBlazor;

namespace Sienar.State;

public class ThemeState : StateProviderBase
{
	private bool _isDarkMode = true;
	private MudTheme _theme = new();

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

	public MudTheme Theme
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