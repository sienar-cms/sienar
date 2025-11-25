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
/// An implementation of <see cref="IEntityWriter{TEntity}"/> which writes entities to an EntityFramework <see cref="DbContext"/>
/// </summary>
/// <typeparam name="TEntity">The type of the entity to write</typeparam>
/// <typeparam name="TContext">The type of the app's <see cref="DbContext"/></typeparam>
public class EfEntityWriter<TEntity, TContext>
	: ServiceBase, IEntityWriter<TEntity>
	where TEntity : EntityBase
	where TContext : DbContext
{
	private readonly TContext _context;
	private readonly ILogger<EfEntityWriter<TEntity, TContext>> _logger;
	private readonly IAccessValidationRunner<TEntity> _accessValidationRunner;
	private readonly IStateValidationRunner<TEntity> _stateValidationRunner;
	private readonly IBeforeActionRunner<TEntity> _beforeActionRunner;
	private readonly IAfterActionRunner<TEntity> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>EfEntityWriter</c>
	/// </summary>
	/// <param name="notifier">The app notifier</param>
	/// <param name="context">The database context</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="stateValidationRunner">The state validation runner</param>
	/// <param name="beforeActionRunner">The before-hook action runner</param>
	/// <param name="afterActionRunner">The after-hook action runner</param>
	public EfEntityWriter(
		INotifier notifier,
		TContext context,
		ILogger<EfEntityWriter<TEntity, TContext>> logger,
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
	public async Task<OperationResult<int?>> Create(TEntity model)
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
				StatusMessages.Crud<TEntity>.NoPermission()));
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
				stateValidationResult.Message ?? StatusMessages.Crud<TEntity>.CreateFailed()));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeActionRunner.Run(model, ActionType.Create);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<int?>(
				OperationStatus.Unknown,
				null,
				beforeHooksResult.Message ?? StatusMessages.Crud<TEntity>.CreateFailed()));
		}

		try
		{
			await _context
				.Set<TEntity>()
				.AddAsync(model);
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<int?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<TEntity>.CreateFailed()));
		}

		// Run after hooks
		await _afterActionRunner.Run(model, ActionType.Create);

		return NotifyOfResult(new OperationResult<int?>(
			OperationStatus.Success,
			model.Id,
			StatusMessages.Crud<TEntity>.CreateSuccessful()));
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Update(TEntity model)
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
				StatusMessages.Crud<TEntity>.NoPermission()));
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
			_context
				.Set<TEntity>()
				.Update(model);
			await _context.SaveChangesAsync();
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
