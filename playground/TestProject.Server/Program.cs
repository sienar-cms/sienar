using Sienar.Infrastructure;
using TestProject;

await SienarAppBuilder
	.Create(args)
	.AddPlugin<TestProjectServerPlugin>()
	.Build()
	.RunAsync();
