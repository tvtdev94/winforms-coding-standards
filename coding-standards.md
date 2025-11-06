# 📘 C# WinForms Project Guide

## 🎯 Mục tiêu

Hướng dẫn quy ước và tiêu chuẩn mã nguồn cho dự án C# WinForms này, giúp đảm bảo code **dễ đọc, dễ bảo trì, và nhất quán** giữa các lập trình viên cũng như các AI assistant (Claude Code).

---

## 🏗️ Kiến trúc & Cấu trúc dự án

### Cấu trúc chuẩn:

```bash
/ProjectName
    ├── /Forms              # Windows Forms (UI Layer)
    ├── /Controls           # Custom user controls
    ├── /Models             # Business/data models
    ├── /Services           # Xử lý logic nghiệp vụ
    ├── /Repositories       # Giao tiếp dữ liệu (DB, file, API)
    ├── /Utils              # Helper, extension methods
    ├── /Resources          # Icons, strings, localization
    ├── Program.cs
    └── App.config
```

### Nguyên tắc:

- ✅ Mỗi form hoặc control phải có **logic tối thiểu** (UI handling), không chứa nghiệp vụ.
- ✅ Logic nghiệp vụ đặt trong **Services**, không trong UI.
- ✅ **Dependency Injection (DI)** nếu có thể (sử dụng `Microsoft.Extensions.DependencyInjection`).

### 🏛️ Architecture Patterns (2024 Recommendations)

**WinForms hỗ trợ các patterns sau:**

1. **MVP (Model-View-Presenter)** - ⭐ RECOMMENDED cho WinForms
   - View (Form) implements IView interface
   - Presenter xử lý logic, không reference trực tiếp view
   - Phù hợp nhất cho WinForms (không có data binding mạnh như WPF)
   - Microsoft CAB framework sử dụng MVP

2. **MVVM (Model-View-ViewModel)** - ⚡ NEW: Có thể dùng từ .NET 7+
   - .NET 8 có data binding engine mới cho WinForms (modeled theo WPF)
   - View binds to ViewModel properties
   - Dùng khi cần strong data binding
   - DevExpress MVVM Framework hỗ trợ MVVM cho WinForms

3. **MVC (Model-View-Controller)** - OK cho WinForms
   - Controller xử lý user input
   - Separation of concerns tốt
   - Ít phổ biến hơn MVP trong WinForms

**Recommendation:**
- ✅ **Small apps**: Service layer + DI (simple, đủ dùng)
- ✅ **Medium apps**: MVP pattern (proven, phù hợp WinForms)
- ✅ **Large apps**: MVVM (.NET 8+) hoặc MVP với DI
- ✅ **All sizes**: LUÔN tách business logic ra Services

---

## 🎨 Convention & Naming

### Quy tắc đặt tên:

| Loại | Quy tắc đặt tên | Ví dụ |
|------|----------------|-------|
| **Class** | PascalCase | `CustomerService`, `MainForm` |
| **Method** | PascalCase | `LoadCustomers()`, `SaveData()` |
| **Variable** | camelCase | `customerList`, `isActive` |
| **Constant** | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT` |
| **Event Handler** | PascalCase + `_Click` / `_Changed` | `btnSave_Click`, `txtName_TextChanged` |
| **Control Name** | prefix + PascalCase | `btnSave`, `lblTitle`, `txtName` |
| **Namespace** | Company.Project.Module | `MyApp.UI.Forms` |

### Control prefix gợi ý:

```csharp
btn  -> Button
lbl  -> Label
txt  -> TextBox
cbx  -> ComboBox
chk  -> CheckBox
dgv  -> DataGridView
grp  -> GroupBox
tab  -> TabControl
pic  -> PictureBox
pnl  -> Panel
tlp  -> TableLayoutPanel
flp  -> FlowLayoutPanel
spl  -> SplitContainer
mnu  -> MenuStrip
ctx  -> ContextMenuStrip
sts  -> StatusStrip
prg  -> ProgressBar
lsv  -> ListView
tvw  -> TreeView
bds  -> BindingSource
err  -> ErrorProvider
```

---

## 💡 Best Practices

### Code Style

- ✅ Sử dụng `var` khi kiểu biến có thể suy luận rõ ràng.
- ✅ Ưu tiên **LINQ** thay vì vòng lặp thủ công khi xử lý tập hợp.
- ✅ Mỗi method nên **< 30 dòng**, mỗi class nên **< 500 dòng**.
- ✅ Đảm bảo **Exception Handling** rõ ràng (sử dụng `try-catch-finally` hoặc `try-catch` + logging).
- ✅ Không để trống `catch { }` — luôn log lỗi.
- ✅ Tất cả form và control phải có **Dispose()** hợp lý.
- ✅ **Mỗi class một file**, file name = class name.
- ✅ Không dùng magic numbers/strings — dùng constants.

### UI & UX

- ✅ Không xử lý nghiệp vụ trong event handler — chỉ gọi service.
- ✅ Đặt tên control rõ nghĩa (không viết tắt mơ hồ).
- ✅ Sử dụng **ToolTip** cho các nút chức năng.
- ✅ Layout responsive (**Anchor/Dock/TableLayoutPanel** chuẩn - xem phần Responsive Design bên dưới).

### Logging & Error Handling

- ✅ Sử dụng `Serilog` hoặc `NLog` nếu có thể.
- ✅ Ghi log quan trọng trong service, **không trong form**.
- ✅ Không hiện messagebox lỗi kỹ thuật cho người dùng cuối (ghi log + hiển thị thông báo thân thiện).

---

## 🧪 Testing

### Nguyên tắc Testing:

- ✅ Unit test cho các **Service** và **Repository** (sử dụng `xUnit` hoặc `NUnit`).
- ✅ Không test UI trực tiếp — chỉ test logic tách riêng.
- ✅ Aim for **>80% code coverage** cho business logic.
- ✅ Dùng **TDD (Test-Driven Development)** khi có thể.
- ✅ Mock dependencies với **Moq** hoặc **NSubstitute**.

### A. Unit Testing Services (xUnit)

```csharp
// Install: xUnit, xUnit.runner.visualstudio, Moq

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<CustomerService>> _mockLogger;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        _service = new CustomerService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomer()
    {
        // Arrange
        var customerId = 1;
        var expectedCustomer = new Customer { Id = customerId, Name = "John Doe" };
        _mockRepository
            .Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(expectedCustomer);

        // Act
        var result = await _service.GetByIdAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCustomer.Id, result.Id);
        Assert.Equal(expectedCustomer.Name, result.Name);
    }

    [Fact]
    public async Task SaveAsync_ValidCustomer_CallsRepository()
    {
        // Arrange
        var customer = new Customer { Name = "Jane Doe", Email = "jane@example.com" };

        // Act
        await _service.SaveAsync(customer);

        // Assert
        _mockRepository.Verify(r => r.SaveAsync(customer), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SaveAsync_InvalidName_ThrowsException(string invalidName)
    {
        // Arrange
        var customer = new Customer { Name = invalidName, Email = "test@example.com" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveAsync(customer));
    }
}
```

### B. Integration Testing với In-Memory Database

```csharp
// Install: Microsoft.EntityFrameworkCore.InMemory

public class CustomerRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        // Arrange
        await _context.Customers.AddRangeAsync(
            new Customer { Name = "Customer 1", Email = "c1@test.com" },
            new Customer { Name = "Customer 2", Email = "c2@test.com" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

### C. UI Testing với FlaUI (Advanced)

```csharp
// Install: FlaUI.UIA3

[Collection("UI Tests")]
public class MainFormUITests : IDisposable
{
    private readonly Application _app;
    private readonly Window _mainWindow;

    public MainFormUITests()
    {
        _app = Application.Launch("YourApp.exe");
        _mainWindow = _app.GetMainWindow(new UIA3Automation());
    }

    [Fact]
    public void ClickSaveButton_ValidData_ShowsSuccessMessage()
    {
        // Arrange
        var txtName = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtName"))
            .AsTextBox();
        var btnSave = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("btnSave"))
            .AsButton();

        // Act
        txtName.Text = "Test Customer";
        btnSave.Click();

        // Assert
        var messageBox = _mainWindow.ModalWindows[0];
        Assert.Contains("thành công", messageBox.Title);
    }

    public void Dispose()
    {
        _app?.Close();
        _app?.Dispose();
    }
}
```

### D. Test Coverage với Coverlet

```bash
# Install coverlet.msbuild as NuGet package

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate HTML report với ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

### E. TDD Workflow Example

```csharp
// 1. Write failing test
[Fact]
public void CalculateDiscount_VIPCustomer_Returns20Percent()
{
    // Arrange
    var customer = new Customer { IsVIP = true };
    var service = new PricingService();

    // Act
    var discount = service.CalculateDiscount(customer, 100);

    // Assert
    Assert.Equal(20, discount);
}

// 2. Write minimal code to pass
public decimal CalculateDiscount(Customer customer, decimal amount)
{
    if (customer.IsVIP)
        return amount * 0.2m;
    return 0;
}

// 3. Refactor
public decimal CalculateDiscount(Customer customer, decimal amount)
{
    var discountRate = customer.IsVIP ? 0.2m : 0m;
    return amount * discountRate;
}
```

---

## ⚡ Advanced Best Practices

### 1. Async/Await Pattern

**❌ BAD - Block UI thread:**
```csharp
private void btnLoad_Click(object sender, EventArgs e)
{
    var data = LoadDataFromDatabase(); // Chặn UI
    dgvData.DataSource = data;
}
```

**✅ GOOD - Async pattern:**
```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    btnLoad.Enabled = false;
    try
    {
        var data = await LoadDataFromDatabaseAsync();
        dgvData.DataSource = data;
    }
    finally
    {
        btnLoad.Enabled = true;
    }
}
```

**Quy tắc:**
- ✅ Luôn dùng `async/await` cho I/O operations (DB, file, network)
- ✅ `async void` CHỈ dùng cho event handlers (các method khác dùng `async Task`)
- ✅ Disable controls trong lúc chờ để tránh double-click
- ✅ Hiển thị loading indicator nếu operation > 1 giây
- ✅ Suffix tên method với `Async`: `LoadDataAsync()`, `SaveCustomerAsync()`
- ⚠️ **TRÁNH** `.Wait()` hoặc `.Result` trong UI thread (có thể gây deadlock)
- ✅ Nếu phải dùng sync code, wrap với `Task.Run()` hoặc dùng `.GetAwaiter().GetResult()`
- ✅ Dùng `ConfigureAwait(false)` trong library code (không trong UI code)
- ⚡ **NEW (.NET 9)**: WinForms giờ có async APIs built-in

---

### 2. Nullable Reference Types (C# 8.0+)

**⚡ Từ .NET 6+: Tất cả project templates MẶC ĐỊNH enable nullable context**

**Bật trong `.csproj`:**
```xml
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

**Hoặc bật per-file:**
```csharp
#nullable enable
// Code của bạn ở đây
#nullable restore
```

**Sử dụng đúng cách:**
```csharp
public class Customer
{
    public string Name { get; set; } = string.Empty;  // Không null
    public string? Email { get; set; }                // Có thể null
    public int? Age { get; set; }                     // Nullable value type
}

// Kiểm tra null an toàn
string displayName = customer.Email ?? "N/A";
int length = customer.Email?.Length ?? 0;

// Null-forgiving operator (dùng khi chắc chắn không null)
string name = customer.Name!; // Bỏ qua warning
```

**Best Practices (2024):**
- ✅ LUÔN enable cho new projects ngay từ đầu
- ✅ Dùng annotations thay vì suppressions (tránh `!` operator)
- ✅ KHÔNG disable feature để "tắt" warnings - fix warnings thay vì hide
- ✅ Migration strategy: Enable từng file/folder cho large codebases
- ✅ Flow analysis của compiler giúp phát hiện NullReferenceException trước runtime

---

### 3. Resource Management

**✅ GOOD - Using statement:**
```csharp
// Pattern 1: Using statement (C# 8.0+)
using var connection = new SqlConnection(connectionString);
using var command = connection.CreateCommand();
// Auto dispose khi ra khỏi scope

// Pattern 2: Using block
using (var stream = File.OpenRead(path))
{
    // Work with stream
}
// Auto dispose
```

**✅ GOOD - Dispose trong Form (Full Pattern):**
```csharp
public partial class MainForm : Form
{
    private ICustomerService? _service;
    private Font? _customFont;
    private Image? _backgroundImage;
    private bool _disposed = false;

    // Protected virtual Dispose method để support inheritance
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return; // Prevent redundant calls

        if (disposing)
        {
            // Dispose managed resources
            components?.Dispose();
            _service?.Dispose();

            // ✅ IMPORTANT: Dispose Font và Image resources
            _customFont?.Dispose();
            _backgroundImage?.Dispose();

            // Unsubscribe events để tránh memory leaks
            Application.Idle -= Application_Idle;
        }

        // Dispose unmanaged resources here (nếu có)
        // ...

        _disposed = true;
        base.Dispose(disposing);
    }

    // Optional: Throw nếu object đã disposed
    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(MainForm));
    }
}
```

**⚡ Async Dispose Pattern (.NET Core 3.0+):**
```csharp
public partial class MainForm : Form, IAsyncDisposable
{
    private bool _disposed = false;

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        // Async cleanup
        if (_service is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            _service?.Dispose();
        }

        Dispose(false);
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            components?.Dispose();
        }

        _disposed = true;
        base.Dispose(disposing);
    }
}
```

**Best Practices (2024):**
- ✅ Implement `IAsyncDisposable` nếu có async cleanup operations
- ✅ Luôn implement cả `IDisposable` và `IAsyncDisposable` để support cả 2 scenarios
- ✅ Dùng `_disposed` flag để detect redundant Dispose() calls
- ✅ Throw `ObjectDisposedException` khi access disposed object
- ✅ WinForms: `Form.Close()` tự động gọi `Dispose()` (documented behavior)

---

### 4. String Handling

```csharp
// ✅ Use string interpolation (C# 6.0+) - Recommended cho simple cases
string message = $"Hello {name}, you are {age} years old";

// ✅ Use StringBuilder for loops (>= 40 concatenations)
// Performance tốt hơn cho nhiều operations
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.AppendLine($"- {item.Name}");
}
string result = sb.ToString();

// ⚡ .NET 6+ Optimization: String interpolation đã được optimize
// Memory allocation giảm >50% so với .NET 5
// Dùng DefaultInterpolatedStringHandler internally

// ✅ Use verbatim strings for paths
string path = @"C:\Users\Documents\file.txt";

// ✅ Use raw string literals (C# 11.0+)
string json = """
{
    "name": "John",
    "age": 30
}
""";

// ⚡ StringBuilder.AppendInterpolatedStringHandler (.NET 6+)
var sb2 = new StringBuilder();
sb2.Append($"Customer: {customer.Name}, Age: {customer.Age}");
// Faster than: sb2.Append("Customer: " + customer.Name + ", Age: " + customer.Age);
```

**When to use what (2024):**
- ✅ **< 10 concatenations**: String interpolation hoặc `+` operator
- ✅ **>= 20-30 concatenations**: StringBuilder (clear performance benefit)
- ✅ **Loops**: ALWAYS StringBuilder
- ✅ **Composite format unknown at compile-time**: StringBuilder.AppendFormat()
- ✅ **Hardcoded strings**: String interpolation (faster từ .NET 6+)
- ⚡ **.NET 6+ optimization**: String interpolation đã tối ưu đáng kể, threshold giảm từ 40 xuống 20-30

---

### 5. LINQ Best Practices

```csharp
// ✅ Prefer method syntax for chaining
var result = customers
    .Where(c => c.IsActive)
    .OrderBy(c => c.Name)
    .Select(c => new { c.Id, c.Name })
    .ToList();

// ✅ Use Any() instead of Count() > 0 (CA1827)
if (customers.Any(c => c.IsActive)) { }  // Fast - stops at first match
// ❌ if (customers.Count(c => c.IsActive) > 0) { }  // Slow - counts all

// ⚡ EXCEPTION: List<T> và Array - dùng Count/Length property
var list = new List<Customer>();
if (list.Count > 0) { }      // O(1) - Fast
if (list.Any()) { }          // Overhead cho known collections

var array = new Customer[10];
if (array.Length > 0) { }    // O(1) - Fast (preferred)

// ✅ Use FirstOrDefault() with null check
var customer = customers.FirstOrDefault(c => c.Id == id);
if (customer != null) { }

// ✅ Deferred execution - ToList() khi cần
var query = customers.Where(c => c.IsActive);  // Not executed yet
var list = query.ToList();                      // Execute now

// ✅ Database queries: ALWAYS use Any() (SQL: SELECT TOP 1)
var exists = await dbContext.Customers
    .AnyAsync(c => c.Email == email);  // Efficient SQL
// ❌ var count = await dbContext.Customers.CountAsync(); // Full table scan
```

**LINQ Performance Rules (2024):**
- ✅ **IEnumerable<T>**: Use `Any()` for existence checks
- ✅ **List<T>/Array**: Use `Count`/`Length` property (O(1))
- ✅ **Database (EF Core)**: ALWAYS `Any()` → generates efficient SQL
- ✅ **Deferred queries**: Use `Any()` to avoid full enumeration
- ⚠️ Code Analysis CA1827: Warns about `Count() > 0` → use `Any()`

---

### 6. Magic Numbers & Strings

**❌ BAD:**
```csharp
if (status == 1) { }
if (userType == "admin") { }
Thread.Sleep(5000);
```

**✅ GOOD:**
```csharp
// Constants
private const int STATUS_ACTIVE = 1;
private const int STATUS_INACTIVE = 0;
private const string USER_TYPE_ADMIN = "admin";
private const int DEFAULT_TIMEOUT_MS = 5000;

// Enums (better)
public enum UserStatus
{
    Inactive = 0,
    Active = 1,
    Suspended = 2
}

if (status == UserStatus.Active) { }
```

---

### 7. Event Handler Best Practices

```csharp
public class MyForm : Form
{
    public MyForm()
    {
        InitializeComponent();

        // Subscribe to events
        btnSave.Click += BtnSave_Click;
        txtName.TextChanged += TxtName_TextChanged;
        this.FormClosing += MyForm_FormClosing;
        Application.Idle += Application_Idle;
    }

    private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Unsubscribe để tránh memory leak
        btnSave.Click -= BtnSave_Click;
        txtName.TextChanged -= TxtName_TextChanged;
        Application.Idle -= Application_Idle;
    }

    // ✅ Event handler naming convention
    private void BtnSave_Click(object sender, EventArgs e)
    {
        // Handle event
    }
}
```

---

### 8. Configuration Management

**Nguyên tắc:**
- ✅ **KHÔNG hardcode** connection strings, API keys trong code
- ✅ Dùng **User Secrets** cho development
- ✅ Dùng **Environment Variables** hoặc **Azure Key Vault** cho production
- ✅ Encrypt sensitive data trong config files
- ✅ Support **multiple environments** (Dev, Staging, Production)

#### A. appsettings.json Pattern

**appsettings.json:**
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=mydb.db",
        "LoggingConnection": "Server=localhost;Database=Logs;Trusted_Connection=true;"
    },
    "AppSettings": {
        "MaxRetryCount": 3,
        "TimeoutSeconds": 30,
        "EnableLogging": true,
        "CacheExpirationMinutes": 15
    },
    "Email": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 587,
        "FromAddress": "noreply@example.com",
        "EnableSsl": true
    }
}
```

**appsettings.Development.json:**
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=dev.db"
    },
    "AppSettings": {
        "EnableLogging": true
    }
}
```

