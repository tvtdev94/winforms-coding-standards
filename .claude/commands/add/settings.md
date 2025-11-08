---
description: Setup application configuration and user settings management
---

You are tasked with setting up comprehensive configuration and settings management for a WinForms application.

## Workflow

1. **Ask for Settings Requirements**
   - What type of settings needed? (App-level, User-level, Connection strings)
   - Configuration format? (appsettings.json, App.config, User settings)
   - Environment-specific configs? (Development, Production)
   - Need encryption for sensitive data?

2. **Read Documentation**
   - Reference `docs/best-practices/configuration.md` for configuration patterns
   - Reference `docs/best-practices/security.md` for securing sensitive data

3. **Determine Configuration Strategy**

   ### Strategy 1: appsettings.json (Modern .NET - Recommended)
   - JSON-based configuration
   - Easy to read and modify
   - Supports hierarchical structure
   - Environment-specific overrides

   ### Strategy 2: App.config (Legacy .NET Framework)
   - XML-based configuration
   - Built-in support in .NET Framework
   - AppSettings and ConnectionStrings sections

   ### Strategy 3: User Settings (Per-User Preferences)
   - Settings.settings file
   - Stored per user in AppData
   - Strongly-typed access
   - Automatic save/load

4. **Install Required NuGet Packages**

For modern configuration (appsettings.json):
```bash
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions
```

5. **Create Configuration Files**

### Option 1: appsettings.json (Recommended for .NET 8)

**File: appsettings.json**
```json
{
  "AppSettings": {
    "AppName": "YourApp",
    "Version": "1.0.0",
    "Theme": "Light",
    "MaxRetryAttempts": 3,
    "EnableDetailedLogging": true,
    "AutoSaveInterval": 300,
    "DataDirectory": "Data"
  },

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db",
    "BackupConnection": "Data Source=backup.db"
  },

  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "SenderEmail": "noreply@yourapp.com",
    "SenderName": "YourApp Notifications"
  },

  "Features": {
    "EnableAdvancedFeatures": false,
    "EnableExportToPdf": true,
    "EnableCloudBackup": false,
    "MaxUploadSizeMB": 10
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

**File: appsettings.Development.json** (for development)
```json
{
  "AppSettings": {
    "EnableDetailedLogging": true,
    "AutoSaveInterval": 60
  },

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app-dev.db"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

**File: appsettings.Production.json** (for production)
```json
{
  "AppSettings": {
    "EnableDetailedLogging": false,
    "AutoSaveInterval": 600
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

6. **Create Strongly-Typed Settings Classes**

```csharp
namespace YourApp.Configuration;

/// <summary>
/// Application settings
/// </summary>
public class AppSettings
{
    public string AppName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Theme { get; set; } = "Light";
    public int MaxRetryAttempts { get; set; } = 3;
    public bool EnableDetailedLogging { get; set; }
    public int AutoSaveInterval { get; set; } = 300;
    public string DataDirectory { get; set; } = "Data";
}

/// <summary>
/// Email settings
/// </summary>
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;

    // Sensitive data - should be encrypted or in user secrets
    public string? Username { get; set; }
    public string? Password { get; set; }
}

/// <summary>
/// Feature flags
/// </summary>
public class FeatureSettings
{
    public bool EnableAdvancedFeatures { get; set; }
    public bool EnableExportToPdf { get; set; }
    public bool EnableCloudBackup { get; set; }
    public int MaxUploadSizeMB { get; set; } = 10;
}
```

7. **Configure in Program.cs**

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Windows.Forms;
using YourApp.Configuration;

namespace YourApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Build configuration
        var configuration = BuildConfiguration();

        // Setup DI container
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Run application
        var mainForm = serviceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

    private static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production";

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            // Load base configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // Load environment-specific configuration
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            // Load user secrets (development only)
#if DEBUG
            .AddUserSecrets<Program>(optional: true)
#endif
            // Environment variables override all
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add configuration itself
        services.AddSingleton(configuration);

        // Bind configuration sections to strongly-typed classes
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<FeatureSettings>(configuration.GetSection("Features"));

        // Add logging
        services.AddLogging(/* ... */);

        // Add DbContext with connection string
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlite(connectionString);
        });

        // Register your services
        services.AddScoped<ICustomerService, CustomerService>();

        // Register forms
        services.AddTransient<MainForm>();
    }
}
```

8. **Use Settings in Services/Forms**

