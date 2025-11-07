# Test Coverage in WinForms

## 📋 Overview

Code coverage measures how much of your application code is executed during testing. It's a critical metric for assessing test effectiveness and identifying untested code paths in your WinForms applications.

This guide covers:
- Understanding coverage metrics (line, branch, method)
- Coverage tools for .NET (Coverlet, dotCover, Visual Studio)
- Setting up and running coverage reports
- Coverage in CI/CD pipelines
- Best practices for meaningful coverage

**Target Audience**: Developers writing tests for WinForms applications
**Prerequisites**: Understanding of [Unit Testing](unit-testing.md) and [Integration Testing](integration-testing.md)

---

## 🎯 Why This Matters

### Measuring Test Effectiveness
Code coverage helps you answer: "Are my tests actually testing my code?"

### Finding Untested Code
Coverage reports highlight exactly which lines, branches, and methods lack test coverage, making it easy to identify gaps.

### Quality Gates
Set minimum coverage thresholds in CI/CD to prevent merging code without adequate tests.

### Risk Assessment
Uncovered code represents potential bugs waiting to happen. Critical business logic should have near-100% coverage.

---

## Understanding Code Coverage

### What is Code Coverage?

Code coverage is a percentage measurement of how much code is executed when your tests run. There are several types:

#### Line Coverage
The percentage of code lines executed during tests.

```csharp
public decimal CalculateDiscount(decimal price, bool isPremium)
{
    decimal discount = 0;           // ✅ Covered

    if (isPremium)                  // ✅ Covered
    {
        discount = price * 0.20m;   // ❌ Not covered (if isPremium never tested as true)
    }

    return price - discount;        // ✅ Covered
}
```

**Line Coverage**: 80% (4 out of 5 lines executed)

#### Branch Coverage
The percentage of decision branches (if/else, switch) tested.

```csharp
public string GetCustomerType(int orderCount)
{
    if (orderCount > 100)           // Branch 1: true
    {
        return "Gold";              // ❌ Not covered
    }
    else if (orderCount > 50)       // Branch 2: true
    {
        return "Silver";            // ✅ Covered
    }
    else                            // Branch 3: false
    {
        return "Bronze";            // ❌ Not covered
    }
}

// Test only: GetCustomerType(75) returns "Silver"
```

**Branch Coverage**: 33% (1 out of 3 branches tested)
**Line Coverage**: Could still be 100% if all lines executed once

#### Method Coverage
The percentage of methods called during tests.

```csharp
public class CustomerService
{
    public void AddCustomer(Customer c) { }      // ✅ Tested
    public void UpdateCustomer(Customer c) { }   // ✅ Tested
    public void DeleteCustomer(int id) { }       // ❌ Not tested
    public Customer GetCustomer(int id) { }      // ✅ Tested
}
```

**Method Coverage**: 75% (3 out of 4 methods called)

### Coverage Metrics Explained

| Metric | Measures | Example |
|--------|----------|---------|
| **Line Coverage** | % of lines executed | `15/20 lines = 75%` |
| **Branch Coverage** | % of decision paths tested | `6/10 branches = 60%` |
| **Method Coverage** | % of methods called | `8/10 methods = 80%` |
| **Cyclomatic Complexity** | Number of code paths | Higher = more tests needed |

### Coverage ≠ Quality

**CRITICAL**: High coverage doesn't guarantee good tests!

```csharp
// BAD: 100% coverage, 0% value
[Fact]
public void CalculateTotal_ReturnsDecimal()
{
    var service = new OrderService();
    var result = service.CalculateTotal(100, 0.1m);

    // Test passes for ANY result! 🚨
    Assert.IsType<decimal>(result);
}

// GOOD: Meaningful test
[Fact]
public void CalculateTotal_WithDiscount_ReturnsCorrectAmount()
{
    var service = new OrderService();
    var result = service.CalculateTotal(100, 0.1m);

    Assert.Equal(90m, result); // Verifies actual behavior
}
```