**Đọc config:**
```csharp
// Using Microsoft.Extensions.Configuration
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>() // Development only
    .Build();

string connString = config.GetConnectionString("DefaultConnection");
int maxRetry = config.GetValue<int>("AppSettings:MaxRetryCount");
```

#### B. Strongly-Typed Configuration Pattern

```csharp
// Configuration classes
public class AppSettings
{
    public int MaxRetryCount { get; set; }
    public int TimeoutSeconds { get; set; }
    public bool EnableLogging { get; set; }
    public int CacheExpirationMinutes { get; set; }
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

// Register trong DI
services.Configure<AppSettings>(config.GetSection("AppSettings"));
services.Configure<EmailSettings>(config.GetSection("Email"));

// Inject vào service
public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public void SendEmail()
    {
        var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort);
        // ...
    }
}
```

#### C. User Settings (Settings.settings)

```csharp
// ✅ GOOD - User preferences (window size, theme, etc.)
// Properties/Settings.settings trong Visual Studio

public partial class MainForm : Form
{
    private void MainForm_Load(object sender, EventArgs e)
    {
        // Restore user settings
        if (Settings.Default.WindowSize != Size.Empty)
        {
            this.Size = Settings.Default.WindowSize;
            this.Location = Settings.Default.WindowLocation;
        }

        this.BackColor = Settings.Default.Theme == "Dark"
            ? Color.FromArgb(30, 30, 30)
            : Color.White;
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Save user settings
        Settings.Default.WindowSize = this.Size;
        Settings.Default.WindowLocation = this.Location;
        Settings.Default.Theme = _currentTheme;
        Settings.Default.Save();
    }
}
```

#### D. User Secrets (Development Only)

```bash
# Initialize user secrets
dotnet user-secrets init

# Add secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=MyDb;User Id=sa;Password=SecretPassword123"
dotnet user-secrets set "ApiKeys:OpenAI" "sk-xxxxxxxxxxxxx"

# List secrets
dotnet user-secrets list
```

```csharp
// Secrets tự động load trong Development environment
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

string apiKey = config["ApiKeys:OpenAI"];
```

#### E. Encrypted Configuration (Production)

```csharp
// ✅ GOOD - Encrypt connection strings trong app.config
// Install: System.Configuration.ConfigurationManager

public static class ConfigurationEncryption
{
    public static void EncryptConnectionStrings()
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var section = config.GetSection("connectionStrings") as ConnectionStringsSection;

        if (section != null && !section.SectionInformation.IsProtected)
        {
            section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            config.Save();
        }
    }

    public static void DecryptConnectionStrings()
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var section = config.GetSection("connectionStrings") as ConnectionStringsSection;

        if (section != null && section.SectionInformation.IsProtected)
        {
            section.SectionInformation.UnprotectSection();
            config.Save();
        }
    }
}
```

#### F. Environment Variables Pattern

```csharp
// ✅ GOOD - Production configuration
public static class EnvironmentConfig
{
    public static string GetConnectionString()
    {
        // Priority: Environment Variable > appsettings.json
        return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }

    public static string GetApiKey(string keyName)
    {
        return Environment.GetEnvironmentVariable($"API_KEY_{keyName}")
            ?? throw new InvalidOperationException($"API key {keyName} not found");
    }
}

// Usage
var connString = EnvironmentConfig.GetConnectionString();
var openAiKey = EnvironmentConfig.GetApiKey("OPENAI");
```

---

### 9. Security Best Practices

```csharp
// ✅ BEST - Parameterized queries - chống SQL Injection (2024 Standard)
using var cmd = new SqlCommand("SELECT * FROM Users WHERE Id = @id", connection);
cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
// HOẶC: cmd.Parameters.AddWithValue("@id", userId); // Acceptable nhưng không ideal

// ⚡ BEST - Use ORM (Entity Framework Core) - Tự động parameterize
var user = await dbContext.Users
    .FirstOrDefaultAsync(u => u.Id == userId);  // Safe từ SQL injection

// ✅ Never store passwords in plain text
// Use BCrypt, PBKDF2, or Argon2
string hashedPassword = BCrypt.HashPassword(plainPassword);
bool isValid = BCrypt.Verify(plainPassword, hashedPassword);

// ✅ Validate user input - LUÔN LUÔN validate cả dropdown values
if (string.IsNullOrWhiteSpace(txtInput.Text))
{
    MessageBox.Show("Input không được rỗng");
    return;
}

// Type checking
if (!int.TryParse(txtAge.Text, out int age) || age < 0 || age > 150)
{
    MessageBox.Show("Tuổi không hợp lệ");
    return;
}

// ✅ Sanitize file paths - Path Traversal attack prevention
string safePath = Path.GetFullPath(userInput);
if (!safePath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
{
    throw new SecurityException("Invalid path");
}

// ✅ WinForms specific: Connection string security
// KHÔNG hardcode connection strings trong code
// Dùng User Secrets (Development) hoặc Azure Key Vault (Production)
// Encrypt connection strings trong app.config

// ✅ Principle of Least Privilege
// Database user chỉ có quyền execute stored procedures cần thiết
// KHÔNG dùng 'sa' hoặc admin accounts trong WinForms apps
```

