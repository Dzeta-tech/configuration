namespace Dzeta.Configuration;

/// <summary>
///     Interface for configuration value providers
/// </summary>
public interface IConfigurationValueProvider
{
    /// <summary>
    ///     Gets configuration value by path
    /// </summary>
    /// <param name="path">Configuration path (e.g., "Database.Host")</param>
    /// <returns>Configuration value or null if not found</returns>
    string? GetValue(string path);
}