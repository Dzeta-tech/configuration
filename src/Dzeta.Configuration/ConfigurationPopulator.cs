using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dzeta.Configuration;

/// <summary>
///     Populates configuration objects from values
/// </summary>
public class ConfigurationPopulator(IConfigurationValueProvider valueProvider)
{
    public void Populate(object obj, string basePath = "")
    {
        IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties()
            .Where(p => p.CanWrite);

        foreach (PropertyInfo prop in properties)
        {
            string propertyPath = string.IsNullOrEmpty(basePath)
                ? prop.Name
                : $"{basePath}.{prop.Name}";

            if (IsSimple(prop.PropertyType))
                SetSimpleValue(obj, prop, propertyPath);
            else if (typeof(BaseConfiguration).IsAssignableFrom(prop.PropertyType))
                SetNestedValue(obj, prop, propertyPath);
        }
    }

    void SetSimpleValue(object obj, PropertyInfo prop, string path)
    {
        string? value = valueProvider.GetValue(path);
        if (value != null)
        {
            prop.SetValue(obj, Convert(value, prop.PropertyType));
        }
        else
        {
            DefaultValueAttribute? defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAttr != null) prop.SetValue(obj, defaultAttr.Value);
        }
    }

    void SetNestedValue(object obj, PropertyInfo prop, string path)
    {
        object nested = Activator.CreateInstance(prop.PropertyType)!;
        Populate(nested, path);
        prop.SetValue(obj, nested);
    }

    static bool IsSimple(Type type)
    {
        Type t = Nullable.GetUnderlyingType(type) ?? type;
        return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) ||
               t == typeof(DateTime) || t == typeof(TimeSpan) || t == typeof(Uri) ||
               t == typeof(Guid) || t.IsEnum;
    }

    static object Convert(string value, Type type)
    {
        Type? t = Nullable.GetUnderlyingType(type);
        if (t != null) return string.IsNullOrEmpty(value) ? null! : Convert(value, t);

        return type.Name switch
        {
            nameof(String) => value,
            nameof(Boolean) => bool.Parse(value),
            nameof(Int32) => int.Parse(value),
            nameof(Int64) => long.Parse(value),
            nameof(Double) => double.Parse(value),
            nameof(Decimal) => decimal.Parse(value),
            nameof(DateTime) => DateTime.Parse(value),
            nameof(TimeSpan) => TimeSpan.Parse(value),
            nameof(Uri) => new Uri(value),
            nameof(Guid) => Guid.Parse(value),
            _ when type.IsEnum => Enum.Parse(type, value, true),
            _ => throw new NotSupportedException($"Type {type.Name} not supported")
        };
    }

    public void Validate(object obj)
    {
        List<string> errors = [];

        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            object? value = prop.GetValue(obj);

            if (prop.GetCustomAttribute<RequiredAttribute>() != null)
                if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                    errors.Add($"{prop.Name} is required");

            if (value != null && typeof(BaseConfiguration).IsAssignableFrom(prop.PropertyType))
                try
                {
                    Validate(value);
                }
                catch (ConfigurationValidationException ex)
                {
                    errors.AddRange(ex.Errors.Select(e => $"{prop.Name}.{e}"));
                }
        }

        if (errors.Count != 0) throw new ConfigurationValidationException("Validation failed", errors);
    }
}