**Security Checklist (2024):**
- ✅ NEVER concatenate SQL strings với user input
- ✅ ALWAYS use parameterized queries hoặc ORM
- ✅ Validate dropdown/ComboBox selections (attacker có thể inject values)
- ✅ Encrypt sensitive data trong config files
- ✅ Use least privilege database accounts
- ✅ Input validation: Type, Range, Length, Whitelist
- ✅ Avoid storing sensitive data (passwords, credit cards) locally
- ⚠️ WinForms apps dễ bị reverse-engineer - không tin connection strings trong client

---

### 10. Performance Optimization

```csharp
// ✅ SuspendLayout khi thêm nhiều controls
pnlContainer.SuspendLayout();
try
{
    for (int i = 0; i < 100; i++)
    {
        var btn = new Button { Text = $"Button {i}" };
        pnlContainer.Controls.Add(btn);
    }
}
finally
{
    pnlContainer.ResumeLayout();
}

// ✅ BeginUpdate cho ListBox/ListView
listBox.BeginUpdate();
try
{
    for (int i = 0; i < 1000; i++)
    {
        listBox.Items.Add($"Item {i}");
    }
}
finally
{
    listBox.EndUpdate();
}

// ✅ DataGridView performance
dgv.SuspendLayout();
dgv.AutoGenerateColumns = false;  // Define columns manually
dgv.DataSource = largeList;
dgv.ResumeLayout();
```

---

### 11. 🎯 Responsive Design & Layout Best Practices

**Nguyên tắc chính:**
- ✅ Sử dụng **TableLayoutPanel** cho layouts phức tạp cần resize tỷ lệ
- ✅ Sử dụng **FlowLayoutPanel** cho layouts động (thêm/xóa controls)
- ✅ Sử dụng **Anchor/Dock** cho layouts đơn giản
- ✅ Tránh hardcode Position (X, Y) và Size (Width, Height)
- ✅ Test UI ở nhiều độ phân giải (1920x1080, 1366x768, 4K)

#### A. TableLayoutPanel Pattern (Recommended cho complex layouts)

```csharp
// ✅ BEST - TableLayoutPanel cho form responsive
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        SetupResponsiveLayout();
    }

    private void SetupResponsiveLayout()
    {
        // TableLayoutPanel: 3 rows, 2 columns
        var tlpMain = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(10)
        };

        // Định nghĩa columns: 30% | 70%
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

        // Định nghĩa rows: Auto | 100% | Auto
        tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Header
        tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Content
        tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Footer

        // Thêm controls vào cells
        tlpMain.Controls.Add(lblTitle, 0, 0);    // Row 0, Col 0
        tlpMain.Controls.Add(txtSearch, 1, 0);   // Row 0, Col 1
        tlpMain.SetColumnSpan(lblTitle, 2);       // Span 2 columns

        this.Controls.Add(tlpMain);
    }
}
```

**TableLayoutPanel Best Practices:**

```csharp
// ✅ GOOD - Kết hợp TableLayoutPanel với Anchor/Dock
tlpMain.Controls.Add(txtName, 1, 0);
txtName.Dock = DockStyle.Fill;  // Fill toàn bộ cell
txtName.Anchor = AnchorStyles.Left | AnchorStyles.Right; // Hoặc dùng Anchor

// ✅ GOOD - AutoSize cho labels
lblName.AutoSize = true;
lblName.Anchor = AnchorStyles.Left | AnchorStyles.Right;

// ✅ GOOD - Span multiple cells
tlpMain.SetColumnSpan(dgvData, 2);  // DataGridView span 2 columns
tlpMain.SetRowSpan(pnlSidebar, 3);  // Panel span 3 rows

// ❌ BAD - Tránh nest quá nhiều TableLayoutPanel (performance issue)
// ✅ GOOD - Chỉ nest khi thực sự cần thiết (< 3 levels)
```

**Khi nào dùng TableLayoutPanel:**
- ✅ Form có nhiều phần cần resize tỷ lệ với nhau
- ✅ Layout động thay đổi ở runtime (thêm/xóa controls)
- ✅ Form cần responsive với nhiều độ phân giải
- ❌ Layout đơn giản (dùng Anchor/Dock thay thế)

---

#### B. Anchor & Dock Pattern (Recommended cho simple layouts)

```csharp
// ✅ GOOD - Dock pattern cho sidebar/content layout
public void SetupDockLayout()
{
    // Panel trái cố định 250px
    var pnlLeft = new Panel
    {
        Dock = DockStyle.Left,
        Width = 250
    };

    // Panel phải fill phần còn lại
    var pnlRight = new Panel
    {
        Dock = DockStyle.Fill
    };

    this.Controls.Add(pnlLeft);
    this.Controls.Add(pnlRight); // Thêm sau để fill correctly
}

// ✅ GOOD - Anchor pattern cho buttons ở góc phải dưới
btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

// ✅ GOOD - TextBox stretch ngang
txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

// ✅ GOOD - DataGridView fill toàn bộ
dgvData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                 AnchorStyles.Left | AnchorStyles.Right;
// Hoặc đơn giản hơn:
dgvData.Dock = DockStyle.Fill;
```

**Anchor/Dock Decision Matrix:**

| Scenario | Solution |
|----------|----------|
| Button ở góc phải dưới | `Anchor = Bottom \| Right` |
| TextBox stretch ngang | `Anchor = Top \| Left \| Right` |
| Panel cố định bên trái | `Dock = DockStyle.Left` |
| Content fill toàn bộ | `Dock = DockStyle.Fill` |
| Header/Footer cố định | `Dock = DockStyle.Top/Bottom` |
| Control giữ nguyên vị trí | `Anchor = Top \| Left` (default) |

---

#### C. FlowLayoutPanel Pattern (Cho dynamic content)

```csharp
// ✅ GOOD - FlowLayoutPanel cho danh sách dynamic cards
var flpCards = new FlowLayoutPanel
{
    Dock = DockStyle.Fill,
    AutoScroll = true,
    FlowDirection = FlowDirection.LeftToRight,
    WrapContents = true,
    Padding = new Padding(10)
};

// Thêm cards động
foreach (var item in items)
{
    var card = new Panel
    {
        Size = new Size(200, 150),
        BorderStyle = BorderStyle.FixedSingle,
        Margin = new Padding(5)
    };
    flpCards.Controls.Add(card);
}
```

---

#### D. MinimumSize & MaximumSize Pattern

```csharp
// ✅ GOOD - Giới hạn kích thước form
public MainForm()
{
    InitializeComponent();

    // Form có thể resize nhưng không nhỏ hơn 800x600
    this.MinimumSize = new Size(800, 600);

    // Không lớn hơn màn hình
    this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

    // Form bắt đầu ở giữa màn hình
    this.StartPosition = FormStartPosition.CenterScreen;
}

// ✅ GOOD - Giới hạn kích thước controls
txtDescription.MinimumSize = new Size(200, 100);
txtDescription.MaximumSize = new Size(800, 300);
```

---

#### E. SplitContainer Pattern (Resizable panels)

```csharp
// ✅ GOOD - SplitContainer cho resizable layout
var splitContainer = new SplitContainer
{
    Dock = DockStyle.Fill,
    Orientation = Orientation.Vertical,
    SplitterDistance = 300,
    FixedPanel = FixedPanel.Panel1  // Panel1 fixed, Panel2 auto-resize
};

// Panel1: TreeView
var treeView = new TreeView { Dock = DockStyle.Fill };
splitContainer.Panel1.Controls.Add(treeView);

// Panel2: Details
var dgvDetails = new DataGridView { Dock = DockStyle.Fill };
splitContainer.Panel2.Controls.Add(dgvDetails);

this.Controls.Add(splitContainer);
```

---

#### F. DPI Awareness & High-DPI Support

```csharp
// ✅ GOOD - Enable DPI awareness trong Program.cs (NET 6+)
[STAThread]
static void Main()
{
    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
    ApplicationConfiguration.Initialize();
    Application.Run(new MainForm());
}

// ✅ GOOD - Font scaling
private void AdjustFontForDpi()
{
    using (Graphics g = this.CreateGraphics())
    {
        float dpiX = g.DpiX;
        if (dpiX > 96) // Standard DPI is 96
        {
            float scaleFactor = dpiX / 96f;
            this.Font = new Font(this.Font.FontFamily,
                                  this.Font.Size * scaleFactor);
        }
    }
}
```

---

#### G. Responsive Layout Checklist

Trước khi release form, kiểm tra:

- [ ] Form có MinimumSize phù hợp (không quá nhỏ)
- [ ] Tất cả controls quan trọng visible ở resolution thấp (1366x768)
- [ ] Buttons không bị crop text ở scaling 125%, 150%
- [ ] DataGridView/ListBox có scrollbar khi cần
- [ ] Form resize mượt mà (không flicker)
- [ ] Layout đúng ở cả portrait và landscape (nếu support tablet)
- [ ] Test ở nhiều độ phân giải: 1366x768, 1920x1080, 2560x1440, 4K

---

### 12. 📊 Data Binding Best Practices (BindingSource Pattern)

**Nguyên tắc:**
- ✅ **ALWAYS** dùng BindingSource làm intermediary (không bind trực tiếp vào data object)
- ✅ BindingSource giúp tránh memory leaks và quản lý "current item"
- ✅ Dùng BindingList<T> hoặc ObservableCollection<T> cho collections
- ✅ Tránh dùng untyped DataSet (dùng typed DataSet hoặc POCO classes)

---

#### A. Basic BindingSource Pattern

```csharp
public partial class CustomerForm : Form
{
    private BindingSource _customerBindingSource;
    private BindingList<Customer> _customers;

    public CustomerForm()
    {
        InitializeComponent();
        InitializeDataBinding();
    }

    private void InitializeDataBinding()
    {
        // Setup BindingSource
        _customerBindingSource = new BindingSource();
        _customers = new BindingList<Customer>();
        _customerBindingSource.DataSource = _customers;

        // Bind controls đến BindingSource (không bind trực tiếp vào _customers)
        txtName.DataBindings.Add("Text", _customerBindingSource,
            nameof(Customer.Name), false, DataSourceUpdateMode.OnPropertyChanged);

        txtEmail.DataBindings.Add("Text", _customerBindingSource,
            nameof(Customer.Email), false, DataSourceUpdateMode.OnPropertyChanged);

        numAge.DataBindings.Add("Value", _customerBindingSource,
            nameof(Customer.Age), false, DataSourceUpdateMode.OnPropertyChanged);

        // Bind DataGridView
        dgvCustomers.DataSource = _customerBindingSource;

        // Navigation buttons
        btnFirst.Click += (s, e) => _customerBindingSource.MoveFirst();
        btnPrevious.Click += (s, e) => _customerBindingSource.MovePrevious();
        btnNext.Click += (s, e) => _customerBindingSource.MoveNext();
        btnLast.Click += (s, e) => _customerBindingSource.MoveLast();
    }
}
```

---

#### B. CRUD Operations với BindingSource

```csharp
// ✅ Add new record
private void BtnAdd_Click(object sender, EventArgs e)
{
    var newCustomer = new Customer
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Email = string.Empty,
        CreatedDate = DateTime.Now
    };

    _customerBindingSource.Add(newCustomer);
    _customerBindingSource.MoveLast(); // Navigate to new record
}

// ✅ Delete current record
private void BtnDelete_Click(object sender, EventArgs e)
{
    if (_customerBindingSource.Current == null) return;

    var result = MessageBox.Show("Xác nhận xóa?", "Xác nhận",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

    if (result == DialogResult.Yes)
    {
        _customerBindingSource.RemoveCurrent();
    }
}

// ✅ Save changes
private async void BtnSave_Click(object sender, EventArgs e)
{
    try
    {
        // End edit để commit changes
        _customerBindingSource.EndEdit();

        // Validate
        if (!ValidateData()) return;

        // Save to database
        await _customerService.SaveAllAsync(_customers.ToList());

        MessageBox.Show("Lưu thành công!", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        _customerBindingSource.CancelEdit();
        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

// ✅ Refresh data from database
private async void BtnRefresh_Click(object sender, EventArgs e)
{
    var customers = await _customerService.GetAllAsync();

    _customerBindingSource.DataSource = new BindingList<Customer>(customers);

    // Hoặc clear và add lại
    // _customers.Clear();
    // foreach (var c in customers) _customers.Add(c);
    // _customerBindingSource.ResetBindings(false);
}
```

