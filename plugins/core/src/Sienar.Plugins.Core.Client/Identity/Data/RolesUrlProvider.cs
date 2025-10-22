using System;
using Sienar.Data;

namespace Sienar.Identity.Data;

public class RolesUrlProvider : IRestfulRepositoryUrlProvider<SienarRole>
{
	/// <inheritdoc />
	public string GenerateReadUrl(Guid id) => throw new InvalidOperationException("Roles can only be read as a list from the \"/api/roles\" endpoint.");

	/// <inheritdoc />
	public string GenerateReadUrl() => "roles";

	/// <inheritdoc />
	public string GenerateCreateUrl(SienarRole entity) => throw new InvalidOperationException("Roles can only be read as a list from the \"/api/roles\" endpoint.");

	/// <inheritdoc />
	public string GenerateUpdateUrl(SienarRole entity) => throw new InvalidOperationException("Roles can only be read as a list from the \"/api/roles\" endpoint.");

	/// <inheritdoc />
	public string GenerateDeleteUrl(Guid id) => throw new InvalidOperationException("Roles can only be read as a list from the \"/api/roles\" endpoint.");
}