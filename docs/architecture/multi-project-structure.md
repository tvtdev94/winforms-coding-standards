# Multi-Project Solution Structure

> **Quick Reference**: Advanced solution structure with multiple projects for separation of concerns, testability, and code reuse.

---

## 📋 Overview

**Multi-Project Structure** separates your application into multiple independent projects (assemblies), each with a specific responsibility. This enforces strict architectural boundaries through **compiler-level dependency control**.

### When to Use Multi-Project

✅ **Use Multi-Project When:**
- Large applications (20+ forms)
- Enterprise/production apps
- Team size 3+ developers
- Need to reuse Business/Data layers in other apps (Web API, Console, etc.)
- Strict architecture enforcement required
- Long-term maintenance expected

❌ **Avoid Multi-Project When:**
- Small/medium apps (< 20 forms)
- Prototypes or POCs
- Single developer or small team
- WinForms-only application
- Quick turnaround needed

---

## 🏗️ Standard Multi-Project Structure

```
/YourSolution.sln
├── src/
│   ├── YourApp.UI/                  # WinForms - Presentation Layer
│   │   ├── /Forms                   # Form classes
│   │   ├── /Controls                # Custom controls
│   │   ├── /Views                   # View interfaces (MVP)
│   │   ├── /Presenters              # Presenters (MVP)
│   │   ├── /Factories               # Form factories
│   │   └── Program.cs               # Entry point
│   │
│   ├── YourApp.Core/                # Core - Domain Models & Interfaces
│   │   ├── /Models                  # Domain entities, DTOs
│   │   ├── /Interfaces              # Service interfaces, Repository interfaces
│   │   ├── /Enums                   # Enumerations
│   │   └── /Exceptions              # Custom exceptions
│   │
│   ├── YourApp.Business/            # Business - Business Logic Layer
│   │   ├── /Services                # Service implementations
│   │   ├── /Validators              # Business validation
│   │   └── /Mapping                 # AutoMapper profiles
│   │
│   └── YourApp.Data/                # Data - Data Access Layer
│       ├── /Repositories            # Repository implementations
│       ├── /Context                 # DbContext
│       ├── /Configurations          # Entity configurations
│       └── /UnitOfWork              # Unit of Work pattern
│
└── tests/
    ├── YourApp.Business.Tests/      # Business logic unit tests
    ├── YourApp.Data.Tests/          # Repository integration tests
    └── YourApp.UI.Tests/            # UI tests (FlaUI - optional)
```

---

## 📦 Project Details

### 1️⃣ **YourApp.UI** - Presentation Layer

**Responsibility**: User interface, UI logic, user interaction

**Contains**:
- Windows Forms (`.cs`, `.Designer.cs`, `.resx`)
- Custom UserControls
- View interfaces (for MVP)
- Presenters (for MVP)
- Form factories

**Dependencies**:
```
YourApp.UI
  ├─> YourApp.Core       (for models, interfaces)
  └─> YourApp.Business   (for service interfaces)
```

**DO**:
- ✅ Handle UI events
- ✅ Display data to users
- ✅ Call services through interfaces
- ✅ Use dependency injection

**DON'T**:
- ❌ Reference `YourApp.Data` directly
- ❌ Contain business logic
- ❌ Access database directly

**Example .csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\YourApp.Core\YourApp.Core.csproj" />
    <ProjectReference Include="..\YourApp.Business\YourApp.Business.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" />
    <PackageReference Include="Serilog" />
    <!-- UI Framework: DevExpress, ReaLTaiizor, etc. -->
  </ItemGroup>
</Project>
```

---

### 2️⃣ **YourApp.Core** - Core Domain

**Responsibility**: Domain models, interfaces, DTOs - **NO implementations**

**Contains**:
- Domain models (POCOs)
- DTOs (Data Transfer Objects)
- Service interfaces (`ICustomerService`, etc.)
- Repository interfaces (`ICustomerRepository`, etc.)
- Custom exceptions
- Enums

**Dependencies**:
```
YourApp.Core
  └─> NO DEPENDENCIES (pure domain)
```

**DO**:
- ✅ Define interfaces
- ✅ Define domain models
- ✅ Define DTOs
- ✅ Data annotations for validation

**DON'T**:
- ❌ Reference other projects
- ❌ Contain implementations
- ❌ Add NuGet packages (except annotations)

**Example .csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- Minimal dependencies - annotations only -->
    <PackageReference Include="System.ComponentModel.Annotations" />
  </ItemGroup>
</Project>
```

**Example Code**:
```csharp
// Models/Customer.cs
namespace YourApp.Core.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

// Interfaces/ICustomerService.cs
namespace YourApp.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<bool> SaveAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
    }
}
```

---

### 3️⃣ **YourApp.Business** - Business Logic

**Responsibility**: Business logic, validation, orchestration

**Contains**:
- Service implementations
- Business rule validation
- Transaction management
- Orchestration of multiple operations

**Dependencies**:
```
YourApp.Business
  └─> YourApp.Core       (for models, interfaces)
```