### In a Service:
```csharp
using Microsoft.Extensions.Options;
using YourApp.Configuration;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To} via {SmtpServer}:{Port}",
            to, _emailSettings.SmtpServer, _emailSettings.SmtpPort);

        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new NetworkCredential(
                _emailSettings.Username,
                _emailSettings.Password)
        };

        var message = new MailMessage(
            _emailSettings.SenderEmail,
            to,
            subject,
            body);

        await client.SendMailAsync(message);

        _logger.LogInformation("Email sent successfully");
    }
}
```

### In a Form:
```csharp
using Microsoft.Extensions.Options;
using YourApp.Configuration;

public partial class MainForm : Form
{
    private readonly AppSettings _appSettings;
    private readonly FeatureSettings _featureSettings;
    private readonly ILogger<MainForm> _logger;

    public MainForm(
        IOptions<AppSettings> appSettings,
        IOptions<FeatureSettings> featureSettings,
        ILogger<MainForm> logger)
    {
        InitializeComponent();

        _appSettings = appSettings.Value;
        _featureSettings = featureSettings.Value;
        _logger = logger;

        ConfigureFromSettings();
    }

    private void ConfigureFromSettings()
    {
        // Set window title with app name and version
        Text = $"{_appSettings.AppName} v{_appSettings.Version}";

        // Apply theme
        ApplyTheme(_appSettings.Theme);

        // Enable/disable features based on settings
        menuAdvancedFeatures.Visible = _featureSettings.EnableAdvancedFeatures;
        menuExportPdf.Visible = _featureSettings.EnableExportToPdf;
        menuCloudBackup.Visible = _featureSettings.EnableCloudBackup;

        // Setup auto-save timer
        if (_appSettings.AutoSaveInterval > 0)
        {
            var timer = new System.Windows.Forms.Timer
            {
                Interval = _appSettings.AutoSaveInterval * 1000 // Convert to milliseconds
            };
            timer.Tick += AutoSave_Tick;
            timer.Start();

            _logger.LogInformation("Auto-save enabled with interval: {Interval}s",
                _appSettings.AutoSaveInterval);
        }
    }

    private void ApplyTheme(string theme)
    {
        // Apply theme based on setting
        if (theme == "Dark")
        {
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.White;
        }
        else
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
        }
    }
}
```

9. **User Settings (Per-User Preferences)**

For user-specific settings that can be modified at runtime:

**Create Settings in Visual Studio:**
1. Right-click project → Properties → Settings
2. Add settings (Name, Type, Scope, Value)

OR manually create `Settings.settings`:

```xml
<?xml version='1.0' encoding='utf-8'?>
<SettingsFile xmlns="http://schemas.microsoft.com/VisualStudio/2004/01/settings"
              CurrentProfile="(Default)">
  <Profiles>
    <Profile Name="(Default)" />
  </Profiles>
  <Settings>
    <Setting Name="WindowWidth" Type="System.Int32" Scope="User">
      <Value Profile="(Default)">800</Value>
    </Setting>
    <Setting Name="WindowHeight" Type="System.Int32" Scope="User">
      <Value Profile="(Default)">600</Value>
    </Setting>
    <Setting Name="LastOpenedFile" Type="System.String" Scope="User">
      <Value Profile="(Default)"></Value>
    </Setting>
    <Setting Name="RecentFiles" Type="System.Collections.Specialized.StringCollection" Scope="User">
      <Value Profile="(Default)" />
    </Setting>
  </Settings>
</SettingsFile>
```

**Using User Settings:**
```csharp
using YourApp.Properties;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        LoadUserSettings();
    }

    private void LoadUserSettings()
    {
        // Restore window size from user settings
        if (Settings.Default.WindowWidth > 0)
        {
            Width = Settings.Default.WindowWidth;
            Height = Settings.Default.WindowHeight;
        }

        // Restore last opened file
        if (!string.IsNullOrEmpty(Settings.Default.LastOpenedFile))
        {
            // Load the file
            LoadFile(Settings.Default.LastOpenedFile);
        }

        // Restore recent files list
        if (Settings.Default.RecentFiles != null)
        {
            PopulateRecentFilesMenu(Settings.Default.RecentFiles);
        }
    }

    private void SaveUserSettings()
    {
        // Save window size
        Settings.Default.WindowWidth = Width;
        Settings.Default.WindowHeight = Height;

        // Save last opened file
        Settings.Default.LastOpenedFile = _currentFilePath;

        // Save settings to disk
        Settings.Default.Save();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SaveUserSettings();
        base.OnFormClosing(e);
    }
}
```

10. **Encrypting Sensitive Configuration**

For sensitive data like passwords:

