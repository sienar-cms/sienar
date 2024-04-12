using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Extensions;

/// <summary>
/// Contains utilities for returning <see cref="HookResult{TResult}"/> instances from processors
/// </summary>
public static class ProcessorExtensions
{
	/// <summary>
	/// Returns a hook result that represents a database concurrency issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Concurrency<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Concurrency, result);

	/// <summary>
	/// Returns a hook result that represents a database concurrency issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Concurrency<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Concurrency, result);

	/// <summary>
	/// Returns a hook result that represents a database conflict
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Conflict<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Conflict, result);

	/// <summary>
	/// Returns a hook result that represents a database conflict
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Conflict<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Conflict, result);

	/// <summary>
	/// Returns a hook result that represents a missing entity
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> NotFound<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.NotFound, result);

	/// <summary>
	/// Returns a hook result that represents a missing entity
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> NotFound<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.NotFound, result);

	/// <summary>
	/// Returns a hook result that represents a successful operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Success<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Success, result);

	/// <summary>
	/// Returns a hook result that represents a successful operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Success<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Success, result);

	/// <summary>
	/// Returns a hook result that represents an unauthorized result
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Unauthorized<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Unauthorized, result);

	/// <summary>
	/// Returns a hook result that represents an unauthorized result
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Unauthorized<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Unauthorized, result);

	/// <summary>
	/// Returns a hook result that represents an unknown issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Unknown<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Unknown, result);

	/// <summary>
	/// Returns a hook result that represents an unknown issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Unknown<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Unknown, result);

	/// <summary>
	/// Returns a hook result that represents an unprocessable operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Unprocessable<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default)
		=> new (HookStatus.Unprocessable, result);

	/// <summary>
	/// Returns a hook result that represents an unprocessable operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static HookResult<TResult> Unprocessable<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default)
		=> new(HookStatus.Unprocessable, result);
}