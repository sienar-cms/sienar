namespace Sienar.Docs;

public static class Urls
{
	public const string Overview = "/";
	public const string TableOfContents = "/table-of-contents";

	public static class Api
	{
		private const string Base = "/api";
		public const string SienarServerAppBuilder = $"{Base}/{nameof(SienarServerAppBuilder)}";
	}

	public static class Introduction
	{
		private const string Base = "/introduction";
		public const string GettingStarted = $"{Base}/getting-started";
		public const string Basics = $"{Base}/basics";
	}
}