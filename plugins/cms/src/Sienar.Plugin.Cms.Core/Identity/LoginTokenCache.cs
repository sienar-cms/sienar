using System;
using Microsoft.Extensions.Caching.Memory;
using Sienar.Identity.Requests;

namespace Sienar.Identity;

public class LoginTokenCache
{
	private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

	public void AddLoginToken(
		Guid token,
		LoginRequest request)
	{
		var options = new MemoryCacheEntryOptions()
			.SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
		_cache.Set(token, request, options);
	}

	public LoginRequest? ConsumeLoginToken(Guid token)
	{
		if (!_cache.TryGetValue(token, out var userId)) return null;
		if (userId is LoginRequest request)
		{
			_cache.Remove(token);
			return request;
		}
		return null;
	}
}