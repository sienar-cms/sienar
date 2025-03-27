using Microsoft.AspNetCore.Builder;
using Sienar.Extensions;
using Sienar.Infrastructure;
using TestProject;

await SienarAppBuilder
	.Create(args)
	.AddWebAdapter()
	.AddPlugin<TestProjectServerPlugin>()
	.Build<WebApplication>()
	.RunAsync();
