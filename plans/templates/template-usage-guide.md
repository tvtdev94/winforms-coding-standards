# Plan Templates - Usage Guide

**Last Updated**: 2025-11-08
**Version**: 1.0

---

## 📚 Available Templates

This directory contains 5 plan templates to help you structure implementation work:

1. **form-implementation-plan.md** - For creating new Forms with MVP/MVVM
2. **service-implementation-plan.md** - For creating Services
3. **repository-implementation-plan.md** - For creating Repositories
4. **refactoring-plan.md** - For refactoring existing code
5. **testing-plan.md** - For comprehensive testing

---

## 🚀 How to Use These Templates

### Step 1: Choose Template

Pick the template that matches your task:

| Your Task | Template to Use |
|-----------|----------------|
| Creating a new Form | `form-implementation-plan.md` |
| Creating a Service | `service-implementation-plan.md` |
| Creating a Repository | `repository-implementation-plan.md` |
| Refactoring to MVP | `refactoring-plan.md` |
| Adding test coverage | `testing-plan.md` |

### Step 2: Copy Template

Copy the template to `plans/` folder with date prefix:

```bash
# Format: YYYYMMDD-feature-name-plan.md
cp plans/templates/form-implementation-plan.md plans/20251108-customer-form-plan.md
```

### Step 3: Fill in Placeholders

Replace all `{{PLACEHOLDERS}}` with your values:

| Placeholder | Replace With | Example |
|-------------|-------------|---------|
| `{{FORM_NAME}}` | Your form name | CustomerForm |
| `{{SERVICE_NAME}}` | Your service name | CustomerService |
| `{{REPOSITORY_NAME}}` | Your repository name | CustomerRepository |
| `{{MODEL_NAME}}` | Your model name | Customer |
| `{{DATE}}` | Current date | 2025-11-08 |
| `{{HOURS}}` | Estimated hours | 6-8 hours |

### Step 4: Customize

- Fill in Executive Summary
- Add/remove requirements
- Adjust phases as needed
- Add project-specific notes

### Step 5: Track Progress

- Check off items as you complete them
- Update "Progress Tracking" section
- Update "Last Updated" date
- Keep plan file up-to-date

---

## 📋 Template Details

### 1. Form Implementation Plan

**Use when**: Creating a new Form with MVP/MVVM pattern

**What it includes**:
- Requirements (functional + non-functional)
- Architecture diagram (MVP pattern)
- 8-phase implementation checklist
- Files to create (11 files tracked)
- Testing strategy
- Progress tracking

**Estimated time**: 6-8 hours

**Example**:
```markdown
# CustomerForm Implementation Plan
Pattern: MVP
Estimated Time: 6-8 hours
```

---

### 2. Service Implementation Plan

**Use when**: Creating a new Service class

**What it includes**:
- Service method definitions
- Validation rules table
- Error handling strategy
- Unit test coverage plan
- DI registration steps

**Estimated time**: 3-4 hours

**Example**:
```markdown
# CustomerService Implementation Plan
Type: Service Layer
Estimated Time: 3-4 hours
```

---

### 3. Repository Implementation Plan

**Use when**: Creating a new Repository for data access

**What it includes**:
- Entity configuration
- CRUD method definitions
- EF Core setup
- Integration test plan
- DbContext registration

**Estimated time**: 2-3 hours

**Example**:
```markdown
# CustomerRepository Implementation Plan
Type: Data Access Layer
Estimated Time: 2-3 hours
```

---

### 4. Refactoring Plan

**Use when**: Refactoring existing code to better patterns

**What it includes**:
- Current vs target architecture
- Problems and goals
- 7-phase refactoring checklist
- Risk mitigation strategy
- Rollback plan

**Estimated time**: Variable (10-20 hours)

**Example**:
```markdown
# Refactoring Plan: CustomerForm to MVP
Type: Code Refactoring
Estimated Time: 12 hours
```

---

### 5. Testing Plan

**Use when**: Adding comprehensive test coverage

**What it includes**:
- Test coverage goals by layer
- Unit test examples
- Integration test examples
- Manual UI test checklist
- Test execution commands

**Estimated time**: 4-6 hours

**Example**:
```markdown
# Testing Plan: Customer Management
Type: Testing Strategy
Estimated Time: 4-6 hours
```

---

## 💡 Best Practices

### DO:
✅ **Copy template before using** - Never edit original templates
✅ **Fill in all sections** - Even if brief, every section has value
✅ **Update progress regularly** - Keep plan current
✅ **Commit plan with code** - Track plans in git
✅ **Review plan before starting** - Catch issues early
✅ **Share plan with team** - Get feedback

### DON'T:
❌ **Skip sections** - They're there for a reason
❌ **Leave placeholders** - Replace all {{PLACEHOLDERS}}
❌ **Forget to update** - Stale plans are useless
❌ **Over-plan** - Keep it practical, not perfect
❌ **Ignore estimates** - They help with project planning

---

## 📊 Plan Template Workflow

```
1. Choose Template
   ↓
2. Copy to plans/ folder
   ↓
3. Replace {{PLACEHOLDERS}}
   ↓
4. Customize for your needs
   ↓
5. Review & get feedback
   ↓
6. Follow plan during implementation
   ↓
7. Check off items as complete
   ↓
8. Update progress tracking
   ↓
9. Mark as "Complete" when done
```

---

## 🎯 Example: Creating Customer Form

### 1. Copy Template
```bash
cp plans/templates/form-implementation-plan.md \
   plans/20251108-customer-form-plan.md
```

### 2. Replace Placeholders
- `{{FORM_NAME}}` → CustomerForm
- `{{SERVICE_NAME}}` → CustomerService
- `{{REPOSITORY_NAME}}` → CustomerRepository
- `{{MODEL_NAME}}` → Customer
- `{{DATE}}` → 2025-11-08

### 3. Customize
Add specific requirements:
```markdown
### Functional Requirements
- [ ] User can view list of customers
- [ ] User can add new customers
- [ ] User can edit existing customers
- [ ] User can delete customers
- [ ] Email validation required
```

### 4. Track Progress
```markdown
## Progress Tracking
**Overall Progress**: 45%

- Phase 1 (Model & Repository): ✅ 100%
- Phase 2 (Service): ✅ 100%
- Phase 3 (View Interface): ✅ 100%
- Phase 4 (Presenter): ⏳ 50%
- Phase 5 (UI Design): ☐ 0%
...
```

---

## 🔍 Tips & Tricks

### For Complex Features
Use multiple templates:
1. Start with `form-implementation-plan.md`
2. Add `service-implementation-plan.md` for business logic
3. Add `repository-implementation-plan.md` for data access
4. Finish with `testing-plan.md` for comprehensive testing

### For Refactoring
1. Use `refactoring-plan.md` as master plan
2. Create sub-plans for each component
3. Track overall progress in master plan

### For Team Projects
- Share plan early for feedback
- Use checklists for code review
- Track decisions in "Technical Decisions" section
- Update regularly in stand-ups

---

## 📞 Questions?

If templates don't fit your needs:
1. Customize them! These are starting points
2. Create your own template based on these
3. Submit PR with improvements
4. Share feedback

---

**Remember**: Plans are guides, not rules. Adapt as needed!

---

**Created**: 2025-11-08
**Version**: 1.0
**Templates**: 5 available
