using Microsoft.Extensions.Configuration;

namespace Dzeta.Configuration;

/// <summary>
/// Configuration value provider for environment variables
/// </summary>
public class EnvironmentValueProvider : IConfigurationValueProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _prefix;

    public EnvironmentValueProvider(IConfiguration configuration, string prefix = "")
    {
        _configuration = configuration;
        _prefix = prefix;
    }

    public string? GetValue(string path)
    {
        var envKey = ConvertToEnvironmentKey(path);
        return _configuration[envKey];
    }

    private string ConvertToEnvironmentKey(string path)
    {
        // Convert "Database.Host" -> "DATABASE_HOST"
        var envPath = path.Replace('.', '_').ToUpperInvariant();
        
        return string.IsNullOrEmpty(_prefix) 
            ? envPath 
            : $"{_prefix.ToUpperInvariant()}_{envPath}";
    }
} 