---

#### C. Master-Detail Binding Pattern

```csharp
public partial class MasterDetailForm : Form
{
    private BindingSource _customerBindingSource;
    private BindingSource _orderBindingSource;

    private void InitializeMasterDetail()
    {
        // Master: Customers
        _customerBindingSource = new BindingSource();
        _customerBindingSource.DataSource = _customers;
        dgvCustomers.DataSource = _customerBindingSource;

        // Detail: Orders (bind to customer's orders)
        _orderBindingSource = new BindingSource();
        _orderBindingSource.DataSource = _customerBindingSource;
        _orderBindingSource.DataMember = nameof(Customer.Orders); // Navigation property
        dgvOrders.DataSource = _orderBindingSource;

        // Khi chọn customer khác, orders tự động update
    }
}
```

---

#### D. BindingSource Events

```csharp
private void SetupBindingSourceEvents()
{
    // CurrentChanged: Khi navigate sang record khác
    _customerBindingSource.CurrentChanged += (s, e) =>
    {
        UpdateNavigationButtons();
        UpdateStatusBar();
    };

    // ListChanged: Khi thêm/xóa/sửa record
    _customerBindingSource.ListChanged += (s, e) =>
    {
        if (e.ListChangedType == ListChangedType.ItemAdded)
        {
            lblStatus.Text = "Đã thêm record mới";
        }
        else if (e.ListChangedType == ListChangedType.ItemDeleted)
        {
            lblStatus.Text = "Đã xóa record";
        }
    };

    // PositionChanged: Khi vị trí current item thay đổi
    _customerBindingSource.PositionChanged += (s, e) =>
    {
        lblPosition.Text = $"{_customerBindingSource.Position + 1} / " +
                           $"{_customerBindingSource.Count}";
    };
}
```

---

#### E. DataSourceUpdateMode Options

```csharp
// OnPropertyChanged: Update ngay khi property thay đổi (real-time)
txtName.DataBindings.Add("Text", _customerBindingSource,
    nameof(Customer.Name), false, DataSourceUpdateMode.OnPropertyChanged);

// OnValidation: Update khi control validated (focus lost)
txtEmail.DataBindings.Add("Text", _customerBindingSource,
    nameof(Customer.Email), false, DataSourceUpdateMode.OnValidation);

// Never: Chỉ read-only, không update data source
lblId.DataBindings.Add("Text", _customerBindingSource,
    nameof(Customer.Id), false, DataSourceUpdateMode.Never);
```

---

#### F. INotifyPropertyChanged Pattern (Advanced)

```csharp
// ✅ BEST - Model implements INotifyPropertyChanged
public class Customer : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _email = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Khi property thay đổi, UI tự động update (two-way binding)
```

---

#### G. Filtering & Sorting với BindingSource

```csharp
// ✅ Filter
private void TxtSearch_TextChanged(object sender, EventArgs e)
{
    string searchText = txtSearch.Text.Trim();

    if (string.IsNullOrWhiteSpace(searchText))
    {
        _customerBindingSource.RemoveFilter();
    }
    else
    {
        // Filter by name or email
        var filtered = _customers
            .Where(c => c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                       c.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _customerBindingSource.DataSource = new BindingList<Customer>(filtered);
    }
}

// ✅ Sort
private void DgvCustomers_ColumnHeaderMouseClick(object sender,
    DataGridViewCellMouseEventArgs e)
{
    var column = dgvCustomers.Columns[e.ColumnIndex];
    var propertyName = column.DataPropertyName;

    var sorted = _customers
        .OrderBy(c => c.GetType().GetProperty(propertyName)?.GetValue(c))
        .ToList();

    _customerBindingSource.DataSource = new BindingList<Customer>(sorted);
}
```

---

#### H. ComboBox Binding Pattern

```csharp
// ✅ GOOD - Bind ComboBox to lookup data
private void SetupComboBoxBinding()
{
    // Lookup data
    var categories = _categoryService.GetAll();
    var categoryBindingSource = new BindingSource
    {
        DataSource = categories
    };

    // ComboBox setup
    cbxCategory.DataSource = categoryBindingSource;
    cbxCategory.DisplayMember = nameof(Category.Name);  // Show to user
    cbxCategory.ValueMember = nameof(Category.Id);      // Internal value

    // Bind selected value to customer's category
    cbxCategory.DataBindings.Add("SelectedValue", _customerBindingSource,
        nameof(Customer.CategoryId), false, DataSourceUpdateMode.OnPropertyChanged);
}
```

---

### 13. 🔗 Form Communication & Dependency Injection Patterns

**Nguyên tắc:**
- ✅ Dùng **Constructor Injection** cho services
- ✅ Dùng **Factory Pattern** cho việc tạo forms với mixed dependencies
- ✅ Dùng **Events/Delegates** để communicate giữa forms
- ❌ TRÁNH Service Locator pattern (anti-pattern)
- ❌ TRÁNH static classes cho state management

---

#### A. Dependency Injection Setup (Program.cs)

```csharp
// ✅ BEST PRACTICE - Setup DI container trong Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();

        // Build DI container
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Resolve main form và run
        var mainForm = serviceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        // Logging
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddDebug();
        });

        // Services (Business logic)
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddTransient<IEmailService, EmailService>();

        // Repositories (Data access)
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();

        // Configuration
        services.AddSingleton<IConfiguration>(provider =>
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        });

        // Forms - Register tất cả forms cần DI
        services.AddTransient<MainForm>();
        services.AddTransient<CustomerForm>();
        services.AddTransient<OrderForm>();

        // Form Factory (xem section B)
        services.AddSingleton<IFormFactory, FormFactory>();
    }
}
```

---

#### B. Form Factory Pattern (Recommended)

```csharp
// ✅ BEST - IFormFactory interface
public interface IFormFactory
{
    CustomerForm CreateCustomerForm();
    CustomerForm CreateCustomerForm(int customerId); // With parameters
    OrderForm CreateOrderForm(int orderId);
    SettingsForm CreateSettingsForm();
}

// ✅ Implementation
public class FormFactory : IFormFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FormFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CustomerForm CreateCustomerForm()
    {
        // Resolve form từ DI container
        return _serviceProvider.GetRequiredService<CustomerForm>();
    }

    public CustomerForm CreateCustomerForm(int customerId)
    {
        // Resolve services từ container
        var customerService = _serviceProvider.GetRequiredService<ICustomerService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<CustomerForm>>();

        // Tạo form với mixed dependencies (services + parameters)
        return new CustomerForm(customerService, logger, customerId);
    }

    public OrderForm CreateOrderForm(int orderId)
    {
        var orderService = _serviceProvider.GetRequiredService<IOrderService>();
        return new OrderForm(orderService, orderId);
    }

    public SettingsForm CreateSettingsForm()
    {
        return _serviceProvider.GetRequiredService<SettingsForm>();
    }
}
```

---

#### C. Form với Constructor Injection

```csharp
// ✅ GOOD - Form receives dependencies via constructor
public partial class CustomerForm : Form
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerForm> _logger;
    private readonly int? _customerId;

    // Constructor 1: Chỉ services (DI container resolve được)
    public CustomerForm(ICustomerService customerService,
                       ILogger<CustomerForm> logger)
    {
        _customerService = customerService;
        _logger = logger;
        InitializeComponent();
    }

    // Constructor 2: Services + parameters (dùng Factory để tạo)
    public CustomerForm(ICustomerService customerService,
                       ILogger<CustomerForm> logger,
                       int customerId)
        : this(customerService, logger)
    {
        _customerId = customerId;
    }

    private async void CustomerForm_Load(object sender, EventArgs e)
    {
        try
        {
            if (_customerId.HasValue)
            {
                // Edit mode: Load existing customer
                var customer = await _customerService.GetByIdAsync(_customerId.Value);
                LoadCustomerData(customer);
            }
            else
            {
                // Add mode: New customer
                InitializeNewCustomer();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading customer form");
            MessageBox.Show("Lỗi khi tải dữ liệu", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

---

#### D. Opening Child Forms với Factory

```csharp
public partial class MainForm : Form
{
    private readonly IFormFactory _formFactory;
    private readonly ILogger<MainForm> _logger;

    public MainForm(IFormFactory formFactory, ILogger<MainForm> logger)
    {
        _formFactory = formFactory;
        _logger = logger;
        InitializeComponent();
    }

    // ✅ GOOD - Open form sử dụng Factory
    private void BtnAddCustomer_Click(object sender, EventArgs e)
    {
        // Tạo form mới (add mode)
        var form = _formFactory.CreateCustomerForm();
        form.CustomerSaved += CustomerForm_CustomerSaved; // Subscribe to event
        form.ShowDialog();
    }

    private void BtnEditCustomer_Click(object sender, EventArgs e)
    {
        if (dgvCustomers.SelectedRows.Count == 0) return;

        int customerId = (int)dgvCustomers.SelectedRows[0].Cells["Id"].Value;

        // Tạo form với parameter (edit mode)
        var form = _formFactory.CreateCustomerForm(customerId);
        form.CustomerSaved += CustomerForm_CustomerSaved;
        form.ShowDialog();
    }

    // Event handler để refresh data sau khi save
    private void CustomerForm_CustomerSaved(object? sender, CustomerEventArgs e)
    {
        _logger.LogInformation($"Customer saved: {e.Customer.Name}");
        RefreshCustomerGrid();
    }
}
```

---

#### E. Form Communication với Events/Delegates

```csharp
// ✅ BEST - Custom EventArgs
public class CustomerEventArgs : EventArgs
{
    public Customer Customer { get; }
    public CustomerEventArgs(Customer customer)
    {
        Customer = customer;
    }
}

// ✅ Child Form exposes events
public partial class CustomerForm : Form
{
    // Event để notify parent form
    public event EventHandler<CustomerEventArgs>? CustomerSaved;
    public event EventHandler? CustomerDeleted;

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var customer = GetCustomerFromForm();
            await _customerService.SaveAsync(customer);

            // Raise event
            CustomerSaved?.Invoke(this, new CustomerEventArgs(customer));

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer");
            MessageBox.Show("Lỗi khi lưu dữ liệu", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnDelete_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("Xác nhận xóa?", "Xác nhận",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            await _customerService.DeleteAsync(_customerId.Value);

            // Raise event
            CustomerDeleted?.Invoke(this, EventArgs.Empty);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
```

---

#### F. Alternative: Callback Pattern

```csharp
// ✅ GOOD - Callback delegate
public partial class CustomerForm : Form
{
    private readonly Action<Customer>? _onSaveCallback;

    public CustomerForm(ICustomerService customerService,
                       Action<Customer>? onSaveCallback = null)
    {
        _customerService = customerService;
        _onSaveCallback = onSaveCallback;
        InitializeComponent();
    }

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        var customer = GetCustomerFromForm();
        await _customerService.SaveAsync(customer);

        // Invoke callback
        _onSaveCallback?.Invoke(customer);

        this.Close();
    }
}

// Usage trong parent form
private void BtnAddCustomer_Click(object sender, EventArgs e)
{
    var form = new CustomerForm(_customerService, customer =>
    {
        // Callback khi save thành công
        _logger.LogInformation($"Customer saved: {customer.Name}");
        RefreshCustomerGrid();
    });
    form.ShowDialog();
}
```

---

#### G. Singleton Form Pattern (MDI hoặc Dockable UI)

```csharp
// ✅ GOOD - Manage singleton child forms
public partial class MainForm : Form
{
    private CustomerListForm? _customerListForm;

    private void MenuCustomers_Click(object sender, EventArgs e)
    {
        // Nếu form đã tồn tại, activate nó
        if (_customerListForm != null && !_customerListForm.IsDisposed)
        {
            _customerListForm.Activate();
            _customerListForm.BringToFront();
            return;
        }

        // Tạo form mới
        _customerListForm = _formFactory.CreateCustomerListForm();
        _customerListForm.MdiParent = this; // MDI child
        _customerListForm.FormClosed += (s, args) => _customerListForm = null;
        _customerListForm.Show();
    }
}
```

---

#### H. Mediator Pattern (Advanced - Complex Apps)

```csharp
// ✅ BEST - Mediator pattern cho complex form communication
// Install: MediatR NuGet package

// Define messages/commands
public record CustomerSavedNotification(Customer Customer) : INotification;
public record RefreshDataCommand : IRequest<bool>;
public record ShowCustomerFormCommand(int? CustomerId) : IRequest<DialogResult>;

// Handlers
public class CustomerSavedHandler : INotificationHandler<CustomerSavedNotification>
{
    private readonly ILogger<CustomerSavedHandler> _logger;

    public CustomerSavedHandler(ILogger<CustomerSavedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CustomerSavedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Customer saved: {notification.Customer.Name}");
        // Multiple forms can listen to this without coupling
        return Task.CompletedTask;
    }
}

// Usage trong forms
public partial class CustomerForm : Form
{
    private readonly IMediator _mediator;

    public CustomerForm(IMediator mediator, ICustomerService service)
    {
        _mediator = mediator;
        _customerService = service;
        InitializeComponent();
    }

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        var customer = GetCustomerFromForm();
        await _customerService.SaveAsync(customer);

        // Notify all interested parties
        await _mediator.Publish(new CustomerSavedNotification(customer));

        this.Close();
    }
}

public partial class CustomerListForm : Form
{
    private readonly IMediator _mediator;

