using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public interface IEntityStateValidator<TEntity>
{
	/// <summary>
	/// Validates that an entity does not violate logical rules of the app state (for example, checking fields for uniqueness against the database)
	/// </summary>
	/// <param name="entity">The entity to validate against the current app state</param>
	/// <param name="adding">True if the entity is being newly created, else false</param>
	Task<bool> IsValid(TEntity entity, bool adding);
}