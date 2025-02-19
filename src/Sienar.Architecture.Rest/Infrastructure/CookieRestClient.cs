﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Hooks;

namespace Sienar.Infrastructure;

/// <summary>
/// A client for interacting with Sienar REST APIs secured by cookie authentication
/// </summary>
public class CookieRestClient : IRestClient
{
	private readonly HttpClient _client;
	private readonly IEnumerable<IBeforeTask<RestClientRequest<CookieRestClient>>> _beforeActionHooks;
	private readonly IEnumerable<IAfterTask<RestClientResponse<CookieRestClient>>> _afterActionHooks;
	private readonly INotificationService _notifier;
	private readonly ILogger<CookieRestClient> _logger;
	private readonly JsonSerializerOptions _jsonOptions;

	/// <exclude />
	public CookieRestClient(
		HttpClient client,
		IEnumerable<IBeforeTask<RestClientRequest<CookieRestClient>>> beforeActionHooks,
		IEnumerable<IAfterTask<RestClientResponse<CookieRestClient>>> afterActionHooks,
		INotificationService notifier,
		ILogger<CookieRestClient> logger)
	{
		_client = client;
		_beforeActionHooks = beforeActionHooks;
		_afterActionHooks = afterActionHooks;
		_notifier = notifier;
		_logger = logger;
		_jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
	}

#region REST methods

	/// <summary>
	/// Sends an HTTP GET request with the specified data
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>the response wrapped with an operation result</returns>
	public Task<OperationResult<TResult?>> Get<TResult>(
		string endpoint,
		object? input = null)
		=> SendRequest<TResult>(endpoint, input, HttpMethod.Get);

	/// <summary>
	/// Sends an HTTP POST request with the specified data
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>the response wrapped with an operation result</returns>
	public Task<OperationResult<TResult?>> Post<TResult>(
		string endpoint,
		object input)
		=> SendRequest<TResult>(endpoint, input, HttpMethod.Post);

	/// <summary>
	/// Sends an HTTP PUT request with the specified data
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>the response wrapped with an operation result</returns>
	public Task<OperationResult<TResult?>> Put<TResult>(
		string endpoint,
		object input)
		=> SendRequest<TResult>(endpoint, input, HttpMethod.Put);

	/// <summary>
	/// Sends an HTTP PATCH request with the specified data
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>the response wrapped with an operation result</returns>
	public Task<OperationResult<TResult?>> Patch<TResult>(
		string endpoint,
		object input)
		=> SendRequest<TResult>(endpoint, input, HttpMethod.Patch);

	/// <summary>
	/// Sends an HTTP DELETE request with the specified data
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>the response wrapped with an operation result</returns>
	public Task<OperationResult<TResult?>> Delete<TResult>(
		string endpoint,
		object? input = null)
		=> SendRequest<TResult>(endpoint, input, HttpMethod.Delete);

	/// <summary>
	/// Sends an HTTP HEAD request with the specified data
	/// </summary>
	/// <remarks>
	/// Because HEAD requests do not return a payload, this method simply returns the <see cref="HttpResponseMessage"/> so the developer can parse the response headers in any way s/he sees fit.
	/// </remarks>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <returns>the HTTP response message</returns>
	public Task<HttpResponseMessage> Head(
		string endpoint,
		object? input = null)
		=> SendRaw(endpoint, input, HttpMethod.Head);

#endregion

	/// <summary>
	/// Sends an HTTP request and parses the response
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <param name="method">the HTTP method</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>an operation result wrapping around the result</returns>
	protected async Task<OperationResult<TResult?>> SendRequest<TResult>(
		string endpoint,
		object? input = null,
		HttpMethod? method = null)
	{
		HttpResponseMessage? result = null;
		try
		{
			result = await SendRaw(endpoint, input, method);

			var parsedResponse = await result.Content.ReadFromJsonAsync<WebResult<TResult>>(_jsonOptions);

			if (parsedResponse is null)
			{
				return new OperationResult<TResult?>(
					OperationStatus.Unknown,
					default,
					"Unable to read response from server");
			}

			foreach (var notification in parsedResponse.Notifications)
            {
            	_notifier.Notify(notification);
            }

			return CreateResult(result, parsedResponse);
		}
		catch (Exception e)
		{
			return HandleException<TResult>(e, result);
		}
	}