**Key Principle**: Aim for high coverage WITH meaningful assertions.

---

## Coverage Targets

### Recommended Targets by Component

| Component | Target | Justification |
|-----------|--------|---------------|
| **Overall Project** | **80%+** | Industry standard for good coverage |
| **Business Logic / Services** | **85-95%** | Critical code, high value |
| **Presenters / ViewModels** | **80-90%** | Core application logic |
| **Repositories** | **70-80%** | Often integration-tested |
| **Utilities / Helpers** | **90-100%** | Easy to test, high reuse |
| **Models / DTOs** | **50-70%** | Mostly data, less logic |
| **Forms (UI)** | **20-40%** | Hard to unit test, consider UI testing |

### Critical Paths: 90-100%
Code that handles:
- Financial calculations
- Security/authentication
- Data validation
- File/database operations
- Error handling

### What to Exclude from Coverage

```xml
<!-- coverlet.runsettings -->
<Exclude>
  [*]*.Designer          <!-- Designer-generated code -->
  [*]Program             <!-- Main entry point -->
  [*]*AssemblyInfo       <!-- Assembly metadata -->
  [*]*.g.cs              <!-- Auto-generated code -->
  [*]Migrations.*        <!-- EF migrations -->
</Exclude>
```

**Exclude**:
- `Form1.Designer.cs` - Auto-generated UI code
- `Program.cs` - Application entry point
- Third-party libraries
- Auto-generated code (EF migrations, etc.)

---

## Coverage Tools for .NET

### 1. Coverlet (RECOMMENDED)

**Best for**: Cross-platform .NET projects

**Pros**:
- ✅ Free and open-source
- ✅ Works with `dotnet test`
- ✅ Cross-platform (Windows, Linux, macOS)
- ✅ Supports all output formats (Cobertura, LCOV, OpenCover)
- ✅ Easy CI/CD integration

**Cons**:
- ❌ Command-line only (no GUI)
- ❌ Requires ReportGenerator for HTML reports

**Installation**:
```bash
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild
```

### 2. dotCover (JetBrains)

**Best for**: ReSharper/Rider users

**Pros**:
- ✅ Visual Studio / Rider integration
- ✅ Continuous testing mode
- ✅ Excellent UI and visualizations
- ✅ Coverage highlighting in code

**Cons**:
- ❌ Commercial license required
- ❌ Windows/JetBrains tools only

**Features**:
- Real-time coverage highlighting
- Coverage tree view
- Risk hotspots detection

### 3. Visual Studio Coverage (Enterprise)

**Best for**: Teams with VS Enterprise licenses

**Pros**:
- ✅ Built-in to Visual Studio Enterprise
- ✅ Easy to use (one-click)
- ✅ Great visualization

**Cons**:
- ❌ Requires VS Enterprise ($$$)
- ❌ Windows-only
- ❌ Not available in VS Community/Professional

### 4. OpenCover (Legacy)

**Best for**: Legacy .NET Framework projects

**Pros**:
- ✅ Free and open-source
- ✅ Well-established

**Cons**:
- ❌ Older, less maintained
- ❌ Complex command-line usage
- ❌ Use Coverlet instead for new projects

### Comparison Table

| Tool | Cost | Platform | CI/CD | IDE Integration |
|------|------|----------|-------|----------------|
| **Coverlet** | Free | All | ✅✅✅ | ❌ |
| **dotCover** | Paid | Win/Mac | ✅✅ | ✅✅✅ |
| **VS Coverage** | Paid | Windows | ✅ | ✅✅✅ |
| **OpenCover** | Free | Windows | ✅✅ | ❌ |

**Recommendation**: Use **Coverlet** for most projects.

---

## Setting Up Coverlet

### Installation

