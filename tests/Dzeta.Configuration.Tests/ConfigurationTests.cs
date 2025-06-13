using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Dzeta.Configuration.Tests;

public class TestConfiguration : BaseConfiguration
{
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [DefaultValue(8080)]
    public int Port { get; set; }

    public DatabaseConnectionConfiguration Database { get; set; } = new();

    public bool Debug { get; set; }


}

public class ConfigurationTests
{
    [Fact]
    public void Load_WithEnvironmentVariables_ShouldPopulateConfiguration() 
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST_APIKEY", "secret123");
        Environment.SetEnvironmentVariable("TEST_PORT", "9000");
        Environment.SetEnvironmentVariable("TEST_DEBUG", "true");
        Environment.SetEnvironmentVariable("TEST_DATABASE_HOST", "localhost");
        Environment.SetEnvironmentVariable("TEST_DATABASE_PORT", "5432");
        Environment.SetEnvironmentVariable("TEST_DATABASE_USERNAME", "user");
        Environment.SetEnvironmentVariable("TEST_DATABASE_PASSWORD", "pass");
        Environment.SetEnvironmentVariable("TEST_DATABASE_DATABASE", "testdb");

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var valueProvider = new EnvironmentValueProvider(configuration, "TEST");
        var populator = new ConfigurationPopulator(valueProvider);

        // Act
        var config = new TestConfiguration();
        populator.Populate(config);

        // Assert  
        Assert.Equal("secret123", config.ApiKey);
        Assert.Equal(9000, config.Port);
        Assert.True(config.Debug);
        Assert.Equal("localhost", config.Database.Host);
        Assert.Equal(5432, config.Database.Port);
        Assert.Equal("user", config.Database.Username);
        Assert.Equal("pass", config.Database.Password);
        Assert.Equal("testdb", config.Database.Database);

        // Cleanup
        Environment.SetEnvironmentVariable("TEST_APIKEY", null);
        Environment.SetEnvironmentVariable("TEST_PORT", null);
        Environment.SetEnvironmentVariable("TEST_DEBUG", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_HOST", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_PORT", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_USERNAME", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_PASSWORD", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_DATABASE", null);
    }

    [Fact]
    public void Load_WithMissingRequiredField_ShouldThrowException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var valueProvider = new EnvironmentValueProvider(configuration, "TEST");
        var populator = new ConfigurationPopulator(valueProvider);

        // Act & Assert
        var config = new TestConfiguration();
        populator.Populate(config);
        Assert.Throws<ConfigurationValidationException>(() => populator.Validate(config));
    }

    [Fact]
    public void Load_WithDefaultValues_ShouldUseDefaults()
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST_APIKEY", "secret123");
        Environment.SetEnvironmentVariable("TEST_DATABASE_HOST", "localhost");
        Environment.SetEnvironmentVariable("TEST_DATABASE_USERNAME", "user");
        Environment.SetEnvironmentVariable("TEST_DATABASE_PASSWORD", "pass");
        Environment.SetEnvironmentVariable("TEST_DATABASE_DATABASE", "testdb");

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var valueProvider = new EnvironmentValueProvider(configuration, "TEST");
        var populator = new ConfigurationPopulator(valueProvider);

        // Act
        var config = new TestConfiguration();
        populator.Populate(config);

        // Assert
        Assert.Equal(8080, config.Port); // Default value
        Assert.Equal(5432, config.Database.Port); // Default value

        // Cleanup
        Environment.SetEnvironmentVariable("TEST_APIKEY", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_HOST", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_USERNAME", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_PASSWORD", null);
        Environment.SetEnvironmentVariable("TEST_DATABASE_DATABASE", null);
    }
} 