    public CustomerListForm(IMediator mediator)
    {
        _mediator = mediator;
        InitializeComponent();

        // Subscribe to notifications
        _mediator.Subscribe<CustomerSavedNotification>(OnCustomerSaved);
    }

    private async Task OnCustomerSaved(CustomerSavedNotification notification)
    {
        // Refresh grid when customer saved
        await RefreshGridAsync();
    }
}

// Register MediatR trong DI
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

**Benefits:**
- ✅ **Loose coupling** - forms không reference nhau
- ✅ **Single Responsibility** - mỗi form chỉ handle own concerns
- ✅ **Testable** - dễ test với mock mediator
- ✅ **Scalable** - thêm handlers mới không ảnh hưởng existing code

---

#### I. WeakEvent Pattern (Memory Leak Prevention)

```csharp
// ✅ GOOD - WeakEvent để tránh memory leaks
public class WeakEventManager<TEventArgs> where TEventArgs : EventArgs
{
    private readonly List<WeakReference<EventHandler<TEventArgs>>> _handlers = new();

    public void AddHandler(EventHandler<TEventArgs> handler)
    {
        _handlers.Add(new WeakReference<EventHandler<TEventArgs>>(handler));
    }

    public void RemoveHandler(EventHandler<TEventArgs> handler)
    {
        _handlers.RemoveAll(wr =>
        {
            if (wr.TryGetTarget(out var target))
            {
                return target == handler;
            }
            return true; // Remove dead references
        });
    }

    public void RaiseEvent(object sender, TEventArgs args)
    {
        var deadReferences = new List<WeakReference<EventHandler<TEventArgs>>>();

        foreach (var weakRef in _handlers)
        {
            if (weakRef.TryGetTarget(out var handler))
            {
                handler(sender, args);
            }
            else
            {
                deadReferences.Add(weakRef);
            }
        }

        // Cleanup dead references
        foreach (var deadRef in deadReferences)
        {
            _handlers.Remove(deadRef);
        }
    }
}

// Usage
public class DataService
{
    private readonly WeakEventManager<DataChangedEventArgs> _dataChanged = new();

    public event EventHandler<DataChangedEventArgs> DataChanged
    {
        add => _dataChanged.AddHandler(value);
        remove => _dataChanged.RemoveHandler(value);
    }

    protected void OnDataChanged(DataChangedEventArgs args)
    {
        _dataChanged.RaiseEvent(this, args);
    }
}
```

---

#### J. ❌ Anti-patterns (TRÁNH)

```csharp
// ❌ BAD - Service Locator pattern
public partial class BadForm : Form
{
    private void BtnSave_Click(object sender, EventArgs e)
    {
        // Directly access static ServiceProvider - ANTI-PATTERN!
        var service = Program.ServiceProvider.GetService<ICustomerService>();
        service.Save(customer);
    }
}

// ❌ BAD - Static state management
public static class GlobalState
{
    public static Customer? CurrentCustomer { get; set; }
    public static List<Order> Orders { get; set; } = new();
}

// ❌ BAD - Form referencing parent form directly
public partial class ChildForm : Form
{
    private MainForm _parentForm;

    public ChildForm(MainForm parent) // Tight coupling!
    {
        _parentForm = parent;
    }
}
```

---

### 14. ✅ Input Validation với ErrorProvider

**Nguyên tắc:**
- ✅ Dùng **ErrorProvider** cho validation UI (non-intrusive)
- ✅ Validate on **button click** (final validation)
- ✅ Optional: Real-time validation trên **Validating event**
- ✅ Set `AutoValidate = EnableAllowFocusChange` để cho phép di chuyển giữa controls
- ✅ Hiển thị **tất cả errors** cùng lúc (không dừng ở error đầu tiên)

---

#### A. Basic ErrorProvider Pattern

```csharp
public partial class CustomerForm : Form
{
    private readonly ErrorProvider _errorProvider;

    public CustomerForm()
    {
        InitializeComponent();

        // Setup ErrorProvider
        _errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink,
            Icon = SystemIcons.Warning
        };

        // Cho phép focus sang control khác khi có lỗi
        this.AutoValidate = AutoValidateMode.EnableAllowFocusChange;
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        // Clear previous errors
        _errorProvider.Clear();

        // Validate tất cả fields
        if (!ValidateForm())
        {
            MessageBox.Show("Vui lòng sửa các lỗi trước khi lưu", "Lỗi nhập liệu",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Save data
        SaveCustomer();
    }

    private bool ValidateForm()
    {
        bool isValid = true;

        // Validate Name
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            _errorProvider.SetError(txtName, "Tên không được để trống");
            isValid = false;
        }

        // Validate Email
        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Email không được để trống");
            isValid = false;
        }
        else if (!IsValidEmail(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Email không đúng định dạng");
            isValid = false;
        }

        // Validate Age
        if (numAge.Value < 18 || numAge.Value > 100)
        {
            _errorProvider.SetError(numAge, "Tuổi phải từ 18 đến 100");
            isValid = false;
        }

        // Validate Phone
        if (!string.IsNullOrWhiteSpace(txtPhone.Text) &&
            !IsValidPhone(txtPhone.Text))
        {
            _errorProvider.SetError(txtPhone, "Số điện thoại không đúng định dạng");
            isValid = false;
        }

        return isValid;
    }

    private bool IsValidEmail(string email)
    {
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

    private bool IsValidPhone(string phone)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phone,
            @"^\+?[0-9]{10,15}$");
    }
}
```

---

#### B. Real-time Validation với Validating Event

```csharp
public partial class CustomerForm : Form
{
    private void SetupValidationEvents()
    {
        // Validate khi focus leave control
        txtName.Validating += TxtName_Validating;
        txtEmail.Validating += TxtEmail_Validating;
        txtPhone.Validating += TxtPhone_Validating;
    }

    private void TxtName_Validating(object? sender, CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            _errorProvider.SetError(txtName, "Tên không được để trống");
            e.Cancel = true; // Prevent focus change nếu muốn force fix
        }
        else
        {
            _errorProvider.SetError(txtName, string.Empty); // Clear error
        }
    }

    private void TxtEmail_Validating(object? sender, CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Email không được để trống");
        }
        else if (!IsValidEmail(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Email không đúng định dạng");
        }
        else
        {
            _errorProvider.SetError(txtEmail, string.Empty);
        }
    }

    private void TxtPhone_Validating(object? sender, CancelEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(txtPhone.Text) &&
            !IsValidPhone(txtPhone.Text))
        {
            _errorProvider.SetError(txtPhone, "Số điện thoại không đúng định dạng");
        }
        else
        {
            _errorProvider.SetError(txtPhone, string.Empty);
        }
    }
}
```

---

#### C. Advanced: Fluent Validation Pattern

```csharp
// ✅ BEST - Sử dụng FluentValidation library
// Install: NuGet -> FluentValidation

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Tên không được để trống")
            .MaximumLength(100).WithMessage("Tên không được quá 100 ký tự");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không đúng định dạng");

        RuleFor(c => c.Age)
            .InclusiveBetween(18, 100).WithMessage("Tuổi phải từ 18 đến 100");

        RuleFor(c => c.Phone)
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Số điện thoại không đúng định dạng")
            .When(c => !string.IsNullOrWhiteSpace(c.Phone)); // Optional field
    }
}

public partial class CustomerForm : Form
{
    private readonly CustomerValidator _validator = new();

    private bool ValidateForm()
    {
        _errorProvider.Clear();

        var customer = GetCustomerFromForm();
        var result = _validator.Validate(customer);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                var control = GetControlByPropertyName(error.PropertyName);
                if (control != null)
                {
                    _errorProvider.SetError(control, error.ErrorMessage);
                }
            }
            return false;
        }

        return true;
    }

    private Control? GetControlByPropertyName(string propertyName)
    {
        return propertyName switch
        {
            nameof(Customer.Name) => txtName,
            nameof(Customer.Email) => txtEmail,
            nameof(Customer.Age) => numAge,
            nameof(Customer.Phone) => txtPhone,
            _ => null
        };
    }
}
```

---

#### D. Custom Validation with Business Rules

```csharp
private async Task<bool> ValidateFormWithBusinessRules()
{
    _errorProvider.Clear();
    bool isValid = true;

    // Basic validation
    isValid = ValidateForm() && isValid;

    // Business rule: Check duplicate email
    if (!string.IsNullOrWhiteSpace(txtEmail.Text))
    {
        bool isDuplicate = await _customerService
            .IsEmailExistsAsync(txtEmail.Text, _customerId);

        if (isDuplicate)
        {
            _errorProvider.SetError(txtEmail, "Email này đã được sử dụng");
            isValid = false;
        }
    }

    // Business rule: Check credit limit
    if (numCreditLimit.Value > 100000 && !chkIsVIP.Checked)
    {
        _errorProvider.SetError(numCreditLimit,
            "Chỉ khách hàng VIP mới được credit limit > 100,000");
        isValid = false;
    }

    return isValid;
}
```

---

#### E. Validation với ValidateChildren()

```csharp
// ✅ GOOD - Validate tất cả controls cùng lúc
private void BtnSave_Click(object sender, EventArgs e)
{
    // ValidateChildren() triggers Validating event của tất cả controls
    if (!this.ValidateChildren(ValidationConstraints.Enabled))
    {
        MessageBox.Show("Vui lòng sửa các lỗi trước khi lưu", "Lỗi nhập liệu",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    SaveCustomer();
}

// Trong Validating event, phải set e.Cancel = true khi có lỗi
private void TxtName_Validating(object? sender, CancelEventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtName.Text))
    {
        _errorProvider.SetError(txtName, "Tên không được để trống");
        e.Cancel = true; // ValidateChildren() sẽ return false
    }
    else
    {
        _errorProvider.SetError(txtName, string.Empty);
    }
}
```

---

#### F. ErrorProvider với Multiple Providers

```csharp
// ✅ GOOD - Phân biệt error types
public partial class CustomerForm : Form
{
    private readonly ErrorProvider _errorProvider;   // Validation errors
    private readonly ErrorProvider _warningProvider; // Warnings
    private readonly ErrorProvider _infoProvider;    // Information

    public CustomerForm()
    {
        InitializeComponent();

        _errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink,
            Icon = SystemIcons.Error
        };

        _warningProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink,
            Icon = SystemIcons.Warning
        };

        _infoProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink,
            Icon = SystemIcons.Information
        };
    }

    private void ValidateWithLevels()
    {
        // Error: Must fix
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            _errorProvider.SetError(txtName, "Tên không được để trống");
        }

        // Warning: Recommend fix
        if (txtPhone.Text.Length < 10)
        {
            _warningProvider.SetError(txtPhone, "Số điện thoại có vẻ ngắn");
        }

        // Info: Helpful hint
        if (numAge.Value < 25)
        {
            _infoProvider.SetError(numAge, "Khách hàng dưới 25 tuổi có ưu đãi đặc biệt");
        }
    }
}
```

---

### 15. 🧵 Thread Safety & Cross-Thread UI Updates

**Nguyên tắc:**
- ✅ UI controls chỉ được access từ **UI thread**
- ✅ Dùng `Invoke()` hoặc `BeginInvoke()` để update UI từ background thread
- ✅ Dùng `InvokeRequired` để check thread
- ✅ Dùng `async/await` thay vì manual threading khi có thể

---

#### A. Basic Invoke Pattern

```csharp
public partial class MainForm : Form
{
    // ✅ GOOD - Safe cross-thread UI update
    private void UpdateStatus(string message)
    {
        if (InvokeRequired)
        {
            // Not on UI thread -> Invoke on UI thread
            Invoke(new Action<string>(UpdateStatus), message);
            return;
        }

        // On UI thread -> Update directly
        lblStatus.Text = message;
        lblLastUpdate.Text = DateTime.Now.ToString("HH:mm:ss");
    }

    // Usage từ background thread
    private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        for (int i = 0; i < 100; i++)
        {
            Thread.Sleep(100);
            UpdateStatus($"Processing... {i}%");
        }
    }
}
```

---

#### B. Generic Invoke Helper

```csharp
// ✅ BEST - Reusable invoke helper
public static class ControlExtensions
{
    public static void InvokeIfRequired(this Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            control.Invoke(action);
        }
        else
        {
            action();
        }
    }

    public static T InvokeIfRequired<T>(this Control control, Func<T> func)
    {
        if (control.InvokeRequired)
        {
            return (T)control.Invoke(func);
        }
        else
        {
            return func();
        }
    }
}

// Usage
private void UpdateUIFromBackgroundThread()
{
    this.InvokeIfRequired(() =>
    {
        lblStatus.Text = "Updated!";
        progressBar.Value = 100;
        btnStart.Enabled = true;
    });

    // Hoặc với return value
    var currentText = txtName.InvokeIfRequired(() => txtName.Text);
}
```

---

#### C. Async/Await Pattern (Recommended)

```csharp
// ✅ BEST - Dùng async/await thay vì manual threading
private async void BtnProcess_Click(object sender, EventArgs e)
{
    btnProcess.Enabled = false;
    progressBar.Value = 0;

    try
    {
        // Progress reporter
        var progress = new Progress<int>(value =>
        {
            // Automatically invoked on UI thread
            progressBar.Value = value;
            lblStatus.Text = $"Processing... {value}%";
        });

        // Run on background thread
        var result = await Task.Run(() => ProcessData(progress));

        // Back on UI thread automatically
        MessageBox.Show($"Completed! Result: {result}", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnProcess.Enabled = true;
    }
}

private int ProcessData(IProgress<int> progress)
{
    int total = 100;
    for (int i = 0; i <= total; i++)
    {
        Thread.Sleep(50); // Simulate work
        progress.Report(i); // Report progress to UI thread
    }
    return 42;
}
```

---

#### D. BackgroundWorker Pattern (Legacy but still valid)

```csharp
// ✅ GOOD - BackgroundWorker cho long-running tasks
public partial class ProcessForm : Form
{
    private BackgroundWorker _worker;

    public ProcessForm()
    {
        InitializeComponent();
        SetupBackgroundWorker();
    }

    private void SetupBackgroundWorker()
    {
        _worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        _worker.DoWork += Worker_DoWork;
        _worker.ProgressChanged += Worker_ProgressChanged;
        _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
    }

    private void BtnStart_Click(object sender, EventArgs e)
    {
        if (!_worker.IsBusy)
        {
            btnStart.Enabled = false;
            btnCancel.Enabled = true;
            _worker.RunWorkerAsync();
        }
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        if (_worker.IsBusy)
        {
            _worker.CancelAsync();
        }
    }

    private void Worker_DoWork(object? sender, DoWorkEventArgs e)
    {
        var worker = sender as BackgroundWorker;

        for (int i = 0; i <= 100; i++)
        {
            if (worker?.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }

            Thread.Sleep(100); // Simulate work
            worker?.ReportProgress(i);
        }

        e.Result = "Completed successfully!";
    }

    private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        // Automatically on UI thread
        progressBar.Value = e.ProgressPercentage;
        lblStatus.Text = $"Processing... {e.ProgressPercentage}%";
    }

    private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        // Automatically on UI thread
        btnStart.Enabled = true;
        btnCancel.Enabled = false;

        if (e.Cancelled)
        {
            lblStatus.Text = "Cancelled by user";
        }
        else if (e.Error != null)
        {
            MessageBox.Show($"Error: {e.Error.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            lblStatus.Text = e.Result?.ToString() ?? "Done";
        }
    }
}
```

---

#### E. Timer Pattern cho Periodic Updates

```csharp
// ✅ GOOD - System.Windows.Forms.Timer (UI thread safe)
public partial class MonitorForm : Form
{
    private System.Windows.Forms.Timer _timer;

    public MonitorForm()
    {
        InitializeComponent();
        SetupTimer();
    }

    private void SetupTimer()
    {
        _timer = new System.Windows.Forms.Timer
        {
            Interval = 1000 // 1 second
        };
        _timer.Tick += Timer_Tick;
    }

    private void BtnStart_Click(object sender, EventArgs e)
    {
        _timer.Start();
    }

    private void BtnStop_Click(object sender, EventArgs e)
    {
        _timer.Stop();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        // Automatically on UI thread - safe to update UI directly
        lblTime.Text = DateTime.Now.ToString("HH:mm:ss");

        // Update status from service
        var status = _monitorService.GetStatus();
        lblStatus.Text = status;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Stop();
            _timer?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

---

#### F. SynchronizationContext Pattern (Advanced)

```csharp
// ✅ GOOD - Capture UI SynchronizationContext
public partial class AdvancedForm : Form
{
    private readonly SynchronizationContext _uiContext;