```bash
# Add to your test project
cd YourApp.Tests
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild

# Optional: ReportGenerator for HTML reports
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Running Coverage

#### Basic Coverage
```bash
dotnet test /p:CollectCoverage=true
```

Output:
```
+---------------+--------+--------+--------+
| Module        | Line   | Branch | Method |
+---------------+--------+--------+--------+
| YourApp       | 85.2%  | 78.5%  | 88.9%  |
+---------------+--------+--------+--------+
```

#### Generate Cobertura Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

Generates: `coverage.cobertura.xml`

#### Generate LCOV Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

Generates: `coverage.info`

#### Generate HTML Report
```bash
# 1. Generate coverage data
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# 2. Convert to HTML
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report

# 3. Open report
start coverage-report/index.html  # Windows
open coverage-report/index.html   # macOS
```

### Configuration File

**coverlet.runsettings**:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>cobertura,lcov,opencover</Format>
          <Exclude>[*]*.Designer,[*]Program,[*]*Migrations.*</Exclude>
          <ExcludeByAttribute>Obsolete,GeneratedCode,CompilerGenerated</ExcludeByAttribute>
          <IncludeDirectory>../YourApp/bin/Debug/</IncludeDirectory>
          <SingleHit>false</SingleHit>
          <UseSourceLink>true</UseSourceLink>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

**Usage**:
```bash
dotnet test --settings coverlet.runsettings
```

---

## Coverage Reports

### Using ReportGenerator

**Installation**:
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

**Generate HTML Report**:
```bash
reportgenerator \
  -reports:coverage.cobertura.xml \
  -targetdir:coverage-report \
  -reporttypes:Html
```

**Advanced Options**:
```bash
reportgenerator \
  -reports:coverage.cobertura.xml \
  -targetdir:coverage-report \
  -reporttypes:Html;Badges;TextSummary \
  -historydir:coverage-history \
  -title:"MyApp Test Coverage"
```

### Coverage Output Formats

| Format | File | Use Case |
|--------|------|----------|
| **Cobertura** | `.xml` | Azure DevOps, Jenkins, GitLab CI |
| **LCOV** | `.info` | GitHub Actions, Codecov, Coveralls |
| **OpenCover** | `.xml` | SonarQube, legacy tools |
| **JSON** | `.json` | Custom processing |

**Example**:
```bash
# Multiple formats
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=\"cobertura,lcov,json\"
```

### Reading Coverage Reports

**HTML Report Structure**:
```
coverage-report/
├── index.html          # Summary dashboard
├── Summary.html        # Detailed summary
└── YourApp/
    ├── CustomerService.cs.html  # Per-file coverage
    └── OrderService.cs.html
```

**Coverage Indicators**:
- 🟢 **Green**: Line covered by tests
- 🔴 **Red**: Line NOT covered
- 🟡 **Yellow**: Branch partially covered (only one path tested)

**Example**:
```
CustomerService.cs - 85% coverage

🟢 10: public void AddCustomer(Customer customer)
🟢 11: {
🟢 12:     if (customer == null)
🟢 13:         throw new ArgumentNullException(nameof(customer));
🟢 14:
🟡 15:     if (customer.Age < 18)          // Only false branch tested
🔴 16:         throw new ValidationException("Must be 18+");
🟢 17:
🟢 18:     _repository.Add(customer);
🟢 19: }
```

---

## Improving Coverage

### Finding Untested Code

**Step 1**: Generate HTML Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report
```

**Step 2**: Analyze Report

Open `coverage-report/index.html` and look for:
- ❌ Files with low coverage (< 70%)
- ❌ Critical services with gaps
- ❌ Error handling not tested

**Step 3**: Prioritize

1. **Critical business logic** (calculations, validations)
2. **Error handling** (try-catch blocks)
3. **Edge cases** (null checks, boundary conditions)
4. **Branching logic** (if/else, switch)

### Writing Tests for Uncovered Code

