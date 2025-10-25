#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <exclude />
public class DefaultStateValidationRunner<T> : IStateValidationRunner<T>
{
	private readonly IEnumerable<IStateValidator<T>> _validators;
	private readonly ILogger<DefaultStateValidationRunner<T>> _logger;

	public DefaultStateValidationRunner(
		IEnumerable<IStateValidator<T>> validators,
		ILogger<DefaultStateValidationRunner<T>> logger)
	{
		_validators = validators;
		_logger = logger;
	}

	public async Task<OperationResult<bool>> Validate(
		T input,
		ActionType action)
	{
		var wasSuccessful = true;
		string? validationMessage = null;

		try
		{
			foreach (var validator in _validators)
			{
				if (await validator.Validate(input, action) != OperationStatus.Success) wasSuccessful = false;
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "One or more state validators failed to run");
			wasSuccessful = false;
			validationMessage = StatusMessages.Processes.InvalidState;
		}

		return new(
			wasSuccessful ? OperationStatus.Success : OperationStatus.Unprocessable,
			wasSuccessful,
			validationMessage);
	}
}
