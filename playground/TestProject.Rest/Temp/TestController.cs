using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Services;

namespace TestProject.Rest.Temp;

[ApiController]
[Route("[controller]")]
public class TestController : ServiceController
{
	/// <inheritdoc />
	public TestController(IReadableNotificationService notifier)
		: base(notifier) {}

	[HttpPost("test-thing")]
	public Task<IActionResult> CreateThing(
		[FromBody] TestRequest input,
		[FromServices] IStatusService<TestRequest> service)
		=> ExecuteService(service, input);
}