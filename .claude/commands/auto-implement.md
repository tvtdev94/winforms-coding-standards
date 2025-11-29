---
description: Automatically implement a complete feature by orchestrating multiple commands
---

You are an **AI orchestrator** that automatically implements complete features by analyzing requirements and executing the appropriate commands in the correct sequence.

---

## 🔥 STEP 0: MANDATORY Context Loading (DO THIS FIRST!)

**⚠️ CRITICAL: Before ANY analysis or code generation, you MUST load context:**

### 1. Read Project Configuration
```
READ: .claude/project-context.md
```
Extract:
- `UI_FRAMEWORK` → Standard / DevExpress / ReaLTaiizor
- `DATABASE` → SQLite / SQL Server / PostgreSQL
- `PATTERN` → MVP / MVVM
- `FRAMEWORK` → .NET 8 / .NET Framework 4.8

### 2. Load Templates Based on UI Framework

| UI Framework | Form Template | Grid Template | Additional |
|--------------|---------------|---------------|------------|
| **Standard** | `form-template.cs` | N/A | `service-template.cs` |
| **DevExpress** | `dx-form-template.cs` | `dx-grid-template.cs` | `dx-lookup-template.cs` |
| **ReaLTaiizor** | `rt-material-form-template.cs` | N/A | `rt-controls-patterns.cs` |

### 3. Load Required Guides (Read these sections)

**Always load:**
- `docs/patterns/mvp-pattern.md` → MVP implementation rules
- `docs/architecture/dependency-injection.md` → DI registration

**For forms:**
- `.claude/guides/production-ui-standards.md` → UI quality rules
- `docs/ui/responsive-layout.md` → Layout patterns

**For data layer:**
- `docs/data-access/unit-of-work-pattern.md` → UoW pattern
- `docs/data-access/repository-pattern.md` → Repository rules

### 4. Critical Rules Summary (MUST FOLLOW)

```
┌─────────────────────────────────────────────────────────┐
│ 🚫 NEVER DO                     │ ✅ ALWAYS DO          │
├─────────────────────────────────┼───────────────────────┤
│ Inject IServiceProvider         │ Use IFormFactory      │
│ Inject IRepository directly     │ Use IUnitOfWork       │
│ SaveChanges in Repository       │ SaveChanges in UoW    │
│ Business logic in Forms         │ Logic in Presenter    │
│ Separate Label + TextBox        │ Floating Label/Hint   │
│ Generate code without template  │ Start from template   │
│ Skip validation                 │ Validate all inputs   │
│ Ignore async/await              │ Async for all I/O     │
└─────────────────────────────────┴───────────────────────┘
```

### 5. Template Loading Checklist

Before generating ANY code, confirm:
- [ ] Read `project-context.md` ✓
- [ ] Identified UI Framework ✓
- [ ] Loaded correct form template ✓
- [ ] Loaded `service-template.cs` ✓
- [ ] Loaded `repository-template.cs` ✓
- [ ] Loaded `unitofwork-template.cs` ✓

**⚠️ If project-context.md doesn't exist**: Ask user for UI framework preference. Do NOT assume any default.

---

## What This Command Does

This command acts as a **meta-command** that:
1. ✅ Analyzes the user's feature request
2. ✅ Creates an execution plan (list of commands to run)
3. ✅ Shows the plan to user for approval
4. ✅ Executes each command in the correct order
5. ✅ Reports progress and results
6. ✅ Handles dependencies between components

## Workflow

### Step 1: Get Feature Requirements

Ask the user:
```
🎯 What feature would you like to implement?

Examples:
- "CRUD for Customer entity"
- "Product management with inventory tracking"
- "Order processing system"
- "User authentication"
- "Report generation for sales"
```

### Step 2: Analyze Requirements

Based on the user's request, analyze what components are needed:

#### Common Feature Patterns:

**Pattern 1: CRUD Feature (Most Common)**
User says: "CRUD for [Entity]"

Components needed:
- ✅ Entity model (if not exists)
- ✅ Repository (data access)
- ✅ Service (business logic)
- ✅ List Form (view/search/delete)
- ✅ Edit Form (create/update)
- ✅ Validation
- ✅ Unit tests

**Pattern 2: Report/View-Only Feature**
User says: "Report for [Data]" or "Dashboard for [Metrics]"

Components needed:
- ✅ Service (to get data)
- ✅ Report Form (display)
- ✅ Export functionality (optional)

**Pattern 3: Process/Workflow Feature**
User says: "Process [Something]" or "Workflow for [Task]"

