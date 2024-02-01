using Sienar.Infrastructure;

namespace Sienar.Identity.Results;

public class PersonalDataResult
{
	public DownloadFile? PersonalDataFile { get; set; }

	public PersonalDataResult(DownloadFile file)
	{
		PersonalDataFile = file;
	}
}