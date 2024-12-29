// var builder = WebAssemblyHostBuilder.CreateDefault(args);
// builder.RootComponents.Add<App>("#app");
// builder.RootComponents.Add<HeadOutlet>("head::after");
//
// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//
// await builder.Build().RunAsync();

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sienar.Extensions;
using Sienar.Infrastructure;
using TestProject.Client;

await SienarAppBuilder
	.Create(args)
	.AddWasmAdapter()
	.AddPlugin<TestProjectClientPlugin>()
	.Build<WebAssemblyHost>()
	.RunAsync();

// await SienarWasmAppBuilder
// 	.Create<Program>(args)
// 	.AddPlugin<Wasm>()
// 	.AddPlugin<SienarCmsClient>()
// 	.AddComponent<SienarApp>("#app")
// 	.AddComponent<SienarHead>("head::after")
// 	.SetupDependencies(builder =>
// 	{
// 		builder.Services
// 			.AddStatusProcessor<LoginRequest, LoginProcessor>()
// 			.AddDefaultTheme();
// 	})
// 	.SetupApp(app =>
// 	{
// 		app.Services
// 			.ConfigureComponents(
// 				p =>
// 				{
// 					p.DefaultLayout = typeof(MainAppLayout);
// 					p.AppbarLeft = typeof(Branding);
// 				})
// 			.ConfigureMenu(p => p.AddMenu())
// 			.ConfigureStyles(p =>
// 			{
// 				p.Add("/styles.css");
// 				p.Add("/TestProject.Client.Wasm.styles.css");
// 			});
// 	})
// 	.Build()
// 	.RunAsync();