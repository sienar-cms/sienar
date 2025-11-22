using System;
using Sienar.Data;

namespace Sienar.Identity.Data;

public class UsersUrlProvider : IRestfulRepositoryUrlProvider<SienarUser>
{
	private const string BaseUrl = "users";

	/// <inheritdoc />
	public string GenerateReadUrl(int id)
		=> $"{BaseUrl}/{id}";

	/// <inheritdoc />
	public string GenerateReadUrl()
		=> BaseUrl;

	/// <inheritdoc />
	public string GenerateCreateUrl(SienarUser entity)
		=> BaseUrl;

	/// <inheritdoc />
	public string GenerateUpdateUrl(SienarUser entity)
		=> BaseUrl;

	/// <inheritdoc />
	public string GenerateDeleteUrl(int id)
		=> $"{BaseUrl}/{id}";
}