Components needed:
- ✅ Multiple services (for each step)
- ✅ Workflow Form (wizard or steps)
- ✅ Progress dialogs
- ✅ Validation at each step

**Pattern 4: Authentication/Security**
User says: "Login system" or "User authentication"

Components needed:
- ✅ User entity
- ✅ Authentication service
- ✅ Login form/dialog
- ✅ Password hashing
- ✅ Session management

### Step 3: Create Execution Plan

Generate a detailed execution plan and show to user:

```markdown
📋 EXECUTION PLAN for "{Feature Name}"

🔍 Analysis:
- Feature type: {CRUD/Report/Workflow/Auth}
- Main entity: {EntityName}
- Complexity: {Low/Medium/High}

📝 Components to Create:
┌─────────────────────────────────────────────────────┐
│ 1. Entity Model                                     │
│    ├─ Create {Entity}.cs in /Models                │
│    └─ Add properties based on requirements         │
├─────────────────────────────────────────────────────┤
│ 2. Data Layer (Repository)                         │
│    ├─ Run: /create-repository                      │
│    ├─ Input: Entity = {Entity}                     │
│    ├─ Creates: I{Entity}Repository.cs              │
│    ├─ Creates: {Entity}Repository.cs               │
│    └─ Updates: AppDbContext.cs (DbSet)             │
├─────────────────────────────────────────────────────┤
│ 3. Business Layer (Service)                        │
│    ├─ Run: /create-service                         │
│    ├─ Input: Entity = {Entity}                     │
│    ├─ Creates: I{Entity}Service.cs                 │
│    └─ Creates: {Entity}Service.cs                  │
├─────────────────────────────────────────────────────┤
│ 4. UI Layer (Forms)                                │
│    ├─ Run: /create-form (List)                     │
│    │  ├─ Creates: {Entity}ListForm.cs              │
│    │  ├─ Creates: I{Entity}ListView.cs             │
│    │  └─ Creates: {Entity}ListPresenter.cs         │
│    ├─ Run: /create-form (Edit)                     │
│    │  ├─ Creates: {Entity}EditForm.cs              │
│    │  ├─ Creates: I{Entity}EditView.cs             │
│    │  └─ Creates: {Entity}EditPresenter.cs         │
│    └─ Run: /create-dialog (Delete Confirmation)    │
├─────────────────────────────────────────────────────┤
│ 5. Validation                                       │
│    ├─ Run: /add-validation                         │
│    └─ Apply to: {Entity}EditForm                   │
├─────────────────────────────────────────────────────┤
│ 6. Testing                                          │
│    ├─ Run: /add-test                               │
│    ├─ Test: {Entity}Service                        │
│    └─ Test: {Entity}Repository (integration)       │
├─────────────────────────────────────────────────────┤
│ 7. DI Registration                                  │
│    ├─ Update Program.cs                            │
│    ├─ Register: I{Entity}Repository                │
│    ├─ Register: I{Entity}Service                   │
│    └─ Register: Forms                              │
└─────────────────────────────────────────────────────┘

⏱️  Estimated Time: {X} minutes
📁 Files to Create: {N} files
🔧 Commands to Run: {M} commands

❓ Proceed with this plan? (yes/no/customize)
```

### Step 4: Get User Confirmation

Wait for user response:
- **"yes"** or **"y"** → Execute the plan
- **"no"** or **"n"** → Cancel
- **"customize"** → Let user modify the plan

If user wants to customize, ask:
```
Which steps would you like to modify?
- Remove steps? (enter numbers, e.g., "5,6")
- Add steps? (describe what to add)
- Change order? (e.g., "move 6 before 5")
```

### Step 5: Execute Plan

For each step in the plan:

1. **Show Current Step**
   ```
   ⏳ Step {N}/{Total}: {Step Description}
   Running: {Command}
   ```

2. **Execute Command**
   - If it's a `/create-*` or `/add-*` command, run it
   - Pass the necessary parameters automatically
   - Capture the output

3. **Show Result**
   ```
   ✅ Step {N} completed
   Created: {list of files}
   ```

4. **Handle Errors**
   ```
   ❌ Step {N} failed: {error}

   Options:
   - Retry this step
   - Skip this step
   - Abort entire process
   ```

5. **Continue to Next Step**

### Step 6: Final Summary

After all steps complete:

