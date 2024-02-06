using Microsoft.AspNetCore.Components;
using Sienar.Extensions;
using Sienar.State;

namespace Sienar.Pages;

public class DocPageBase : ComponentBase
{
	protected string Title = string.Empty;

	[Inject]
	private ArticleSeriesStateProvider ArticleState { get; set; } = default!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		ArticleState.Freeze();
		var type = GetType();
		ArticleState.CurrentRoute = type.GetRoute();
		ArticleState.Series = type.GetSeries();
		ArticleState.Unfreeze();

		Title = type.GetTitle()!;
	}
}