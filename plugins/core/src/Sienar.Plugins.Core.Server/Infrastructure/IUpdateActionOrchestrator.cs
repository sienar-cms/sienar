using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Orchestrates and calls the services needed to respond to a CRUD PUT request for a single entity
/// </summary>
/// <typeparam name="TDto">The type of the DTO</typeparam>
/// <typeparam name="TEntity">The type of the entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IUpdateActionOrchestrator<TDto, TEntity>
	where TDto : EntityBase
	where TEntity : EntityBase, new()
{
	/// <summary>
	/// Executes the orchestrated action
	/// </summary>
	/// <param name="dto">The DTO representing the updated entity</param>
	/// <returns>The final action result</returns>
	Task<IActionResult> Execute(TDto dto);
}