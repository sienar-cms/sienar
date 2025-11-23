using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Extensions;
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
	/// Creates a new instance of <c>CoreClientPlugin</c>
	/// </summary>
	/// <param name="adapter"></param>
	public CoreClientPlugin(IApplicationAdapter adapter)
	{
		_adapter = adapter;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_adapter.AddServices(sp =>
		{
			sp.AddSienarBlazorUtilities();

			if (_adapter.ApplicationType is not ApplicationType.Client)
			{
				return;
			}

			sp.TryAddScoped<IUserAccessor, BlazorUserAccessor>();

			sp.AddAuthorizationCore();
		});
	}
}