    public AdvancedForm()
    {
        InitializeComponent();
        _uiContext = SynchronizationContext.Current!;
    }

    private void StartBackgroundTask()
    {
        Task.Run(() =>
        {
            // Background work
            var data = LoadDataFromDatabase();

            // Post back to UI thread
            _uiContext.Post(_ =>
            {
                // Update UI
                dgvData.DataSource = data;
                lblStatus.Text = "Data loaded";
            }, null);
        });
    }

    // Hoặc dùng Send (synchronous)
    private void StartBackgroundTaskSync()
    {
        Task.Run(() =>
        {
            var data = LoadDataFromDatabase();

            // Send (blocking until UI update completes)
            _uiContext.Send(_ =>
            {
                dgvData.DataSource = data;
            }, null);
        });
    }
}
```

---

#### G. ❌ Common Mistakes

```csharp
// ❌ BAD - Direct UI access from background thread
private void BackgroundThread()
{
    Task.Run(() =>
    {
        // CRASH! InvalidOperationException: Cross-thread operation not valid
        lblStatus.Text = "Updated";
    });
}

// ❌ BAD - Blocking UI thread
private void BtnLoad_Click(object sender, EventArgs e)
{
    // UI freezes during 5 seconds
    Thread.Sleep(5000);
    var data = LoadData();
    dgvData.DataSource = data;
}

// ✅ GOOD - Async version
private async void BtnLoad_Click(object sender, EventArgs e)
{
    btnLoad.Enabled = false;
    try
    {
        var data = await Task.Run(() => LoadData());
        dgvData.DataSource = data;
    }
    finally
    {
        btnLoad.Enabled = true;
    }
}
```

---

### 16. 📊 DataGridView Best Practices

**Nguyên tắc:**
- ✅ **Luôn define columns manually** (set `AutoGenerateColumns = false`)
- ✅ Dùng **VirtualMode** cho datasets lớn (>10,000 rows)
- ✅ **SuspendLayout/ResumeLayout** khi load data
- ✅ Handle **CellFormatting** cho custom display
- ✅ Implement **paging** hoặc **lazy loading** cho large datasets

#### A. Basic DataGridView Setup

```csharp
public partial class CustomerListForm : Form
{
    private void SetupDataGridView()
    {
        // Performance settings
        dgvCustomers.SuspendLayout();

        // Manual columns (better performance)
        dgvCustomers.AutoGenerateColumns = false;
        dgvCustomers.AllowUserToAddRows = false;
        dgvCustomers.AllowUserToDeleteRows = false;
        dgvCustomers.ReadOnly = true;
        dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCustomers.MultiSelect = false;

        // Define columns
        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colId",
            HeaderText = "ID",
            DataPropertyName = nameof(Customer.Id),
            Width = 80,
            ReadOnly = true
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colName",
            HeaderText = "Tên khách hàng",
            DataPropertyName = nameof(Customer.Name),
            Width = 200,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colEmail",
            HeaderText = "Email",
            DataPropertyName = nameof(Customer.Email),
            Width = 200
        });

