using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Extensions;

public static class ProcessorExtensions
{
	public static HookResult<TResult> Concurrency<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Concurrency, result);

	public static HookResult<TResult> Concurrency<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Concurrency, result);

	public static HookResult<TResult> Conflict<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Conflict, result);

	public static HookResult<TResult> Conflict<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Conflict, result);

	public static HookResult<TResult> NotFound<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.NotFound, result);

	public static HookResult<TResult> NotFound<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.NotFound, result);

	public static HookResult<TResult> Success<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Success, result);

	public static HookResult<TResult> Success<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Success, result);

	public static HookResult<TResult> Unauthorized<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Unauthorized, result);

	public static HookResult<TResult> Unauthorized<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Unauthorized, result);

	public static HookResult<TResult> Unknown<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Unknown, result);

	public static HookResult<TResult> Unknown<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Unknown, result);

	public static HookResult<TResult> Unprocessable<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Unprocessable, result);

	public static HookResult<TResult> Unprocessable<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Unprocessable, result);
}