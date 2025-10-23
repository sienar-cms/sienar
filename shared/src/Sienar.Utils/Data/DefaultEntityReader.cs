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
public class DefaultEntityReader<TEntity> : ServiceBase, IEntityReader<TEntity>
	where TEntity : EntityBase, new()
{
	private readonly IRepository<TEntity> _repository;
	private readonly ILogger<DefaultEntityReader<TEntity>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidationRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;

	public DefaultEntityReader(
		INotifier notifier,
		IRepository<TEntity> repository,
		ILogger<DefaultEntityReader<TEntity>> logger,
		IAccessValidationRunner<TEntity> accessValidationRunner,
		IAfterActionRunner<TEntity> afterActionRunner)
		: base(notifier)
	{
		_repository = repository;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_afterActionRunner = afterActionRunner;
	}

	public async Task<OperationResult<TEntity?>> Read(
		int id,
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
		var accessValidationResult = await _accessValidationRunner.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TEntity?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TEntity>.NoPermission()));
		}

		await _afterActionRunner.Run(entity, ActionType.Read);
		return NotifyOfResult(new OperationResult<TEntity?>(result: entity));
	}

	public async Task<OperationResult<PagedQueryResult<TEntity>>> Read(Filter? filter = null)
	{
		PagedQueryResult<TEntity> queryResult;

		try
		{
			queryResult = await _repository.Read(filter);
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<PagedQueryResult<TEntity>>(
				OperationStatus.Unknown,
				new PagedQueryResult<TEntity>(),
				StatusMessages.Crud<TEntity>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterActionRunner.Run(entity, ActionType.ReadAll);
		}

		return NotifyOfResult(new OperationResult<PagedQueryResult<TEntity>>(result: queryResult));
	}
}