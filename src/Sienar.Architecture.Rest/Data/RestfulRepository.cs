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
	public async Task<TEntity?> Read(Guid id, Filter? filter = null)
	{
		var response = await _client.Get<TEntity>(
			_urlProvider.GenerateReadUrl(id),
			filter);
		return response.Result;
	}

	/// <inheritdoc />
	public async Task<PagedQuery<TEntity>> Read(Filter? filter = null)
	{
		var response = await _client.Get<PagedQuery<TEntity>>(
			_urlProvider.GenerateReadUrl(),
			filter);
		return response.Result ?? new PagedQuery<TEntity>();
	}

	/// <inheritdoc />
	public async Task<Guid> Create(TEntity entity)
	{
		var response = await _client.Post<Guid?>(
			_urlProvider.GenerateCreateUrl(entity),
			entity);
		return response.Result ?? Guid.Empty;
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
	public async Task<bool> Delete(Guid id)
	{
		var response = await _client.Delete<bool?>(_urlProvider.GenerateDeleteUrl(id));
		return response.Result ?? false;
	}

	/// <inheritdoc />
	public async Task<Guid?> ReadConcurrencyStamp(Guid id)
	{
		var response = await _client.Get<TEntity>(_urlProvider.GenerateReadUrl(id));
		return response.Result?.ConcurrencyStamp;
	}
}