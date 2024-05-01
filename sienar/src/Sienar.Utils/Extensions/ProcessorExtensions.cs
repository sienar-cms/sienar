using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Extensions;

/// <summary>
/// Contains utilities for returning <see cref="OperationResult{TResult}"/> instances from processors
/// </summary>
public static class ProcessorExtensions
{
	/// <summary>
	/// Returns a hook result that represents a database concurrency issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Concurrency<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.Concurrency, result, message);

	/// <summary>
	/// Returns a hook result that represents a database concurrency issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Concurrency<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.Concurrency, result, message);

	/// <summary>
	/// Returns a hook result that represents a database conflict
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Conflict<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.Conflict, result, message);

	/// <summary>
	/// Returns a hook result that represents a database conflict
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Conflict<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.Conflict, result, message);

	/// <summary>
	/// Returns a hook result that represents a missing entity
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> NotFound<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.NotFound, result, message);

	/// <summary>
	/// Returns a hook result that represents a missing entity
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> NotFound<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.NotFound, result, message);

	/// <summary>
	/// Returns a hook result that represents a successful operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Success<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.Success, result, message);

	/// <summary>
	/// Returns a hook result that represents a successful operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Success<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.Success, result, message);

	/// <summary>
	/// Returns a hook result that represents an unauthorized result
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Unauthorized<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.Unauthorized, result, message);

	/// <summary>
	/// Returns a hook result that represents an unauthorized result
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Unauthorized<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.Unauthorized, result, message);

	/// <summary>
	/// Returns a hook result that represents an unknown issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Unknown<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.Unknown, result, message);

	/// <summary>
	/// Returns a hook result that represents an unknown issue
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Unknown<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.Unknown, result, message);

	/// <summary>
	/// Returns a hook result that represents an unprocessable operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TRequest">the type of the request</typeparam>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Unprocessable<TRequest, TResult>(
		this IProcessor<TRequest, TResult> self,
		TResult? result = default,
		string? message = null)
		=> new (OperationStatus.Unprocessable, result, message);

	/// <summary>
	/// Returns a hook result that represents an unprocessable operation
	/// </summary>
	/// <param name="self">the processor</param>
	/// <param name="result">the result to return, if any</param>
	/// <param name="message">the error message to display, if any</param>
	/// <typeparam name="TResult">the type of the result</typeparam>
	/// <returns>the resulting hook result</returns>
	public static OperationResult<TResult> Unprocessable<TResult>(
		this IProcessor<TResult> self,
		TResult? result = default,
		string? message = null)
		=> new(OperationStatus.Unprocessable, result, message);
}