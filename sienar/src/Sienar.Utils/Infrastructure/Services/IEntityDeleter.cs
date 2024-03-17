using System;
using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

public interface IEntityDeleter<TEntity>
{
	/// <summary>
	/// Deletes an entity by primary key
	/// </summary>
	/// <param name="id">The primary key of the entity to delete</param>
	/// <returns>whether the delete operation was successful</returns>
	Task<bool> Delete(Guid id);
}