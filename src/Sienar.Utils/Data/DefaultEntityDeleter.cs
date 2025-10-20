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
public class DefaultEntityDeleter<TEntity> : ServiceBase, IEntityDeleter<TEntity>
	where TEntity : EntityBase
{
	private readonly IRepository<TEntity> _repository;
	private readonly ILogger<DefaultEntityDeleter<TEntity>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidator;
	private readonly IStateValidationRunner<TEntity> _stateValidationRunner;
	private readonly IBeforeActionRunner<TEntity> _beforeActionRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;

	public DefaultEntityDeleter(
		INotificationService notifier,
		IRepository<TEntity> repository,
		ILogger<DefaultEntityDeleter<TEntity>> logger,
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

	public async Task<OperationResult<bool>> Delete(Guid id)
	{
		TEntity? entity;
		try
		{
			entity = await _repository.Read(id);
			if (entity is null)
			{
				return NotifyOfResult(new OperationResult<bool>(
					OperationStatus.NotFound,
					false,
					StatusMessages.Crud<TEntity>.NotFound(id)));
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(entity, ActionType.Delete);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unauthorized,
				false,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		// Run state validation
		var stateValidationResult = await _stateValidationRunner.Validate(entity, ActionType.Delete);
		if (!stateValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				stateValidationResult.Message ?? StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(entity, ActionType.Delete);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		try
		{
			await _repository.Delete(entity.Id);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(entity, ActionType.Delete);

		return NotifyOfResult(new OperationResult<bool>(
			OperationStatus.Success,
			true,
			StatusMessages.Crud<TEntity>.DeleteSuccessful()));
	}
}