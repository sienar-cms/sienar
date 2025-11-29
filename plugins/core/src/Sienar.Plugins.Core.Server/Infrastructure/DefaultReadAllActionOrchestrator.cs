using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultReadAllActionOrchestrator<TDto, TEntity> : IReadAllActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase, new()
	where TEntity : EntityBase
{
	private readonly IMapper<TDto, TEntity> _dtoMapper;
	private readonly IEntityReader<TEntity> _entityReader;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new <c>DefaultReadAllActionOrchestrator</c>
	/// </summary>
	/// <param name="dtoMapper">The DTO mapper</param>
	/// <param name="entityReader">The entity reader</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultReadAllActionOrchestrator(
		IMapper<TDto, TEntity> dtoMapper,
		IEntityReader<TEntity> entityReader,
		IOperationResultMapper resultMapper)
	{
		_dtoMapper = dtoMapper;
		_entityReader = entityReader;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(Filter? filter)
	{
		var result = await _entityReader.Read(filter);

		if (result.Status is not OperationStatus.Success)
		{
			return _resultMapper.MapToObjectResult(result);
		}

		var mappedDtos = new List<TDto>();

		foreach (var entity in result.Result!.Items)
		{
			var dto = new TDto();
			_dtoMapper.MapToDto(dto, entity);
			mappedDtos.Add(dto);
		}

		return _resultMapper.MapToObjectResult(
			new OperationResult<PagedQueryResult<TDto>>(
				result.Status,
				new()
				{
					Items = mappedDtos,
					TotalCount = result.Result!.TotalCount
				},
				result.Message));
	}
}