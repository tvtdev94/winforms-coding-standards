# Security Best Practices in WinForms

## 📋 Overview

Security is a critical aspect of any application, including desktop WinForms applications. While desktop apps may seem less vulnerable than web applications, they still face significant security risks including data breaches, unauthorized access, and malicious attacks. This guide covers essential security practices for building secure WinForms applications.

**Key Security Principles**:
- **Defense in Depth**: Multiple layers of security
- **Principle of Least Privilege**: Minimal permissions required
- **Fail Securely**: Default to denying access
- **Never Trust User Input**: Always validate and sanitize
- **Security by Design**: Build security in from the start

---

## 🎯 Why This Matters

### The Cost of Security Breaches

- **Data Breaches**: Customer data theft can destroy trust and business
- **Compliance Violations**: GDPR, HIPAA, PCI-DSS fines can be devastating
- **Reputation Damage**: Recovery from security incidents is difficult
- **Financial Loss**: Direct costs, legal fees, and lost revenue
- **Legal Liability**: Lawsuits from affected customers

### WinForms Security Landscape

Desktop applications have unique security concerns:
- Direct database access (higher risk than web apps with middleware)
- Local data storage (sensitive data on user machines)
- Reverse engineering risks (code can be decompiled)
- DLL injection and hijacking vulnerabilities
- Network communication security

---

## 🛡️ Input Validation and Sanitization

### Always Validate User Input

**Never trust client input** - even in desktop applications. Users may intentionally or accidentally provide malicious input.

#### Validation Strategies

**Whitelist vs Blacklist**:
- ✅ **Whitelist** (Preferred): Define what IS allowed
- ❌ **Blacklist**: Define what is NOT allowed (easy to bypass)

#### ❌ Unsafe: No Validation

```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    // DANGEROUS: Direct use without validation
    var customer = new Customer
    {
        Name = txtName.Text,
        Email = txtEmail.Text,
        Age = int.Parse(txtAge.Text) // Can throw exception
    };

    _customerService.Save(customer);
}
```

#### ✅ Safe: Proper Validation

```csharp
using System.Text.RegularExpressions;

private void btnSave_Click(object sender, EventArgs e)
{
    // Clear previous errors
    errorProvider.Clear();

    // Validate name (whitelist: letters, spaces, hyphens only)
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        errorProvider.SetError(txtName, "Name is required");
        return;
    }

    if (!Regex.IsMatch(txtName.Text, @"^[a-zA-Z\s\-]{1,100}$"))
    {
        errorProvider.SetError(txtName, "Name contains invalid characters");
        return;
    }

    // Validate email
    if (!IsValidEmail(txtEmail.Text))
    {
        errorProvider.SetError(txtEmail, "Invalid email format");
        return;
    }

    // Validate age with safe parsing
    if (!int.TryParse(txtAge.Text, out int age) || age < 0 || age > 150)
    {
        errorProvider.SetError(txtAge, "Age must be between 0 and 150");
        return;
    }

    var customer = new Customer
    {
        Name = txtName.Text.Trim(),
        Email = txtEmail.Text.Trim().ToLowerInvariant(),
        Age = age
    };

    _customerService.Save(customer);
}

private bool IsValidEmail(string email)
{
    if (string.IsNullOrWhiteSpace(email))
        return false;

    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}
```

---

## 💉 SQL Injection Prevention

### CRITICAL: Always Use Parameterized Queries

SQL injection is one of the most dangerous vulnerabilities. **NEVER** concatenate user input into SQL strings.

#### ❌ VULNERABLE: String Concatenation

```csharp
// EXTREMELY DANGEROUS - DO NOT USE
public List<User> GetUsersByName(string name)
{
    var sql = "SELECT * FROM Users WHERE Name = '" + name + "'";
    // Attacker input: "' OR '1'='1" returns ALL users
    // Attacker input: "'; DROP TABLE Users; --" deletes table!

    return _context.Database.SqlQueryRaw<User>(sql).ToList();
}
```

#### ✅ SAFE: Parameterized Queries

```csharp
// SAFE: Using EF Core with parameters
public List<User> GetUsersByName(string name)
{
    // EF Core automatically parameterizes
    return _context.Users
        .Where(u => u.Name == name)
        .ToList();
}

// SAFE: Raw SQL with parameters
public List<User> SearchUsers(string searchTerm)
{
    return _context.Users
        .FromSqlRaw("SELECT * FROM Users WHERE Name = {0}", searchTerm)
        .ToList();
}

// SAFE: Using ADO.NET with parameters
public List<User> GetUsersAdoNet(string name)
{
    var users = new List<User>();

    using (var connection = new SqlConnection(_connectionString))
    using (var command = new SqlCommand(
        "SELECT * FROM Users WHERE Name = @Name", connection))
    {
        command.Parameters.AddWithValue("@Name", name);

        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
        }
    }

    return users;
}
```

