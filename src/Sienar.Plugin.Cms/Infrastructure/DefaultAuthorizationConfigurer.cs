#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.AspNetCore.Authorization;

namespace Sienar.Infrastructure;

/// <exclude />
public class DefaultAuthorizationConfigurer : IConfigurer<AuthorizationOptions>
{
	public void Configure(AuthorizationOptions options) {}
}