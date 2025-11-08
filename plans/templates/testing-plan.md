# Testing Plan: {{FEATURE_NAME}}

**Type**: Testing Strategy
**Estimated Time**: 4-6 hours
**Created**: {{DATE}}

---

## Executive Summary

Comprehensive testing plan for {{FEATURE_NAME}} covering unit, integration, and UI testing.

---

## Test Coverage Goals

| Layer | Coverage Target | Test Type |
|-------|----------------|-----------|
| **Services** | 80%+ | Unit tests with mocks |
| **Repositories** | 70%+ | Integration tests (InMemory DB) |
| **Presenters** | 75%+ | Unit tests with mocks |
| **Forms** | Manual | UI testing checklist |

---

## Testing Checklist

### Phase 1: Unit Tests - Services (2 hours)
- [ ] Create test project (if not exists)
- [ ] Add testing dependencies (xUnit, Moq, FluentAssertions)
- [ ] Create {{SERVICE}}Tests.cs
- [ ] Test success paths
- [ ] Test error cases
- [ ] Test edge cases
- [ ] Verify 80%+ coverage
- [ ] All tests pass

### Phase 2: Integration Tests - Repositories (1-2 hours)
- [ ] Create {{REPOSITORY}}Tests.cs
- [ ] Setup InMemory database
- [ ] Test CRUD operations
- [ ] Test query operations
- [ ] Test transaction handling
- [ ] Verify 70%+ coverage
- [ ] All tests pass

### Phase 3: Unit Tests - Presenters (1 hour)
- [ ] Create {{PRESENTER}}Tests.cs
- [ ] Mock IView and IService
- [ ] Test user interaction flows
- [ ] Test error handling
- [ ] Verify 75%+ coverage
- [ ] All tests pass

### Phase 4: Manual UI Tests (1-2 hours)
- [ ] Test form loading
- [ ] Test all user interactions
- [ ] Test validation
- [ ] Test error messages
- [ ] Test responsive behavior
- [ ] Test keyboard navigation

---

## Unit Test Examples

### Service Test Template

```csharp
public class {{SERVICE}}Tests
{
    private readonly Mock<I{{REPOSITORY}}> _mockRepository;
    private readonly Mock<ILogger<{{SERVICE}}>> _mockLogger;
    private readonly {{SERVICE}} _service;

    public {{SERVICE}}Tests()
    {
        _mockRepository = new Mock<I{{REPOSITORY}}>();
        _mockLogger = new Mock<ILogger<{{SERVICE}}>>();
        _service = new {{SERVICE}}(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsData()
    {
        // Arrange
        var expected = new List<{{MODEL}}>
        {
            new {{MODEL}} { Id = 1, Name = "Test" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAsync(null));
    }
}
```

---

## Integration Test Examples

### Repository Test Template

```csharp
public class {{REPOSITORY}}Tests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly {{REPOSITORY}} _repository;

    public {{REPOSITORY}}Tests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new {{REPOSITORY}}(_context);
    }

    [Fact]
    public async Task AddAsync_ValidEntity_AddsToDatabase()
    {
        // Arrange
        var entity = new {{MODEL}} { Name = "Test" };

        // Act
        var result = await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        (await _context.{{MODEL}}s.CountAsync()).Should().Be(1);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

---

## Test Scenarios

### Success Path Tests
- [ ] GetAll returns list of items
- [ ] GetById returns item when found
- [ ] Create adds new item successfully
- [ ] Update modifies existing item
- [ ] Delete removes item successfully

### Error Case Tests
- [ ] GetById returns null when not found
- [ ] Create with null throws ArgumentNullException
- [ ] Create with invalid data throws ValidationException
- [ ] Update non-existent item returns false
- [ ] Delete non-existent item returns false

### Edge Case Tests
- [ ] GetAll returns empty list when no data
- [ ] Create with minimum valid data
- [ ] Create with maximum length strings
- [ ] Update with same data (no changes)
- [ ] Concurrent operations handled correctly

---

## Manual UI Testing Checklist

### Form Loading
- [ ] Form opens without errors
- [ ] All controls visible
- [ ] Data loads asynchronously
- [ ] Loading indicator shown
- [ ] UI not frozen during load

### User Interactions
- [ ] Add button creates new item
- [ ] Edit button opens edit mode
- [ ] Delete button removes item
- [ ] Cancel button discards changes
- [ ] Save button persists changes

### Validation
- [ ] Required fields validated
- [ ] Email format validated
- [ ] Number ranges validated
- [ ] Error messages clear and helpful
- [ ] ErrorProvider shows validation errors

### Error Handling
- [ ] Network errors handled gracefully
- [ ] Database errors show user-friendly messages
- [ ] Validation errors prevent save
- [ ] Errors logged correctly

### Accessibility
- [ ] Tab order logical
- [ ] Keyboard shortcuts work
- [ ] Alt+Key accelerators work
- [ ] Screen reader compatible (if required)

### Performance
- [ ] Form loads in <2 seconds
- [ ] Large lists virtualized
- [ ] No UI freezing
- [ ] Memory not leaking

---

## Test Execution

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"

# Run with verbosity
dotnet test --verbosity detailed
```

### Coverage Report

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# View HTML report
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

---

## Test Results Template

### Test Summary

**Date**: {{DATE}}
**Tester**: {{NAME}}

| Category | Total | Passed | Failed | Skipped | Coverage |
|----------|-------|--------|--------|---------|----------|
| Unit Tests (Services) | X | X | X | X | X% |
| Integration Tests (Repos) | X | X | X | X | X% |
| Unit Tests (Presenters) | X | X | X | X | X% |
| Manual UI Tests | X | X | X | X | N/A |

### Issues Found

1. **Issue 1**
   - Severity: High/Medium/Low
   - Description: [What went wrong]
   - Steps to Reproduce: [How to trigger]
   - Expected: [What should happen]
   - Actual: [What actually happened]

---

## Progress Tracking

**Overall Progress**: 0%

- Phase 1 (Service Tests): ☐ 0%
- Phase 2 (Repository Tests): ☐ 0%
- Phase 3 (Presenter Tests): ☐ 0%
- Phase 4 (Manual UI Tests): ☐ 0%

---

**Plan Created**: {{DATE}}
**Last Updated**: {{DATE}}
**Status**: Not Started