        // Checkbox column
        dgvCustomers.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "colActive",
            HeaderText = "Kích hoạt",
            DataPropertyName = nameof(Customer.IsActive),
            Width = 80
        });

        // Button column
        var btnColumn = new DataGridViewButtonColumn
        {
            Name = "colAction",
            HeaderText = "Action",
            Text = "Sửa",
            UseColumnTextForButtonValue = true,
            Width = 80
        };
        dgvCustomers.Columns.Add(btnColumn);

        dgvCustomers.ResumeLayout();
    }
}
```

#### B. Loading Data Efficiently

```csharp
// ✅ GOOD - Async loading with progress
private async Task LoadCustomersAsync()
{
    try
    {
        dgvCustomers.SuspendLayout();
        dgvCustomers.DataSource = null;

        var customers = await _customerService.GetAllAsync();

        // Use BindingList for automatic updates
        var bindingList = new BindingList<Customer>(customers);
        var bindingSource = new BindingSource(bindingList, null);
        dgvCustomers.DataSource = bindingSource;

        dgvCustomers.ResumeLayout();
        lblStatus.Text = $"Loaded {customers.Count} customers";
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading customers");
        MessageBox.Show("Lỗi khi tải dữ liệu", "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

#### C. Virtual Mode (Large Datasets)

```csharp
public partial class LargeDataForm : Form
{
    private List<Customer> _customers = new();
    private const int PAGE_SIZE = 100;

    private void SetupVirtualMode()
    {
        dgvCustomers.VirtualMode = true;
        dgvCustomers.CellValueNeeded += DgvCustomers_CellValueNeeded;
        dgvCustomers.RowCount = 0; // Will update after loading
    }

    private async Task LoadDataAsync()
    {
        _customers = await _customerService.GetAllAsync();
        dgvCustomers.RowCount = _customers.Count;
        dgvCustomers.Invalidate();
    }

    private void DgvCustomers_CellValueNeeded(object? sender,
        DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex >= _customers.Count) return;

        var customer = _customers[e.RowIndex];

        e.Value = e.ColumnIndex switch
        {
            0 => customer.Id,
            1 => customer.Name,
            2 => customer.Email,
            3 => customer.IsActive,
            _ => null
        };
    }
}
```

#### D. Cell Formatting

```csharp
private void DgvCustomers_CellFormatting(object? sender,
    DataGridViewCellFormattingEventArgs e)
{
    if (dgvCustomers.Columns[e.ColumnIndex].Name == "colAmount")
    {
        if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal amount))
        {
            // Format currency
            e.Value = amount.ToString("C2");
            e.FormattingApplied = true;

            // Color negative amounts red
            if (amount < 0)
            {
                e.CellStyle.ForeColor = Color.Red;
                e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
        }
    }

    if (dgvCustomers.Columns[e.ColumnIndex].Name == "colStatus")
    {
        // Color-code status
        if (e.Value?.ToString() == "Active")
        {
            e.CellStyle.BackColor = Color.LightGreen;
        }
        else if (e.Value?.ToString() == "Inactive")
        {
            e.CellStyle.BackColor = Color.LightGray;
        }
    }
}
```

#### E. Context Menu

```csharp
private void SetupContextMenu()
{
    var contextMenu = new ContextMenuStrip();

    var editMenuItem = new ToolStripMenuItem("Sửa", null, EditMenuItem_Click);
    editMenuItem.ShortcutKeys = Keys.F2;

    var deleteMenuItem = new ToolStripMenuItem("Xóa", null, DeleteMenuItem_Click);
    deleteMenuItem.ShortcutKeys = Keys.Delete;

    var separator = new ToolStripSeparator();

    var exportMenuItem = new ToolStripMenuItem("Xuất Excel", null, ExportMenuItem_Click);

    contextMenu.Items.AddRange(new ToolStripItem[]
    {
        editMenuItem,
        deleteMenuItem,
        separator,
        exportMenuItem
    });

    dgvCustomers.ContextMenuStrip = contextMenu;
}

private void EditMenuItem_Click(object? sender, EventArgs e)
{
    if (dgvCustomers.SelectedRows.Count == 0) return;

    var customer = dgvCustomers.SelectedRows[0].DataBoundItem as Customer;
    if (customer != null)
    {
        var form = _formFactory.CreateCustomerForm(customer.Id);
        form.ShowDialog();
    }
}
```

#### F. Export to Excel/CSV

```csharp
// ✅ GOOD - Export to CSV
private void ExportToCsv(string filePath)
{
    var sb = new StringBuilder();

    // Headers
    var headers = dgvCustomers.Columns.Cast<DataGridViewColumn>()
        .Where(c => c.Visible)
        .Select(c => c.HeaderText);
    sb.AppendLine(string.Join(",", headers));

    // Data rows
    foreach (DataGridViewRow row in dgvCustomers.Rows)
    {
        if (row.IsNewRow) continue;

        var cells = row.Cells.Cast<DataGridViewCell>()
            .Where(c => c.OwningColumn.Visible)
            .Select(c => $"\"{c.Value?.ToString()?.Replace("\"", "\"\"")}\"");

        sb.AppendLine(string.Join(",", cells));
    }

    File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
}

// ✅ GOOD - Export to Excel (với EPPlus)
// Install: EPPlus NuGet package
private async Task ExportToExcelAsync(string filePath)
{
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    using var package = new ExcelPackage();
    var worksheet = package.Workbook.Worksheets.Add("Customers");

    // Headers
    for (int i = 0; i < dgvCustomers.Columns.Count; i++)
    {
        if (!dgvCustomers.Columns[i].Visible) continue;
        worksheet.Cells[1, i + 1].Value = dgvCustomers.Columns[i].HeaderText;
    }

    // Data
    for (int row = 0; row < dgvCustomers.Rows.Count; row++)
    {
        if (dgvCustomers.Rows[row].IsNewRow) continue;

        for (int col = 0; col < dgvCustomers.Columns.Count; col++)
        {
            if (!dgvCustomers.Columns[col].Visible) continue;
            worksheet.Cells[row + 2, col + 1].Value =
                dgvCustomers.Rows[row].Cells[col].Value;
        }
    }

    // Auto-fit columns
    worksheet.Cells.AutoFitColumns();

    // Save
    await package.SaveAsAsync(new FileInfo(filePath));
}
```

#### G. Sorting & Filtering

```csharp
// ✅ GOOD - Custom sorting
private void DgvCustomers_ColumnHeaderMouseClick(object? sender,
    DataGridViewCellMouseEventArgs e)
{
    var column = dgvCustomers.Columns[e.ColumnIndex];
    var propertyName = column.DataPropertyName;

    if (_currentSortColumn == propertyName)
    {
        _sortDirection = _sortDirection == "ASC" ? "DESC" : "ASC";
    }
    else
    {
        _sortDirection = "ASC";
        _currentSortColumn = propertyName;
    }

    ApplySorting();
}

private void ApplySorting()
{
    var bindingSource = dgvCustomers.DataSource as BindingSource;
    if (bindingSource == null) return;

    var sortProperty = typeof(Customer).GetProperty(_currentSortColumn);
    if (sortProperty == null) return;

    var customers = (bindingSource.DataSource as BindingList<Customer>)?.ToList();
    if (customers == null) return;

    IEnumerable<Customer> sorted = _sortDirection == "ASC"
        ? customers.OrderBy(c => sortProperty.GetValue(c))
        : customers.OrderByDescending(c => sortProperty.GetValue(c));

    bindingSource.DataSource = new BindingList<Customer>(sorted.ToList());
}
```

#### H. Pagination Pattern

```csharp
public partial class PaginatedGridForm : Form
{
    private int _currentPage = 1;
    private const int PAGE_SIZE = 50;
    private int _totalRecords = 0;

    private async Task LoadPageAsync(int pageNumber)
    {
        _currentPage = pageNumber;

        var skip = (pageNumber - 1) * PAGE_SIZE;
        var result = await _customerService.GetPagedAsync(skip, PAGE_SIZE);

        _totalRecords = result.TotalCount;
        dgvCustomers.DataSource = new BindingList<Customer>(result.Items);

        UpdatePaginationControls();
    }

    private void UpdatePaginationControls()
    {
        var totalPages = (int)Math.Ceiling(_totalRecords / (double)PAGE_SIZE);

        lblPageInfo.Text = $"Page {_currentPage} of {totalPages} " +
                          $"({_totalRecords} total records)";

        btnPrevious.Enabled = _currentPage > 1;
        btnNext.Enabled = _currentPage < totalPages;
        btnFirst.Enabled = _currentPage > 1;
        btnLast.Enabled = _currentPage < totalPages;
    }

    private async void BtnNext_Click(object sender, EventArgs e)
    {
        await LoadPageAsync(_currentPage + 1);
    }

    private async void BtnPrevious_Click(object sender, EventArgs e)
    {
        await LoadPageAsync(_currentPage - 1);
    }
}
```

---

### 17. 🗄️ Database Access Patterns (Entity Framework Core)

**Nguyên tắc:**
- ✅ Dùng **Entity Framework Core** cho data access (modern, cross-platform)
- ✅ **Repository Pattern** để tách biệt data access logic
- ✅ **Unit of Work Pattern** cho transactions
- ✅ **Async operations** cho tất cả database calls
- ✅ **Connection pooling** và proper DbContext lifecycle

#### A. DbContext Setup

```csharp
// ✅ BEST - DbContext configuration
public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entities
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();

            // One-to-many relationship
            entity.HasMany(e => e.Orders)
                  .WithOne(e => e.Customer)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Admin User", Email = "admin@example.com" }
        );
    }
}
```

#### B. Repository Pattern

```csharp
// ✅ GOOD - Generic repository interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}

// Implementation
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}

// Specific repository
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    Task<PagedResult<Customer>> GetPagedAsync(int skip, int take);
}

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(int skip, int take)
    {
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PagedResult<Customer>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = take,
            CurrentPage = (skip / take) + 1
        };
    }
}
```

#### C. Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public ICustomerRepository Customers { get; }
    public IOrderRepository Orders { get; }

    public UnitOfWork(AppDbContext context,
                      ICustomerRepository customerRepository,
                      IOrderRepository orderRepository)
    {
        _context = context;
        Customers = customerRepository;
        Orders = orderRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            await _transaction!.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction!.RollbackAsync();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

#### D. Service Layer với UoW

```csharp
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Customer> CreateCustomerWithOrderAsync(
        Customer customer, Order order)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Add customer
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            // Add order with customer ID
            order.CustomerId = customer.Id;
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation($"Created customer {customer.Id} with order {order.Id}");
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer with order");
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

#### E. Connection String Management

```csharp
// Program.cs - DI setup
static class Program
{
    [STAThread]
    static void Main()
    {
        var services = new ServiceCollection();

        // Configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();

        // DbContext với connection string
        services.AddDbContext<AppDbContext>(options =>
        {
            var connString = config.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connString); // hoặc UseSqlite, UseNpgsql
            options.EnableSensitiveDataLogging(isDevelopment);
            options.EnableDetailedErrors(isDevelopment);
        });

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<ICustomerService, CustomerService>();

        var serviceProvider = services.BuildServiceProvider();
        Application.Run(serviceProvider.GetRequiredService<MainForm>());
    }
}
```

#### F. Migrations

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Add migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script --output migration.sql
```

---

### 18. 💾 Memory Management & Performance Profiling

**Nguyên tắc:**
- ✅ **Profile before optimizing** - đo lường trước khi tối ưu
- ✅ Identify **memory leaks** sớm
- ✅ Monitor **GC pressure** và allocation patterns
- ✅ Dispose resources properly (images, fonts, controls)

#### A. Common Memory Leaks trong WinForms

```csharp
// ❌ BAD - Event handler memory leak
public partial class ParentForm : Form
{
    private void OpenChildForm()
    {
        var childForm = new ChildForm();
        childForm.DataChanged += ChildForm_DataChanged; // LEAK!
        childForm.Show();
    }

    private void ChildForm_DataChanged(object sender, EventArgs e)
    {
        RefreshData();
    }
}

// ✅ GOOD - Properly unsubscribe events
public partial class ParentForm : Form
{
    private ChildForm? _childForm;

    private void OpenChildForm()
    {
        _childForm = new ChildForm();
        _childForm.DataChanged += ChildForm_DataChanged;
        _childForm.FormClosed += (s, e) =>
        {
            if (_childForm != null)
            {
                _childForm.DataChanged -= ChildForm_DataChanged;
                _childForm = null;
            }
        };
        _childForm.Show();
    }
}

// ❌ BAD - Image resource leak
private void LoadImage()
{
    pictureBox.Image = new Bitmap("image.png"); // LEAK if not disposed
}

// ✅ GOOD - Dispose old image before assigning new one
private void LoadImage(string path)
{
    var oldImage = pictureBox.Image;
    pictureBox.Image = null;
    oldImage?.Dispose();

    pictureBox.Image = new Bitmap(path);
}

// ❌ BAD - Font leak
private void CreateDynamicLabels()
{
    for (int i = 0; i < 100; i++)
    {
        var label = new Label
        {
            Font = new Font("Arial", 12, FontStyle.Bold) // LEAK!
        };
        panel.Controls.Add(label);
    }
}

// ✅ GOOD - Reuse font or dispose properly
private Font? _sharedFont;

private void CreateDynamicLabels()
{
    _sharedFont = new Font("Arial", 12, FontStyle.Bold);

    for (int i = 0; i < 100; i++)
    {
        var label = new Label
        {
            Font = _sharedFont // Share font instance
        };
        panel.Controls.Add(label);
    }
}

protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        _sharedFont?.Dispose();
        components?.Dispose();
    }
    base.Dispose(disposing);
}
```

#### B. Memory Profiling Tools

**1. Visual Studio Diagnostic Tools**

```csharp
// Enable memory profiling trong Visual Studio
// Debug > Performance Profiler > Memory Usage

// Trigger snapshot programmatically
#if DEBUG
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
// Take snapshot here
#endif
```

**2. dotMemory (JetBrains)**

```bash
# Install dotMemory
# Analyze memory snapshots
# Compare snapshots to find leaks
# View retention graph
```

**3. PerfView (Microsoft)**

```bash
# Download PerfView from GitHub
# Collect memory trace:
perfview.exe collect -DataFile:myapp.etl

# Analyze GC heap
perfview.exe gcheap myapp.etl
```

#### C. Weak References Pattern

```csharp
// ✅ GOOD - Cache với WeakReference
public class ImageCache
{
    private readonly Dictionary<string, WeakReference<Bitmap>> _cache = new();

    public Bitmap GetImage(string path)
    {
        if (_cache.TryGetValue(path, out var weakRef))
        {
            if (weakRef.TryGetTarget(out var bitmap))
            {
                return bitmap; // Image still in memory
            }
        }

        // Load new image
        var newBitmap = new Bitmap(path);
        _cache[path] = new WeakReference<Bitmap>(newBitmap);
        return newBitmap;
    }

    public void Clear()
    {
        foreach (var weakRef in _cache.Values)
        {
            if (weakRef.TryGetTarget(out var bitmap))
            {
                bitmap.Dispose();
            }
        }
        _cache.Clear();
    }
}
```

#### D. Object Pooling

```csharp
// ✅ BEST - Object pool cho frequent allocations
public class StringBuilderPool
{
    private readonly ConcurrentBag<StringBuilder> _pool = new();
    private const int MAX_POOL_SIZE = 20;

    public StringBuilder Rent()
    {
        if (_pool.TryTake(out var sb))
        {
            sb.Clear();
            return sb;
        }
        return new StringBuilder();
    }

    public void Return(StringBuilder sb)
    {
        if (_pool.Count < MAX_POOL_SIZE)
        {
            _pool.Add(sb);
        }
    }
}

// Usage
var pool = new StringBuilderPool();
var sb = pool.Rent();
try
{
    sb.AppendLine("Data...");
    return sb.ToString();
}
finally
{
    pool.Return(sb);
}
```

---

### 19. 🌐 Localization & Internationalization (i18n)

**Nguyên tắc:**
- ✅ Tách **tất cả user-visible strings** ra resource files
- ✅ Support **multiple languages** từ đầu
- ✅ **Dynamic language switching** không cần restart
- ✅ **Culture-aware** formatting (dates, numbers, currency)

#### A. Resource Files Setup

```xml
<!-- Resources/Strings.resx (default - English) -->
<data name="WelcomeMessage" xml:space="preserve">
  <value>Welcome to our application!</value>
</data>
<data name="SaveButton" xml:space="preserve">
  <value>Save</value>
</data>
<data name="CancelButton" xml:space="preserve">
  <value>Cancel</value>
</data>

<!-- Resources/Strings.vi.resx (Vietnamese) -->
<data name="WelcomeMessage" xml:space="preserve">
  <value>Chào mừng đến với ứng dụng!</value>
</data>
<data name="SaveButton" xml:space="preserve">
  <value>Lưu</value>
</data>
<data name="CancelButton" xml:space="preserve">
  <value>Hủy</value>
</data>
```

#### B. Using Resources

```csharp
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        lblWelcome.Text = Resources.Strings.WelcomeMessage;
        btnSave.Text = Resources.Strings.SaveButton;
        btnCancel.Text = Resources.Strings.CancelButton;

        // Form title
        this.Text = Resources.Strings.ApplicationTitle;
    }
}
```

#### C. Dynamic Language Switching

```csharp
public static class LocalizationManager
{
    public static event EventHandler? LanguageChanged;

    public static void SetLanguage(string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        // Save preference
        Settings.Default.Language = cultureName;
        Settings.Default.Save();

        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Initialize()
    {
        var savedLanguage = Settings.Default.Language;
        if (!string.IsNullOrEmpty(savedLanguage))
        {
            SetLanguage(savedLanguage);
        }
    }
}

// Usage trong form
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        LocalizationManager.LanguageChanged += OnLanguageChanged;
        ApplyLocalization();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        ApplyLocalization();

        // Refresh child controls
        foreach (Control control in this.Controls)
        {
            if (control is ILocalizable localizable)
            {
                localizable.ApplyLocalization();
            }
        }
    }

    private void MenuLanguageEnglish_Click(object sender, EventArgs e)
    {
        LocalizationManager.SetLanguage("en-US");
    }

    private void MenuLanguageVietnamese_Click(object sender, EventArgs e)
    {
        LocalizationManager.SetLanguage("vi-VN");
    }
}
```

#### D. Culture-Aware Formatting

```csharp
// ✅ GOOD - Culture-aware date/number formatting
public class CultureAwareFormatter
{
    public static string FormatDate(DateTime date)
    {
        return date.ToString("d"); // Short date pattern for current culture
    }

    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("C"); // Currency format for current culture
    }

    public static string FormatNumber(decimal number)
    {
        return number.ToString("N2"); // Number with 2 decimals
    }
}

// Usage
lblDate.Text = CultureAwareFormatter.FormatDate(DateTime.Now);
// en-US: "1/15/2025"
// vi-VN: "15/01/2025"

lblAmount.Text = CultureAwareFormatter.FormatCurrency(1234.56m);
// en-US: "$1,234.56"
// vi-VN: "1.234,56 ₫"
```

---

### 20. 📦 Deployment & Packaging

**Deployment Options cho WinForms (.NET 6+):**

#### A. Self-Contained Deployment

```bash
# Single-file executable (all dependencies included)
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

# Trimmed (smaller size, requires testing)
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishTrimmed=true
```

#### B. Framework-Dependent Deployment

```bash
# Requires .NET runtime installed on target machine (smaller package)
dotnet publish -c Release -r win-x64 --self-contained false
```

#### C. MSIX Packaging (Recommended 2024)

```xml
<!-- .csproj configuration -->
<PropertyGroup>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <SelfContained>true</SelfContained>
  <PublishSingleFile>true</PublishSingleFile>
  <EnableMsixTooling>true</EnableMsixTooling>
  <GenerateAppInstallerFile>true</GenerateAppInstallerFile>
</PropertyGroup>
```

```bash
# Build MSIX package
msbuild /t:Publish /p:Configuration=Release /p:Platform=x64

# Sign package
signtool sign /fd SHA256 /a /f certificate.pfx /p password MyApp.msix
```

**Benefits:**
- ✅ Auto-updates via Microsoft Store
- ✅ Clean install/uninstall
- ✅ Sandboxed security
- ✅ Modern Windows integration

#### D. ClickOnce Deployment

```xml
<!-- .csproj configuration -->
<PropertyGroup>
  <PublishUrl>\\server\share\app\</PublishUrl>
  <UpdateEnabled>true</UpdateEnabled>
  <UpdateMode>Background</UpdateMode>
  <UpdateInterval>7</UpdateInterval>
  <UpdateIntervalUnits>Days</UpdateIntervalUnits>
</PropertyGroup>
```

#### E. Version Management

```xml
<!-- .csproj -->
<PropertyGroup>
  <Version>1.2.3</Version>
  <AssemblyVersion>1.2.3.0</AssemblyVersion>
  <FileVersion>1.2.3.0</FileVersion>
  <InformationalVersion>1.2.3-beta</InformationalVersion>
</PropertyGroup>
```

```csharp
// Display version trong About dialog
public partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        lblVersion.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";
    }
}
```

---

### 21. ♿ Keyboard Navigation & Accessibility

**Nguyên tắc:**
- ✅ **Tất cả controls** phải accessible via keyboard
- ✅ **Tab order** logical và intuitive
- ✅ **Keyboard shortcuts** cho actions quan trọng
- ✅ **Screen reader support** (AccessibleName, AccessibleDescription)

#### A. TabIndex Setup

```csharp
// ✅ GOOD - Logical tab order
private void SetupTabOrder()
{
    // Input form tab order: top to bottom, left to right
    txtName.TabIndex = 0;
    txtEmail.TabIndex = 1;
    txtPhone.TabIndex = 2;
    cbxCategory.TabIndex = 3;
    txtNotes.TabIndex = 4;

    // Buttons at end
    btnSave.TabIndex = 10;
    btnCancel.TabIndex = 11;

    // Focus first control
    this.ActiveControl = txtName;
}
```

#### B. Keyboard Shortcuts

```csharp
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        SetupKeyboardShortcuts();
    }

    private void SetupKeyboardShortcuts()
    {
        // Accept/Cancel buttons
        this.AcceptButton = btnSave;      // Enter key
        this.CancelButton = btnCancel;    // Escape key

        // Mnemonic keys (Alt+S, Alt+C)
        btnSave.Text = "&Save";           // Alt+S
        btnCancel.Text = "&Cancel";       // Alt+C
        lblName.Text = "&Name:";          // Alt+N focuses txtName
        lblName.UseMnemonic = true;

        // Custom shortcuts
        this.KeyPreview = true;
        this.KeyDown += MainForm_KeyDown;
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Ctrl+S = Save
        if (e.Control && e.KeyCode == Keys.S)
        {
            btnSave.PerformClick();
            e.Handled = true;
        }

        // Ctrl+N = New
        if (e.Control && e.KeyCode == Keys.N)
        {
            BtnNew_Click(this, EventArgs.Empty);
            e.Handled = true;
        }

        // F5 = Refresh
        if (e.KeyCode == Keys.F5)
        {
            RefreshData();
            e.Handled = true;
        }

        // Delete key = Delete selected
        if (e.KeyCode == Keys.Delete && dgvCustomers.Focused)
        {
            DeleteSelected();
            e.Handled = true;
        }
    }
}
```

#### C. Accessibility Properties

```csharp
// ✅ GOOD - Accessible controls
private void SetupAccessibility()
{
    // Descriptive names for screen readers
    txtName.AccessibleName = "Customer Name";
    txtName.AccessibleDescription = "Enter the customer's full name";

    btnSave.AccessibleName = "Save Customer";
    btnSave.AccessibleDescription = "Click to save customer information";

    // DataGridView accessibility
    dgvCustomers.AccessibleName = "Customer List";
    dgvCustomers.AccessibleDescription = "List of all customers in the system";

    // PictureBox (images need descriptions)
    picLogo.AccessibleName = "Company Logo";
    picLogo.AccessibleDescription = "ABC Company Logo";
    picLogo.AccessibleRole = AccessibleRole.Graphic;
}
```

#### D. High Contrast Support

```csharp
public partial class MainForm : Form
{
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);

        if (SystemInformation.HighContrast)
        {
            ApplyHighContrastTheme();
        }
        else
        {
            ApplyNormalTheme();
        }
    }

    private void ApplyHighContrastTheme()
    {
        // Use system colors
        this.BackColor = SystemColors.Control;
        this.ForeColor = SystemColors.ControlText;

        foreach (Control control in GetAllControls(this))
        {
            if (control is Button btn)
            {
                btn.BackColor = SystemColors.ButtonFace;
                btn.ForeColor = SystemColors.ButtonText;
            }
            else if (control is TextBox txt)
            {
                txt.BackColor = SystemColors.Window;
                txt.ForeColor = SystemColors.WindowText;
            }
        }
    }

    private IEnumerable<Control> GetAllControls(Control container)
    {
        foreach (Control control in container.Controls)
        {
            yield return control;

            if (control.HasChildren)
            {
                foreach (Control child in GetAllControls(control))
                {
                    yield return child;
                }
            }
        }
    }
}
```

---

## 📝 Comment & Docstring

### Dùng XML comments cho public class, method, property:

```csharp
/// <summary>
/// Lưu thông tin khách hàng mới vào cơ sở dữ liệu.
/// </summary>
/// <param name="customer">Đối tượng khách hàng cần lưu.</param>
/// <returns>True nếu lưu thành công, ngược lại False.</returns>
public bool SaveCustomer(Customer customer) { ... }
```

---

## ⚙️ Coding Rules cho AI (Claude Code)

Khi Claude Code được sử dụng để sinh hoặc sửa mã:

1. ✅ **Tuân thủ đầy đủ quy ước đặt tên ở trên.**
2. ✅ **Không thay đổi logic nghiệp vụ hiện có** trừ khi được yêu cầu.
3. ✅ Khi refactor, **tách logic khỏi Form sang Service** nếu thấy phù hợp.
4. ✅ **Tự động thêm XML comment** cho tất cả public members mới.
5. ✅ Code phải **biên dịch được ngay**, không tạo placeholder trừ khi được yêu cầu (`TODO`, `// stub`, ...).

---

## 📚 References

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [WinForms Best Practices](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/best-practices-for-developing-world-ready-applications)
- [Clean Code Principles](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882) *(ý tưởng chung, không ngôn ngữ cụ thể)*

---

## ✅ Checklist (dành cho Claude Code & devs)

Trước khi commit hoặc submit code, kiểm tra:

### General Code Quality
- [ ] Tên biến, hàm, class đúng convention
- [ ] UI không chứa logic nghiệp vụ
- [ ] Code có XML comment đầy đủ cho public members
- [ ] Logging chuẩn (dùng ILogger, không dùng Console.WriteLine)
- [ ] Có xử lý lỗi rõ ràng (try-catch với logging)
- [ ] Dễ mở rộng và bảo trì
- [ ] Không có magic numbers/strings

### Resource Management
- [ ] Image resources được dispose properly (PictureBox)
- [ ] Font resources được dispose nếu tạo dynamically
- [ ] BindingSource có dispose/unsubscribe events
- [ ] Event handlers được unsubscribe khi form close
- [ ] IDisposable resources sử dụng `using` statement

### UI & UX
- [ ] Keyboard navigation tested (Tab order correct)
- [ ] Form resize tested ở nhiều resolutions (1366x768, 1920x1080)
- [ ] High DPI tested (125%, 150%, 200% scaling)
- [ ] Buttons không bị crop text
- [ ] AcceptButton/CancelButton được set (Enter/Escape keys)
- [ ] Keyboard shortcuts hoạt động (Ctrl+S, F5, etc.)
- [ ] ToolTips added cho buttons/icons

### Performance
- [ ] Async/await used cho I/O operations
- [ ] DataGridView uses AutoGenerateColumns=false
- [ ] SuspendLayout/ResumeLayout khi add nhiều controls
- [ ] No blocking operations trên UI thread

### Data & Database
- [ ] Entity Framework migrations applied
- [ ] Database operations sử dụng async methods
- [ ] Parameterized queries (no SQL injection)
- [ ] Repository pattern used nếu có database

### Testing
- [ ] Unit tests viết cho business logic
- [ ] Test coverage >= 80% cho services
- [ ] Integration tests cho database operations

### Deployment Ready
- [ ] Version number updated (.csproj)
- [ ] appsettings.json không chứa sensitive data
- [ ] Connection strings encrypted hoặc in environment variables
- [ ] User secrets configured cho development

---

**Lưu ý:** Tài liệu này là tài liệu sống (living document) và sẽ được cập nhật khi dự án phát triển.

✨ *Happy Coding with Claude Code!* ✨
