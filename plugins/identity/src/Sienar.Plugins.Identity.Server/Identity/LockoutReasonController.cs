#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;
using Sienar.Infrastructure;
using Sienar.Services;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/lockout-reasons")]
[Authorize(Roles = Roles.Admin)]
public class LockoutReasonController : SienarController
{
	public LockoutReasonController(IOperationResultMapper mapper)
		: base(mapper) {}

	[HttpGet]
	public Task<IActionResult> Read(
		[FromQuery] Filter? filter,
		[FromServices] IReadAllActionOrchestrator<LockoutReasonDto, LockoutReason> orchestrator)
		=> orchestrator.Execute(filter);

	[HttpGet("{id:int}")]
	public Task<IActionResult> Read(
		int id,
		[FromQuery] Filter? filter,
		[FromServices] IReadActionOrchestrator<LockoutReasonDto, LockoutReason> orchestrator)
		=> orchestrator.Execute(id, filter);

	[HttpPost]
	public Task<IActionResult> Create(
		LockoutReasonDto lockoutReason,
		[FromServices] ICreateActionOrchestrator<LockoutReasonDto, LockoutReason> orchestrator)
		=> orchestrator.Execute(lockoutReason);

	[HttpPut]
	public Task<IActionResult> Update(
		LockoutReasonDto lockoutReason,
		[FromServices] IUpdateActionOrchestrator<LockoutReasonDto, LockoutReason> orchestrator)
		=> orchestrator.Execute(lockoutReason);

	[HttpDelete("{id:int}")]
	public Task<IActionResult> Delete(
		int id,
		[FromServices] IDeleteActionOrchestrator<LockoutReason> orchestrator)
		=> orchestrator.Execute(id);
}
