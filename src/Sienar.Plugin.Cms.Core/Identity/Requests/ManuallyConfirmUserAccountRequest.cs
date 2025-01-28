using System;
using Sienar.Services;

namespace Sienar.Identity.Requests;

public class ManuallyConfirmUserAccountRequest : IRequest
{
	public Guid UserId { get; set; }
}