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
public class LockoutReasonController : ServiceController
{
	public LockoutReasonController(IOperationResultMapper mapper)
		: base(mapper) {}

	[HttpGet]
	public Task<IActionResult> Read(
		[FromQuery] Filter? filter,
		[FromServices] IEntityReader<LockoutReason> service)
		=> Execute(() => service.Read(filter));

	[HttpGet("{id:guid}")]
	public Task<IActionResult> Read(
		Guid id,
		[FromQuery] Filter? filter,
		[FromServices] IEntityReader<LockoutReason> service)
		=> Execute(() => service.Read(id, filter));

	[HttpPost]
	public Task<IActionResult> Create(
		LockoutReason entity,
		[FromServices] IEntityWriter<LockoutReason> service)
		=> Execute(() => service.Create(entity));

	[HttpPut]
	public Task<IActionResult> Update(
		LockoutReason entity,
		[FromServices] IEntityWriter<LockoutReason> service)
		=> Execute(() => service.Update(entity));

	[HttpDelete("{id:guid}")]
	public Task<IActionResult> Delete(
		Guid id,
		[FromServices] IEntityDeleter<LockoutReason> service)
		=> Execute(() => service.Delete(id));
}
