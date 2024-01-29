using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

public interface IResultService<TResult>
{
	Task<TResult?> Execute();
}