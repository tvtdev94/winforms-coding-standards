# Configuration Management in WinForms

> **Quick Reference**: Best practices for managing application configuration in WinForms applications using modern .NET approaches.

---

## 📋 Overview

Configuration management is critical for:
- **Environment-specific settings** - Different configs for Dev, Test, Production
- **Secure credential storage** - Protecting sensitive data
- **Flexibility** - Changing behavior without recompilation
- **Maintainability** - Centralized configuration management

---

## 🎯 Why This Matters

✅ **Deployment Flexibility** - Same code, different environments
✅ **Security** - Proper handling of secrets and credentials
✅ **Maintainability** - Easy to update settings without code changes
✅ **Testability** - Mock configurations for testing

---

## 🗂️ Configuration Approaches

### App.config (.NET Framework)

Traditional XML-based configuration for .NET Framework applications.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <!-- App Settings -->
  <appSettings>
    <add key="MaxRetryCount" value="3" />
    <add key="TimeoutSeconds" value="30" />
    <add key="ApiUrl" value="https://api.example.com" />
  </appSettings>

  <!-- Connection Strings -->
  <connectionStrings>
    <add name="DefaultConnection"
         connectionString="Server=localhost;Database=MyApp;Integrated Security=true;"
         providerName="System.Data.SqlClient" />
  </connectionStrings>

  <!-- Custom Sections -->
  <system.diagnostics>
    <trace autoflush="true">
      <listeners>
        <add name="textWriterTraceListener"
             type="System.Diagnostics.TextWriterTraceListener"
             initializeData="app.log" />
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```

**Accessing in Code:**
```csharp
using System.Configuration;

public class ConfigurationHelper
{
    public static string GetAppSetting(string key)
    {
        return ConfigurationManager.AppSettings[key]
            ?? throw new ConfigurationErrorsException($"Missing setting: {key}");
    }

    public static string GetConnectionString(string name = "DefaultConnection")
    {
        return ConfigurationManager.ConnectionStrings[name]?.ConnectionString
            ?? throw new ConfigurationErrorsException($"Missing connection: {name}");
    }
}

// Usage
var apiUrl = ConfigurationHelper.GetAppSetting("ApiUrl");
var connectionString = ConfigurationHelper.GetConnectionString();
```

---

### appsettings.json (.NET 6+)

Modern JSON-based configuration for .NET 6+ WinForms applications.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Integrated Security=true;"
  },
  "AppSettings": {
    "MaxRetryCount": 3,
    "TimeoutSeconds": 30,
    "ApiUrl": "https://api.example.com",
    "Features": {
      "EnableCache": true,
      "EnableAudit": true
    }
  }
}
```

**Setup in Program.cs:**
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static class Program
{
    [STAThread]
    static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                    optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(Environment.GetCommandLineArgs());
            })
            .ConfigureServices((context, services) =>
            {
                // Bind configuration to strongly-typed objects
                services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));

                // Register services
                services.AddSingleton<MainForm>();
            })
            .Build();

        Application.Run(host.Services.GetRequiredService<MainForm>());
    }
}
```

---

### User Settings

User-specific and application-scoped settings using Visual Studio Settings Designer.

**Creating Settings:**
1. Right-click project → Properties → Settings
2. Add settings with scope (User/Application)

```csharp
// Settings.settings (auto-generated)
namespace MyApp.Properties
{
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        private static Settings defaultInstance =
            ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(
                new Settings())));

        public static Settings Default => defaultInstance;

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("800")]
        public int WindowWidth
        {
            get => (int)this["WindowWidth"];
            set => this["WindowWidth"] = value;
        }
    }
}
```

**Usage:**
```csharp
// Read settings
var width = Properties.Settings.Default.WindowWidth;

// Update settings
Properties.Settings.Default.WindowWidth = 1024;
Properties.Settings.Default.Save();

// Restore window position on load
private void MainForm_Load(object sender, EventArgs e)
{
    this.Width = Properties.Settings.Default.WindowWidth;
    this.Height = Properties.Settings.Default.WindowHeight;

    if (Properties.Settings.Default.RememberPosition)
    {
        this.StartPosition = FormStartPosition.Manual;
        this.Location = Properties.Settings.Default.WindowLocation;
    }
}

