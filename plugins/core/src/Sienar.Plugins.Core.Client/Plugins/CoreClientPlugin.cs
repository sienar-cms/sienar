using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Plugins;

/// <summary>
/// configures the Sienar app to run with Blazor WASM application support
/// </summary>
public class CoreClientPlugin : IPlugin
{
	private readonly IApplicationAdapter _adapter;

	/// <summary>
	/// Creates a new instance of <c>WasmPlugin</c>
	/// </summary>
	/// <param name="adapter"></param>
	public CoreClientPlugin(IApplicationAdapter adapter)
	{
		_adapter = adapter;
	}

	/// <inheritdoc />
	public void Configure()
	{
		if (_adapter.ApplicationType is not ApplicationType.Client) return;

		_adapter.AddServices(sp =>
		{
			sp.TryAddScoped<IUserAccessor, BlazorUserAccessor>();

			sp.AddAuthorizationCore();
		});
	}
}
