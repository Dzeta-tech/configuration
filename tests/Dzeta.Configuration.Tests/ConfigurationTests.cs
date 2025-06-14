using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Dzeta.Configuration.Tests;

public class TestConfiguration : BaseConfiguration
{
    [Required] public string ApiKey { get; set; } = string.Empty;

    [DefaultValue(8080)] public int Port { get; set; }

    public DatabaseConnectionConfiguration Database { get; set; } = new();

    public bool Debug { get; set; }
}

// Add TonWatcher configuration for testing the exact scenario
public class TonWatcherConfiguration : BaseConfiguration
{
    [Required] public string WalletAddress { get; set; } = string.Empty;

    [Required] public string WebhookUrl { get; set; } = string.Empty;

    public string? WebhookToken { get; set; }

    [DefaultValue(30)] public int PollingIntervalSeconds { get; set; }

    [DefaultValue("https://tonapi.io")] public string TonApiUrl { get; set; } = "https://tonapi.io";

    public string? TonApiKey { get; set; }

    [DefaultValue(3)] public int WebhookMaxRetries { get; set; }

    [DefaultValue(30)] public int HttpTimeoutSeconds { get; set; }

    [Required] public DatabaseConnectionConfiguration Database { get; set; } = new();
}

public class ConfigurationTests
{
    [Fact]
    public void Load_WithEnvironmentVariables_ShouldPopulateConfiguration()
    {
        // Arrange
        Environment.SetEnvironmentVariable("TEST_API_KEY", "secret123");
        Environment.SetEnvironmentVariable("TEST_PORT", "9000");
        Environment.SetEnvironmentVariable("TEST_DEBUG", "true");
        Environment.SetEnvironmentVariable("TEST_DATABASE_HOST", "localhost");
        Environment.SetEnvironmentVariable("TEST_DATABASE_PORT", "5432");
        Environment.SetEnvironmentVariable("TEST_DATABASE_USERNAME", "user");
        Environment.SetEnvironmentVariable("TEST_DATABASE_PASSWORD", "pass");
        Environment.SetEnvironmentVariable("TEST_DATABASE_DATABASE", "testdb");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        EnvironmentValueProvider valueProvider = new(configuration, "TEST");
        ConfigurationPopulator populator = new(valueProvider);

        // Act
        TestConfiguration config = new();
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
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        EnvironmentValueProvider valueProvider = new(configuration, "TEST");
        ConfigurationPopulator populator = new(valueProvider);

        // Act & Assert
        TestConfiguration config = new();
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

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        EnvironmentValueProvider valueProvider = new(configuration, "TEST");
        ConfigurationPopulator populator = new(valueProvider);

        // Act
        TestConfiguration config = new();
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

    [Fact]
    public void TonWatcher_WithDockerComposeEnvironment_ShouldLoadSuccessfully()
    {
        // Arrange - Set up environment variables exactly as in docker-compose
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_HOST", "postgres");
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_PORT", "5432");
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_DATABASE", "tonwatcher");
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_USERNAME", "tonwatcher");
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_PASSWORD", "tonwatcher123");

        Environment.SetEnvironmentVariable("TONWATCHER_WALLET_ADDRESS",
            "0:6f3aab06db6b19504c9c0eaee61346ad860fd476b529c041ece7416cc9b15b57");
        Environment.SetEnvironmentVariable("TONWATCHER_WEBHOOK_URL", "http://webhook-listener:8080/webhook");
        Environment.SetEnvironmentVariable("TONWATCHER_WEBHOOK_TOKEN", "test-token-123");
        Environment.SetEnvironmentVariable("TONWATCHER_WEBHOOK_MAX_RETRIES", "3");
        Environment.SetEnvironmentVariable("TONWATCHER_POLLING_INTERVAL_SECONDS", "30");
        Environment.SetEnvironmentVariable("TONWATCHER_TON_API_URL", "https://tonapi.io");
        Environment.SetEnvironmentVariable("TONWATCHER_HTTP_TIMEOUT_SECONDS", "30");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        EnvironmentValueProvider valueProvider = new(configuration, "TONWATCHER");
        ConfigurationPopulator populator = new(valueProvider);

        // Act
        TonWatcherConfiguration config = new();
        populator.Populate(config);

        // This should not throw - if it does, we'll see the validation error
        Exception? exception = Record.Exception(() => populator.Validate(config));

        // Assert
        Assert.Null(exception); // Should not throw
        Assert.Equal("0:6f3aab06db6b19504c9c0eaee61346ad860fd476b529c041ece7416cc9b15b57", config.WalletAddress);
        Assert.Equal("http://webhook-listener:8080/webhook", config.WebhookUrl);
        Assert.Equal("test-token-123", config.WebhookToken);
        Assert.Equal(30, config.PollingIntervalSeconds);
        Assert.Equal("https://tonapi.io", config.TonApiUrl);
        Assert.Equal(3, config.WebhookMaxRetries);
        Assert.Equal(30, config.HttpTimeoutSeconds);

        // Database assertions
        Assert.Equal("postgres", config.Database.Host);
        Assert.Equal(5432, config.Database.Port);
        Assert.Equal("tonwatcher", config.Database.Database);
        Assert.Equal("tonwatcher", config.Database.Username);
        Assert.Equal("tonwatcher123", config.Database.Password);

        // Cleanup
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_HOST", null);
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_PORT", null);
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_DATABASE", null);
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_USERNAME", null);
        Environment.SetEnvironmentVariable("TONWATCHER_DATABASE_PASSWORD", null);
        Environment.SetEnvironmentVariable("TONWATCHER_WALLET_ADDRESS", null);
        Environment.SetEnvironmentVariable("TONWATCHER_WEBHOOK_URL", null);
        Environment.SetEnvironmentVariable("TONWATCHER_WEBHOOK_TOKEN", null);
        Environment.SetEnvironmentVariable("TONWATCHER_WEBHOOK_MAX_RETRIES", null);
        Environment.SetEnvironmentVariable("TONWATCHER_POLLING_INTERVAL_SECONDS", null);
        Environment.SetEnvironmentVariable("TONWATCHER_TON_API_URL", null);
        Environment.SetEnvironmentVariable("TONWATCHER_HTTP_TIMEOUT_SECONDS", null);
    }

    [Fact]
    public void TonWatcher_ValidationError_ShouldProvideDetailedErrorMessage()
    {
        // Arrange - Missing required fields to test error reporting
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        EnvironmentValueProvider valueProvider = new(configuration, "TONWATCHER");
        ConfigurationPopulator populator = new(valueProvider);

        // Act
        TonWatcherConfiguration config = new();
        populator.Populate(config);

        // Assert - Should throw with detailed error message
        ConfigurationValidationException exception =
            Assert.Throws<ConfigurationValidationException>(() => populator.Validate(config));

        // The error message should tell us exactly what's missing
        Assert.Contains("WalletAddress", exception.Message);
        Assert.Contains("WebhookUrl", exception.Message);
        Assert.Contains("Database", exception.Message);

        // Check that Errors collection contains specific field names
        Assert.Contains(exception.Errors, e => e.Contains("WalletAddress"));
        Assert.Contains(exception.Errors, e => e.Contains("WebhookUrl"));
        Assert.Contains(exception.Errors, e => e.Contains("Database"));
    }
}