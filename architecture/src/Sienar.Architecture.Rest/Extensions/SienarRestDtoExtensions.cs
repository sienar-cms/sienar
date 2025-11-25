using System;
using System.Reflection;
using Sienar.Data;

namespace Sienar.Extensions;

public static class SienarRestDtoExtensions
{
	/// <summary>
	/// Looks for the presence of a <see cref="RestEndpointAttribute"/> and returns its endpoint value
	/// </summary>
	/// <param name="self">The DTO</param>
	/// <typeparam name="TDto">The type of the DTO</typeparam>
	/// <returns>The REST endpoint, if defined</returns>
	public static string? GetRestEndpoint<TDto>(this TDto? self)
		where TDto : EntityBase
		=> GetRestEndpoint(typeof(TDto));

	/// <summary>
	/// Looks for the presence of a <see cref="RestEndpointAttribute"/> and returns its endpoint value
	/// </summary>
	/// <param name="self">The type of the DTO</param>
	/// <returns>The REST endpoint, if defined</returns>
	public static string? GetRestEndpoint(this Type self)
		=> self.GetCustomAttribute<RestEndpointAttribute>()?.Endpoint;
}
