using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultReadActionOrchestrator<TDto, TEntity>
	: IReadActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase, new()
	where TEntity : EntityBase
{
	private readonly IMapper<TDto, TEntity> _dtoMapper;
	private readonly IEntityReader<TEntity> _entityReader;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultReadActionOrchestrator</c>
	/// </summary>
	/// <param name="dtoMapper">The DTO mapper</param>
	/// <param name="entityReader">The entity reader</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultReadActionOrchestrator(
		IMapper<TDto,TEntity> dtoMapper,
		IEntityReader<TEntity> entityReader,
		IOperationResultMapper resultMapper)
	{
		_dtoMapper = dtoMapper;
		_entityReader = entityReader;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(int id, Filter? filter)
	{
		var dto = new TDto();
		var result = await _entityReader.Read(id, filter);

		if (result.Status is not OperationStatus.Success ||
			result.Result is null)
		{
			return _resultMapper.MapToObjectResult(result);
		}

		_dtoMapper.MapToDto(dto, result.Result);

		return _resultMapper.MapToObjectResult(new OperationResult<TDto>(
			result.Status,
			dto,
			result.Message));
	}
}
