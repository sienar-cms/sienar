using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Articles;
using Sienar.State;

namespace Sienar.Pages;

public class DocPageBase : ComponentBase
{
	[Inject]
	private ArticleSeriesStateProvider ArticleState { get; set; } = default!;

	[Inject]
	private ILogger<DocPageBase> Logger { get; set; } = default!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		ArticleState.Freeze();
		var type = GetType();
		ArticleState.CurrentRoute = type
			.GetCustomAttribute<RouteAttribute>()?
			.Template;
		ArticleState.Series = type
			.GetCustomAttribute<SeriesAttribute>()?
			.Series;
		ArticleState.Unfreeze();
	}
}