### Entity Framework Protection

Entity Framework Core automatically protects against SQL injection when using LINQ:

```csharp
// ✅ SAFE: EF Core parameterizes automatically
public async Task<List<Product>> SearchProducts(string category, decimal minPrice)
{
    return await _context.Products
        .Where(p => p.Category == category && p.Price >= minPrice)
        .ToListAsync();
}
```

---

## ⚠️ Command Injection

### Process.Start Risks

Running external processes with user input can lead to command injection.

#### ❌ DANGEROUS: Unvalidated Process Execution

```csharp
// DANGEROUS: User controls file path
private void btnOpenFile_Click(object sender, EventArgs e)
{
    var filePath = txtFilePath.Text;
    Process.Start(filePath); // Can execute malicious programs!
}
```

#### ✅ SAFE: Validated and Controlled Execution

```csharp
private void btnOpenFile_Click(object sender, EventArgs e)
{
    var filePath = txtFilePath.Text;

    // Validate file exists
    if (!File.Exists(filePath))
    {
        MessageBox.Show("File not found", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    // Validate file extension (whitelist)
    var allowedExtensions = new[] { ".txt", ".pdf", ".docx" };
    var extension = Path.GetExtension(filePath).ToLowerInvariant();

    if (!allowedExtensions.Contains(extension))
    {
        MessageBox.Show("File type not allowed", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    // Use ProcessStartInfo for better control
    var startInfo = new ProcessStartInfo
    {
        FileName = filePath,
        UseShellExecute = true,
        Verb = "open" // Prevents command execution
    };

    try
    {
        Process.Start(startInfo);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to open file: {FilePath}", filePath);
        MessageBox.Show("Failed to open file", "Error");
    }
}
```

---

## 📁 Path Traversal Prevention

### File Path Validation

Prevent users from accessing files outside allowed directories.

#### ❌ VULNERABLE: No Path Validation

```csharp
// DANGEROUS: Path traversal attack
// User input: "../../Windows/System32/config/SAM"
public string ReadFile(string fileName)
{
    var filePath = Path.Combine(@"C:\AppData", fileName);
    return File.ReadAllText(filePath); // Can access any file!
}
```

#### ✅ SAFE: Path Validation

```csharp
public string ReadFile(string fileName)
{
    var baseDirectory = @"C:\AppData";

    // Combine paths
    var filePath = Path.Combine(baseDirectory, fileName);

    // Get full canonical path
    var fullPath = Path.GetFullPath(filePath);

    // Ensure the file is within the allowed directory
    if (!fullPath.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
    {
        throw new UnauthorizedAccessException("Access denied");
    }

    // Additional validation: check file exists
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException("File not found");
    }

    return File.ReadAllText(fullPath);
}
```

---

## 🔐 Authentication and Authorization

### Password Handling

#### ❌ NEVER Store Plain Text Passwords

```csharp
// EXTREMELY DANGEROUS - NEVER DO THIS
public class User
{
    public string Username { get; set; }
    public string Password { get; set; } // Plain text - WRONG!
}

public bool Login(string username, string password)
{
    var user = _context.Users.FirstOrDefault(u => u.Username == username);
    return user?.Password == password; // Comparing plain text - WRONG!
}
```

#### ✅ SAFE: Password Hashing with BCrypt

```csharp
using BCrypt.Net;

public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; } // Hashed, not plain text
    public string Salt { get; set; } // BCrypt includes salt in hash
}

public class AuthenticationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthenticationService> _logger;

    // Register new user
    public async Task<bool> RegisterUser(string username, string password)
    {
        // Validate password strength
        if (!IsPasswordStrong(password))
        {
            throw new ArgumentException("Password does not meet requirements");
        }

        // Hash password with BCrypt (automatically includes salt)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, 12);

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    // Authenticate user
    public async Task<User?> Login(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            // Don't reveal whether username exists
            _logger.LogWarning("Login attempt for non-existent user: {Username}", username);
            return null;
        }

        // Verify password against hash
        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogInformation("Successful login: {Username}", username);
            return user;
        }

        _logger.LogWarning("Failed login attempt: {Username}", username);
        return null;
    }

    private bool IsPasswordStrong(string password)
    {
        // Minimum 8 characters, at least one uppercase, one lowercase, one digit, one special char
        if (password.Length < 8)
            return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
```