	/// <summary>
	/// Sends an HTTP request without parsing the response
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <param name="method">the HTTP method</param>
	/// <returns>the response message</returns>
	public async Task<HttpResponseMessage> SendRaw(
		string endpoint,
		object? input = null,
		HttpMethod? method = null)
	{
		method ??= HttpMethod.Get;
		var message = CreateRequestMessage(method, endpoint, input);
		var restClientRequest = new RestClientRequest<CookieRestClient> { RequestMessage = message };

		foreach (var beforeHook in _beforeActionHooks)
		{
			await beforeHook.Handle(restClientRequest);
		}

		var response = await _client.SendAsync(message);
		var restClientResponse = new RestClientResponse<CookieRestClient> { ResponseMessage = response };

		foreach (var afterHook in _afterActionHooks)
		{
			await afterHook.Handle(restClientResponse);
		}

		return response;
	}

	private HttpRequestMessage CreateRequestMessage(
		HttpMethod method,
		string endpoint,
		object? input = null)
	{
		var message = new HttpRequestMessage(method, endpoint);
		message.Headers.Add("X-Requested-With", "XMLHttpRequest");

		if (input is null)
		{
			return message;
		}

		if (method == HttpMethod.Get)
		{
			message.RequestUri = CreateQueryPayload(message, input);
		}
		else
		{
			message.Content = CreateContentPayload(input);
		}

		return message;
	}

	private StringContent CreateContentPayload(object input)
		=> new(
			JsonSerializer.Serialize(input, _jsonOptions),
			Encoding.UTF8,
			"application/json");

	private static Uri CreateQueryPayload(HttpRequestMessage m, object input)
	{
		var sb = new StringBuilder(m.RequestUri!.OriginalString);
		sb.Append('?');

		var inputType = input.GetType();
		var defaultInstance = Activator.CreateInstance(inputType);

		foreach (var prop in inputType.GetProperties())
		{
			var instanceValue = prop.GetValue(input)?.ToString();
			var defaultValue = prop.GetValue(defaultInstance)?.ToString();
			if (instanceValue != defaultValue)
			{
				sb.Append($"{prop.Name}={instanceValue}&");
			}
		}

		return new(sb.ToString(), UriKind.Relative);
	}

	private OperationResult<TResult?> HandleException<TResult>(
		Exception e,
		HttpResponseMessage? response)
	{
		// ASP.NET doesn't return a message body on 401/403 errors by default
		// So if the framework returns these, then the HTTP response will not
		// be null, and the status code will be either 401 or 403
		// In this event, deserializing the response will fail, so if that's what
		// happened, just return unauthorized and skip logging errors
		if (response?.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
		{
			return new OperationResult<TResult?>(OperationStatus.Unauthorized);
		}

		string logMessage;
		string errorMessage;
		switch (e)
		{
			case HttpRequestException:
				logMessage = "Network error";
				errorMessage = StatusMessages.Rest.NetworkFailed;
				break;
			case TaskCanceledException:
				logMessage =  "Network request timed out";
				errorMessage = StatusMessages.Rest.NetworkTimeout;
				break;
			default:
				logMessage = StatusMessages.General.Unknown;
				errorMessage = StatusMessages.General.Unknown;
				break;
		}

		_logger.LogError(e, "{}", logMessage);
		return new(OperationStatus.Unknown, default, errorMessage);
	}

	private OperationResult<TResult?> CreateResult<TResult>(
		HttpResponseMessage message,
		WebResult<TResult> parsedResult)
	{
		if (message.IsSuccessStatusCode)
		{
			return new OperationResult<TResult?>(result: parsedResult.Result);
		}

		string logMessage;
		string errorMessage;
		OperationStatus status;

		// Use the status code to generate an error message
		switch (message.StatusCode)
		{
			case HttpStatusCode.BadRequest:
				logMessage = "The request payload was not understood";
				errorMessage = StatusMessages.Rest.BadRequest;
				status = OperationStatus.Unprocessable;
				break;
			case HttpStatusCode.Unauthorized:
				logMessage = "Unauthorized user";
				errorMessage = StatusMessages.General.Unauthorized;
				status = OperationStatus.Unauthorized;
				break;
			case HttpStatusCode.Conflict:
				logMessage = "Database conflict";
				errorMessage = StatusMessages.General.Conflict;
				status = OperationStatus.Conflict;
				break;
			case HttpStatusCode.UnprocessableEntity:
				logMessage = "There was a problem with the request data";
				errorMessage = StatusMessages.General.Unprocessable;
				status = OperationStatus.Unprocessable;
				break;
			default:
				logMessage = StatusMessages.General.Unknown;
				errorMessage = StatusMessages.General.Unknown;
				status = OperationStatus.Unknown;
				break;
		}

		_logger.LogError("{}", logMessage);

		return new OperationResult<TResult?>(
			status,
			parsedResult.Result,
			parsedResult.Notifications.Length == 0
				? errorMessage
				: null);
	}
}