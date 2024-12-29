#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Services;

namespace Sienar.Identity;

/// <exclude />
[ApiController]
[Route("/api/account")]
[Authorize]
public class AccountController : ServiceController
{
	public AccountController(IOperationResultMapper mapper)
		: base(mapper) {}

	[HttpPost]
	[AllowAnonymous]
	public Task<IActionResult> Register(
		[FromForm] RegisterRequest data,
		[FromServices] IStatusService<RegisterRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpGet]
	public Task<IActionResult> GetAccountData(
		[FromServices] IResultService<AccountDataResult> service)
		=> Execute(service.Execute);

	[HttpDelete]
	public Task<IActionResult> DeleteAccount(
		[FromForm] DeleteAccountRequest data,
		[FromServices] IStatusService<DeleteAccountRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpPost("confirm")]
	[AllowAnonymous]
	public Task<IActionResult> Confirm(
		[FromForm] ConfirmAccountRequest data,
		[FromServices] IStatusService<ConfirmAccountRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpPost("login")]
	[AllowAnonymous]
	public Task<IActionResult> Login(
		[FromForm] LoginRequest data,
		[FromServices] IService<LoginRequest, LoginResult> service)
		=> Execute(() => service.Execute(data));

	[HttpDelete("login")]
	public Task<IActionResult> Logout(
		[FromForm] LogoutRequest data,
		[FromServices] IStatusService<LogoutRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpDelete("password")]
	[AllowAnonymous]
	public Task<IActionResult> RequestPasswordReset(
		[FromForm] ForgotPasswordRequest data,
		[FromServices] IStatusService<ForgotPasswordRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpPatch("password")]
	[AllowAnonymous]
	public Task<IActionResult> PerformPasswordReset(
		[FromForm] ResetPasswordRequest data,
		[FromServices] IStatusService<ResetPasswordRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpPatch("change-password")]
	public Task<IActionResult> ChangePassword(
		[FromForm] ChangePasswordRequest data,
		[FromServices] IStatusService<ChangePasswordRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpPost("change-email")]
	public Task<IActionResult> ChangeEmail(
		[FromForm] InitiateEmailChangeRequest data,
		[FromServices] IStatusService<InitiateEmailChangeRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpPatch("email")]
	public Task<IActionResult> UpdateEmail(
		[FromForm] PerformEmailChangeRequest data,
		[FromServices] IStatusService<PerformEmailChangeRequest> service)
		=> Execute(() => service.Execute(data));

	[HttpGet("personal-data")]
	public async Task<IActionResult> GetPersonalData(
		[FromServices] IResultService<PersonalDataResult> service)
	{
		var result = await service.Execute();

		if (result.Status != OperationStatus.Success
			|| result.Result?.PersonalDataFile?.Contents is null)
		{
			return new ObjectResult("Unable to download personal data")
			{
				StatusCode = StatusCodes.Status500InternalServerError
			};
		}

		var file = result.Result.PersonalDataFile;
		Response.Headers.Append("Content-Disposition", $"attachment; filename={file.Name}");

		return new FileContentResult(
			result.Result.PersonalDataFile.Contents,
			result.Result.PersonalDataFile.Mime);
	}

	[HttpPost("lockout-reasons")]
	[AllowAnonymous]
	public Task<IActionResult> GetLockoutReaons(
		[FromForm] AccountLockoutRequest data,
		[FromServices] IService<AccountLockoutRequest, AccountLockoutResult> service)
		=> Execute(() => service.Execute(data));
}
