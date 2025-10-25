using System.Net.Http;

namespace Sienar.Infrastructure;

/// <summary>
/// A wrapper around an <see cref="HttpRequestMessage"/> that enables developers to create hooks typed to specific HTTP clients
/// </summary>
/// <typeparam name="TClient">The type of the HTTP client for which a hook should run</typeparam>
public class RestClientRequest<TClient>
{
	/// <summary>
	/// The current request's <see cref="HttpRequestMessage"/>
	/// </summary>
	public required HttpRequestMessage RequestMessage { get; set; }
}
