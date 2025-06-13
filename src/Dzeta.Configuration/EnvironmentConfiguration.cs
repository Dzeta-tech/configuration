using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Dzeta.Configuration;

/// <summary>
/// Service for loading configuration from environment variables
/// </summary>
public class EnvironmentConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly string _prefix;

    public EnvironmentConfigurationService(IConfiguration configuration, string prefix = "")
    {
        _configuration = configuration;
        _prefix = prefix;
    }

    /// <summary>
    /// Loads configuration from environment variables
    /// </summary>
    public T Load<T>() where T : BaseConfiguration, new()
    {
        var valueProvider = new EnvironmentValueProvider(_configuration, _prefix);
        var populator = new ConfigurationPopulator(valueProvider);
        
        var instance = new T();
        populator.Populate(instance);
        populator.Validate(instance);
        
        return instance;
    }

    /// <summary>
    /// Gets expected environment variables for documentation
    /// </summary>
    public List<string> GetExpectedVariables<T>() where T : BaseConfiguration, new()
    {
        var variables = new List<string>();
        CollectVariables(typeof(T), "", variables);
        return variables.OrderBy(x => x).ToList();
    }

    private void CollectVariables(Type type, string basePath, List<string> variables)
    {
        var properties = type.GetProperties()
            .Where(p => p.CanWrite);

        foreach (var prop in properties)
        {
            var propertyPath = string.IsNullOrEmpty(basePath) 
                ? prop.Name 
                : $"{basePath}.{prop.Name}";

            if (IsSimple(prop.PropertyType))
            {
                var envKey = ConvertToEnvironmentKey(propertyPath);
                variables.Add(envKey);
            }
            else if (typeof(BaseConfiguration).IsAssignableFrom(prop.PropertyType))
            {
                CollectVariables(prop.PropertyType, propertyPath, variables);
            }
        }
    }

    private string ConvertToEnvironmentKey(string path)
    {
        var envPath = path.Replace('.', '_').ToUpperInvariant();
        return string.IsNullOrEmpty(_prefix) 
            ? envPath 
            : $"{_prefix.ToUpperInvariant()}_{envPath}";
    }

    private static bool IsSimple(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || 
               t == typeof(DateTime) || t == typeof(TimeSpan) || t == typeof(Uri) || 
               t == typeof(Guid) || t.IsEnum;
    }
} 