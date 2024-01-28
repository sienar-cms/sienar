using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

// ReSharper disable once TypeParameterCanBeVariant
public interface IService<TRequest>
{
	Task<bool> Execute(TRequest request);
}