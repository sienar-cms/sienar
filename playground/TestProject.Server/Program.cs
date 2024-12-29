using Microsoft.AspNetCore.Builder;
using Sienar.Extensions;
using Sienar.Infrastructure;
using TestProject;
using TestProject.Client;

await SienarAppBuilder
	.Create(args)
	.AddWebAdapter()
	.AddPlugin<TestProjectServerPlugin>()
	.AddPlugin<TestProjectClientPlugin>()
	.Build<WebApplication>()
	.RunAsync();
