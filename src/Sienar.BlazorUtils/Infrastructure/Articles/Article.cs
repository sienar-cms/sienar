namespace Sienar.Infrastructure.Articles;

public class Article
{
	public string Name { get; set; } = string.Empty;
	public string Url { get; set; } = string.Empty;

	public Article() {}

	public Article(string name, string url)
	{
		Name = name;
		Url = url;
	}
}