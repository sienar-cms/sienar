using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

namespace TestProject.Web;

public class AppRequestConfigurer : IRequestConfigurer
{
	private readonly IStyleProvider _styleProvider;

	public AppRequestConfigurer(IStyleProvider styleProvider)
	{
		_styleProvider = styleProvider;
	}

	/// <inheritdoc />
	public void Configure()
	{
		_styleProvider
			.Add("/styles.css")
			.Add("/TestProject.Web.styles.css");
	}
}