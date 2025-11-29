using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultUpdateActionOrchestrator<TDto, TEntity> : IUpdateActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase
	where TEntity : EntityBase, new()
{
	private readonly IDbContext _context;
	private readonly IMapper<TDto, TEntity> _dtoMapper;
	private readonly IEntityWriter<TEntity> _entityWriter;
	private readonly IOperationResultMapper _resultMapper;

	/// <summary>
	/// Creates a new instance of <c>DefaultUpdateActionOrchestrator</c>
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="dtoMapper">The DTO mapper</param>
	/// <param name="entityWriter">The entity writer</param>
	/// <param name="resultMapper">The result mapper</param>
	public DefaultUpdateActionOrchestrator(
		IDbContext context,
		IMapper<TDto, TEntity> dtoMapper,
		IEntityWriter<TEntity> entityWriter,
		IOperationResultMapper resultMapper)
	{
		_context = context;
		_dtoMapper = dtoMapper;
		_entityWriter = entityWriter;
		_resultMapper = resultMapper;
	}

	/// <inheritdoc />
	public async Task<IActionResult> Execute(TDto dto)
	{
		var entity = await _context
			.Set<TEntity>()
			.FindAsync(dto.Id) ?? new TEntity();
		_dtoMapper.MapToEntity(dto, entity);
		var result = await _entityWriter.Update(entity);
		return _resultMapper.MapToObjectResult(result);
	}
}