**Before** (50% coverage):
```csharp
public class OrderService
{
    public decimal CalculateTotal(Order order)
    {
        if (order == null)                          // ❌ Not tested
            throw new ArgumentNullException();

        var subtotal = order.Items.Sum(i => i.Price);  // ✅ Tested

        if (order.Customer.IsPremium)               // ❌ true branch not tested
            return subtotal * 0.9m;

        return subtotal;                            // ✅ Tested
    }
}

[Fact]
public void CalculateTotal_RegularCustomer_NoDiscount()
{
    var order = new Order { Customer = new Customer { IsPremium = false } };
    Assert.Equal(100m, _service.CalculateTotal(order));
}
```

**After** (100% coverage):
```csharp
[Fact]
public void CalculateTotal_NullOrder_ThrowsException()
{
    Assert.Throws<ArgumentNullException>(() => _service.CalculateTotal(null));
}

[Fact]
public void CalculateTotal_RegularCustomer_NoDiscount()
{
    var order = new Order
    {
        Items = new[] { new Item { Price = 100m } },
        Customer = new Customer { IsPremium = false }
    };
    Assert.Equal(100m, _service.CalculateTotal(order));
}

[Fact]
public void CalculateTotal_PremiumCustomer_Applies10PercentDiscount()
{
    var order = new Order
    {
        Items = new[] { new Item { Price = 100m } },
        Customer = new Customer { IsPremium = true }
    };
    Assert.Equal(90m, _service.CalculateTotal(order));
}
```

### Avoiding Coverage Traps

❌ **DON'T**: Write meaningless tests just to increase numbers
```csharp
// BAD: Tests nothing useful
[Fact]
public void Constructor_DoesNotThrow()
{
    var service = new CustomerService();
    Assert.NotNull(service);  // Worthless assertion
}
```

✅ **DO**: Write tests that verify actual behavior
```csharp
// GOOD: Tests real functionality
[Fact]
public void GetCustomer_ValidId_ReturnsCustomer()
{
    var customer = _service.GetCustomer(1);
    Assert.Equal("John Doe", customer.Name);
    Assert.Equal(1, customer.Id);
}
```

---

## Coverage in CI/CD

### GitHub Actions Example

**.github/workflows/test-coverage.yml**:
```yaml
name: Test Coverage

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Run tests with coverage
        run: dotnet test --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=lcov

      - name: Upload to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: coverage.info
          fail_ci_if_error: true

      - name: Generate HTML report
        run: |
          dotnet tool install -g dotnet-reportgenerator-globaltool
          reportgenerator -reports:coverage.info -targetdir:coverage-report -reporttypes:Html

      - name: Upload coverage report
        uses: actions/upload-artifact@v3
        with:
          name: coverage-report
          path: coverage-report/
```

### Azure DevOps Pipeline

**azure-pipelines.yml**:
```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: DotNetCoreCLI@2
  displayName: 'Restore packages'
  inputs:
    command: 'restore'

- task: DotNetCoreCLI@2
  displayName: 'Run tests with coverage'
  inputs:
    command: 'test'
    arguments: '--configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura'
    publishTestResults: true

- task: PublishCodeCoverageResults@1
  displayName: 'Publish coverage report'
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/coverage.cobertura.xml'
    failIfCoverageEmpty: true
```

### Coverage Gates

**Fail build if coverage drops below threshold**:

```bash
# Using Coverlet threshold
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line
```

**GitHub Actions with threshold**:
```yaml
- name: Check coverage threshold
  run: |
    dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line,branch
```

**When to use**:
- ✅ New projects: Set 80% threshold from day one
- ✅ Critical projects: Prevent coverage regression
- ❌ Legacy projects: Gradually increase threshold (60% → 70% → 80%)

### Coverage Badges

**Add to README.md**:

```markdown
# MyApp

![Coverage](https://img.shields.io/codecov/c/github/username/repo)

## Test Coverage: 85%
```

**Using Codecov**:
```yaml
# In GitHub Actions (see above)
- name: Upload to Codecov
  uses: codecov/codecov-action@v3
```

Then add to README:
```markdown
[![codecov](https://codecov.io/gh/username/repo/branch/main/graph/badge.svg)](https://codecov.io/gh/username/repo)
```

