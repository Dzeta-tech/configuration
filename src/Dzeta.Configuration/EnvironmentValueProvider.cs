using Microsoft.Extensions.Configuration;

namespace Dzeta.Configuration;

/// <summary>
///     Configuration value provider for environment variables
/// </summary>
public class EnvironmentValueProvider(IConfiguration configuration, string prefix = "") : IConfigurationValueProvider
{
    public string? GetValue(string path)
    {
        string envKey = ConvertToEnvironmentKey(path);
        return configuration[envKey];
    }

    string ConvertToEnvironmentKey(string path)
    {
        // Convert "Database.Host" -> "DATABASE_HOST"
        string envPath = path.Replace('.', '_').ToUpperInvariant();

        return string.IsNullOrEmpty(prefix)
            ? envPath
            : $"{prefix.ToUpperInvariant()}_{envPath}";
    }
}