using System.Threading.Tasks;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Processors;

// ReSharper disable once TypeParameterCanBeVariant
public interface IProcessor<TRequest, TResult>
{
	Task<HookResult<TResult>> Process(TRequest request);
	void NotifySuccess();
	void NotifyFailure();
	void NotifyNoPermission();
}

// ReSharper disable once TypeParameterCanBeVariant
public interface IProcessor<TResult>
{
	Task<HookResult<TResult>> Process();
	void NotifySuccess();
	void NotifyFailure();
	void NotifyNoPermission();
}