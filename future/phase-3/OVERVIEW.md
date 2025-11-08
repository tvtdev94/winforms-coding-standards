# Phase 3: Plan Templates & Project Scaffolding

**Status**: 📋 High-Level Plan
**Duration**: 4-6 hours
**Priority**: 🟡 High
**Prerequisites**: Phase 1 complete

---

## 🎯 Goals

1. Add **plan templates** for structured implementation planning
2. Create **scaffold-feature.ps1** script for rapid feature generation
3. Enable **10x faster** project setup with automation

---

## 📦 Deliverables

### Plan Templates (5 templates)

```
plans/templates/
├── form-implementation-plan.md
├── service-implementation-plan.md
├── repository-implementation-plan.md
├── refactoring-plan.md
├── testing-plan.md
└── template-usage-guide.md
```

**Each template includes**:
- Executive Summary section (2-3 sentences)
- Requirements checklist
- Architecture overview with diagram
- Implementation phases with tasks
- Testing strategy
- Files to create/modify
- Estimated time
- Notes & decisions

### Scaffolding Script

```
scripts/scaffold-feature.ps1
```

**Capabilities**:
- Generate complete feature skeleton (Model + Repository + Service + Form + Tests)
- Support MVP and MVVM patterns
- Auto-wire dependency injection
- Create plan file from template
- Estimate implementation time

**Usage**:
```powershell
.\scripts\scaffold-feature.ps1 -FeatureName "Customer" -Pattern "MVP"
```

**Generates**:
```
Models/Customer.cs
Data/ICustomerRepository.cs
Data/CustomerRepository.cs
Services/ICustomerService.cs
Services/CustomerService.cs
Forms/CustomerForm.cs
Forms/ICustomerView.cs
Presenters/CustomerPresenter.cs
Tests/Services/CustomerServiceTests.cs
Tests/Data/CustomerRepositoryTests.cs
Tests/Presenters/CustomerPresenterTests.cs
plans/YYMMDD-customer-implementation-plan.md
```

---

## 📊 Impact

### Before Phase 3
- **Manual planning**: Inconsistent, often skipped
- **Manual file creation**: 30+ min per feature
- **Copy-paste from templates**: Error-prone
- **No structured approach**: Varies by developer

### After Phase 3
- **Structured planning**: Consistent, comprehensive
- **One-command scaffolding**: <2 min setup
- **Auto-generated from templates**: Error-free
- **Guided workflow**: Follow proven patterns

**Time Savings**: ~30-60 min per feature

---

## 🔧 Implementation Approach

### Step 1: Create Plan Templates (3 hours)

**form-implementation-plan.md** (60 min):
```markdown
# [Form Name] Implementation Plan

**Pattern**: MVP / MVVM
**Estimated Time**: 6-8 hours

## Requirements
- [ ] Functional requirement 1
- [ ] Non-functional requirement 1

## Architecture
[Diagram of Form → Presenter → Service → Repository]

## Implementation Checklist
### Phase 1: Model & Repository
- [ ] Create Customer.cs
- [ ] Create ICustomerRepository
- [ ] Implement CustomerRepository
- [ ] Add unit tests

### Phase 2: Service Layer
...

## Files to Create
- Models/Customer.cs (NEW)
- Data/ICustomerRepository.cs (NEW)
...

## Testing Strategy
- Unit tests for Service
- Integration tests for Repository
- Manual UI testing checklist
```

Similar templates for:
- **service-implementation-plan.md** (45 min)
- **repository-implementation-plan.md** (45 min)
- **refactoring-plan.md** (45 min)
- **testing-plan.md** (30 min)
- **template-usage-guide.md** (30 min)

### Step 2: Create Scaffolding Script (2 hours)

**scaffold-feature.ps1**:

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$FeatureName,

    [Parameter(Mandatory=$false)]
    [ValidateSet("MVP", "MVVM")]
    [string]$Pattern = "MVP",

    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "."
)

# 1. Create directories
# 2. Generate files from templates/
# 3. Replace {{PLACEHOLDERS}} with $FeatureName
# 4. Create plan file from template
# 5. Wire up DI registration code
# 6. Display next steps
```

### Step 3: Enhance Templates with Placeholders (1 hour)

Update existing templates to support placeholders:
- `templates/form-template.cs` → Add {{FORM_NAME}}, {{SERVICE_NAME}}
- `templates/service-template.cs` → Add {{SERVICE_NAME}}, {{REPOSITORY_NAME}}
- `templates/repository-template.cs` → Add {{REPOSITORY_NAME}}, {{ENTITY_NAME}}

### Step 4: Test Scaffolding (1 hour)

Run scaffold script and verify:
```powershell
# Test MVP pattern
.\scripts\scaffold-feature.ps1 -FeatureName "Product" -Pattern "MVP"

# Verify all files created
# Verify files compile
# Verify plan file generated
# Verify DI wiring correct
```

---

## ✅ Success Criteria

Phase 3 is complete when:

- [ ] All 6 plan templates created
- [ ] scaffold-feature.ps1 script works
- [ ] Script generates all files correctly
- [ ] Generated code compiles without errors
- [ ] Plan file created from template
- [ ] Documentation updated
- [ ] Script tested on both MVP and MVVM patterns

---

## 📋 Features of scaffold-feature.ps1

### Smart Placeholder Replacement

```csharp
// Before (in template):
public class {{SERVICE_NAME}} : I{{SERVICE_NAME}}
{
    private readonly I{{REPOSITORY_NAME}} _repository;
}

// After scaffolding with -FeatureName "Customer":
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
}
```

### Automatic DI Registration

Generates Program.cs snippet:
```csharp
// Add to Program.cs:
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<CustomerPresenter>();
```

### Plan File Generation

Creates `plans/20251108-customer-implementation-plan.md` with:
- Pre-filled feature name
- Checklist of all files to implement
- Estimated time based on complexity
- Testing strategy

### Progress Tracking

Plan file includes checkboxes:
```markdown
## Implementation Progress

### Phase 1: Model & Repository (0%)
- [ ] Create Customer.cs
- [ ] Create ICustomerRepository.cs
- [ ] Implement CustomerRepository.cs
- [ ] Add CustomerRepositoryTests.cs

### Phase 2: Service Layer (0%)
...
```

---

## 🔗 Dependencies

**Requires**:
- Phase 1 plans directory (for plan file storage)
- Existing templates/ (to generate code from)

**Enables**:
- Phase 4 init script (can integrate scaffolding wizard)

---

## 📝 Notes for Future Implementation

### Context Token Management

Plan templates follow claudekit-engineer best practices:
- **Executive Summary**: Max 3 sentences
- **Context Links**: Reference files, don't duplicate content
- **Tasks**: Max 10 per phase
- **Total**: <200 words for context token budget

### Template Hierarchy

```
plans/templates/
├── _base-plan-template.md        # Common structure
├── form-implementation-plan.md   # Extends base
├── service-implementation-plan.md
└── ...
```

### Wizard Mode (Optional Enhancement)

```powershell
.\scripts\scaffold-feature.ps1 -Wizard

# Interactive prompts:
# "Feature name: " → Customer
# "Pattern (MVP/MVVM): " → MVP
# "Add validation? (Y/n): " → Y
# "Add logging? (Y/n): " → Y
# "Database entity? (Y/n): " → Y
```

---

**Next Phase**: [Phase 4: Enhance init-project.ps1](../phase-4/OVERVIEW.md)

---

**Last Updated**: 2025-11-08
**Status**: High-Level Plan