---

## Best Practices

### ✅ DO

1. **DO: Aim for 80%+ overall coverage**
   ```bash
   dotnet test /p:CollectCoverage=true /p:Threshold=80
   ```

2. **DO: Focus on critical business logic**
   - Financial calculations: 95%+
   - Validation logic: 90%+
   - Data access: 80%+

3. **DO: Test all branches**
   ```csharp
   // Test BOTH branches
   [Fact] public void WhenTrue_ReturnsA() { }
   [Fact] public void WhenFalse_ReturnsB() { }
   ```

4. **DO: Exclude generated code**
   ```xml
   <Exclude>[*]*.Designer,[*]Program</Exclude>
   ```

5. **DO: Run coverage locally before pushing**
   ```bash
   dotnet test /p:CollectCoverage=true
   ```

6. **DO: Review coverage reports regularly**
   - Weekly team review
   - Check for regression

7. **DO: Use coverage to find missing tests**
   - Not as a goal itself
   - As a tool to identify gaps

8. **DO: Combine unit and integration tests for accurate picture**
   ```bash
   dotnet test --filter "Category!=Integration" /p:CollectCoverage=true
   dotnet test --filter "Category=Integration" /p:CollectCoverage=true
   ```

9. **DO: Track coverage trends over time**
   ```bash
   reportgenerator -historydir:coverage-history
   ```

10. **DO: Write meaningful assertions, not just coverage**
    ```csharp
    Assert.Equal(expected, actual);  // ✅ GOOD
    Assert.NotNull(result);          // ❌ Weak
    ```

### ❌ DON'T

1. **DON'T: Aim for 100% coverage at all costs**
   - 80-90% is excellent
   - Diminishing returns after 90%

2. **DON'T: Write tests just to increase coverage**
   ```csharp
   // BAD: No value
   [Fact] public void PropertyExists() { Assert.NotNull(obj.Property); }
   ```

3. **DON'T: Ignore branch coverage**
   - Line coverage can be misleading
   - Always check branch coverage too

4. **DON'T: Include Designer code in coverage**
   - Wastes time
   - Skews metrics

5. **DON'T: Rely on coverage alone**
   - Code review is still essential
   - Coverage doesn't catch logic errors

6. **DON'T: Test private methods directly**
   - Test public API
   - Private methods get covered indirectly

7. **DON'T: Ignore uncovered error handling**
   ```csharp
   catch (Exception ex)  // ❌ If uncovered, test it!
   {
       _logger.LogError(ex.Message);
   }
   ```

8. **DON'T: Forget to test async code**
   ```csharp
   [Fact]
   public async Task LoadAsync_ReturnsList() { /* Test async */ }
   ```

---

## Coverage by Component

### Presenters Coverage (Target: 85-90%)

**What to cover**:
- ✅ All view model population logic
- ✅ Command/button click handlers
- ✅ Validation logic
- ✅ Navigation logic

**What can be excluded**:
- ❌ Simple property getters/setters
- ❌ Purely UI-related code

**Example**:
```csharp
public class CustomerPresenterTests
{
    [Fact]
    public async Task LoadCustomers_PopulatesView()
    {
        // Arrange
        var mockView = new Mock<ICustomerView>();
        var mockService = new Mock<ICustomerService>();
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new[]
        {
            new Customer { Name = "John" }
        });

        var presenter = new CustomerPresenter(mockView.Object, mockService.Object);

        // Act
        await presenter.LoadCustomersAsync();

        // Assert
        mockView.Verify(v => v.SetCustomers(It.Is<IEnumerable<Customer>>(
            c => c.Count() == 1)), Times.Once);
    }
}
```

### Services Coverage (Target: 90-95%)

**Critical**:
- ✅ All business logic
- ✅ Validation rules
- ✅ Error handling
- ✅ Edge cases

