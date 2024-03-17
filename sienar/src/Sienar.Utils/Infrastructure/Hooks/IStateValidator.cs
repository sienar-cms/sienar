using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IStateValidator<TEntity>
{
	/// <summary>
	/// Validates that an entity does not violate logical rules of the app state (for example, checking fields for uniqueness against the database)
	/// </summary>
	/// <param name="entity">The entity to validate against the current app state</param>
	/// <param name="action">The type of action</param>
	Task<HookStatus> Validate(TEntity entity, ActionType action);
}