### Account Lockout

Prevent brute force attacks:

```csharp
public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
}

public async Task<(bool Success, string Message)> LoginWithLockout(
    string username, string password)
{
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Username == username);

    if (user == null)
        return (false, "Invalid username or password");

    // Check if account is locked
    if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
    {
        var remainingTime = user.LockoutEnd.Value - DateTime.UtcNow;
        return (false, $"Account locked. Try again in {remainingTime.Minutes} minutes");
    }

    // Verify password
    if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
    {
        // Increment failed attempts
        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= 5)
        {
            // Lock account for 30 minutes
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Account locked due to failed attempts: {Username}", username);
            return (false, "Too many failed attempts. Account locked for 30 minutes");
        }

        await _context.SaveChangesAsync();
        return (false, "Invalid username or password");
    }

    // Successful login - reset failed attempts
    user.FailedLoginAttempts = 0;
    user.LockoutEnd = null;
    await _context.SaveChangesAsync();

    return (true, "Login successful");
}
```

### Role-Based Access Control

```csharp
public enum UserRole
{
    User,
    Manager,
    Administrator
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
}

// Service layer authorization
public class CustomerService
{
    private readonly User _currentUser;

    public async Task<bool> DeleteCustomer(int customerId)
    {
        // Check authorization
        if (_currentUser.Role != UserRole.Administrator)
        {
            throw new UnauthorizedAccessException(
                "Only administrators can delete customers");
        }

        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}

// UI authorization
public class MainForm : Form
{
    private readonly User _currentUser;

    private void MainForm_Load(object sender, EventArgs e)
    {
        // Hide/disable UI elements based on role
        btnDeleteCustomer.Visible = _currentUser.Role == UserRole.Administrator;
        menuItemAdminPanel.Enabled = _currentUser.Role == UserRole.Administrator;

        // Note: UI hiding is NOT security - still validate in business logic!
    }
}
```

---

## 🔒 Sensitive Data Protection

### Connection Strings

#### ❌ NEVER Hardcode Connection Strings

```csharp
// DANGEROUS: Exposed in source code
public class DatabaseService
{
    private const string ConnectionString =
        "Server=myserver;Database=mydb;User Id=sa;Password=P@ssw0rd123;";
}
```

#### ✅ SAFE: Configuration-Based Storage

**appsettings.json** (for development):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyApp;Trusted_Connection=True;"
  }
}
```

**User Secrets** (for development):
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Password=..."
```

**Environment Variables** (for production):
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Reads from environment variables
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Connection string not configured");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
```

### Encrypted Configuration

```csharp
using System.Security.Cryptography;
using System.Text;