**Example**:
```csharp
public class OrderServiceTests
{
    [Theory]
    [InlineData(0, false)]      // Zero items
    [InlineData(1, true)]       // One item
    [InlineData(100, true)]     // Many items
    public void ValidateOrder_ItemCount_ReturnsCorrectResult(int count, bool expected)
    {
        var order = new Order { Items = Enumerable.Range(1, count).Select(i => new Item()).ToList() };
        Assert.Equal(expected, _service.ValidateOrder(order));
    }

    [Fact]
    public void CalculateTotal_NullOrder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.CalculateTotal(null));
    }
}
```

### Repositories Coverage (Target: 70-80%)

**Debate**: Unit tests vs integration tests for repositories

**Recommendation**: Combination approach
- Unit tests with mocked DbContext (fast, fragile)
- Integration tests with real database (slow, reliable)

**Example**:
```csharp
// Integration test (preferred for repositories)
public class CustomerRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCustomer()
    {
        // Arrange
        await _repository.AddAsync(new Customer { Id = 1, Name = "Test" });

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }
}
```

---

## Complete Coverage Workflow

### Step 1: Run Tests with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Step 2: Generate HTML Report
```bash
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report
```

### Step 3: Analyze Results
```bash
# Open report
start coverage-report/index.html

# Look for:
# - Files with < 80% coverage
# - Red lines (uncovered)
# - Yellow lines (partial branch coverage)
```

### Step 4: Write Missing Tests
```csharp
// Found: Error handling not covered
catch (DbUpdateException ex)  // ❌ RED LINE
{
    _logger.LogError(ex, "Database error");
    throw new DataAccessException("Failed to save", ex);
}

// Add test:
[Fact]
public async Task SaveAsync_DatabaseError_ThrowsDataAccessException()
{
    _mockContext.Setup(c => c.SaveChangesAsync(default))
        .ThrowsAsync(new DbUpdateException());

    await Assert.ThrowsAsync<DataAccessException>(() =>
        _repository.SaveAsync(new Customer()));
}
```

### Step 5: Verify Improvement
```bash
dotnet test /p:CollectCoverage=true

# Check new coverage percentage
# Before: 75%
# After:  85% ✅
```

---

## Coverage Metrics Dashboard

### Local Development

**Visual Studio Enterprise**:
```
Test → Analyze Code Coverage → All Tests
```

**JetBrains Rider / ReSharper**:
```
ReSharper → Cover All Tests
```

**NCrunch (Continuous Testing)**:
- Real-time coverage as you code
- Coverage markers in editor
- Commercial tool

### Team Dashboard

**SonarQube**:
```yaml
# In CI/CD
- name: SonarQube Scan
  run: |
    dotnet sonarscanner begin /k:"project-key" /d:sonar.cs.opencover.reportsPaths=coverage.xml
    dotnet build
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
    dotnet sonarscanner end
```

**Codecov**:
- Automatic PR comments with coverage diff
- Coverage graphs over time
- Free for open source

**Coveralls**:
- Similar to Codecov
- GitHub integration
- Badge generation

---

## Common Pitfalls

### 1. Testing Trivial Code
❌ **Don't waste time on**:
```csharp
[Fact]
public void Constructor_SetsProperty()
{
    var obj = new Customer { Name = "Test" };
    Assert.Equal("Test", obj.Name);  // Trivial!
}
```

### 2. Ignoring Async Void Methods
❌ **These are hard to test and dangerous**:
```csharp
private async void btnLoad_Click(object sender, EventArgs e)  // ❌ async void
{
    await LoadDataAsync();
}
```

✅ **Solution**: Extract logic to testable method:
```csharp
private async void btnLoad_Click(object sender, EventArgs e)
{
    await LoadDataImplAsync();  // ✅ Testable
}

internal async Task LoadDataImplAsync()  // ✅ Can be tested
{
    // Logic here
}
```

### 3. Not Excluding Generated Code
❌ Including `*.Designer.cs` inflates coverage artificially.

✅ **Always exclude**:
```xml
<Exclude>[*]*.Designer,[*]Program,[*]*Migrations.*</Exclude>
```

