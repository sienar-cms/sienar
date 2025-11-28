// ReSharper disable TypeParameterCanBeVariant
namespace Sienar.Data;

/// <summary>
/// Maps between entities and their associated DTOs
/// </summary>
/// <typeparam name="TDto">The type of the DTO</typeparam>
/// <typeparam name="TEntity">The type of the entity</typeparam>
public interface IMapper<TDto, TEntity>
	where TDto : class
	where TEntity : EntityBase
{
	/// <summary>
	/// Maps a DTO to its associated entity
	/// </summary>
	/// <param name="dto">The DTO to map</param>
	/// <param name="entity">The entity to which to map</param>
	void MapToEntity(TDto dto, TEntity entity);

	/// <summary>
	/// Maps an entity to its associated DTO
	/// </summary>
	/// <param name="dto">The DTO to which to map</param>
	/// <param name="entity">The entity to map</param>
	void MapToDto(TDto dto, TEntity entity);
}