public class SecureConfigurationManager
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public SecureConfigurationManager()
    {
        // In production, retrieve from secure storage
        _key = Encoding.UTF8.GetBytes("your-32-byte-key-here-12345678");
        _iv = Encoding.UTF8.GetBytes("your-16-byte-iv!");
    }

    public string EncryptValue(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string DecryptValue(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var buffer = Convert.FromBase64String(cipherText);

        using var msDecrypt = new MemoryStream(buffer);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
```

### DPAPI for Local Storage

```csharp
using System.Security.Cryptography;

public class SecureLocalStorage
{
    // Encrypt data using Windows DPAPI (user-specific)
    public void SaveSecureData(string key, string value)
    {
        var plainBytes = Encoding.UTF8.GetBytes(value);
        var encryptedBytes = ProtectedData.Protect(
            plainBytes,
            null, // Optional entropy
            DataProtectionScope.CurrentUser); // User-specific

        var encryptedValue = Convert.ToBase64String(encryptedBytes);

        // Save to file or registry
        File.WriteAllText($"{key}.encrypted", encryptedValue);
    }

    public string LoadSecureData(string key)
    {
        var encryptedValue = File.ReadAllText($"{key}.encrypted");
        var encryptedBytes = Convert.FromBase64String(encryptedValue);

        var plainBytes = ProtectedData.Unprotect(
            encryptedBytes,
            null,
            DataProtectionScope.CurrentUser);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
```

---

## 🌐 Secure Communication

### Always Use HTTPS

#### ❌ UNSAFE: HTTP Communication

```csharp
// DANGEROUS: Unencrypted communication
var client = new HttpClient();
var response = await client.GetAsync("http://api.example.com/users");
```

#### ✅ SAFE: HTTPS with Certificate Validation

```csharp
public class SecureApiClient
{
    private readonly HttpClient _httpClient;

    public SecureApiClient()
    {
        var handler = new HttpClientHandler
        {
            // Enforce TLS 1.2 or higher
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                         | System.Security.Authentication.SslProtocols.Tls13,

            // Validate server certificate
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                // In production, implement proper validation
                if (errors == System.Net.Security.SslPolicyErrors.None)
                    return true;

                // Log certificate errors
                Console.WriteLine($"Certificate error: {errors}");
                return false;
            }
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.com")
        };
    }

    public async Task<List<User>> GetUsers()
    {
        var response = await _httpClient.GetAsync("/users");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<User>>();
    }
}
```

---

## 🛠️ Best Practices Summary

### ✅ DO

1. **Always validate and sanitize user input**
   ```csharp
   if (!Regex.IsMatch(input, @"^[a-zA-Z0-9]+$"))
       throw new ArgumentException("Invalid input");
   ```

2. **Use parameterized queries for database access**
   ```csharp
   var users = _context.Users.Where(u => u.Name == name).ToList();
   ```

3. **Hash passwords with BCrypt or Argon2**
   ```csharp
   var hash = BCrypt.Net.BCrypt.HashPassword(password, 12);
   ```

4. **Store sensitive configuration in secure locations**
   ```csharp
   var apiKey = Environment.GetEnvironmentVariable("API_KEY");
   ```

5. **Use HTTPS for all network communication**
   ```csharp
   var client = new HttpClient { BaseAddress = new Uri("https://...") };
   ```

6. **Implement proper authentication and authorization**
   ```csharp
   if (_currentUser.Role != UserRole.Admin)
       throw new UnauthorizedAccessException();
   ```

7. **Encrypt sensitive data at rest**
   ```csharp
   var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
   ```

8. **Log security events (without sensitive data)**
   ```csharp
   _logger.LogWarning("Failed login attempt for user: {Username}", username);
   ```

9. **Keep dependencies updated**
   ```bash
   dotnet list package --vulnerable
   dotnet add package PackageName
   ```

10. **Implement account lockout mechanisms**
    ```csharp
    if (failedAttempts >= 5)
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
    ```

11. **Validate file paths to prevent traversal attacks**
    ```csharp
    if (!fullPath.StartsWith(baseDirectory))
        throw new UnauthorizedAccessException();
    ```

12. **Use least privilege principle**
    ```csharp
    // Run with minimum required permissions
    ```

### ❌ DON'T

1. **Never store plain text passwords**
2. **Never concatenate SQL queries with user input**
3. **Never trust user input without validation**
4. **Never hardcode sensitive credentials**
5. **Never use HTTP for sensitive data**
6. **Never log passwords or sensitive PII**
7. **Never disable certificate validation in production**
8. **Never execute user-provided file paths without validation**
9. **Never reveal system details in error messages**
10. **Never commit secrets to source control**

---

## ✅ Security Checklist

Before deploying your application:

- [ ] All user input is validated and sanitized
- [ ] SQL queries use parameterized statements
- [ ] Passwords are hashed (BCrypt/Argon2)
- [ ] Connection strings are not hardcoded
- [ ] API keys are stored securely (not in code)
- [ ] HTTPS is enforced for network communication
- [ ] File paths are validated for traversal attacks
- [ ] Authentication and authorization are implemented
- [ ] Sensitive data is encrypted at rest
- [ ] Security events are logged (without PII)
- [ ] Account lockout is implemented
- [ ] Dependencies are scanned for vulnerabilities
- [ ] Error messages don't reveal system details
- [ ] Least privilege principle is applied
- [ ] Code has been reviewed for security issues

---

## 📝 Secure Logging

### What to Log

```csharp
// ✅ SAFE: Log security events without sensitive data
_logger.LogInformation("User logged in: {Username}", username);
_logger.LogWarning("Failed login attempt for: {Username}", username);
_logger.LogError("Access denied for user {UserId} to resource {ResourceId}",
    userId, resourceId);
```

### What NOT to Log

```csharp
// ❌ DANGEROUS: Logging sensitive data
_logger.LogInformation("User {Username} logged in with password {Password}",
    username, password); // NEVER log passwords!

_logger.LogInformation("Credit card: {CardNumber}", cardNumber); // NEVER log PII!

_logger.LogError("Database connection failed: {ConnectionString}",
    connectionString); // NEVER log credentials!
```

### Secure Logging Example

```csharp
public class SecureLogger
{
    private readonly ILogger _logger;

    public void LogUserAction(User user, string action)
    {
        // Log user ID, not entire user object (may contain sensitive data)
        _logger.LogInformation(
            "User action: {Action} by UserId: {UserId} at {Timestamp}",
            action, user.Id, DateTime.UtcNow);
    }

    public void LogException(Exception ex, string context)
    {
        // Log exception without sensitive data from inner properties
        _logger.LogError(ex,
            "Error in {Context}: {Message}",
            context, ex.Message);

        // Don't log entire exception.ToString() - may contain sensitive data
    }
}
```

---

## 🔍 Complete Working Example: Secure Login Form

```csharp
using Microsoft.Extensions.Logging;
using BCrypt.Net;

public class LoginForm : Form
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginForm> _logger;

    private TextBox txtUsername;
    private TextBox txtPassword;
    private Button btnLogin;
    private ErrorProvider errorProvider;

    public LoginForm(
        IAuthenticationService authService,
        ILogger<LoginForm> logger)
    {
        _authService = authService;
        _logger = logger;
        InitializeComponent();
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        errorProvider.Clear();

        // Validate input
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            errorProvider.SetError(txtUsername, "Username is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            errorProvider.SetError(txtPassword, "Password is required");
            return;
        }

        // Validate username format (alphanumeric only)
        if (!Regex.IsMatch(txtUsername.Text, @"^[a-zA-Z0-9]{3,20}$"))
        {
            errorProvider.SetError(txtUsername,
                "Username must be 3-20 alphanumeric characters");
            return;
        }

        try
        {
            btnLogin.Enabled = false;

            var (success, message) = await _authService.LoginAsync(
                txtUsername.Text.Trim(),
                txtPassword.Text);

            if (success)
            {
                _logger.LogInformation("Successful login: {Username}",
                    txtUsername.Text);

                // Clear password from memory
                txtPassword.Clear();

                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _logger.LogWarning("Failed login attempt: {Username}",
                    txtUsername.Text);

                MessageBox.Show(message, "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Clear password
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for user: {Username}",
                txtUsername.Text);

            MessageBox.Show("An error occurred. Please try again.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Clear sensitive data from memory
        txtPassword.Clear();
        base.OnFormClosing(e);
    }
}

public interface IAuthenticationService
{
    Task<(bool Success, string Message)> LoginAsync(string username, string password);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthenticationService> _logger;

    public async Task<(bool Success, string Message)> LoginAsync(
        string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            // Don't reveal whether username exists
            await Task.Delay(100); // Timing attack mitigation
            return (false, "Invalid username or password");
        }

        // Check lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            var remaining = user.LockoutEnd.Value - DateTime.UtcNow;
            return (false, $"Account locked. Try again in {remaining.Minutes} minutes");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();

                _logger.LogWarning("Account locked: {Username}", username);
                return (false, "Too many failed attempts. Account locked for 30 minutes");
            }

            await _context.SaveChangesAsync();
            return (false, "Invalid username or password");
        }

        // Success - reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successful login: {Username}", username);
        return (true, "Login successful");
    }
}
```

---

## 🧪 Security Testing

### Code Review Checklist

- [ ] Review all user input handling
- [ ] Check for SQL injection vulnerabilities
- [ ] Verify password hashing implementation
- [ ] Ensure secrets are not hardcoded
- [ ] Validate file path handling
- [ ] Review authentication/authorization logic
- [ ] Check logging for sensitive data exposure
- [ ] Verify HTTPS enforcement

### Tools

- **OWASP Dependency-Check**: Scan for vulnerable dependencies
- **SonarQube**: Static code analysis for security issues
- **Snyk**: Vulnerability scanning
- **dotnet list package --vulnerable**: Check for vulnerable NuGet packages

```bash
# Check for vulnerable packages
dotnet list package --vulnerable

# Update packages
dotnet add package PackageName --version x.x.x
```

---

## 🔗 Related Topics

- [Error Handling & Logging](error-handling.md) - Secure logging practices
- [Configuration Management](configuration.md) - Secure configuration storage
- [Input Validation](../ui-ux/input-validation.md) - UI validation techniques
- [Thread Safety](thread-safety.md) - Secure multi-threading

---

**Key Takeaways**:
1. Never trust user input - always validate
2. Use parameterized queries - prevent SQL injection
3. Hash passwords with BCrypt - never plain text
4. Store secrets securely - never hardcode
5. Use HTTPS always - encrypt in transit
6. Implement proper authentication and authorization
7. Log security events - but never sensitive data
8. Keep dependencies updated - patch vulnerabilities

Security is not optional. Build it in from the start, not as an afterthought.
