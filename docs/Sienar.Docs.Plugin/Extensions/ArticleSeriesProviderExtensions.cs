using System;
using Sienar.Infrastructure.Articles;
using Sienar.Pages;
using Sienar.Pages.API;
using Sienar.Pages.Introduction;

namespace Sienar.Extensions;

public static class ArticleSeriesProviderExtensions
{
	public static IArticleSeriesProvider AddSeries(this IArticleSeriesProvider self)
		=> self
			.AddArticlePage<GettingStarted>()
			.AddArticlePage<Basics>()
			.AddArticlePage<SienarServerAppBuilder>();

	public static IArticleSeriesProvider AddArticlePage<TPage>(
		this IArticleSeriesProvider self)
		where TPage : DocPageBase
	{
		var invalid = new InvalidOperationException(
			"Article series pages must define [Route], [Series],"
			+ " and [Title] attributes with non-null values");

		var pageType = typeof(TPage);
		var series = pageType.GetSeries() ?? throw invalid;
		var route = pageType.GetRoute() ?? throw invalid;
		var title = pageType.GetTitle() ?? throw invalid;

		self
			.Access(series)
			.AddArticle(title, route);

		return self;
	}
}