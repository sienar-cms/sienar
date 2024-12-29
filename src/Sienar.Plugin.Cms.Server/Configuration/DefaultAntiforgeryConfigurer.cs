using Microsoft.AspNetCore.Antiforgery;
using Sienar.Infrastructure;

namespace Sienar.Configuration;

/// <summary>
/// Configures ASP.NET Antiforgery to expect a header named <c>X-XSRF-TOKEN</c>
/// </summary>
public class DefaultAntiforgeryConfigurer : IConfigurer<AntiforgeryOptions>
{
	/// <inheritdoc />
	public void Configure(AntiforgeryOptions options)
	{
		options.HeaderName = "X-XSRF-TOKEN";
	}
}