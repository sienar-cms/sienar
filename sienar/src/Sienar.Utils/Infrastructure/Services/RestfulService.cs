using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Data;

namespace Sienar.Infrastructure.Services;

/// <summary>
/// A base class for services that interact with REST APIs
/// </summary>
public class RestfulService
{
	private readonly HttpClient _client;
	private readonly JsonSerializerOptions _jsonOptions;

	/// <summary>
	/// The service's logger
	/// </summary>
	protected readonly ILogger<RestfulService> Logger;

	/// <exclude />
	protected RestfulService(
		HttpClient client,
		ILogger<RestfulService> logger)
	{
		_client = client;
		Logger = logger;
		_jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
	}

#region REST methods

	/// <summary>
	/// Sends an HTTP GET request with the specified data
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>the response wrapped with an operation result</returns>
	protected Task<OperationResult<TResult>> Get<TResult>(
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
	protected Task<OperationResult<TResult>> Post<TResult>(
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
	protected Task<OperationResult<TResult>> Put<TResult>(
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
	protected Task<OperationResult<TResult>> Patch<TResult>(
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
	protected Task<OperationResult<TResult>> Delete<TResult>(
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
	protected Task<HttpResponseMessage> Head(
		string endpoint,
		object? input = null)
		=> SendWithRawResponse(endpoint, input, HttpMethod.Head);

#endregion

	/// <summary>
	/// Sends an HTTP request and parses the response
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <param name="method">the HTTP method</param>
	/// <typeparam name="TResult">the type of the response</typeparam>
	/// <returns>an operation result wrapping around the result</returns>
	protected async Task<OperationResult<TResult>> SendRequest<TResult>(
		string endpoint,
		object? input = null,
		HttpMethod? method = null)
	{
		method ??= HttpMethod.Get;
		var message = CreateRequestMessage(method, endpoint, input);

		try
		{
			var result = await _client.SendAsync(message);
			if (result.IsSuccessStatusCode)
			{
				var parsedResponse = await GetResponse<TResult>(result);
				if (parsedResponse is not null)
				{
					return new(OperationStatus.Success, parsedResponse);
				}

				return new(
					OperationStatus.Unknown,
					default,
					"The request was successful, but the server's response was not understood.");
			}

			return HandleFailureResponse<TResult>(result);
		}
		catch (Exception e)
		{
			return HandleException<TResult>(e);
		}
	}

	/// <summary>
	/// Sends an HTTP request without parsing the response
	/// </summary>
	/// <param name="endpoint">the destination URL</param>
	/// <param name="input">the request payload, if any</param>
	/// <param name="method">the HTTP method</param>
	/// <returns>the response message</returns>
	protected Task<HttpResponseMessage> SendWithRawResponse(
		string endpoint,
		object? input = null,
		HttpMethod? method = null)
	{
		method ??= HttpMethod.Get;
		var message = CreateRequestMessage(method, endpoint, input);
		return _client.SendAsync(message);
	}

	private Task<TReturn?> GetResponse<TReturn>(HttpResponseMessage response)
		=> response.Content.ReadFromJsonAsync<TReturn>(_jsonOptions);

	private HttpRequestMessage CreateRequestMessage(
		HttpMethod method,
		string endpoint,
		object? input = null)
	{
		var message = new HttpRequestMessage(method, endpoint);
		if (input is null)
		{
			return message;
		}

		if (method == HttpMethod.Get)
		{
			CreateQueryPayload(message, input);
		}
		else
		{
			CreateContentPayload(message, input);
		}

		return message;
	}

	private void CreateContentPayload(HttpRequestMessage m, object input)
	{
		m.Content = new StringContent(
			JsonSerializer.Serialize(input, _jsonOptions),
			Encoding.UTF8,
			"application/json");
	}

	private static void CreateQueryPayload(HttpRequestMessage m, object input)
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

		m.RequestUri = new Uri(sb.ToString(), UriKind.Relative);
	}

	private OperationResult<TResult> HandleException<TResult>(Exception e)
	{
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

		Logger.LogError(e, "{}", logMessage);
		return new(OperationStatus.Unknown, default, errorMessage);
	}

	private OperationResult<TResult> HandleFailureResponse<TResult>(
		HttpResponseMessage message)
	{
		string logMessage;
		string errorMessage;
		var status = OperationStatus.Unknown;

		// Use the status code to generate an error message
		switch (message.StatusCode)
		{
			case HttpStatusCode.BadRequest:
				logMessage = "The request payload was not understood";
				errorMessage = StatusMessages.Rest.BadRequest;
				break;
			case HttpStatusCode.UnprocessableEntity:
				logMessage = "There was a problem with the request data";
				errorMessage = StatusMessages.General.Unprocessable;
				break;
			default:
				logMessage = StatusMessages.General.Unknown;
				errorMessage = StatusMessages.General.Unknown;
				break;
		}

		Logger.LogError("{}", logMessage);

		return new(status, default, errorMessage);
	}
}