### 4. Relying Only on Line Coverage
Line coverage can be 100% with only 50% branch coverage!

✅ **Check both**:
```bash
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line,branch
```

### 5. Not Testing Exception Paths
```csharp
try
{
    await _service.SaveAsync(customer);  // ✅ Tested
}
catch (Exception ex)                      // ❌ Not tested!
{
    _logger.LogError(ex.Message);
}
```

✅ **Test error handling**:
```csharp
[Fact]
public async Task Save_ServiceThrows_LogsError()
{
    _mockService.Setup(s => s.SaveAsync(It.IsAny<Customer>()))
        .ThrowsAsync(new Exception("Test error"));

    await _presenter.SaveCustomerAsync();

    _mockLogger.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
}
```

### 6. Coverage Without Assertions
```csharp
[Fact]
public void Calculate_RunsWithoutError()
{
    _service.Calculate(10, 20);  // ❌ No assertion!
}
```

### 7. Ignoring Integration Test Coverage
Unit tests alone may show 90% coverage, but integration tests reveal real-world issues.

---

## Practical Example

### Before Coverage: Untested Code

**OrderService.cs**:
```csharp
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;

    public async Task<decimal> CalculateTotalAsync(int orderId)
    {
        var order = await _repository.GetByIdAsync(orderId);

        if (order == null)
            throw new NotFoundException($"Order {orderId} not found");

        decimal total = 0;
        foreach (var item in order.Items)
        {
            total += item.Price * item.Quantity;
        }

        if (order.DiscountCode != null)
        {
            total *= 0.9m; // 10% discount
        }

        return total;
    }
}
```

**Coverage**: 0%

### After Coverage: Comprehensive Tests

**OrderServiceTests.cs**:
```csharp
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        _service = new OrderService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CalculateTotalAsync_ValidOrder_ReturnsCorrectTotal()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem>
            {
                new() { Price = 10m, Quantity = 2 },  // 20
                new() { Price = 5m, Quantity = 3 }    // 15
            }
        };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        // Act
        var total = await _service.CalculateTotalAsync(1);

        // Assert
        Assert.Equal(35m, total);  // 20 + 15
    }

    [Fact]
    public async Task CalculateTotalAsync_WithDiscount_AppliesDiscount()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem> { new() { Price = 100m, Quantity = 1 } },
            DiscountCode = "SAVE10"
        };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        // Act
        var total = await _service.CalculateTotalAsync(1);

        // Assert
        Assert.Equal(90m, total);  // 100 * 0.9
    }

    [Fact]
    public async Task CalculateTotalAsync_OrderNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.CalculateTotalAsync(999));
    }
}
```

**Coverage Report**:
```
OrderService.cs - 100% coverage

Module      Line   Branch  Method
OrderService 100%   100%    100%

✅ All lines covered
✅ Both discount branches tested (with/without)
✅ Exception path tested
```

---

## Related Topics

- **[Unit Testing](unit-testing.md)** - Writing effective unit tests
- **[Integration Testing](integration-testing.md)** - Testing with real dependencies
- **[Testing Overview](testing-overview.md)** - Testing strategy for WinForms

---

## Summary

### Key Takeaways

1. **Coverage is a tool, not a goal** - High coverage with bad tests is worthless
2. **Aim for 80%+ overall** - With 90%+ on critical business logic
3. **Use Coverlet** - Free, cross-platform, CI/CD friendly
4. **Check branch coverage** - Line coverage alone is misleading
5. **Exclude generated code** - Focus on code you write
6. **Integrate with CI/CD** - Prevent coverage regression
7. **Review reports regularly** - Find gaps and improve

### Quick Reference Commands

```bash
# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Generate HTML report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report

# Enforce threshold
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line,branch

# CI/CD (GitHub Actions)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
# Upload coverage.info to Codecov
```

---

**Last Updated**: 2025-11-07
**Version**: 1.0
**Author**: WinForms Coding Standards Team
