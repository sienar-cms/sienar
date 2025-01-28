using Sienar.Data;
using Sienar.Infrastructure;

namespace Sienar.Services;

/// <summary>
/// A base service that provides a <see cref="NotifyOfResult{TResult}"/> method which uses <see cref="INotificationService"/> to notify the user of the status of the operation if a status message is present on the <see cref="OperationResult{TResult}"/>
/// </summary>
public abstract class ServiceBase
{
	/// <summary>
	/// The notification service
	/// </summary>
	protected readonly INotificationService Notifier;

	/// <summary>
	/// Creates a new instance of <c>ServiceBase</c>
	/// </summary>
	/// <param name="notifier">The notification service</param>
	protected ServiceBase(INotificationService notifier)
	{
		Notifier = notifier;
	}

	/// <summary>
	/// Handles notifying the user of the result of an operation if appropriate
	/// </summary>
	/// <param name="result">The operation result</param>
	/// <typeparam name="TResult">The type returned from the result</typeparam>
	/// <returns>The operation result</returns>
	protected OperationResult<TResult> NotifyOfResult<TResult>(
		OperationResult<TResult> result)
	{
		if (result.Message is not null)
		{
			if (result.Status is not OperationStatus.Success)
			{
				Notifier.Error(result.Message);
			}
			else if (result.Status is OperationStatus.Success)
			{
				Notifier.Success(result.Message);
			}
		}

		return result;
	}
}
