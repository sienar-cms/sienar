#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;
using Sienar.Services;

namespace Sienar.Data;

/// <exclude />
public class DefaultEntityWriter<TEntity> : ServiceBase, IEntityWriter<TEntity>
	where TEntity : EntityBase
{
	private readonly IRepository<TEntity> _repository;
	private readonly ILogger<DefaultEntityWriter<TEntity>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidator;
	private readonly IStateValidationRunner<TEntity> _stateValidationRunner;
	private readonly IBeforeActionRunner<TEntity> _beforeActionRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;

	public DefaultEntityWriter(
		INotificationService notifier,
		IRepository<TEntity> repository,
		ILogger<DefaultEntityWriter<TEntity>> logger,
		IAccessValidationRunner<TEntity> accessValidator,
		IStateValidationRunner<TEntity> stateValidationRunner,
		IBeforeActionRunner<TEntity> beforeActionRunner,
		IAfterActionRunner<TEntity> afterActionRunner)
		: base(notifier)
	{
		_repository = repository;
		_logger = logger;
		_accessValidator = accessValidator;
		_stateValidationRunner = stateValidationRunner;
		_beforeActionRunner = beforeActionRunner;
		_afterActionRunner = afterActionRunner;
	}

	public async Task<OperationResult<Guid?>> Create(TEntity model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(model, ActionType.Create);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<Guid?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(model, ActionType.Create);
		if (!stateValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<Guid?>(
				OperationStatus.Unprocessable,
				null,
				stateValidationResult.Message ?? StatusMessages.Crud<TEntity>.CreateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model, ActionType.Create);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<Guid?>(
				OperationStatus.Unknown,
				null,
				beforeHooksResult.Message ?? StatusMessages.Crud<TEntity>.CreateFailed()));
		}

		try
		{
			await _repository.Create(model);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<Guid?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<TEntity>.CreateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Create);

		return NotifyOfResult(new OperationResult<Guid?>(
			OperationStatus.Success,
			model.Id,
			StatusMessages.Crud<TEntity>.CreateSuccessful()));
	}

	public async Task<OperationResult<bool>> Update(TEntity model)
	{
		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(model, ActionType.Update);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unauthorized,
				false,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(model, ActionType.Update);
		if (!stateValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<TEntity>.UpdateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model, ActionType.Update);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<TEntity>.UpdateFailed()));
		}

		try
		{
			await _repository.Update(model);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<TEntity>.UpdateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Update);

		return NotifyOfResult(new OperationResult<bool>(
			OperationStatus.Success,
			true,
			StatusMessages.Crud<TEntity>.UpdateSuccessful()));
	}
}