**DO**:
- ✅ Implement service interfaces
- ✅ Enforce business rules
- ✅ Validate data
- ✅ Orchestrate multiple repository calls
- ✅ Use Unit of Work for transactions

**DON'T**:
- ❌ Reference `YourApp.UI`
- ❌ Reference `YourApp.Data` (only through interfaces)
- ❌ Contain UI logic
- ❌ Access database directly

**Example .csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\YourApp.Core\YourApp.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation" />
    <PackageReference Include="AutoMapper" />
  </ItemGroup>
</Project>
```

**Example Code**:
```csharp
// Services/CustomerService.cs
namespace YourApp.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> SaveAsync(Customer customer)
        {
            // Business validation
            if (string.IsNullOrWhiteSpace(customer.Email))
                throw new ValidationException("Email is required");

            // Check for duplicates (business rule)
            var existing = await _unitOfWork.Customers.FindByEmailAsync(customer.Email);
            if (existing != null && existing.Id != customer.Id)
                throw new DuplicateException("Email already exists");

            // Save via Unit of Work
            if (customer.Id == 0)
                await _unitOfWork.Customers.AddAsync(customer);
            else
                await _unitOfWork.Customers.UpdateAsync(customer);

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
```

---

### 4️⃣ **YourApp.Data** - Data Access

**Responsibility**: Database access, EF Core, repositories

**Contains**:
- DbContext
- Repository implementations
- Entity configurations
- Unit of Work implementation
- Migrations

**Dependencies**:
```
YourApp.Data
  └─> YourApp.Core       (for models, interfaces)
```

**DO**:
- ✅ Implement repository interfaces
- ✅ Configure Entity Framework
- ✅ Handle database operations
- ✅ Return domain models (from Core)

**DON'T**:
- ❌ Reference `YourApp.UI`
- ❌ Reference `YourApp.Business`
- ❌ Contain business logic
- ❌ Call `SaveChangesAsync` in repositories (use Unit of Work)

**Example .csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\YourApp.Core\YourApp.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
  </ItemGroup>
</Project>
```

**Example Code**:
```csharp
// Context/AppDbContext.cs
namespace YourApp.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

// Repositories/CustomerRepository.cs
namespace YourApp.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            // DON'T call SaveChangesAsync here! Unit of Work handles it.
        }
    }
}

// UnitOfWork/UnitOfWork.cs
namespace YourApp.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ICustomerRepository Customers { get; }

        public UnitOfWork(AppDbContext context, ICustomerRepository customerRepository)
        {
            _context = context;
            Customers = customerRepository;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose() => _context.Dispose();
    }
}
```

---

## 🔗 Dependency Graph

```
┌──────────────┐
│  YourApp.UI  │ (Presentation - WinForms)
└──────┬───────┘
       │ uses
       ├──────────────────┐
       ▼                  ▼
┌────────────────┐  ┌──────────────────┐
│ YourApp.Core   │  │ YourApp.Business │ (Business Logic)
│ (Models, IFs)  │◄─┤                  │
└────────────────┘  └──────────────────┘
       ▲
       │ uses
       │
┌──────────────┐
│ YourApp.Data │ (Data Access)
└──────────────┘

Key:
─> : Project reference (compile-time)
```

**Rules**:
1. ✅ `UI` references `Core` + `Business`
2. ✅ `Business` references `Core` only
3. ✅ `Data` references `Core` only
4. ❌ `Core` references **NO ONE** (pure domain)
5. ❌ `Business` and `Data` **NEVER** reference `UI`
6. ❌ `Business` and `Data` **NEVER** reference each other

---

## 🎯 Dependency Injection Setup

In `YourApp.UI/Program.cs`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using YourApp.Core.Interfaces;
using YourApp.Business.Services;
using YourApp.Data.Context;
using YourApp.Data.Repositories;
using YourApp.Data.UnitOfWork;
using YourApp.UI.Forms;

namespace YourApp.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Build DI container
            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Run application
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services.AddSingleton(configuration);

            // Logging
            services.AddLogging(builder => builder.AddSerilog());

            // Database (Data layer)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories (Data layer)
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Unit of Work (Data layer)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services (Business layer)
            services.AddScoped<ICustomerService, CustomerService>();

            // Forms (UI layer)
            services.AddTransient<MainForm>();
            services.AddTransient<CustomerForm>();
        }
    }
}
```

---

## 📊 Comparison: Single Project vs Multi-Project

| Aspect | Single Project | Multi-Project |
|--------|---------------|---------------|
| **Complexity** | ⭐ Simple | ⭐⭐⭐ Complex |
| **Setup Time** | ⭐⭐⭐ Fast (5 min) | ⭐ Slow (15+ min) |
| **Build Speed** | ⭐⭐⭐ Fast | ⭐⭐ Slower (multiple assemblies) |
| **Dependency Control** | ⚠️ Convention-based | ✅ Compiler-enforced |
| **Testability** | ⭐⭐ Good | ⭐⭐⭐ Excellent |
| **Code Reuse** | ❌ Difficult | ✅ Easy (Business/Data reusable) |
| **Team Collaboration** | ⭐⭐ OK | ⭐⭐⭐ Excellent |
| **Best For** | Small/medium apps | Large/enterprise apps |
| **Team Size** | 1-3 developers | 3+ developers |
| **Maintenance** | ⭐⭐ Manual enforcement | ⭐⭐⭐ Compiler helps |

---

## 🛠️ Creating Multi-Project Solution

### Using init-project.ps1

```powershell
.\scripts\init-project.ps1

