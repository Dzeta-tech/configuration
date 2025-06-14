using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

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
        // Convert camelCase to snake_case: "WalletAddress" -> "WALLET_ADDRESS"
        // Convert nested paths: "Database.Host" -> "DATABASE_HOST"
        string envPath = path.Replace('.', '_');
        
        // Insert underscore before uppercase letters (except the first one)
        envPath = Regex.Replace(envPath, "(?<!^)(?<!_)([A-Z])", "_$1");
        
        // Convert to uppercase
        envPath = envPath.ToUpperInvariant();

        return string.IsNullOrEmpty(prefix)
            ? envPath
            : $"{prefix.ToUpperInvariant()}_{envPath}";
    }
}