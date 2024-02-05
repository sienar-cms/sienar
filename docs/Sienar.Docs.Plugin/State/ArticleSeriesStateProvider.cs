namespace Sienar.State;

public class ArticleSeriesStateProvider : StateProviderBase
{
	private string? _currentRoute;
	private string? _series;

	public string? CurrentRoute
	{
		get => _currentRoute;
		set
		{
			if (_currentRoute == value) return;
			_currentRoute = value;
			NotifyStateChanged();
		}
	}

	public string? Series
	{
		get => _series;
		set
		{
			if (_series == value) return;
			_series = value;
			NotifyStateChanged();
		}
	}
}