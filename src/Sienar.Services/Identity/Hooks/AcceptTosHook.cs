using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class AcceptTosHook : IBeforeProcess<RegisterRequest>
{
	private readonly INotificationService _notifier;

	public AcceptTosHook(INotificationService notifier)
	{
		_notifier = notifier;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(RegisterRequest request)
	{
		if (!request.AcceptTos)
		{
			_notifier.Error(ErrorMessages.Account.MustAcceptTos);
			return Task.FromResult(HookStatus.Unprocessable);
		}

		return Task.FromResult(HookStatus.Success);
	}
}