using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IProcessor<TRequest>
{
	Task<HookStatus> Process(TRequest request);
	void NotifySuccess();
	void NotifyBeforeHookFailure();
	void NotifyProcessFailure();
	void NotifyAfterHookFailure();
}