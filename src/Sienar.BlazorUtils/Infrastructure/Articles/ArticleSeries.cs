using System.Collections.Generic;

namespace Sienar.Infrastructure.Articles;

public class ArticleSeries
{
	private readonly List<Article> _articles = [];

	/// <summary>
	/// Adds an article to the current series
	/// </summary>
	/// <param name="article">the article to add</param>
	public ArticleSeries AddArticle(Article article)
	{
		_articles.Add(article);
		return this;
	}

	/// <summary>
	/// Creates an article and adds it to the current series using the supplied data
	/// </summary>
	/// <param name="name">the name of the article</param>
	/// <param name="url">the URL of the article</param>
	public ArticleSeries AddArticle(string name, string url)
		=> AddArticle(new(name, url));

	/// <summary>
	/// Gets the next article in the series based on the URL of the current article
	/// </summary>
	/// <param name="url">the URL of the current article</param>
	/// <returns>the next article if it exists, else <c>null</c></returns>
	public Article? GetNextArticle(string url)
	{
		var i = _articles.FindIndex(a => a.Url == url);
		if (i == -1 || i >= _articles.Count - 1) return null;
		return _articles[i + 1];
	}

	/// <summary>
	/// Gets the previous article in the series based on the URL of the current article
	/// </summary>
	/// <param name="url">the URL of the current article</param>
	/// <returns>the previous article if it exists, else <c>null</c></returns>
	public Article? GetPreviousArticle(string url)
	{
		var i = _articles.FindIndex(a => a.Url == url);
		return i < 1
			? null
			: _articles[i - 1];
	}
}