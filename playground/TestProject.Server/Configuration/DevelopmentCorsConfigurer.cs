using Microsoft.AspNetCore.Cors.Infrastructure;
using Sienar.Configuration;

namespace TestProject.Configuration;

public class DevelopmentCorsConfigurer : IConfigurer<CorsOptions>
{
	public void Configure(CorsOptions options)
	{
		options.AddDefaultPolicy(p =>
		{
			p.AllowAnyHeader();
			p.AllowAnyMethod();
			p.AllowAnyOrigin();
		});
	}
}
