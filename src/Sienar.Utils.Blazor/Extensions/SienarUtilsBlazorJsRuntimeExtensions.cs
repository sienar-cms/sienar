using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IJSRuntime"/> extension methods for the <c>Sienar.Utils.Blazor</c> assembly
/// </summary>
public static class SienarUtilsBlazorJsRuntimeExtensions
{
	/// <summary>
	/// Gets the value of a cookie in the browser. Only works on cookies accessible to JavaScript
	/// </summary>
	/// <param name="self">The <see cref="IJSRuntime"/></param>
	/// <param name="cookieName">The name of the cookie whose value to retrieve</param>
	/// <returns>The value of the cookie if it exists, else <c>null</c></returns>
	public static async Task<string?> GetCookieValue(
		this IJSRuntime self,
		string cookieName)
	{
		var cookies = await self.InvokeAsync<IEnumerable<string>>(
			"eval",
			"document.cookie.split(';')");

		foreach (var cookie in cookies)
		{
			var parts = cookie
				.Split(
					'=',
					StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
				.ToList();
			if (parts.Count < 2) continue;
		
			if (parts[0] == cookieName)
			{
				return string.Join('=', parts[1..]);
			}
		}

		return null;
	}
}
