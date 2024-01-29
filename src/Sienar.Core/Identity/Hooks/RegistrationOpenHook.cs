using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class RegistrationOpenHook : IBeforeProcess<RegisterRequest>
{
	private readonly SienarOptions _sienarOptions;
	private readonly INotificationService _notifier;

	public RegistrationOpenHook(
		IOptions<SienarOptions> sienarOptions,
		INotificationService notifier)
	{
		_sienarOptions = sienarOptions.Value;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public Task<HookStatus> Handle(RegisterRequest request)
	{
		if (!_sienarOptions.RegistrationOpen)
		{
			_notifier.Error(ErrorMessages.Account.RegistrationDisabled);
			return Task.FromResult(HookStatus.Conflict);
		}

		return Task.FromResult(HookStatus.Success);
	}
}