using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure;

namespace Sienar.Pages;

public abstract class ActionPage<TModel> : ActionPage
	where TModel : new()
{
	[SupplyParameterFromForm]
	public TModel Model { get; set; } = new ();
}

public abstract class ActionPage : ComponentBase
{
	private int _counter;

	protected bool Loading => _counter > 0;

	protected bool WasSuccessful { get; set; }

	[Inject]
	protected ILogger<ActionPage> Logger { get; set; } = default!;

	[Inject]
	protected NavigationManager NavManager { get; set; } = default!;

	[Inject]
	protected INotificationService Notifier { get; set; } = default!;

	protected async Task SubmitRequest(Func<Task<bool>> submitFunc)
	{
		WasSuccessful = false;

		Logger.LogInformation("Submitting service request");

		_counter++;
		StateHasChanged();

		try
		{
			WasSuccessful = await submitFunc();
			Logger.LogInformation("Service request submitted");
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Service request failed");
			Notifier.Error(e);
		}

		--_counter;
		StateHasChanged();
	}

	protected async Task<TReturn?> SubmitRequest<TReturn>(Func<Task<TReturn>> submitFunc)
	{
		WasSuccessful = false;

		Logger.LogInformation("Submitting service request");

		_counter++;
		StateHasChanged();

		var result = default(TReturn);
		try
		{
			result = await submitFunc();
			Logger.LogInformation("Service request submitted successfully");
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Service request failed");
			Notifier.Error(e);
		}

		_counter--;
		WasSuccessful = (!result?.Equals(default(TReturn))) ?? false;
		StateHasChanged();

		return result;
	}

	protected async Task SubmitRequest(Func<Task> submitFunc)
	{
		WasSuccessful = false;

		Logger.LogInformation("Submitting service request");

		++_counter;
		StateHasChanged();

		try
		{
			await submitFunc();
			WasSuccessful = true;
			Logger.LogInformation("Service request submitted successfully");
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Service request failed");
			Notifier.Error(e.Message);
		}

		--_counter;
		StateHasChanged();
	}
}