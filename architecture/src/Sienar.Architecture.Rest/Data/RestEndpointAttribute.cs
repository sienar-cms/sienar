using System;

namespace Sienar.Data;

/// <summary>
/// Describes a RESTful endpoint for a DTO
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RestEndpointAttribute : Attribute
{
	/// <summary>
	/// The REST endpoint
	/// </summary>
	public string Endpoint { get; }

	/// <summary>
	/// Creates a new instance of <c>RestEndpointAttribute</c>
	/// </summary>
	/// <param name="endpoint"></param>
	public RestEndpointAttribute(string endpoint)
		=> Endpoint = endpoint;
}
