using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to a CRUD GET request for a single entity
/// </summary>
/// <typeparam name="TDto">The type of the DTO</typeparam>
/// <typeparam name="TEntity">The type of the entity</typeparam>
public interface IReadActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase, new()
	where TEntity : EntityBase
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="id">The ID of the entity to read</param>
	/// <param name="filter">The <see cref="Filter"/></param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(int id, Filter? filter);
}
