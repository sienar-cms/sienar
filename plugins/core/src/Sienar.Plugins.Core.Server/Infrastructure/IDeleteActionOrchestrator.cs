using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to a CRUD DELETE request for a single entity
/// </summary>
/// <typeparam name="TEntity">The type of the entity</typeparam>
public interface IDeleteActionOrchestrator<TEntity>
	where TEntity : EntityBase
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="id">The ID of the entity to delete</param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(int id);
}