// Save window position on close
private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    Properties.Settings.Default.WindowWidth = this.Width;
    Properties.Settings.Default.WindowHeight = this.Height;
    Properties.Settings.Default.WindowLocation = this.Location;
    Properties.Settings.Default.Save();
}
```

---

### Environment Variables

Reading environment-specific configuration from OS environment variables.

```csharp
public class EnvironmentConfig
{
    public static string GetEnvironmentVariable(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }

    public static string DatabaseConnection =>
        GetEnvironmentVariable("DATABASE_CONNECTION",
            "Server=localhost;Database=Dev;Integrated Security=true;");

    public static string ApiKey =>
        GetEnvironmentVariable("API_KEY")
        ?? throw new InvalidOperationException("API_KEY environment variable not set");
}

// Usage
var connectionString = EnvironmentConfig.DatabaseConnection;
```

---

## 📁 Configuration Sources

### File-Based Configuration

```csharp
// JSON Configuration
public class JsonConfigLoader
{
    public static T LoadConfig<T>(string filePath) where T : new()
    {
        if (!File.Exists(filePath))
            return new T();

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json) ?? new T();
    }

    public static void SaveConfig<T>(string filePath, T config)
    {
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(filePath, json);
    }
}

// Usage
var config = JsonConfigLoader.LoadConfig<AppConfig>("config.json");
```

---

### Database Configuration

```csharp
public interface IConfigurationRepository
{
    Task<string?> GetSettingAsync(string key);
    Task SetSettingAsync(string key, string value);
}

public class DatabaseConfigurationService
{
    private readonly IConfigurationRepository _repository;
    private readonly Dictionary<string, string> _cache = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public DatabaseConfigurationService(IConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GetSettingAsync(string key, string defaultValue = "")
    {
        // Check cache first
        if (_cache.TryGetValue(key, out var cached))
            return cached;

        await _lock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(key, out cached))
                return cached;

            var value = await _repository.GetSettingAsync(key) ?? defaultValue;
            _cache[key] = value;
            return value;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void ClearCache() => _cache.Clear();
}
```

---

### Registry (Legacy - Use Sparingly)

```csharp
using Microsoft.Win32;

public class RegistryConfigReader
{
    private const string APP_KEY = @"SOFTWARE\MyCompany\MyApp";

    // ⚠️ Use only when absolutely necessary (legacy integration)
    public static string? ReadRegistryValue(string valueName)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(APP_KEY);
            return key?.GetValue(valueName)?.ToString();
        }
        catch (Exception ex)
        {
            // Log error
            return null;
        }
    }

    public static void WriteRegistryValue(string valueName, string value)
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(APP_KEY);
            key?.SetValue(valueName, value);
        }
        catch (Exception ex)
        {
            // Log error
        }
    }
}
```

---

### Command-Line Arguments

```csharp
public class CommandLineConfig
{
    private readonly Dictionary<string, string> _args = new();

    public CommandLineConfig(string[] args)
    {
        ParseArguments(args);
    }

    private void ParseArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--") && i + 1 < args.Length)
            {
                var key = args[i].Substring(2);
                var value = args[i + 1];
                _args[key] = value;
                i++; // Skip next arg
            }
        }
    }

    public string? GetValue(string key) => _args.TryGetValue(key, out var value) ? value : null;
}

// Usage: MyApp.exe --environment Production --port 8080
var cmdConfig = new CommandLineConfig(Environment.GetCommandLineArgs());
var environment = cmdConfig.GetValue("environment") ?? "Development";
```

---

## 🔒 Configuration Best Practices

### Sensitive Data - NEVER Store Passwords in Plain Text

```csharp
// ❌ WRONG - Plain text password
// appsettings.json
{
  "Database": {
    "Server": "localhost",
    "Username": "sa",
    "Password": "MyP@ssw0rd123" // ❌ NEVER DO THIS!
  }
}

