// ReSharper disable InconsistentNaming
namespace Sienar.Docs;

public static class Urls
{
	public const string Overview = "/";
	public const string TableOfContents = "/table-of-contents";

	public static class Api
	{
		private const string Base = "/api";
		public const string SienarServerAppBuilder = $"{Base}/{nameof(SienarServerAppBuilder)}";
		public const string ISienarPlugin = $"{Base}/{nameof(ISienarPlugin)}";
		public const string ISienarServerPlugin = $"{Base}/{nameof(ISienarServerPlugin)}";
	}

	public static class Guides
	{
		private const string Base = "/guides";
		public const string SubApps = $"{Base}/sub-apps";
	}

	public static class Introduction
	{
		private const string Base = "/introduction";
		public const string GettingStarted = $"{Base}/getting-started";
		public const string Basics = $"{Base}/basics";
		public const string Plugins = $"{Base}/plugins";
		public const string Hooks = $"{Base}/hooks";
	}
}