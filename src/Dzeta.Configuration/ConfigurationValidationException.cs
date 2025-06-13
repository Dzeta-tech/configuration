using System;
using System.Collections.Generic;

namespace Dzeta.Configuration;

/// <summary>
/// Exception thrown when configuration validation fails
/// </summary>
public class ConfigurationValidationException : Exception
{
    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; }

    /// <summary>
    /// Creates a new configuration validation exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of validation errors</param>
    public ConfigurationValidationException(string message, List<string> errors) : base(message)
    {
        Errors = errors ?? new List<string>();
    }

    /// <summary>
    /// Creates a new configuration validation exception
    /// </summary>
    /// <param name="message">Error message</param>
    public ConfigurationValidationException(string message) : base(message)
    {
        Errors = new List<string>();
    }

    /// <summary>
    /// Создает новый экземпляр исключения валидации конфигурации
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public ConfigurationValidationException(string message, Exception innerException) : base(message, innerException)
    {
        Errors = new List<string>();
    }
} 