// ✅ CORRECT - User Secrets (Development)
// Right-click project → Manage User Secrets
// secrets.json (outside source control)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;User=sa;Password=MyP@ssw0rd123;"
  }
}

// ✅ CORRECT - Environment Variables (Production)
// Set in deployment environment
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

// ✅ CORRECT - Azure Key Vault (Production)
var keyVaultUrl = "https://myvault.vault.azure.net/";
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
var secret = await client.GetSecretAsync("DatabasePassword");
```

---

### Connection Strings - Secure Storage

```csharp
// ❌ WRONG - Hardcoded
public class DataContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost;Database=MyDb;Password=secret;"); // ❌
    }
}

// ✅ CORRECT - From Configuration
public class DataContext : DbContext
{
    private readonly string _connectionString;

    public DataContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_connectionString);
    }
}

// ✅ CORRECT - Connection String Builder
public static string BuildConnectionString(IConfiguration config)
{
    var builder = new SqlConnectionStringBuilder
    {
        DataSource = config["Database:Server"],
        InitialCatalog = config["Database:Name"],
        IntegratedSecurity = true,
        TrustServerCertificate = true,
        ConnectTimeout = 30
    };
    return builder.ConnectionString;
}
```

---

### Configuration Hierarchy

Configuration sources are applied in order (later overrides earlier):

1. **appsettings.json** - Base configuration
2. **appsettings.{Environment}.json** - Environment-specific
3. **User Secrets** - Development only
4. **Environment Variables** - Production
5. **Command-Line Arguments** - Override all

```csharp
// Configuration is automatically merged
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddUserSecrets<Program>() // Development only
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
```

---

### Configuration Validation

```csharp
public class AppSettings
{
    public string ApiUrl { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; }
    public int TimeoutSeconds { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiUrl))
            throw new InvalidOperationException("ApiUrl is required");

        if (MaxRetryCount < 1 || MaxRetryCount > 10)
            throw new InvalidOperationException("MaxRetryCount must be between 1 and 10");

        if (TimeoutSeconds < 1)
            throw new InvalidOperationException("TimeoutSeconds must be positive");
    }
}

// Validate on startup
var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
appSettings.Validate();
```

---

## ⚙️ .NET Configuration System

### IConfiguration Interface

```csharp
public class MyService
{
    private readonly IConfiguration _configuration;

    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void UseConfiguration()
    {
        // Read simple value
        var apiUrl = _configuration["AppSettings:ApiUrl"];

        // Read with GetValue (with type and default)
        var timeout = _configuration.GetValue<int>("AppSettings:TimeoutSeconds", 30);

        // Read section
        var loggingConfig = _configuration.GetSection("Logging");

        // Read connection string
        var connStr = _configuration.GetConnectionString("DefaultConnection");
    }
}
```

---

### Options Pattern (Strongly-Typed)

```csharp
// Configuration class
public class AppSettings
{
    public const string SectionName = "AppSettings";

    public string ApiUrl { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; }
    public int TimeoutSeconds { get; set; }
    public FeatureSettings Features { get; set; } = new();
}

public class FeatureSettings
{
    public bool EnableCache { get; set; }
    public bool EnableAudit { get; set; }
}

// Register in DI
services.Configure<AppSettings>(
    configuration.GetSection(AppSettings.SectionName));

// Inject IOptions
public class CustomerService
{
    private readonly AppSettings _settings;

    public CustomerService(IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }

    public async Task LoadAsync()
    {
        var url = _settings.ApiUrl;
        var retries = _settings.MaxRetryCount;
        // Use settings...
    }
}
```

---

## 🌍 Environment-Specific Configuration

### Multiple Environment Files

```
/MyApp
  ├── appsettings.json                    # Base config
  ├── appsettings.Development.json        # Dev overrides
  ├── appsettings.Staging.json            # Staging overrides
  └── appsettings.Production.json         # Production overrides
