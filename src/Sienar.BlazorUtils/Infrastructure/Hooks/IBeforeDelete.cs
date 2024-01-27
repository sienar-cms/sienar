using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

public interface IBeforeDelete<TEntity>
{
	Task<HookStatus> Handle(TEntity entity);
}