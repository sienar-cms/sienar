using System.Threading.Tasks;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Processors;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A processor which accepts a <c>TRequest</c> as input and returns a <see cref="HookResult{TResult}"/>
/// </summary>
/// <typeparam name="TRequest">the type of the processor input</typeparam>
/// <typeparam name="TResult">the type of the processor output</typeparam>
public interface IProcessor<TRequest, TResult>
{
	Task<HookResult<TResult>> Process(TRequest request);
	void NotifySuccess();
	void NotifyFailure();
	void NotifyNoPermission();
}

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A processor which accepts no input and returns a <see cref="HookResult{TResult}"/>
/// </summary>
/// <typeparam name="TResult">the type of the processor output</typeparam>
public interface IProcessor<TResult>
{
	Task<HookResult<TResult>> Process();
	void NotifySuccess();
	void NotifyFailure();
	void NotifyNoPermission();
}