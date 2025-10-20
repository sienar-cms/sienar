#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;

namespace Sienar.Services;

/// <exclude />
public class DefaultEntityReader<TEntity> : ServiceBase, IEntityReader<TEntity>
	where TEntity : EntityBase, new()
{
	private readonly IRepository<TEntity> _repository;
	private readonly ILogger<DefaultEntityReader<TEntity>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidator;
	private readonly IAfterActionRunner<TEntity> _afterHooks;

	public DefaultEntityReader(
		INotificationService notifier,
		IRepository<TEntity> repository,
		ILogger<DefaultEntityReader<TEntity>> logger,
		IAccessValidationRunner<TEntity> accessValidator,
		IAfterActionRunner<TEntity> afterHooks)
		: base(notifier)
	{
		_repository = repository;
		_logger = logger;
		_accessValidator = accessValidator;
		_afterHooks = afterHooks;
	}

	public async Task<OperationResult<TEntity?>> Read(
		Guid id,
		Filter? filter = null)
	{
		TEntity? entity;
		try
		{
			entity = await _repository.Read(id, filter);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<TEntity>.ReadSingleFailed()));
		}

		if (entity is null)
		{
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.NotFound,
				null,
				StatusMessages.Crud<TEntity>.NotFound(id)));
		}

		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		await _afterHooks.Run(entity, ActionType.Read);
		return NotifyOfResult(new OperationResult<TEntity?>(result: entity));
	}

	public async Task<OperationResult<PagedQuery<TEntity>>> Read(Filter? filter = null)
	{
		PagedQuery<TEntity> queryResult;

		try
		{
			queryResult = await _repository.Read(filter);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<PagedQuery<TEntity>>(
				OperationStatus.Unknown,
				new PagedQuery<TEntity>(),
				StatusMessages.Crud<TEntity>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterHooks.Run(entity, ActionType.ReadAll);
		}

		return NotifyOfResult(new OperationResult<PagedQuery<TEntity>>(result: queryResult));
	}
}