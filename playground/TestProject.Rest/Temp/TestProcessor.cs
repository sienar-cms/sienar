using System.Threading.Tasks;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace TestProject.Rest.Temp;

public class TestProcessor : IProcessor<TestRequest, bool>
{
	private readonly INotificationService _notifier;

	public TestProcessor(INotificationService notifier)
	{
		_notifier = notifier;
	}

	/// <inheritdoc />
	public async Task<OperationResult<bool>> Process(TestRequest request)
	{
		await Task.Delay(2000);

		_notifier.Info($"Your username is {request.Name}");
		_notifier.Warning("Do you wanna try me, punk?");
		_notifier.Warning($"Go on, {request.Name}...make my day!");
		_notifier.Error($"Okay, {request.Name}, you done did it now!");

		return this.Success(true, "You have succeeded!");
	}
}