```

**appsettings.json** (base):
```json
{
  "Logging": {
    "LogLevel": { "Default": "Information" }
  },
  "AppSettings": {
    "ApiUrl": "https://api.example.com",
    "MaxRetryCount": 3
  }
}
```

**appsettings.Development.json** (overrides):
```json
{
  "Logging": {
    "LogLevel": { "Default": "Debug" }
  },
  "AppSettings": {
    "ApiUrl": "https://dev-api.example.com"
  }
}
```

**Set Environment:**
```csharp
// Set via environment variable
Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

// Or via launchSettings.json (Visual Studio)
{
  "profiles": {
    "MyApp": {
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## 🛠️ Application Settings Manager

Complete centralized configuration service:

```csharp
public interface IAppSettingsService
{
    string GetSetting(string key, string defaultValue = "");
    T GetSetting<T>(string key, T defaultValue = default!);
    void SetSetting(string key, string value);
    void SaveSettings();
}

public class AppSettingsService : IAppSettingsService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, string> _runtimeSettings = new();

    public AppSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetSetting(string key, string defaultValue = "")
    {
        // Check runtime overrides first
        if (_runtimeSettings.TryGetValue(key, out var value))
            return value;

        // Check configuration
        return _configuration[key] ?? defaultValue;
    }

    public T GetSetting<T>(string key, T defaultValue = default!)
    {
        var value = GetSetting(key);
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public void SetSetting(string key, string value)
    {
        _runtimeSettings[key] = value;
    }

    public void SaveSettings()
    {
        // Save to file, database, etc.
        Properties.Settings.Default.Save();
    }
}
```

---

## ✅ Best Practices Summary

### DO:
✅ Store configuration in `appsettings.json` or `App.config`
✅ Use environment-specific config files
✅ Use User Secrets for development credentials
✅ Use Environment Variables for production secrets
✅ Validate configuration on startup
✅ Use strongly-typed configuration with Options pattern
✅ Use Connection String Builders for database connections
✅ Implement configuration hierarchy (defaults → environment → user)
✅ Cache database configuration for performance
✅ Document all configuration settings

### DON'T:
❌ Don't store passwords in plain text
❌ Don't commit secrets to source control
❌ Don't hardcode connection strings
❌ Don't use magic strings for config keys
❌ Don't ignore missing required settings
❌ Don't store sensitive data in User Settings
❌ Don't use Registry unless absolutely necessary
❌ Don't forget to validate configuration values

---

## 🔄 Configuration Patterns

### Hot Reload Configuration

```csharp
// Enable reload on change
config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Monitor changes with IOptionsMonitor
public class ConfigMonitorService
{
    private readonly IOptionsMonitor<AppSettings> _optionsMonitor;

    public ConfigMonitorService(IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;

        // Subscribe to changes
        _optionsMonitor.OnChange(settings =>
        {
            Console.WriteLine("Configuration changed!");
            // React to changes
        });
    }

    public AppSettings CurrentSettings => _optionsMonitor.CurrentValue;
}
```

---

## 📝 Configuration Checklist

- [ ] Configuration files not committed to source control (if containing secrets)
- [ ] User Secrets configured for local development
- [ ] Environment variables documented for production deployment
- [ ] Connection strings stored securely
- [ ] Configuration validated on application startup
- [ ] Strongly-typed configuration classes created
- [ ] Default values provided for optional settings
- [ ] Configuration hierarchy properly set up
- [ ] Sensitive data encrypted or stored in Key Vault
- [ ] Configuration documented in README

---

## 🔧 Troubleshooting

| Issue | Solution |
|-------|----------|
| Missing appsettings.json | Ensure "Copy to Output Directory" = "Copy if newer" |
| Configuration not loading | Check file path and build action |
| Secrets in source control | Add to .gitignore, use User Secrets |
| Environment not detected | Set DOTNET_ENVIRONMENT variable |
| Connection string fails | Validate format with ConnectionStringBuilder |
| Changes not reflecting | Restart app or enable reloadOnChange |

---

## 🔗 Related Topics

- [Dependency Injection](../architecture/dependency-injection.md) - Using IConfiguration with DI
- [Security](security.md) - Protecting sensitive configuration
- [Error Handling](error-handling.md) - Handling configuration errors

---

**Last Updated**: 2025-11-07
