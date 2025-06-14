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
    public ConfigurationValidationException(string message, List<string>? errors) : base(BuildMessage(message, errors))
    {
        Errors = errors ?? [];
    }

    /// <summary>
    ///     List of validation errors
    /// </summary>
    public List<string> Errors { get; }

    static string BuildMessage(string message, List<string>? errors)
    {
        if (errors == null || errors.Count == 0)
            return message;

        return $"{message}:\n" + string.Join("\n", errors.Select(e => $"  - {e}"));
    }
}