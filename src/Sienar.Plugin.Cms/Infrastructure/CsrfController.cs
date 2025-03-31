#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sienar.Infrastructure;

/// <exclude />
[ApiController]
[AllowAnonymous]
[Route("/api/csrf")]
public class CsrfController : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> RefreshTokens(
		[FromServices] ICsrfTokenRefresher tokenRefresher)
	{
		await tokenRefresher.RefreshToken();
		return NoContent();
	}
}