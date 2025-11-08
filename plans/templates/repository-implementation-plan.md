# {{REPOSITORY_NAME}} Implementation Plan

**Type**: Data Access Layer
**Estimated Time**: 2-3 hours
**Created**: {{DATE}}

---

## Executive Summary

Data access layer for {{MODEL_NAME}} entity using Entity Framework Core.

---

## Requirements

### Functional Requirements
- [ ] CRUD operations for {{MODEL_NAME}}
- [ ] Query operations (search, filter)
- [ ] Soft delete support (if needed)
- [ ] Async operations throughout

### Non-Functional Requirements
- [ ] EF Core best practices followed
- [ ] Proper DbContext disposal
- [ ] No business logic (data access only)
- [ ] 70%+ integration test coverage

---

## Architecture

### Repository Pattern

```
┌────────────┐
│  Service   │
└─────┬──────┘
      │
┌─────▼──────────────┐
│ I{{REPOSITORY_NAME}} │ ← Interface
└─────┬──────────────┘
      │
┌─────▼──────────────┐
│ {{REPOSITORY_NAME}}  │ ← Implementation
│  uses DbContext    │
└─────┬──────────────┘
      │
┌─────▼──────┐
│  Database  │
└────────────┘
```

---

## Implementation Checklist

### Phase 1: Entity Configuration - 30 min
- [ ] Create {{MODEL_NAME}}.cs (if not exists)
- [ ] Configure EF Core entity:
  - [ ] Primary key
  - [ ] Required fields
  - [ ] String lengths
  - [ ] Relationships
  - [ ] Indexes

### Phase 2: Repository Interface - 15 min
- [ ] Create I{{REPOSITORY_NAME}}.cs
- [ ] Define CRUD methods
- [ ] Define query methods
- [ ] Add XML documentation

### Phase 3: Repository Implementation - 1 hour
- [ ] Create {{REPOSITORY_NAME}}.cs
- [ ] Implement IRepository interface
- [ ] Add constructor with DbContext
- [ ] Implement async methods:
  - [ ] GetAllAsync()
  - [ ] GetByIdAsync(id)
  - [ ] AddAsync(entity)
  - [ ] UpdateAsync(entity)
  - [ ] DeleteAsync(id)
  - [ ] Query methods
- [ ] Add proper error handling

### Phase 4: Integration Tests - 1-2 hours
- [ ] Create {{REPOSITORY_NAME}}Tests.cs
- [ ] Setup InMemory database
- [ ] Write CRUD tests
- [ ] Write query tests
- [ ] Verify 70%+ coverage
- [ ] Run all tests

### Phase 5: DbContext Registration - 15 min
- [ ] Register DbContext in DI
- [ ] Register Repository in DI
- [ ] Configure connection string
- [ ] Test instantiation

---

## Files to Create

| File | Type | Location | Status |
|------|------|----------|--------|
| {{MODEL_NAME}}.cs | Entity | Models/ | ☐ Exists/New |
| I{{REPOSITORY_NAME}}.cs | Interface | Data/ | ☐ Not Started |
| {{REPOSITORY_NAME}}.cs | Repository | Data/ | ☐ Not Started |
| {{REPOSITORY_NAME}}Tests.cs | Tests | Tests/Data/ | ☐ Not Started |

**Total Files**: 3-4

---

## Repository Methods

### CRUD Operations

```csharp
public interface I{{REPOSITORY_NAME}}
{
    Task<List<{{MODEL_NAME}}>> GetAllAsync();
    Task<{{MODEL_NAME}}?> GetByIdAsync(int id);
    Task<{{MODEL_NAME}}> AddAsync({{MODEL_NAME}} entity);
    Task UpdateAsync({{MODEL_NAME}} entity);
    Task DeleteAsync(int id);
}
```

### Query Operations

```csharp
Task<List<{{MODEL_NAME}}>> FindByNameAsync(string name);
Task<List<{{MODEL_NAME}}>> GetActiveAsync();
Task<bool> ExistsAsync(int id);
```

---

## Entity Configuration

### {{MODEL_NAME}} Entity

```csharp
public class {{MODEL_NAME}}
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; } // Soft delete
}
```

### EF Core Configuration

```csharp
modelBuilder.Entity<{{MODEL_NAME}}>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
    entity.Property(e => e.CreatedDate).IsRequired();
    entity.HasQueryFilter(e => !e.IsDeleted); // Soft delete filter
});
```

---

## Testing Strategy

### Integration Tests (InMemory DB)

```csharp
public class {{REPOSITORY_NAME}}Tests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly {{REPOSITORY_NAME}} _repository;

    public {{REPOSITORY_NAME}}Tests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new {{REPOSITORY_NAME}}(_context);
    }

    [Fact]
    public async Task AddAsync_ValidEntity_AddsToDatabase()
    {
        // Arrange
        var entity = new {{MODEL_NAME}} { Name = "Test" };

        // Act
        var result = await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(1, await _context.{{MODEL_NAME}}s.CountAsync());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

### Test Cases to Cover

- [ ] GetAllAsync returns all entities
- [ ] GetAllAsync returns empty list when no data
- [ ] GetByIdAsync returns entity when found
- [ ] GetByIdAsync returns null when not found
- [ ] AddAsync adds entity to database
- [ ] AddAsync sets ID after insert
- [ ] UpdateAsync modifies existing entity
- [ ] DeleteAsync removes entity (hard delete)
- [ ] DeleteAsync marks as deleted (soft delete)
- [ ] Query filters work correctly

---

## Progress Tracking

**Overall Progress**: 0%

- Phase 1 (Entity Config): ☐ 0%
- Phase 2 (Interface): ☐ 0%
- Phase 3 (Implementation): ☐ 0%
- Phase 4 (Tests): ☐ 0%
- Phase 5 (DI Registration): ☐ 0%

---

**Plan Created**: {{DATE}}
**Last Updated**: {{DATE}}
**Status**: Not Started
