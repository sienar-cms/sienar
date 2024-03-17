using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sienar.Infrastructure;

public class SienarRequestConfigurerMiddleware
{
	private readonly RequestDelegate _next;

	public SienarRequestConfigurerMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(
		HttpContext ctx,
		IEnumerable<IRequestConfigurer> configurers)
	{
		foreach (var configurer in configurers) configurer.Configure();
		await _next(ctx);
	}
}