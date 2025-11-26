using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure;
using Sienar.Services;

namespace Sienar.Data;

/// <summary>
/// An implementation of <see cref="IEntityDeleter{T}"/> which deletes entities from a REST endpoint
/// </summary>
/// <typeparam name="T">The type of the entity to write</typeparam>
public class RestEntityDeleter<T> : ServiceBase, IEntityDeleter<T>
	where T : EntityBase
{
	private readonly IRestClient _client;
	private readonly ICrudEndpointGenerator<T> _endpointGenerator;
	private readonly ILogger<RestEntityDeleter<T>> _logger;

	/// <summary>
	/// Creates a new instance of <c>RestEntityDeleter</c>
	/// </summary>
	/// <param name="notifier">The notifier</param>
	/// <param name="client">The rest client</param>
	/// <param name="endpointGenerator">The endpoint generator for the given DTO</param>
	/// <param name="logger">The logger</param>
	public RestEntityDeleter(
		INotifier notifier,
		IRestClient client,
		ICrudEndpointGenerator<T> endpointGenerator,
		ILogger<RestEntityDeleter<T>> logger)
		: base(notifier)
	{
		_client = client;
		_endpointGenerator = endpointGenerator;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Delete(int id)
	{
		bool wasSuccessful;
		try
		{
			var endpoint = _endpointGenerator.GenerateDeleteUrl(id);
			wasSuccessful = (await _client.Delete<bool?>(endpoint)).Result ?? false;
		}
		catch (Exception e)
		{
			_logger.LogError(e, StatusMessages.Database.QueryFailed);
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.DeleteFailed()));
		}

		if (!wasSuccessful)
		{
			return NotifyOfResult(new OperationResult<bool>(
				OperationStatus.Unknown,
				false,
				StatusMessages.Crud<T>.DeleteFailed()));
		}

		return NotifyOfResult(new OperationResult<bool>(
			OperationStatus.Success,
			true,
			StatusMessages.Crud<T>.DeleteSuccessful()));
	}
}
