using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <summary>
/// A service to delete instances of <c>TEntity</c> from the appropriate repository
/// </summary>
/// <typeparam name="TEntity">the type of the entity</typeparam>
public interface IEntityDeleter<TEntity>
{
	/// <summary>
	/// Deletes an entity by primary key
	/// </summary>
	/// <param name="id">The primary key of the entity to delete</param>
	/// <returns>whether the delete operation was successful</returns>
	Task<OperationResult<bool>> Delete(int id);
}