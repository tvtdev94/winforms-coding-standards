# {{SERVICE_NAME}} Implementation Plan

**Type**: Service Layer
**Estimated Time**: 3-4 hours
**Created**: {{DATE}}

---

## Executive Summary

Brief description of what this service does and its responsibilities.

---

## Requirements

### Functional Requirements
- [ ] Business logic requirement 1
- [ ] Business logic requirement 2
- [ ] Data validation requirement 3
- [ ] Integration requirement 4

### Non-Functional Requirements
- [ ] All I/O operations are async
- [ ] Proper error handling and logging
- [ ] Input validation on all public methods
- [ ] 80%+ unit test coverage
- [ ] XML documentation on public APIs

---

## Architecture

### Service Layer Pattern

```
┌────────────────┐
│   Presenter    │ ← Calls service methods
└───────┬────────┘
        │
┌───────▼────────┐
│ {{SERVICE_NAME}} │ ← Business Logic Layer
│  implements    │   - Validation
│ I{{SERVICE_NAME}} │   - Business rules
└───────┬────────┘   - Orchestration
        │
┌───────▼────────────┐
│ {{REPOSITORY_NAME}} │ ← Data Access Layer
└────────────────────┘
```

### Dependencies

- **I{{REPOSITORY_NAME}}** - Data access interface
- **ILogger<{{SERVICE_NAME}}>** - Logging
- **IValidator<{{MODEL_NAME}}>** (optional) - Validation

---

## Implementation Checklist

### Phase 1: Interface Definition - 30 min
- [ ] Create I{{SERVICE_NAME}}.cs
- [ ] Define all public method signatures
- [ ] Add async suffix to I/O methods
- [ ] Add XML documentation
- [ ] Review interface with team

### Phase 2: Service Implementation - 1-2 hours
- [ ] Create {{SERVICE_NAME}}.cs
- [ ] Implement I{{SERVICE_NAME}} interface
- [ ] Add constructor with DI
- [ ] Implement each method:
  - [ ] Method 1: {{METHOD_NAME}}
  - [ ] Method 2: {{METHOD_NAME}}
  - [ ] Method 3: {{METHOD_NAME}}
- [ ] Add input validation (ArgumentNullException, etc.)
- [ ] Add try-catch with logging
- [ ] Add XML documentation

### Phase 3: Business Logic - 1 hour
- [ ] Implement validation rules
- [ ] Implement business rules
- [ ] Add domain-specific logic
- [ ] Handle edge cases
- [ ] Add proper error messages

### Phase 4: Unit Tests - 1-2 hours
- [ ] Create {{SERVICE_NAME}}Tests.cs
- [ ] Setup test fixtures (Moq)
- [ ] Write success path tests
- [ ] Write error case tests (null, invalid, etc.)
- [ ] Write edge case tests
- [ ] Verify 80%+ coverage
- [ ] Run all tests, ensure pass

### Phase 5: Integration - 15 min
- [ ] Register in DI container (Program.cs)
- [ ] Wire up dependencies
- [ ] Test service instantiation
- [ ] Verify logging works

---

## Files to Create

| File | Type | Location | Status |
|------|------|----------|--------|
| I{{SERVICE_NAME}}.cs | Interface | Services/ | ☐ Not Started |
| {{SERVICE_NAME}}.cs | Service | Services/ | ☐ Not Started |
| {{SERVICE_NAME}}Tests.cs | Tests | Tests/Services/ | ☐ Not Started |

**Total Files**: 3

---

## Service Methods

### Method 1: Get All
```csharp
Task<List<{{MODEL_NAME}}>> GetAllAsync();
```
- Retrieves all entities
- Returns empty list if none found

### Method 2: Get By ID
```csharp
Task<{{MODEL_NAME}}?> GetByIdAsync(int id);
```
- Retrieves single entity by ID
- Returns null if not found

### Method 3: Create
```csharp
Task<{{MODEL_NAME}}> CreateAsync({{MODEL_NAME}} entity);
```
- Validates input
- Creates new entity
- Returns created entity with ID

### Method 4: Update
```csharp
Task<bool> UpdateAsync({{MODEL_NAME}} entity);
```
- Validates input
- Updates existing entity
- Returns true if successful

### Method 5: Delete
```csharp
Task<bool> DeleteAsync(int id);
```
- Soft delete if supported
- Returns true if successful

---

## Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| Name | Required, max 100 chars | "Name is required and must be less than 100 characters" |
| Email | Required, valid email format | "Valid email address is required" |
| Age | Range 0-120 | "Age must be between 0 and 120" |

---

## Testing Strategy

### Unit Tests
- **Mock Repository**: All repository calls mocked
- **Mock Logger**: Verify logging calls
- **Test Coverage**: 80%+ target

### Test Cases to Cover
- [ ] GetAllAsync returns list when data exists
- [ ] GetAllAsync returns empty list when no data
- [ ] GetByIdAsync returns entity when found
- [ ] GetByIdAsync returns null when not found
- [ ] CreateAsync with valid input succeeds
- [ ] CreateAsync with null throws ArgumentNullException
- [ ] CreateAsync with invalid data throws ValidationException
- [ ] UpdateAsync with valid input succeeds
- [ ] UpdateAsync with non-existent ID returns false
- [ ] DeleteAsync with valid ID succeeds
- [ ] DeleteAsync with non-existent ID returns false

---

## Error Handling

### Exception Types
- **ArgumentNullException**: Null parameters
- **ValidationException**: Invalid input
- **NotFoundException**: Entity not found (custom)
- **Exception**: General errors (logged)

### Example Error Handling
```csharp
public async Task<Customer> CreateAsync(Customer customer)
{
    ArgumentNullException.ThrowIfNull(customer);

    if (string.IsNullOrWhiteSpace(customer.Name))
        throw new ValidationException("Customer name is required");

    try
    {
        var result = await _repository.AddAsync(customer);
        _logger.LogInformation("Created customer {Id}", result.Id);
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating customer");
        throw;
    }
}
```

---

## Progress Tracking

**Overall Progress**: 0%

- Phase 1 (Interface): ☐ 0%
- Phase 2 (Implementation): ☐ 0%
- Phase 3 (Business Logic): ☐ 0%
- Phase 4 (Unit Tests): ☐ 0%
- Phase 5 (Integration): ☐ 0%

---

**Plan Created**: {{DATE}}
**Last Updated**: {{DATE}}
**Status**: Not Started
