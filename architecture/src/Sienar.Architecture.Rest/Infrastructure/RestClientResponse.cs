using System.Net.Http;

namespace Sienar.Infrastructure;

/// <summary>
/// A wrapper around an <see cref="HttpResponseMessage"/> that enables developers to create hooks typed to specific HTTP clients
/// </summary>
/// <typeparam name="TClient">The type of the HTTP client for which a hook should run</typeparam>
public class RestClientResponse<TClient>
{
	/// <summary>
	/// The current request's <see cref="HttpResponseMessage"/>
	/// </summary>
	public required HttpResponseMessage ResponseMessage { get; set; }
}
