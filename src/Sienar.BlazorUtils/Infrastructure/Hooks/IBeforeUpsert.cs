using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IBeforeUpsert<TEntity>
{
	Task<HookStatus> Handle(TEntity entity, bool isAdding);
}