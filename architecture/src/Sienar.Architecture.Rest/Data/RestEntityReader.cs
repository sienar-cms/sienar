using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Security;
using Sienar.Services;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityReader{TEntity}"/> which reads entities from a REST endpoint
/// </summary>
/// <typeparam name="TDto">The type of the entity to read</typeparam>
public class RestEntityReader<TDto> : ServiceBase, IEntityReader<TDto>
	where TDto : EntityBase
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<TDto> _endpointGenerator;
	private readonly ILogger<RestEntityReader<TDto>> _logger;
	private readonly IAccessValidationRunner<TDto> _accessValidationRunner;
	private readonly IAfterActionRunner<TDto> _afterActionRunner;

	/// <summary>
	/// Creates a new instance of <c>RestEntityReader</c>
	/// </summary>
	/// <param name="notifier">The notifier</param>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="logger">The logger</param>
	/// <param name="accessValidationRunner">The access validation runner</param>
	/// <param name="afterActionRunner">The after-action runner</param>
	public RestEntityReader(
		INotifier notifier,
		IRestClient client,
		ICrudEndpointGenerator<TDto> endpointGenerator,
		ILogger<RestEntityReader<TDto>> logger,
		IAccessValidationRunner<TDto> accessValidationRunner,
		IAfterActionRunner<TDto> afterActionRunner)
		: base(notifier)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_logger = logger;
		_accessValidationRunner = accessValidationRunner;
		_afterActionRunner = afterActionRunner;
	}

	/// <inheritdoc />
	public async Task<OperationResult<TDto?>> Read(
		int id,
		Filter? filter = null)
	{
		TDto? entity;
		try
		{
			var endpoint = _endpointGenerator.GenerateReadUrl(id);
			entity = (await _client.Get<TDto>(
				endpoint,
				filter)).Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<TDto?>(
				OperationStatus.Unknown,
				null,
				StatusMessages.Crud<TDto>.ReadSingleFailed()));
		}

		if (entity is null)
		{
			return NotifyOfResult(new OperationResult<TDto?>(
				OperationStatus.NotFound,
				null,
				StatusMessages.Crud<TDto>.NotFound(id)));
		}

		// Run access validation
		var accessValidationResult = await _accessValidationRunner.Validate(entity, ActionType.Read);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TDto?>(
				OperationStatus.Unauthorized,
				null,
				StatusMessages.Crud<TDto>.NoPermission()));
		}

		await _afterActionRunner.Run(entity, ActionType.Read);
		return NotifyOfResult(new OperationResult<TDto?>(result: entity));
	}

	/// <inheritdoc />
	public async Task<OperationResult<PagedQueryResult<TDto>>> Read(Filter? filter = null)
	{
		PagedQueryResult<TDto> queryResult;

		try
		{
			var endpoint = _endpointGenerator.GenerateReadUrl();
			queryResult = (await _client.Get<PagedQueryResult<TDto>>(
				endpoint,
				filter)).Result ?? new();
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<PagedQueryResult<TDto>>(
				OperationStatus.Unknown,
				new PagedQueryResult<TDto>(),
				StatusMessages.Crud<TDto>.ReadMultipleFailed()));
		}

		foreach (var entity in queryResult.Items)
		{
			await _afterActionRunner.Run(entity, ActionType.ReadAll);
		}

		return NotifyOfResult(new OperationResult<PagedQueryResult<TDto>>(result: queryResult));
	}
}
