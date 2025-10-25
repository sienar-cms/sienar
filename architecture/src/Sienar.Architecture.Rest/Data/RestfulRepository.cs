using System;
using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Data;

/// <summary>
/// An <see cref="IRepository{TEntity}"/> that supports REST API datastores
/// </summary>
/// <typeparam name="TEntity">the type of the entity</typeparam>
public class RestfulRepository<TEntity> : IRepository<TEntity>
	where TEntity : EntityBase
{
	private readonly IRestClient _client;
	private readonly IRestfulRepositoryUrlProvider<TEntity> _urlProvider;

	public RestfulRepository(
		IRestClient client,
		IRestfulRepositoryUrlProvider<TEntity> urlProvider)
	{
		_client = client;
		_urlProvider = urlProvider;
	}

	/// <inheritdoc />
	public async Task<TEntity?> Read(int id, Filter? filter = null)
	{
		var response = await _client.Get<TEntity>(
			_urlProvider.GenerateReadUrl(id),
			filter);
		return response.Result;
	}

	/// <inheritdoc />
	public async Task<PagedQueryResult<TEntity>> Read(Filter? filter = null)
	{
		var response = await _client.Get<PagedQueryResult<TEntity>>(
			_urlProvider.GenerateReadUrl(),
			filter);
		return response.Result ?? new PagedQueryResult<TEntity>();
	}

	/// <inheritdoc />
	public async Task<int> Create(TEntity entity)
	{
		var response = await _client.Post<int?>(
			_urlProvider.GenerateCreateUrl(entity),
			entity);
		return response.Result ?? 0;
	}

	/// <inheritdoc />
	public async Task<bool> Update(TEntity entity)
	{
		var response = await _client.Put<bool?>(
			_urlProvider.GenerateUpdateUrl(entity),
			entity);
		return response.Result ?? false;
	}

	/// <inheritdoc />
	public async Task<bool> Delete(int id)
	{
		var response = await _client.Delete<bool?>(_urlProvider.GenerateDeleteUrl(id));
		return response.Result ?? false;
	}

	/// <inheritdoc />
	public async Task<Guid?> ReadConcurrencyStamp(int id)
	{
		var response = await _client.Get<TEntity>(_urlProvider.GenerateReadUrl(id));
		return response.Result?.ConcurrencyStamp;
	}
}