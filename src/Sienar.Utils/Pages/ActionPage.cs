using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sienar.Infrastructure;

namespace Sienar.Pages;

/// <summary>
/// A component that provides tracking of the success state and loading state of HTTP requests
/// </summary>
public abstract class ActionPage : ComponentBase
{
	private int _counter;

	/// <summary>
	/// Whether the last request submitted was successful
	/// </summary>
	protected bool WasSuccessful { get; private set; }

	/// <summary>
	/// Whether a request is currently running or not
	/// </summary>
	protected bool IsLoading => _counter > 0;

	/// <summary>
	/// The Blazor navigation manager
	/// </summary>
	[Inject]
	protected NavigationManager NavManager { get; set; } = null!;

	/// <summary>
	/// Submits a request that tracks the loading state and success state of the operation
	/// </summary>
	/// <param name="action">The request to submit</param>
	/// <typeparam name="TResult">The type of the result from the request</typeparam>
	protected virtual async Task<TResult?> SubmitRequest<TResult>(
		Func<Task<OperationResult<TResult>>> action)
	{
		// Reset for the current request
		WasSuccessful = false;

		// Increment counter and re-render
		_counter++;
		StateHasChanged();

		// Do work
		var result = await action();

		// Decrement counter, update status, and re-render
		_counter--;
		WasSuccessful = result.Status == OperationStatus.Success;
		StateHasChanged();

		return result.Result;
	}

	/// <summary>
	/// Submits a request that tracks the loading state of the operation
	/// </summary>
	/// <param name="action">The request to submit</param>
	protected virtual async Task SubmitRequest(Func<Task> action)
	{
		// Increment counter and re-render
		_counter++;
		StateHasChanged();

		// Do work
		await action();

		// Decrement counter, update status, and re-render
		_counter--;
		StateHasChanged();
	}
}

/// <summary>
/// A component that provides tracking of the success state and loading state of form requests
/// </summary>
/// <typeparam name="TModel">The type of the form model</typeparam>
public abstract class ActionPage<TModel> : ActionPage
	where TModel : new()
{
	/// <summary>
	/// The model used in requests sent from the page
	/// </summary>
	protected TModel Model = new();
}