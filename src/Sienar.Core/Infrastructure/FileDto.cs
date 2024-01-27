#nullable disable

namespace Sienar.Infrastructure;

public class FileDto
{
	public byte[] Contents { get; set; }
	public string Mime { get; set; }
	public string Name { get; set; }
}