```
🎉 IMPLEMENTATION COMPLETE!

✅ Summary:
- Entity: {Entity} created
- Repository: {Entity}Repository created
- Service: {Entity}Service created
- Forms: {List} and {Edit} forms created
- Validation: Added to edit form
- Tests: {N} test files created
- DI: All components registered

📁 Files Created ({N} total):
/Models/{Entity}.cs
/Repositories/I{Entity}Repository.cs
/Repositories/{Entity}Repository.cs
/Services/I{Entity}Service.cs
/Services/{Entity}Service.cs
/Forms/{Entity}ListForm.cs
/Forms/{Entity}EditForm.cs
/Views/I{Entity}ListView.cs
/Views/I{Entity}EditView.cs
/Presenters/{Entity}ListPresenter.cs
/Presenters/{Entity}EditPresenter.cs
/Tests/{Entity}ServiceTests.cs
/Tests/{Entity}RepositoryTests.cs

📝 Next Steps:
1. Review generated code
2. Customize business logic in service
3. Run tests: dotnet test
4. Run application and test UI
5. Commit changes

🚀 Feature "{Feature Name}" is ready to use!
```

## Example Usage

### Example 1: CRUD for Customer

**User Input:**
```
/auto-implement
```

**Assistant asks:**
```
What feature would you like to implement?
```

**User:**
```
CRUD for Customer entity with Name, Email, Phone, and Address
```

**Assistant analyzes and creates plan:**
```
📋 EXECUTION PLAN for "Customer CRUD"

🔍 Analysis:
- Feature type: CRUD
- Main entity: Customer
- Fields: Name, Email, Phone, Address
- Complexity: Medium

📝 Components to Create:
1. Customer entity model
2. /create-repository → CustomerRepository (with SearchByName, SearchByEmail)
3. /create-service → CustomerService (CRUD + validation)
4. /create-form → CustomerListForm (DataGridView with search)
5. /create-form → CustomerEditForm (Input fields with validation)
6. /add-validation → Email format, required fields
7. /add-test → CustomerServiceTests (10+ tests)

⏱️  Estimated Time: 8-10 minutes
📁 Files to Create: 13 files
🔧 Commands to Run: 5 commands

❓ Proceed with this plan?
```

**User:** `yes`

**Assistant executes:**
```
⏳ Step 1/7: Creating Customer entity model
✅ Created /Models/Customer.cs

⏳ Step 2/7: Creating repository
Running: /create-repository
✅ Created I Repository.cs, ICustomerRepository.cs, CustomerRepository.cs
✅ Updated AppDbContext.cs

⏳ Step 3/7: Creating service
Running: /create-service
✅ Created ICustomerService.cs, CustomerService.cs

⏳ Step 4/7: Creating list form
Running: /create-form
✅ Created CustomerListForm.cs, ICustomerListView.cs, CustomerListPresenter.cs

⏳ Step 5/7: Creating edit form
Running: /create-form
✅ Created CustomerEditForm.cs, ICustomerEditView.cs, CustomerEditPresenter.cs

⏳ Step 6/7: Adding validation
Running: /add-validation
✅ Added validation to CustomerEditForm

⏳ Step 7/7: Generating tests
Running: /add-test
✅ Created CustomerServiceTests.cs (15 tests)
✅ Created CustomerRepositoryTests.cs (8 tests)

🎉 IMPLEMENTATION COMPLETE!
```

### Example 2: Report Feature

**User:**
```
Monthly sales report with charts
```

**Plan:**
```
1. Create SalesReport model
2. /create-service → SalesReportService (GetMonthlySales, GenerateReport)
3. /create-form → SalesReportForm (with chart controls)
4. /add-settings → Add report parameters to appsettings.json
```

### Example 3: Login System

**User:**
```
User login with remember me feature
```

**Plan:**
```
1. Create User entity
2. /create-repository → UserRepository (FindByUsername)
3. /create-service → AuthenticationService (Login, Logout, ValidatePassword)
4. /create-dialog → LoginDialog (username, password, remember me)
5. /add-logging → Log authentication attempts
6. /add-settings → Add JWT/session config
7. /add-test → AuthenticationServiceTests
```

## Implementation Strategy

When executing this command:

### 1. Feature Analysis

```csharp
// Pseudo-code for analysis
var featureType = DetermineFeatureType(userRequest);
var entities = ExtractEntities(userRequest);
var operations = ExtractOperations(userRequest);
var components = MapToComponents(featureType, entities, operations);
```

### 2. Dependency Resolution

Ensure correct order:
```
Entity → Repository → Service → Forms → Validation → Tests → DI
```

Never create a service before its repository!
Never create a form before its service!

### 3. Parameter Passing

When calling sub-commands, pass parameters automatically:

