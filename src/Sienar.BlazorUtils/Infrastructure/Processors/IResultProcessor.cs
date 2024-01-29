using System.Threading.Tasks;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Processors;

public interface IResultProcessor<TResult>
{
	Task<(HookStatus Status, TResult? Result)> Process();
	void NotifySuccess();
	void NotifyProcessFailure();
}