﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Hooks;
using Sienar.Ui;

namespace Sienar.Infrastructure;

/// <summary>
/// Ensures that the application has a valid CSRF token on startup by calling the CSRF endpoint in the Sienar REST API
/// </summary>
public class InitializeCsrfTokenOnAppStartHook : IBeforeTask<SienarStartupActor>
{
	private readonly IRestClient _client;
	private readonly ILogger<InitializeCsrfTokenOnAppStartHook> _logger;

	/// <exclude />
	public InitializeCsrfTokenOnAppStartHook(
		IRestClient client,
		ILogger<InitializeCsrfTokenOnAppStartHook> logger)
	{
		_client = client;
		_logger = logger;
	}

	/// <exclude />
	public async Task Handle(SienarStartupActor? request = null)
	{
		try
		{
			await _client.RefreshCsrfToken();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to update CSRF token");
		}
	}
}