using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IAfterDelete<TEntity>
{
	Task<HookStatus> Handle(TEntity entity);
}