namespace Sienar.Infrastructure.Plugins;

/// <summary>
/// A container for <see cref="ScriptResource"/> instances that should be rendered on each page
/// </summary>
public interface IScriptProvider : IListProvider<ScriptResource>;