# Implementation Plan: [Feature Name]

## Overview
[Brief description of the feature - 2-3 sentences]

## Project Context
- **UI Framework**: [Standard / DevExpress / ReaLTaiizor]
- **Database**: [SQLite / SQL Server / PostgreSQL]
- **Pattern**: MVP
- **Target Framework**: [.NET 8 / .NET Framework 4.8]

## Requirements

### Functional Requirements
- [ ] FR1: [Requirement description]
- [ ] FR2: [Requirement description]
- [ ] FR3: [Requirement description]

### Non-Functional Requirements
- [ ] NFR1: Performance - [target, e.g., "Load under 2 seconds"]
- [ ] NFR2: Security - [consideration]
- [ ] NFR3: Usability - [consideration]

## Architecture

### Component Diagram
```
┌─────────────────────────────────────────────────────────┐
│                     UI Layer                             │
│  ┌──────────────┐    ┌──────────────────────────────┐   │
│  │  XxxForm     │───▶│  XxxPresenter                │   │
│  │  (IXxxView)  │◀───│                              │   │
│  └──────────────┘    └──────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────┐
│                  Application Layer                       │
│           ┌──────────────────────────────┐              │
│           │      IXxxService             │              │
│           │      XxxService              │              │
│           └──────────────────────────────┘              │
└─────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────┐
│                Infrastructure Layer                      │
│  ┌──────────────┐    ┌──────────────────────────────┐   │
│  │ IUnitOfWork  │───▶│  IXxxRepository              │   │
│  │              │    │  XxxRepository               │   │
│  └──────────────┘    └──────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### Components

| Component | Type | Responsibility |
|-----------|------|----------------|
| `IXxxView` | Interface | View contract for Form |
| `XxxForm` | Form | UI display (Designer-compatible) |
| `XxxPresenter` | Presenter | UI logic, orchestration |
| `IXxxService` | Interface | Business logic contract |
| `XxxService` | Service | Business logic implementation |
| `IXxxRepository` | Interface | Data access contract |
| `XxxRepository` | Repository | Data access implementation |

### Data Flow
```
User Action → Form → Presenter → Service → UnitOfWork → Repository → Database
                ↑                                              │
                └──────────────────────────────────────────────┘
                              (Result returned)
```

## Implementation Steps

### Step 1: Domain Layer
**Files to create:**
- [ ] `Domain/Models/Xxx.cs` - Entity model
- [ ] `Domain/Interfaces/IXxxRepository.cs` - Repository interface

**Template:** `templates/model-template.cs`

### Step 2: Application Layer
**Files to create:**
- [ ] `Application/Services/IXxxService.cs` - Service interface
- [ ] `Application/Services/XxxService.cs` - Service implementation

**Template:** `templates/service-template.cs`

### Step 3: Infrastructure Layer
**Files to create:**
- [ ] `Infrastructure/Persistence/Repositories/XxxRepository.cs`
- [ ] `Infrastructure/Persistence/Configurations/XxxConfiguration.cs` (if EF Core)

**Files to modify:**
- [ ] `Infrastructure/Persistence/UnitOfWork/IUnitOfWork.cs` - Add repository property
- [ ] `Infrastructure/Persistence/UnitOfWork/UnitOfWork.cs` - Implement property

**Template:** `templates/repository-template.cs`

### Step 4: UI Layer
**Files to create:**
- [ ] `UI/Views/IXxxView.cs` - View interface
- [ ] `UI/Presenters/XxxPresenter.cs` - Presenter
- [ ] `UI/Forms/XxxForm.cs` - Form (Designer-compatible)
- [ ] `UI/Forms/XxxForm.Designer.cs` - Designer file

**Files to modify:**
- [ ] `UI/Factories/IFormFactory.cs` - Add create method
- [ ] `UI/Factories/FormFactory.cs` - Implement create method

**Template:** `templates/form-template.cs`, `templates/presenter-template.cs`

### Step 5: Dependency Injection
**Files to modify:**
- [ ] `Program.cs` - Register services

```csharp
// Add to ConfigureServices
services.AddScoped<IXxxRepository, XxxRepository>();
services.AddScoped<IXxxService, XxxService>();
services.AddTransient<XxxPresenter>();
```

### Step 6: Testing
**Files to create:**
- [ ] `Tests/Unit/Services/XxxServiceTests.cs`
- [ ] `Tests/Unit/Presenters/XxxPresenterTests.cs`

**Template:** See `tester` agent patterns

## Files Summary

### Files to Create
| File Path | Template | Priority |
|-----------|----------|----------|
| `Domain/Models/Xxx.cs` | model-template.cs | High |
| `Application/Services/XxxService.cs` | service-template.cs | High |
| `UI/Forms/XxxForm.cs` | form-template.cs | High |
| ... | ... | ... |

### Files to Modify
| File Path | Changes | Priority |
|-----------|---------|----------|
| `IUnitOfWork.cs` | Add `IXxxRepository Xxx { get; }` | High |
| `FormFactory.cs` | Add `CreateXxxForm()` | Medium |
| `Program.cs` | Register DI | Medium |

## Testing Strategy

### Unit Tests
- [ ] `XxxService.GetByIdAsync_WhenExists_ReturnsEntity`
- [ ] `XxxService.GetByIdAsync_WhenNotExists_ReturnsNull`
- [ ] `XxxService.CreateAsync_WhenValid_SavesEntity`
- [ ] `XxxPresenter.LoadAsync_WhenSuccess_PopulatesView`
- [ ] `XxxPresenter.SaveAsync_WhenValid_CallsService`

### Integration Tests (Optional)
- [ ] `XxxRepository` with InMemory database

## Security Considerations
- [ ] Input validation in Presenter before passing to Service
- [ ] Use parameterized queries (EF Core handles this)
- [ ] No sensitive data in error messages
- [ ] Authorization checks if applicable

## Performance Considerations
- [ ] Pagination for list views (if > 100 records)
- [ ] Async/await for all I/O operations
- [ ] Eager loading to prevent N+1 queries

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| [Risk 1] | [High/Medium/Low] | [Mitigation strategy] |
| [Risk 2] | [High/Medium/Low] | [Mitigation strategy] |

## TODO Checklist

### Domain Layer
- [ ] Create entity model
- [ ] Create repository interface

### Application Layer
- [ ] Create service interface
- [ ] Implement service

### Infrastructure Layer
- [ ] Implement repository
- [ ] Update UnitOfWork

### UI Layer
- [ ] Create view interface
- [ ] Create presenter
- [ ] Create form (Designer-compatible)
- [ ] Update FormFactory

### Integration
- [ ] Register DI in Program.cs
- [ ] Test manually

### Testing
- [ ] Write service tests
- [ ] Write presenter tests
- [ ] Run all tests

### Review
- [ ] Code review
- [ ] MVP pattern compliance

## Unresolved Questions
- [ ] Question 1?
- [ ] Question 2?

---
**Plan Created:** [Date]
**Plan Author:** [Agent/User]
**Status:** Draft / In Progress / Completed
