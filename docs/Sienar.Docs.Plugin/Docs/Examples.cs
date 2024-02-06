// ReSharper disable MemberHidesStaticFromOuterClass
namespace Sienar.Docs;

public static class Examples
{
	private const string Root = "/_content/Sienar.Docs.Plugin/Examples";

	public static class Introduction
	{
		private const string Root = $"{Examples.Root}/Introduction";
		public const string StandardBoilerplate = $"{Root}/StandardBoilerplate";
	}

	public static class Api
	{
		private const string Root = $"{Examples.Root}/API";

		public static class SienarServerAppBuilder
		{
			private const string Root = $"{Api.Root}/SienarServerAppBuilder";
			public const string Create = $"{Root}/Create";
			public const string CreateWithDbContext = $"{Root}/CreateWithDbContext";
			public const string Build = $"{Root}/Build";
			public const string AddPlugin = $"{Root}/AddPlugin";
			public const string AddServices = $"{Root}/AddServices";
			public const string ConfigureTheme = $"{Root}/ConfigureTheme";
		}
	}
}