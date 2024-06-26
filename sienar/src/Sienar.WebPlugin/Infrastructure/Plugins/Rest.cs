using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sienar.Infrastructure.Plugins;

public class Rest : IWebPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar REST Plugin",
		Description = "",
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Homepage = "https://sienar.levesque.dev",
		Version = Version.Parse("0.1.0")
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddScoped<IReadableNotificationService, RestNotificationService>();
		builder.Services.AddScoped<INotificationService>(
			sp => sp.GetRequiredService<IReadableNotificationService>());
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.MapControllers();
	}
}