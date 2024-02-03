using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public class ScriptProvider : IScriptProvider
{
	private readonly List<ScriptResource> _resources = [];

	/// <inheritdoc />
	public IScriptProvider Enqueue(
		string url,
		bool isModule = false,
		bool isAsync = false,
		bool shouldDefer = true,
		CrossOriginMode? crossOrigin = null,
		ReferrerPolicy? referrerPolicy = null,
		string? integrity = null)
		=> Enqueue(
			new()
			{
				Url = url,
				IsModule = isModule,
				IsAsync = isAsync,
				ShouldDefer = shouldDefer,
				Mode = crossOrigin,
				Referrer = referrerPolicy,
				Integrity = integrity
			});

	/// <inheritdoc />
	public IScriptProvider Enqueue(ScriptResource resource)
	{
		_resources.Add(resource);
		return this;
	}

	/// <inheritdoc />
	public IEnumerable<ScriptResource> GetScripts() => _resources;
}