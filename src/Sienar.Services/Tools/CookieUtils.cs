using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Sienar.Tools;

public static class CookieUtils
{
	public const string SienarSessionIdCookieName = "Sienar.SessionId";

	public static Guid? GetSienarSessionId(this HttpContext self)
	{
		return self.Request.GetSienarSessionId()
			?? self.Response.GetSienarSessionId();
	}

	public static Guid? GetSienarSessionId(this HttpRequest self)
	{
		if (!self.Cookies.ContainsKey(SienarSessionIdCookieName))
		{
			return null;
		}

		var cookie = self.Cookies[SienarSessionIdCookieName];
		if (string.IsNullOrWhiteSpace(cookie))
		{
			return null;
		}

		return Guid.Parse(cookie);
	}

	public static Guid? GetSienarSessionId(this HttpResponse self)
	{
		var cookie = self
			.GetTypedHeaders()
			.SetCookie.FirstOrDefault(c => c.Name == SienarSessionIdCookieName);
		if (cookie is null)
		{
			return null;
		}

		return Guid.Parse(cookie.Value);
	}
}