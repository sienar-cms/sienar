using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public interface IAfterDelete<TEntity>
{
	Task<HookStatus> Handle(TEntity entity);
}