using Sienar.Infrastructure;
using Sienar.Services;

namespace Sienar.Identity.Results;

public class PersonalDataResult : IResult
{
	public DownloadFile? PersonalDataFile { get; set; }

	public PersonalDataResult(DownloadFile file)
	{
		PersonalDataFile = file;
	}
}