using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dzeta.Configuration;

/// <summary>
/// Extension methods for registering Dzeta.Configuration in DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up environment configuration provider
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="prefix">Environment variables prefix (optional)</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection UseEnvironmentConfigurationProvider(this IServiceCollection services, string prefix = "")
    {
        services.AddSingleton<IConfigurationValueProvider>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return new EnvironmentValueProvider(configuration, prefix);
        });
        
        return services;
    }

    /// <summary>
    /// Sets up custom configuration provider
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="providerFactory">Provider factory function</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection UseConfigurationProvider(this IServiceCollection services, 
        Func<IServiceProvider, IConfigurationValueProvider> providerFactory)
    {
        services.AddSingleton(providerFactory);
        return services;
    }

    /// <summary>
    /// Adds configuration class using registered provider
    /// </summary>
    /// <typeparam name="T">Configuration class type</typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services) 
        where T : BaseConfiguration, new()
    {
        services.AddSingleton<T>(serviceProvider =>
        {
            var valueProvider = serviceProvider.GetRequiredService<IConfigurationValueProvider>();
            var populator = new ConfigurationPopulator(valueProvider);
            
            var instance = new T();
            populator.Populate(instance);
            populator.Validate(instance);
            
            return instance;
        });
        
        return services;
    }
} 