```csharp
using System.Security.Cryptography;
using System.Text;

public static class ConfigurationEncryption
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourSecretKey123"); // Use secure key
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourSecretIV1234");  // Use secure IV

    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);

        sw.Write(plainText);
        sw.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}

// Usage:
var encryptedPassword = ConfigurationEncryption.Encrypt("MyPassword123");
// Store encryptedPassword in config

// Later, decrypt when needed:
var password = ConfigurationEncryption.Decrypt(encryptedPassword);
```

11. **Settings Manager Service (Optional)**

```csharp
public interface ISettingsService
{
    T GetSetting<T>(string key, T defaultValue = default);
    void SetSetting<T>(string key, T value);
    void SaveSettings();
}

public class SettingsService : ISettingsService
{
    private readonly IConfiguration _configuration;

    public SettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T GetSetting<T>(string key, T defaultValue = default)
    {
        var value = _configuration[key];
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public void SetSetting<T>(string key, T value)
    {
        // For runtime changes, you'd need to write back to file
        // This is more complex with IConfiguration
        // Consider using IOptionsSnapshot for reloadable config
    }

    public void SaveSettings()
    {
        // Implementation depends on your needs
    }
}
```

12. **Configuration Best Practices**

```csharp
// ✅ GOOD: Use strongly-typed configuration
public class MyService
{
    public MyService(IOptions<AppSettings> settings) { }
}

// ❌ BAD: Access configuration directly with magic strings
public class MyService
{
    public MyService(IConfiguration config)
    {
        var value = config["AppSettings:SomeValue"]; // Hard to refactor
    }
}

// ✅ GOOD: Validate configuration on startup
services.PostConfigure<AppSettings>(settings =>
{
    if (settings.MaxRetryAttempts < 1)
        throw new InvalidOperationException("MaxRetryAttempts must be >= 1");
});

// ✅ GOOD: Use IOptionsSnapshot for reloadable config
public class MyService
{
    public MyService(IOptionsSnapshot<AppSettings> settings)
    {
        // Settings reload when appsettings.json changes
    }
}

// ✅ GOOD: Separate sensitive data
// Don't put passwords in appsettings.json
// Use User Secrets (dev) or Azure Key Vault (prod)
```

13. **Verification Checklist**

```
✅ appsettings.json created in project root
✅ appsettings.{Environment}.json for each environment
✅ Settings classes created (AppSettings, etc.)
✅ Configuration loaded in Program.cs
✅ Settings bound to classes via Configure<T>
✅ IOptions<T> injected where needed
✅ Connection strings in ConnectionStrings section
✅ Sensitive data encrypted or in user secrets
✅ User settings (Settings.settings) for user prefs
✅ Settings loaded/saved on app start/close
✅ Application runs and settings are applied
```

## Best Practices Checklist

Before finishing, verify:
- ✅ JSON files set to "Copy if newer"
- ✅ Strongly-typed classes for all settings
- ✅ IOptions<T> used for dependency injection
- ✅ Sensitive data NOT in source control
- ✅ Environment-specific configs for dev/prod
- ✅ Default values provided in code
- ✅ Configuration validated on startup
- ✅ User settings saved on form close
- ✅ No hard-coded values in code
- ✅ Connection strings in separate section

## Configuration Hierarchy (Order of Priority)

1. **Environment Variables** (highest priority)
2. **User Secrets** (development only)
3. **appsettings.{Environment}.json**
4. **appsettings.json**
5. **Default values in code** (lowest priority)

## Common Settings Categories

### Application Settings
- App name, version
- Theme, UI preferences
- Feature flags
- Performance tuning

### Connection Strings
- Database connections
- API endpoints
- External services

### Email Settings
- SMTP configuration
- Sender information
- Templates

### User Preferences
- Window size/position
- Recent files
- Language
- Custom shortcuts

## Security Notes

- ❌ **NEVER** commit sensitive data (passwords, API keys) to source control
- ✅ Use **User Secrets** for development (right-click project → Manage User Secrets)
- ✅ Use **Azure Key Vault** or similar for production
- ✅ Encrypt sensitive data in config files
- ✅ Use **environment variables** for server-specific config
- ✅ Restrict file permissions on config files

## Notes

- **appsettings.json** is for application-level config
- **Settings.settings** is for user-level preferences
- **IOptions<T>** provides snapshot of config at injection time
- **IOptionsSnapshot<T>** reloads config when file changes
- **IOptionsMonitor<T>** provides change notifications
- Always validate configuration on startup to fail fast
