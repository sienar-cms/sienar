using Sienar.Infrastructure;

namespace Sienar.Identity.Results;

public class PersonalDataResult
{
	public FileDto? PersonalDataFile { get; set; }

	public PersonalDataResult(FileDto file)
	{
		PersonalDataFile = file;
	}
}