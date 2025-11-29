using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultDeleteActionOrchestrator<TEntity> : IDeleteActionOrchestrator<TEntity>
	where TEntity : EntityBase
{
	private readonly IEntityDeleter<TEntity> _entityDeleter;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultDeleteActionOrchestrator</c>
	/// </summary>
	/// <param name="entityDeleter">The entity deleter</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultDeleteActionOrchestrator(
		IEntityDeleter<TEntity> entityDeleter,
		IOperationResultMapper resultMapper)
	{
		_entityDeleter = entityDeleter;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(int id)
	{
		var result = await _entityDeleter.Delete(id);
		return _resultMapper.MapToObjectResult(result);
	}
}
