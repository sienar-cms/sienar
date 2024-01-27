using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IBeforeProcess<TRequest>
{
	Task<HookStatus> Handle(TRequest request);
}