```
/create-repository
  → Entity: {extracted from request}
  → Custom queries: {inferred from operations}

/create-service
  → Entity: {same as repository}
  → Operations: {CRUD + custom from request}

/create-form
  → Entity: {same as above}
  → Form type: {List/Edit/Dialog}
  → Service: {created in previous step}
```

### 4. Progress Tracking

Keep state of execution:
```
ExecutionState {
  TotalSteps: 7,
  CurrentStep: 3,
  CompletedSteps: [1, 2],
  FailedSteps: [],
  FilesCreated: [...],
  ComponentsRegistered: [...]
}
```

### 5. Rollback on Failure

If a step fails, offer to:
- Retry
- Skip (if non-critical)
- Rollback (delete created files)
- Abort

## Feature Type Detection

Use keywords to determine feature type:

### CRUD Keywords
- "CRUD", "manage", "administer", "edit", "create", "update", "delete"
- "customer management", "product catalog", "employee database"

### Report Keywords
- "report", "dashboard", "chart", "graph", "summary", "statistics"
- "monthly report", "sales dashboard", "analytics"

### Workflow Keywords
- "process", "workflow", "wizard", "step-by-step", "approval"
- "order processing", "invoice workflow", "approval system"

### Auth Keywords
- "login", "authentication", "sign in", "user management", "security"
- "login system", "user authentication", "access control"

## Best Practices

### DO ✅
- Always show execution plan before executing
- Ask for confirmation
- Show progress for each step
- Handle errors gracefully
- Create complete, working features
- Follow all coding standards
- Register everything in DI
- Generate tests

### DON'T ❌
- Don't execute without user approval
- Don't skip error handling
- Don't create incomplete features
- Don't ignore dependencies
- Don't create code without tests
- Don't forget DI registration

## Advanced Features

### 1. Template Selection

Ask user which template/pattern to use:
```
Which architecture pattern?
1. MVP (recommended)
2. MVVM (.NET 8+)
3. Code-behind (simple)
```

### 2. Database Choice

```
Which database?
1. SQLite (recommended for desktop)
2. SQL Server
3. PostgreSQL
```

### 3. Additional Features

```
Include additional features?
□ Logging (Serilog)
□ Configuration (appsettings.json)
□ Export to Excel
□ Print functionality
□ Advanced search/filtering
```

### 4. Test Coverage Level

```
Test coverage level?
1. Basic (happy path only)
2. Standard (happy path + errors)
3. Comprehensive (all edge cases)
```

## Error Handling

### If Repository Creation Fails
```
❌ Repository creation failed: DbContext not found

Suggested fix:
Would you like me to:
1. Create DbContext first (/create-dbcontext)
2. Skip repository and use ADO.NET directly
3. Abort
```

### If Service Creation Fails
```
❌ Service creation failed: ILogger not configured

Suggested fix:
Would you like me to:
1. Setup logging first (/add-logging)
2. Create service without logging
3. Abort
```

## Performance Considerations

- Show progress to keep user informed
- Don't wait for user input between automated steps
- Only pause for important decisions
- Execute steps in parallel when possible (e.g., tests can run while user reviews code)

## Notes

- **This is a meta-command** - it orchestrates other commands
- **User maintains control** - always show plan and ask confirmation
- **Intelligent analysis** - infer what's needed from user description
- **Proper ordering** - respect dependencies between components
- **Complete features** - create everything needed for a working feature
- **Best practices** - follow all coding standards automatically
- **Time saver** - replaces 5-10 manual command runs with 1 automated flow

## Command Signature

```bash
/auto-implement [optional: feature description]

# Interactive mode (recommended)
/auto-implement

# Direct mode (skip first question)
/auto-implement "CRUD for Customer entity"
```

## Success Metrics

After running this command, user should have:
- ✅ Complete working feature
- ✅ All layers implemented (data, business, UI)
- ✅ Proper separation of concerns
- ✅ Validation and error handling
- ✅ Unit tests with good coverage
- ✅ Everything registered in DI
- ✅ Ready to run and test immediately

## Limitations

- Can't handle extremely complex custom requirements (use manual commands)
- Assumes standard architecture patterns (MVP/MVVM)
- Works best for common feature types (CRUD, reports, workflows)
- May need manual adjustments for very specific business logic

## When to Use Manual Commands Instead

Use individual commands when:
- You need fine-grained control
- You're creating something non-standard
- You want to understand each step
- You're learning the architecture

Use `/auto-implement` when:
- You want to quickly scaffold a feature
- You're implementing standard patterns
- You trust the automated workflow
- You want to save time
