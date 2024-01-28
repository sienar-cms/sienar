using System.Threading.Tasks;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Processors;

// ReSharper disable once TypeParameterCanBeVariant
public interface IProcessor<TRequest>
{
	Task<HookStatus> Process(TRequest request);
	void NotifySuccess();
	void NotifyBeforeHookFailure();
	void NotifyProcessFailure();
	void NotifyAfterHookFailure();
}