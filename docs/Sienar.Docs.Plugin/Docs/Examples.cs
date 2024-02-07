// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable InconsistentNaming
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

		public static class ISienarPlugin
		{
			private const string Root = $"{Api.Root}/{nameof(ISienarPlugin)}";
			public const string PluginData = $"{Root}/{nameof(PluginData)}";
			public const string PluginSettings = $"{Root}/{nameof(PluginSettings)}";
			public const string PluginShouldExecute = $"{Root}/{nameof(PluginShouldExecute)}";
			public const string SetupApp = $"{Root}/{nameof(SetupApp)}";
			public const string SetupComponents = $"{Root}/{nameof(SetupComponents)}";
			public const string SetupDashboard = $"{Root}/{nameof(SetupDashboard)}";
			public const string SetupDependencies = $"{Root}/{nameof(SetupDependencies)}";
			public const string SetupMenu = $"{Root}/{nameof(SetupMenu)}";
			public const string SetupScripts = $"{Root}/{nameof(SetupScripts)}";
			public const string SetupStyles = $"{Root}/{nameof(SetupStyles)}";
		}

		public static class ISienarServerPlugin
		{
			private const string Root = $"{Api.Root}/{nameof(ISienarServerPlugin)}";
			public const string PluginShouldExecute = $"{Root}/{nameof(PluginShouldExecute)}";
			public const string SetupApp = $"{Root}/{nameof(SetupApp)}";
			public const string SetupDependencies = $"{Root}/{nameof(SetupDependencies)}";
		}

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