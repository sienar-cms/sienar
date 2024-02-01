using System;
using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

// ReSharper disable once TypeParameterCanBeVariant
public interface IEntityWriter<TEntity>
{
	/// <summary>
	/// Creates a new entry in the database
	/// </summary>
	/// <param name="model">The entity to create</param>
	/// <returns>the <see cref="Guid"/> representing the entity's primary key</returns>
	Task<Guid> Add(TEntity model);

	/// <summary>
	/// Updates an existing entity in the database
	/// </summary>
	/// <param name="model">The entity to update</param>
	/// <returns>whether the edit operation was successful</returns>
	Task<bool> Edit(TEntity model);
}