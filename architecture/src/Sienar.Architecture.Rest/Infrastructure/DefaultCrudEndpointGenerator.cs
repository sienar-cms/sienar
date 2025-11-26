using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Generates endpoints for a given CRUD DTO by locating its <see cref="RestEndpointAttribute"/>
/// </summary>
public class DefaultCrudEndpointGenerator<T> : ICrudEndpointGenerator<T>
	where T : EntityBase
{
	/// <inheritdoc />
	public string GenerateReadUrl(int id)
	{
		var endpoint = GetRestEndpoint();
		return $"{endpoint}/{id}";
	}

	/// <inheritdoc />
	public string GenerateReadUrl() => GetRestEndpoint();

	/// <inheritdoc />
	public string GenerateCreateUrl(T dto) => GetRestEndpoint();

	/// <inheritdoc />
	public string GenerateUpdateUrl(T dto) => GetRestEndpoint();

	/// <inheritdoc />
	public string GenerateDeleteUrl(int id)
	{
		var endpoint = GetRestEndpoint();
		return $"{endpoint}/{id}";
	}

	private static string GetRestEndpoint()
	{
		return typeof(T)
			.GetCustomAttribute<RestEndpointAttribute>()
			?.Endpoint
				?? throw new InvalidOperationException($"Cannot determine REST endpoint for DTO of type {typeof(T)}");
	}
}
