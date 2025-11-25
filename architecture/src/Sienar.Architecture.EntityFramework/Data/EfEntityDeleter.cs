using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;
using Sienar.Services;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityDeleter{TEntity}"/> which deletes entities from an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="TEntity">The type of the entity to delete</typeparam>
/// <typeparam name="TContext">The type of the app's <see cref="DbContext"/></typeparam>
public class EfEntityDeleter<TEntity, TContext>
	: ServiceBase, IEntityDeleter<TEntity>
	where TEntity : EntityBase
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly ILogger<EfEntityDeleter<TEntity, TContext>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidationRunner;
	private readonly IStateValidationRunner<TEntity> _stateValidationRunner;
	private readonly IBeforeActionRunner<TEntity> _beforeActionRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="notifier">The app notifier</param>
	/// <param name="context">The database context</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-hook action runner</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	public EfEntityDeleter(
		INotifier notifier,
		TContext context,
		ILogger<EfEntityDeleter<TEntity, TContext>> logger,
		IAccessValidationRunner<TEntity> accessValidationRunner,
		IStateValidationRunner<TEntity> stateValidationRunner,
		IBeforeActionRunner<TEntity> beforeActionRunner,
		IAfterActionRunner<TEntity> afterActionRunner)
		: base(notifier)
	{
		_context = context;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_stateValidationRunner = stateValidationRunner;
		_beforeActionRunner = beforeActionRunner;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Delete(int id)
	{
		TEntity? entity;
		var entitySet = _context.Set<TEntity>();

		try
		{
			entity = await entitySet.FindAsync(id);
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
		var accessValidationResult = await _accessValidationRunner.Validate(
			entity,
			ActionType.Delete);
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
		var beforeHooksResult = await _beforeActionRunner.Run(
			entity,
			ActionType.Delete);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				beforeHooksResult.Message ?? StatusMessages.Crud<TEntity>.DeleteFailed()));
		}

		try
		{
			entitySet.Remove(entity);
			await _context.SaveChangesAsync();
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
