using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Pages;

/// <summary>
/// A component that provides time tracking and form resetting
/// </summary>
/// <typeparam name="TModel">The type of the form model</typeparam>
public class FormPage<TModel> : ActionPage<TModel>
	where TModel : new()
{
	/// <summary>
	/// The time when the form was started. Used for bot tracking purposes
	/// </summary>
	protected DateTime FormStartedTime { get; private set; } = DateTime.Now;

	/// <summary>
	/// Resets the form
	/// </summary>
	protected void Reset()
	{
		Model = new TModel();
		FormStartedTime = DateTime.Now;
	}

	/// <inheritdoc />
	protected override Task<TResult?> SubmitRequest<TResult>(Func<Task<OperationResult<TResult>>> action)
		where TResult : default
	{
		if (Model is Honeypot honeypot)
		{
			honeypot.TimeToComplete = DateTime.Now - FormStartedTime;
		}

		return base.SubmitRequest(action);
	}
}
