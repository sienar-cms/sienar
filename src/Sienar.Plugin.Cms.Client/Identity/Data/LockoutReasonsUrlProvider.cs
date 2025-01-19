﻿using System;
using Sienar.Data;

namespace Sienar.Identity.Data;

public class LockoutReasonsUrlProvider : IRestfulRepositoryUrlProvider<LockoutReason>
{
	private const string BaseUrl = "lockout-reasons";

	/// <inheritdoc />
	public string GenerateReadUrl(Guid id)
		=> $"{BaseUrl}/{id}";

	/// <inheritdoc />
	public string GenerateReadUrl()
		=> BaseUrl;

	/// <inheritdoc />
	public string GenerateCreateUrl(LockoutReason entity)
		=> BaseUrl;

	/// <inheritdoc />
	public string GenerateUpdateUrl(LockoutReason entity)
		=> BaseUrl;

	/// <inheritdoc />
	public string GenerateDeleteUrl(Guid id)
		=> $"{BaseUrl}/{id}";
}
