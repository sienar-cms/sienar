using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public interface IAfterRead<TEntity>
{
	Task<HookStatus> Handle(TEntity entity, bool isSingle);
}