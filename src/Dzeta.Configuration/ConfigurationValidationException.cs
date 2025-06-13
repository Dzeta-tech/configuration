namespace Dzeta.Configuration;

/// <summary>
///     Exception thrown when configuration validation fails
/// </summary>
public class ConfigurationValidationException : Exception
{
    /// <summary>
    ///     Creates a new configuration validation exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of validation errors</param>
    public ConfigurationValidationException(string message, List<string>? errors) : base(message)
    {
        Errors = errors ?? [];
    }

    /// <summary>
    ///     List of validation errors
    /// </summary>
    public List<string> Errors { get; }
}