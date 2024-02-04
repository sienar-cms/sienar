using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

await SienarClientAppBuilder
	.Create(args)
	.AddPlugin(new SienarDocsPlugin())
	.ConfigureTheme(new MudTheme())
	.Build()
	.RunAsync();

// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Components.Web;
// using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
// using Microsoft.Extensions.DependencyInjection;
// using MudBlazor;
// using MudBlazor.Services;
// using Sienar;
// using Sienar.Docs;
// using Sienar.Docs.Layouts;
// using Sienar.Extensions;
// using Sienar.Infrastructure;
// using Sienar.Infrastructure.Plugins;
// using Sienar.State;
//
// await SienarClientAppBuilder
// 	.Create(args)
// 	.ConfigureTheme(new MudTheme())
// 	.Build()
// 	.RunAsync();
//
// builder.Builder.RootComponents.Add<SienarApp>("#app");
// builder.RootComponents.Add<HeadOutlet>("head::after");
//
// builder.Services
// 	.AddScoped<IComponentProvider>(_ => new ComponentProvider(typeof(DocsLayout)))
// 	.AddScoped<IUserAccessor, NullUserAccessor>()
// 	.AddScoped<AuthenticationStateProvider, AuthStateProvider>()
// 	.AddAuthorizationCore()
// 	.AddCascadingAuthenticationState()
// 	.AddMudServices();
//
// await builder.Build().RunAsync();
//
// class AuthStateProvider : AuthenticationStateProvider
// {
// 	/// <inheritdoc />
// 	public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
// }