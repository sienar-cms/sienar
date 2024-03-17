using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

// ReSharper disable once TypeParameterCanBeVariant
public interface IService<TRequest, TResult>
{
	Task<TResult?> Execute(TRequest request);
}