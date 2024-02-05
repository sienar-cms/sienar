using System;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Sienar.Infrastructure.Articles;

namespace Sienar.Extensions;

public static class PageExtensions
{
	public static string? GetRoute<TPage>()
		where TPage : ComponentBase
		=> GetRoute(typeof(TPage));

	public static string? GetRoute(this Type pageType)
		=> pageType.GetCustomAttribute<RouteAttribute>()?.Template;

	public static string? GetSeries<TPage>()
		where TPage : ComponentBase
		=> GetSeries(typeof(TPage));

	public static string? GetSeries(this Type pageType)
		=> pageType.GetCustomAttribute<SeriesAttribute>()?.Series;

	public static string? GetTitle<TPage>()
		where TPage : ComponentBase
		=> GetTitle(typeof(TPage));

	public static string? GetTitle(this Type pageType)
		=> pageType.GetCustomAttribute<TitleAttribute>()?.Title;
}