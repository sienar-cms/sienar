#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Sienar.Configuration;
using Sienar.Extensions;
using Sienar.Hooks;

namespace Sienar.Infrastructure;

/// <exclude />
public class AddCsrfTokenToHttpRequestHook : IBeforeAction<RestClientRequest<CookieRestClient>>
{
	private readonly IJSRuntime _js;

	public AddCsrfTokenToHttpRequestHook(IJSRuntime js)
	{
		_js = js;
	}

	public async Task Handle(RestClientRequest<CookieRestClient> request, ActionType action)
	{
		var token = await _js.GetCookieValue("XSRF-TOKEN");
		if (!string.IsNullOrEmpty(token))
		{
			request.RequestMessage.Headers.Add("X-XSRF-TOKEN", token);
		}
	}
}
