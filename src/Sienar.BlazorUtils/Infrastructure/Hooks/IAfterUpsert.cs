using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public interface IAfterUpsert<TEntity>
{
	Task<HookStatus> Handle(TEntity entity, bool isAdding);
}