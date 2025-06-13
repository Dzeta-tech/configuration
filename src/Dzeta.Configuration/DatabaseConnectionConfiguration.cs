using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dzeta.Configuration;

/// <summary>
/// Database connection configuration (example of nested configuration)
/// </summary>
public class DatabaseConnectionConfiguration : BaseConfiguration
{
    /// <summary>
    /// Database host
    /// </summary>
    [Required]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Database port
    /// </summary>
    [DefaultValue(5432)]
    public int Port { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Database name
    /// </summary>
    [Required]
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// Auto-generated connection string
    /// </summary>
    public string ConnectionString => 
        $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database}";
} 