using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Generates endpoints for a given CRUD DTO
/// </summary>
// ReSharper disable once TypeParameterCanBeVariant
public interface ICrudEndpointGenerator<T>
	where T : EntityBase
{
	/// <summary>
	/// Generates a URL to perform a read operation on a single DTO
	/// </summary>
	/// <param name="id">The ID of the DTO to read</param>
	/// <returns>The URL</returns>
	string GenerateReadUrl(int id);

	/// <summary>
	/// Generates a URL to perform a read operation on multiple DTOs
	/// </summary>
	/// <returns>The URL</returns>
	string GenerateReadUrl();

	/// <summary>
	/// Generates a URL to perform a create operation on a DTO
	/// </summary>
	/// <param name="dto">The DTO to create</param>
	/// <returns>The URL</returns>
	string GenerateCreateUrl(T dto);

	/// <summary>
	/// Generates a URL to perform an update operation on a DTO
	/// </summary>
	/// <param name="dto">The DTO to update</param>
	/// <returns>The URL</returns>
	string GenerateUpdateUrl(T dto);

	/// <summary>
	/// Generates a URL to perform a delete operation on a DTO
	/// </summary>
	/// <param name="id">The ID of the entity to delete</param>
	/// <returns>The URL</returns>
	string GenerateDeleteUrl(int id);
}
