using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to a CRUD GET request for multiple entities
/// </summary>
/// <typeparam name="TDto">The type of the DTO</typeparam>
/// <typeparam name="TEntity">The type of the entity</typeparam>
public interface IReadAllActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase, new()
	where TEntity : EntityBase
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="filter">The <see cref="Filter"/></param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(Filter? filter);
}