# When prompted:
# Question 6: Project Structure
#   [1] Single Project (recommended for most apps)
#   [2] Multi-Project (for large/enterprise apps)
# Select: 2
```

### Manual Creation

```bash
# 1. Create solution
dotnet new sln -n YourApp

# 2. Create projects
dotnet new winforms -n YourApp.UI -f net8.0
dotnet new classlib -n YourApp.Core -f net8.0
dotnet new classlib -n YourApp.Business -f net8.0
dotnet new classlib -n YourApp.Data -f net8.0

# 3. Add projects to solution
dotnet sln add YourApp.UI/YourApp.UI.csproj
dotnet sln add YourApp.Core/YourApp.Core.csproj
dotnet sln add YourApp.Business/YourApp.Business.csproj
dotnet sln add YourApp.Data/YourApp.Data.csproj

# 4. Add project references
dotnet add YourApp.UI reference YourApp.Core YourApp.Business
dotnet add YourApp.Business reference YourApp.Core
dotnet add YourApp.Data reference YourApp.Core

# 5. Create test projects
dotnet new xunit -n YourApp.Business.Tests -f net8.0
dotnet new xunit -n YourApp.Data.Tests -f net8.0
dotnet sln add YourApp.Business.Tests/YourApp.Business.Tests.csproj
dotnet sln add YourApp.Data.Tests/YourApp.Data.Tests.csproj
dotnet add YourApp.Business.Tests reference YourApp.Business
dotnet add YourApp.Data.Tests reference YourApp.Data
```

---

## 🧪 Testing Strategy

### Unit Tests (YourApp.Business.Tests)

Test business logic **in isolation** - mock all dependencies:

```csharp
public class CustomerServiceTests
{
    [Fact]
    public async Task SaveAsync_WithDuplicateEmail_ThrowsException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var existingCustomer = new Customer { Id = 1, Email = "test@example.com" };
        mockUnitOfWork.Setup(u => u.Customers.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(existingCustomer);

        var service = new CustomerService(mockUnitOfWork.Object, Mock.Of<ILogger<CustomerService>>());
        var newCustomer = new Customer { Id = 2, Email = "test@example.com" };

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateException>(() => service.SaveAsync(newCustomer));
    }
}
```

### Integration Tests (YourApp.Data.Tests)

Test database access with **in-memory database**:

```csharp
public class CustomerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
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
        _context.Customers.AddRange(
            new Customer { Name = "Alice", Email = "alice@example.com" },
            new Customer { Name = "Bob", Email = "bob@example.com" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    public void Dispose() => _context.Dispose();
}
```

---

## 📋 Migration to Multi-Project

If you have an existing **Single Project** and want to convert to **Multi-Project**:

### Step-by-Step Migration

1. **Create new projects**:
   ```bash
   dotnet new classlib -n YourApp.Core
   dotnet new classlib -n YourApp.Business
   dotnet new classlib -n YourApp.Data
   ```

2. **Move Models**:
   - Move `/Models` → `YourApp.Core/Models`

3. **Move Interfaces**:
   - Extract interfaces from Services → `YourApp.Core/Interfaces`
   - Extract interfaces from Repositories → `YourApp.Core/Interfaces`

4. **Move Services**:
   - Move `/Services` → `YourApp.Business/Services`

5. **Move Data Access**:
   - Move `/Repositories` → `YourApp.Data/Repositories`
   - Move `/Data` → `YourApp.Data/Context`

6. **Move UI**:
   - Keep `/Forms` in original project
   - Rename original project to `YourApp.UI`

7. **Add Project References**:
   ```bash
   dotnet add YourApp.UI reference YourApp.Core YourApp.Business
   dotnet add YourApp.Business reference YourApp.Core
   dotnet add YourApp.Data reference YourApp.Core
   ```

8. **Fix Namespaces**:
   - Update all `using` statements
   - Run build to find errors

9. **Update DI Registration**:
   - Update `Program.cs` to reference new namespaces

---

## 🔗 Related Topics

- [Single Project Structure](project-structure.md) - Simpler alternative
- [MVP Pattern](mvp-pattern.md) - Works with both structures
- [Dependency Injection](dependency-injection.md) - DI setup
- [Unit of Work Pattern](../data-access/unit-of-work-pattern.md) - Transaction management

---

## 📚 References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft - Project Structure](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

**Last Updated**: 2025-11-19
**Version**: 1.0
