#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Data;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class AcceptTosValidator : IStateValidator<RegisterRequest>
{
	private readonly INotificationService _notifier;

	public AcceptTosValidator(INotificationService notifier)
	{
		_notifier = notifier;
	}

	/// <inheritdoc />
	public Task<OperationStatus> Validate(RegisterRequest request, ActionType action)
	{
		if (!request.AcceptTos)
		{
			_notifier.Error(CmsErrors.Account.MustAcceptTos);
			return Task.FromResult(OperationStatus.Unprocessable);
		}

		return Task.FromResult(OperationStatus.Success);
	}
}