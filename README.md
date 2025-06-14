# 🛠️ Dzeta.Configuration

[![NuGet Version](https://img.shields.io/nuget/v/Dzeta.Configuration?style=flat-square&logo=nuget&logoColor=white&label=NuGet)](https://www.nuget.org/packages/Dzeta.Configuration/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Dzeta.Configuration?style=flat-square&logo=nuget&logoColor=white&label=Downloads)](https://www.nuget.org/packages/Dzeta.Configuration/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/Dzeta-tech/configuration/ci.yml?branch=master&style=flat-square&logo=github&logoColor=white&label=Build)](https://github.com/Dzeta-tech/configuration/actions)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/github/license/Dzeta-tech/configuration?style=flat-square&logo=github&logoColor=white&label=License)](https://github.com/Dzeta-tech/configuration/blob/master/LICENSE)
[![GitHub Stars](https://img.shields.io/github/stars/Dzeta-tech/configuration?style=flat-square&logo=github&logoColor=white&label=Stars)](https://github.com/Dzeta-tech/configuration)

A simple, flexible, and readable .NET configuration library with pluggable providers for environment variables, JSON,
and more.

## 📋 Table of Contents

- [✨ Features](#-features)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
    - [1. Define Configuration Classes](#1-define-configuration-classes)
    - [2. Set Environment Variables](#2-set-environment-variables)
    - [3. Register with Dependency Injection](#3-register-with-dependency-injection)
    - [4. Use in Your Application](#4-use-in-your-application)
- [🏗️ Advanced Usage](#️-advanced-usage)
    - [Direct Usage](#direct-usage)
    - [Custom Providers](#custom-providers)
- [🔧 Configuration Classes](#-configuration-classes)
    - [Simple Properties](#simple-properties)
    - [Nested Configurations](#nested-configurations)
    - [Built-in Database Configuration](#built-in-database-configuration)
- [📝 Environment Variable Naming](#-environment-variable-naming)
- [✅ Validation](#-validation)
- [🔗 Supported Types](#-supported-types)
- [🏛️ Architecture](#️-architecture)
- [📄 License](#-license)

## ✨ Features

- 🚀 **Simple**: Clean, minimal API
- 🎯 **Type-safe**: Strong typing with validation
- 🔧 **Flexible**: Support for simple types and nested objects
- 📦 **DI Ready**: Seamless dependency injection integration
- ✅ **Validated**: Built-in validation using data annotations
- 🗂️ **Nested**: Full support for nested configuration objects
- 🏗️ **Extensible**: Pluggable provider architecture
- 🎨 **Clean**: Beautiful, readable code

## 📦 Installation

```bash
dotnet add package Dzeta.Configuration
```

## 🚀 Quick Start

### 1. Define Configuration Classes

```csharp
public class ApplicationConfiguration : BaseConfiguration
{
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [DefaultValue(8080)]
    public int Port { get; set; }

    [Required]
    public DatabaseConnectionConfiguration Database { get; set; } = new();

    public bool Debug { get; set; }
}
```

### 2. Set Environment Variables

```bash
export MYAPP_APIKEY=your-secret-key
export MYAPP_PORT=9000
export MYAPP_DEBUG=true
export MYAPP_DATABASE_HOST=localhost
export MYAPP_DATABASE_PORT=5432
export MYAPP_DATABASE_USERNAME=user
export MYAPP_DATABASE_PASSWORD=password
export MYAPP_DATABASE_DATABASE=mydb
```

### 3. Register with Dependency Injection

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// First, setup the configuration provider
builder.Services.UseEnvironmentConfigurationProvider("MYAPP");

// Then, register your configuration classes
builder.Services.AddConfiguration<ApplicationConfiguration>();

var app = builder.Build();
```

### 4. Use in Your Application

```csharp
public class ApiController : ControllerBase
{
    private readonly ApplicationConfiguration _config;

    public ApiController(ApplicationConfiguration config)
    {
        _config = config;
    }

    public IActionResult GetStatus()
    {
        return Ok(new 
        {
            ApiKey = _config.ApiKey.Substring(0, 4) + "***",
            Port = _config.Port,
            Debug = _config.Debug,
            DatabaseConnection = _config.Database.ConnectionString
        });
    }
}
```

## 🏗️ Advanced Usage

### Direct Usage

```csharp
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var valueProvider = new EnvironmentValueProvider(configuration, "MYAPP");
var populator = new ConfigurationPopulator(valueProvider);

var config = new ApplicationConfiguration();
populator.Populate(config);
populator.Validate(config);

Console.WriteLine($"Database: {config.Database.ConnectionString}");
```

### Custom Providers

```csharp
// Register a custom provider
builder.Services.UseConfigurationProvider(serviceProvider => 
    new MyCustomValueProvider());

// Or combine multiple providers
builder.Services.UseConfigurationProvider(serviceProvider => 
    new CompositeValueProvider(
        new EnvironmentValueProvider(config, "MYAPP"),
        new JsonValueProvider("appsettings.json")
    ));
```

## 🔧 Configuration Classes

### Simple Properties

All configuration classes must inherit from `BaseConfiguration`:

```csharp
public class ServerConfiguration : BaseConfiguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 8080;
    public bool UseHttps { get; set; } = false;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
```

### Nested Configurations

Create complex configuration hierarchies with nested objects:

```csharp
public class ApplicationConfiguration : BaseConfiguration
{
    // Simple properties
    public string ApplicationName { get; set; } = "MyApp";
    public string Version { get; set; } = "1.0.0";
    
    // Nested configurations
    public ServerConfiguration Server { get; set; } = new();
    public DatabaseConnectionConfiguration Database { get; set; } = new();
    public LoggingConfiguration Logging { get; set; } = new();
    public CacheConfiguration Cache { get; set; } = new();
}

public class LoggingConfiguration : BaseConfiguration
{
    public string Level { get; set; } = "Information";
    public bool EnableConsole { get; set; } = true;
    public bool EnableFile { get; set; } = false;
    public string FilePath { get; set; } = "logs/app.log";
}

public class CacheConfiguration : BaseConfiguration
{
    public string Provider { get; set; } = "Memory";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public int MaxSize { get; set; } = 1000;
}
```

### Built-in Database Configuration

The library includes a ready-to-use database configuration class:

```csharp
public class DatabaseConnectionConfiguration : BaseConfiguration
{
    [Required]
    public string Host { get; set; } = string.Empty;

    [DefaultValue(5432)]
    public int Port { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Database { get; set; } = string.Empty;

    // Auto-generated connection string
    public string ConnectionString => 
        $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database}";
}
```

This is just an example of how to create nested configuration classes. You can create your own:

```csharp
public class RedisConfiguration : BaseConfiguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    public string Password { get; set; } = string.Empty;
    public int Database { get; set; } = 0;
    
    public string ConnectionString => 
        $"{Host}:{Port},password={Password},defaultDatabase={Database}";
}

public class EmailConfiguration : BaseConfiguration
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
}
```

## 📝 Environment Variable Naming

Environment variables are automatically mapped from nested property paths (camelCase to snake_case):

| Configuration Path | Environment Variable  | Example Value    |
|--------------------|-----------------------|------------------|
| `ApiKey`           | `MYAPP_API_KEY`        | `secret123`      |
| `Server.Host`      | `MYAPP_SERVER_HOST`   | `localhost`      |
| `Server.Port`      | `MYAPP_SERVER_PORT`   | `8080`           |
| `Database.Host`    | `MYAPP_DATABASE_HOST` | `db.example.com` |
| `Database.Port`    | `MYAPP_DATABASE_PORT` | `5432`           |
| `Logging.Level`    | `MYAPP_LOGGING_LEVEL` | `Debug`          |
| `Cache.MaxSize`    | `MYAPP_CACHE_MAXSIZE` | `2000`           |

**Naming Rules:**

- Property names are converted to UPPERCASE
- Nested objects use underscore (`_`) as separator
- Prefix is added at the beginning if specified

## ✅ Validation

Use data annotations for automatic validation:

```csharp
public class ApiConfiguration : BaseConfiguration
{
    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string ApiKey { get; set; } = string.Empty;

    [Range(1000, 65535)]
    [DefaultValue(8080)]
    public int Port { get; set; }

    [Url]
    public string BaseUrl { get; set; } = "https://api.example.com";
}
```

If validation fails, a `ConfigurationValidationException` is thrown with detailed error information.

## 🔗 Supported Types

- **Primitives**: `int`, `long`, `double`, `bool`, `byte`, `short`, etc.
- **Common types**: `string`, `decimal`, `DateTime`, `TimeSpan`, `Uri`, `Guid`
- **Nullable types**: `int?`, `DateTime?`, etc.
- **Enums**: Any enum type (parsed case-insensitive)
- **Nested objects**: Classes inheriting from `BaseConfiguration`

```csharp
public enum LogLevel { Debug, Information, Warning, Error }

public class ExampleConfiguration : BaseConfiguration
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool Enabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeSpan Timeout { get; set; }
    public Uri Website { get; set; } = new("https://example.com");
    public Guid Id { get; set; }
    public LogLevel Level { get; set; } = LogLevel.Information;
    public int? OptionalNumber { get; set; }
}
```

## 🏛️ Architecture

The library follows a clean, modular architecture:

```
 ┌───────────────────────┐
 │   ServiceCollection   │
 │      Extensions       │
 └──────────┬────────────┘
            │
            ▼
 ┌───────────────────────┐
 │ IConfigurationValue   │
 │      Provider         │
 └──────────┬────────────┘
            │
            ▼
┌─────────────────────────┐
│  ConfigurationPopulator │
└───────────┬─────────────┘
            │
            ▼
 ┌───────────────────────┐
 │   BaseConfiguration   │
 │       Classes         │
 └───────────────────────┘
```

**Components:**

- **`IConfigurationValueProvider`**: Interface for value sources
- **`EnvironmentValueProvider`**: Reads from environment variables
- **`ConfigurationPopulator`**: Fills objects from providers
- **`BaseConfiguration`**: Base class for all configurations

This architecture allows easy extension with new providers (JSON, Database, etc.) without changing existing code.

## 📄 License

MIT License - feel free to use in your projects! 