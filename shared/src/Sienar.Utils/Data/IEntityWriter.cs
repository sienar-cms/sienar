using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Data;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A service to write or update instances of <c>TEntity</c> in the appropriate repository
/// </summary>
/// <typeparam name="TEntity">the type of the entity</typeparam>
public interface IEntityWriter<TEntity>
{
	/// <summary>
	/// Creates a new entry in the database
	/// </summary>
	/// <param name="model">The entity to create</param>
	/// <returns>the entity's primary key</returns>
	Task<OperationResult<int?>> Create(TEntity model);

	/// <summary>
	/// Updates an existing entity in the database
	/// </summary>
	/// <param name="model">The entity to update</param>
	/// <returns>whether the edit operation was successful</returns>
	Task<OperationResult<bool>> Update(TEntity model);
}