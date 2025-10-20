#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Configuration;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Data;
using Sienar.Hooks;

namespace Sienar.Identity.Hooks;

/// <exclude />
public class RegistrationOpenValidator : IStateValidator<RegisterRequest>
{
	private readonly SienarOptions _sienarOptions;
	private readonly INotifier _notifier;

	public RegistrationOpenValidator(
		IOptions<SienarOptions> sienarOptions,
		INotifier notifier)
	{
		_sienarOptions = sienarOptions.Value;
		_notifier = notifier;
	}

	public Task<OperationStatus> Validate(RegisterRequest request, ActionType action)
	{
		if (!_sienarOptions.RegistrationOpen)
		{
			_notifier.Error(CmsErrors.Account.RegistrationDisabled);
			return Task.FromResult(OperationStatus.Conflict);
		}

		return Task.FromResult(OperationStatus.Success);
	}
}