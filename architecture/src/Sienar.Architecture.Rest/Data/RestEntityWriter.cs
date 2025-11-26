using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;
using Sienar.Services;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityWriter{TDto}"/> which writes entities to a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the entity to write</typeparam>
public class RestEntityWriter<T> : ServiceBase, IEntityWriter<T>
	where T : EntityBase
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly ILogger<RestEntityWriter<
T>> _logger;
	private readonly IAccessValidationRunner<T> _accessValidationRunner;
	private readonly IStateValidationRunner<T> _stateValidationRunner;
	private readonly IBeforeActionRunner<T> _beforeActionRunner;
	private readonly IAfterActionRunner<T> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityWriter</c>
	/// </summary>
	/// <param name="notifier">The notifier</param>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-action runner</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityWriter(
		INotifier notifier,
		IRestClient client,
		ICrudEndpointGenerator<T> endpointGenerator,
		ILogger<RestEntityWriter<T>> logger,
		IAccessValidationRunner<T> accessValidationRunner,
		IStateValidationRunner<T> stateValidationRunner,
		IBeforeActionRunner<T> beforeActionRunner,
		IAfterActionRunner<T> afterActionRunner)
		: base(notifier)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_stateValidationRunner = stateValidationRunner;
		_beforeActionRunner = beforeActionRunner;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<int?>> Create(T model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			model,
			ActionType.Create);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<int?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(
			model,
			ActionType.Create);
		if (!stateValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<int?>(
				OperationStatus.Unprocessable,
				null,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.CreateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(
			model,
			ActionType.Create);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<int?>(
				OperationStatus.Unknown,
				null,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.CreateFailed()));
		}

		int id;
		try
		{
			var endpoint = _endpointGenerator.GenerateCreateUrl(model);
			id = (await _client.Post<int>(
				endpoint,
				model)).Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<int?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<T>.CreateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Create);

		return NotifyOfResult(new OperationResult<int?>(
			OperationStatus.Success,
			id,
			StatusMessages.Crud<T>.CreateSuccessful()));
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Update(T model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(
			model,
			ActionType.Update);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unauthorized,
				false,
				StatusMessages.Crud<T>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(
			model,
			ActionType.Update);
		if (!stateValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<T>.UpdateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model, ActionType.Update);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<T>.UpdateFailed()));
		}

		bool success;
		try
		{
			var endpoint = _endpointGenerator.GenerateUpdateUrl(model);
			success = (await _client.Put<bool?>(
				endpoint,
				model)).Result ?? false;
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.UpdateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Update);

		return NotifyOfResult(new OperationResult<bool>(
			OperationStatus.Success,
			success,
			StatusMessages.Crud<T>.UpdateSuccessful()));
	}
}
