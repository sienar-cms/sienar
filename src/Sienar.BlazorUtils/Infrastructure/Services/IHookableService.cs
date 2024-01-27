using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

// ReSharper disable once TypeParameterCanBeVariant
public interface IHookableService<TRequest>
{
	Task<bool> Execute(TRequest request);
}