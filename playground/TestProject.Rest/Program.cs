using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;
using TestProject.Rest.Temp;

await SienarWebAppBuilder
	.Create(args, typeof(Program).Assembly)
	.AddPlugin<SienarRestPlugin>()
	.SetupDependencies(builder =>
	{
		builder.Services
			.AddSienarCoreUtilities()
			.AddStatusProcessor<TestRequest, TestProcessor>();
	})
	.Build()
	.RunAsync();