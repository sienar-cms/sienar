namespace Sienar.Infrastructure.Plugins;

/// <summary>
/// A container for <see cref="PluginData"/> that can be used to display information to end users about which plugins are active in the app
/// </summary>
public interface IPluginDataProvider : IListProvider<PluginData>;