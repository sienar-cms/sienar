using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure;

namespace Sienar.Configuration;

/// <summary>
/// Configures the <see cref="IMvcBuilder"/> to 
/// </summary>
public class DefaultMvcBuilderConfigurer : IConfigurer<IMvcBuilder>
{
	/// <inheritdoc />
	public void Configure(IMvcBuilder builder)
	{
		builder
			.ConfigureApiBehaviorOptions(o =>
			{
				o.InvalidModelStateResponseFactory = context =>
				{
					var details = new ValidationProblemDetails(context.ModelState)
					{
						Extensions =
						{
							["traceId"] = context.HttpContext.TraceIdentifier
						}
					};

					return new UnprocessableEntityObjectResult(details)
					{
						ContentTypes =
						{
							MediaTypeNames.Application.Json,
							MediaTypeNames.Application.Xml
						}
					};
				};
			})
			.AddJsonOptions(o =>
			{
				o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
			});
	}
}