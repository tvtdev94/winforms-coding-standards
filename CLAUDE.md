# C# WinForms Coding Standards - Claude Code Guide

> **Project**: C# WinForms Coding Standards and Best Practices Documentation
> **Purpose**: Quick reference guide for AI assistants building WinForms applications
> **Repository Type**: Submodule reference - Link this repo into C# projects for Claude Code integration

---

## 📊 Project Status

**Repository Completion**: **100%** (70/70 files) 🎉
**Last Updated**: 2025-11-18
**Version**: 5.5.0 (ReaLTaiizor Integration!)

### What's New ✨
- ⭐ **NEW! ReaLTaiizor Support**: Free, open-source UI framework with 20+ themes
- ✅ **3 UI Framework Choices**: Standard, DevExpress (commercial), ReaLTaiizor (free)
- ✅ **5 ReaLTaiizor Docs**: Complete guides for Material, Metro, and Poison themes
- ✅ **3 ReaLTaiizor Templates**: MaterialForm, MetroForm, and control patterns

### What's Complete ✅
- ✅ **Documentation** (70/70 files) - 100% complete! 🎉
- ✅ **Templates** (14/14) - Standard, DevExpress, ReaLTaiizor templates
- ✅ **Slash Commands** (19/19) - Complete command suite
- ✅ **AI Agents** (4/4) - WinForms Reviewer, Test Generator, Docs Manager, MVP Validator
- ✅ **Workflows** (5/5) - Development, Testing, Code Review, PR Review, Expert Behavior
- ✅ **Plan Templates** (6/6) - Feature planning templates
- ✅ **Example Project** - Complete Customer Management app with tests

📊 **Stats**: **~48,000+ lines** of documentation | **250+ code examples** | **65+ tests**

---

## 📦 Tech Stack

- **.NET**: 8.0 (recommended) / .NET Framework 4.8
- **Language**: C# 12.0 / C# 10.0
- **UI Framework**: Windows Forms (Standard) **OR** DevExpress **OR** ReaLTaiizor
- **ORM**: Entity Framework Core 8.0
- **Testing**: xUnit / NUnit + Moq
- **DI Container**: Microsoft.Extensions.DependencyInjection
- **Logging**: Serilog / NLog

### UI Framework Options

| Framework | When to Use | Key Benefits |
|-----------|-------------|--------------|
| **ReaLTaiizor Material** ⭐ | **DEFAULT** - Hầu hết apps | Free, modern Material Design, floating labels, MIT license |
| **DevExpress** | Enterprise apps, có license | Advanced controls, responsive design, built-in features ($) |
| **Standard WinForms** | Chỉ khi project yêu cầu | Free, simple, lightweight, legacy compatibility |

> **⚠️ RULE**: Luôn dùng **Material Design (ReaLTaiizor)** làm default. Chỉ dùng framework khác khi:
> - Project đã có `.claude/project-context.md` chỉ định framework khác
> - User explicitly yêu cầu framework khác

---

## 🏗️ Quick Reference

### Project Structure Options

**Two structure options available** (selected during `init-project.ps1`):

#### **Option 1: Single Project (Monolith)** ✅ Recommended for most WinForms apps
```
/ProjectName (Single Project)
├── /Domain/                   # Domain Layer
│   ├── /Models                # Entities
│   ├── /Interfaces            # Contracts
│   ├── /Enums                 # Enumerations
│   └── /Exceptions            # Custom exceptions
├── /Application/              # Application Layer
│   ├── /Services              # Business logic
│   └── /Validators            # Validation rules
├── /Infrastructure/           # Infrastructure Layer
│   └── /Persistence/          # Database
│       ├── /Repositories      # Data access
│       ├── /Context           # DbContext
│       ├── /Configurations    # Entity configs
│       └── /UnitOfWork        # Transaction coordinator
├── /UI/                       # Presentation Layer
│   ├── /Forms                 # WinForms
│   ├── /Presenters            # MVP Presenters
│   ├── /Views                 # View interfaces
│   └── /Factories             # Form factories
└── Program.cs
```
- ✅ **When to use**: Small/medium apps (< 20 forms), 1-3 developers
- ✅ **Benefits**: Simple, fast build, easy to manage, **same structure as Multi-Project**
- ✅ **Architecture**: Folder-based Clean Architecture (convention-enforced)

#### **Option 2: Multi-Project** ⚡ For large/enterprise apps
```
/ProjectName.sln (Multi-Project Solution)
├── ProjectName.UI/             # Presentation Layer (WinForms)
│   ├── /Forms
│   ├── /Presenters
│   └── Program.cs
├── ProjectName.Domain/           # Domain Layer (Models, Interfaces)
│   ├── /Models
│   ├── /Interfaces
│   ├── /Enums
│   └── /Exceptions
├── ProjectName.Application/      # Application Layer (Use Cases, Services)
│   ├── /Services
│   └── /Validators
└── ProjectName.Infrastructure/   # Infrastructure Layer (Data Access, External)
    └── /Persistence/             # Database layer
        ├── /Repositories
        ├── /Context
        ├── /Configurations
        └── /UnitOfWork
```
- ✅ **When to use**: Large apps (20+ forms), 3+ developers, code reuse needed
- ✅ **Benefits**: Compiler-enforced architecture, better separation, reusable layers
- ✅ **Architecture**: Project-based separation (compiler-enforced)

📖 **Full docs**:
- [Single Project Structure](docs/architecture/project-structure.md)
- [Multi-Project Structure](docs/architecture/multi-project-structure.md)

### Architecture Patterns
- **MVP Pattern** ⭐ (recommended) - View + Presenter + Service
- **Factory Pattern** ⭐ (required) - Use `IFormFactory`, NOT `IServiceProvider`
- **Unit of Work** ⭐ (required) - Inject `IUnitOfWork`, NOT `IRepository`
- **Dependency Injection** ⭐ (mandatory)

### Quick Conventions
```
Classes: PascalCase          Methods: PascalCase
Variables: camelCase         Constants: UPPER_SNAKE_CASE
Controls: prefix+PascalCase  (btn, txt, dgv, etc.)
Async methods: MethodNameAsync
```

---

## 🎯 CRITICAL: Project Context

**⚡ ALWAYS READ PROJECT-SPECIFIC CONTEXT FIRST!**

When working on a WinForms project that was initialized with `init-project.ps1`, **BEFORE doing ANY task**, you **MUST**:

1. **Check if `.claude/project-context.md` exists** in the project root
2. **Read it immediately** - This file contains project-specific configuration:
   - UI Framework (Standard / DevExpress / ReaLTaiizor)
   - Target Framework (.NET 8 / .NET 6 / .NET Framework 4.8)
   - Database Provider (SQLite / SQL Server / PostgreSQL / MySQL)
   - Architecture Pattern (MVP / MVVM / Simple)
   - Which templates to use
   - Project-specific DO/DON'T rules

3. **Use the configuration from that file** instead of asking the user

**Example workflow**:
```
User: "Create a CustomerEditForm"

AI:
Step 1: Check if .claude/project-context.md exists
Step 2: Read .claude/project-context.md
Step 3: See "UI Framework: DevExpress"
Step 4: Use templates/dx-form-template.cs (NOT form-template.cs)
Step 5: Create form with DevExpress controls
```

**Why this is critical**:
- ❌ **WITHOUT** project context: AI asks "Which UI framework?" every time
- ✅ **WITH** project context: AI already knows → uses correct templates automatically

**File location**:
- In standards repo: `.claude/project-context-template.md` (template)
- In user project: `.claude/project-context.md` (generated by init-project.ps1)

---

## ⚠️ CRITICAL RULE: Documentation Confirmation Required

**🛑 ALWAYS ASK FOR CONFIRMATION BEFORE CREATING DOCUMENTATION FILES**

Before writing ANY specification, planning document, or feature documentation, you **MUST**:
1. Ask the user for explicit confirmation
2. Wait for a clear "yes" or approval from the user
3. Only proceed after receiving confirmation

**This applies to**:
- Creating new specs in `specs/` folder
- Writing `spec.md`, `tasks.md`, `data-model.md`, or similar planning documents
- Creating feature development documentation
- Writing implementation guides or quickstart documents

**This does NOT apply to**:
- Code files (`.cs`, `.csproj`, etc.)
- Code templates
- Test files
- Configuration files

**Example**:
```
❌ WRONG: "I'll create a spec.md file for this feature..." [proceeds to create file]
✅ CORRECT: "Should I create a spec.md file to document this feature's requirements?"
```

---

## 🚀 AI Assistant Quick Rules

### ✅ ALWAYS DO:
1. **Use Factory Pattern** - Inject `IFormFactory`, NOT `IServiceProvider`
2. **Use Unit of Work** - Inject `IUnitOfWork`, NOT `IRepository`
3. **Call SaveChangesAsync** - In Unit of Work only, NEVER in repositories
4. **Use async/await** - For all I/O operations
5. **Validate input** - Before processing
6. **Handle errors** - Try-catch with logging
7. **Use templates** - Start with templates from `/templates/` folder
8. **Write tests** - Unit tests for services, integration tests for repositories
9. **Follow MVP** - Separate UI from business logic
10. **Dispose resources** - Use `using` statements
11. **Use responsive layout** - Anchor/Dock (Standard), LayoutControl (DevExpress), TableLayoutPanel
12. **Follow Production UI Standards** ⭐ - See [production-ui-standards.md](.claude/guides/production-ui-standards.md)
13. **Write Designer-compatible code** ⭐ - All UI initialization code MUST be in `InitializeComponent()` method, NO helper methods, fully inlined for Visual Studio Designer compatibility

### ❌ NEVER DO:
1. ❌ Business logic in Forms
2. ❌ Inject `IServiceProvider` (use `IFormFactory`)
3. ❌ Call `SaveChangesAsync` in repositories
4. ❌ Inject `IRepository` directly (use `IUnitOfWork`)
5. ❌ Synchronous I/O
6. ❌ Ignore exceptions silently
7. ❌ Magic numbers/strings
8. ❌ Create UI from background threads
9. ❌ Skip validation
10. ❌ Write code without tests
11. ❌ Fixed-size forms without responsive layout
12. ❌ **Student-level UI** - No grid without sort/filter/paging, no low-contrast buttons, no missing loading states
13. ❌ **Separate Label + TextBox** - ALWAYS use Floating Label (see below)
14. ❌ **Helper methods in InitializeComponent()** - Designer cannot parse helper methods like `CreateLabel()`, `CreateButton()`. Must inline ALL code

### 🎯 Input Fields: MUST Use Floating Label

**NEVER create separate Label + TextBox. ALWAYS use Floating Label (Material Design):**

```csharp
// ✅ CORRECT - Floating Label
var txtEmail = new MaterialTextBoxEdit { Hint = "Email Address" };

// ❌ WRONG - Separate Label + TextBox (wastes space, old-school)
var lblEmail = new Label { Text = "Email:" };
var txtEmail = new TextBox();
```

| Framework | Control | Property |
|-----------|---------|----------|
| ReaLTaiizor | `MaterialTextBoxEdit` | `Hint` |
| DevExpress | `TextEdit` | `NullValuePrompt` |
| Standard | Custom `FloatingLabelTextBox` | Implement floating |

📖 **Full rules**: [.claude/guides/ai-instructions.md](.claude/guides/ai-instructions.md)

---

## 🎨 Visual Studio Designer Compatibility

**⚠️ CRITICAL RULE**: All WinForms UI code MUST be written to support Visual Studio Designer view.

### Why This Matters

Visual Studio Designer only parses code inside `InitializeComponent()` method:
- ✅ Designer CAN read: Direct property assignments, `new` statements, control initialization
- ❌ Designer CANNOT read: Helper methods, code in other methods, dynamic code in constructor

### The Problem

```csharp
// ❌ WRONG - Designer cannot display this
private void InitializeComponent()
{
    this.Controls.Add(pnlFilter);
}

private void InitializeControls()  // ← Designer never reads this!
{
    pnlFilter = new Panel { Dock = DockStyle.Top };
    // ... 100 lines of UI code
}
```

**Result**: Designer shows empty form, developer cannot visually edit UI

### The Solution

```csharp
// ✅ CORRECT - Designer can display this
private void InitializeComponent()
{
    this.pnlFilter = new System.Windows.Forms.Panel();
    this.pnlFilter.SuspendLayout();
    this.SuspendLayout();

    // pnlFilter
    this.pnlFilter.BackColor = System.Drawing.Color.White;
    this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
    this.pnlFilter.Location = new System.Drawing.Point(0, 0);
    this.pnlFilter.Name = "pnlFilter";
    this.pnlFilter.Size = new System.Drawing.Size(900, 220);
    this.pnlFilter.TabIndex = 0;

    this.Controls.Add(this.pnlFilter);
    this.pnlFilter.ResumeLayout(false);
    this.ResumeLayout(false);
}
```

**Result**: Designer shows form correctly, developer can click and edit controls visually

### Rules for Designer Compatibility

1. **ALL controls must be initialized in `InitializeComponent()`**
   - No `InitializeControls()`, `CreateUI()`, or similar methods
   - No helper methods like `CreateLabel()`, `CreateButton()`

2. **Inline ALL property assignments**
   ```csharp
   // ❌ WRONG
   var lbl = CreateLabel("Name:");  // Helper method

   // ✅ CORRECT
   this.lblName = new System.Windows.Forms.Label();
   this.lblName.Text = "Name:";
   this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F);
   this.lblName.Location = new System.Drawing.Point(10, 10);
   ```

3. **Use fully qualified type names**
   ```csharp
   // ❌ WRONG
   pnlFilter = new Panel();

   // ✅ CORRECT
   this.pnlFilter = new System.Windows.Forms.Panel();
   ```

4. **Follow Designer pattern structure**
   ```csharp
   private void InitializeComponent()
   {
       // 1. Declare all controls first
       this.pnlFilter = new System.Windows.Forms.Panel();
       this.lblName = new System.Windows.Forms.Label();

       // 2. SuspendLayout
       this.pnlFilter.SuspendLayout();
       this.SuspendLayout();

       // 3. Configure each control (grouped with comments)
       // pnlFilter
       this.pnlFilter.BackColor = ...;
       this.pnlFilter.Dock = ...;

       // lblName
       this.lblName.Text = ...;
       this.lblName.Font = ...;

       // 4. Add controls to containers
       this.pnlFilter.Controls.Add(this.lblName);
       this.Controls.Add(this.pnlFilter);

       // 5. ResumeLayout
       this.pnlFilter.ResumeLayout(false);
       this.ResumeLayout(false);
   }
   ```

5. **Dynamic logic goes in event handlers**
   ```csharp
   // ✅ CORRECT - Initialization in InitializeComponent
   private void InitializeComponent()
   {
       this.cboCategory = new System.Windows.Forms.ComboBox();
       this.cboCategory.Items.AddRange(new object[] {
           "Option 1", "Option 2", "Option 3"
       });
       this.Load += new System.EventHandler(this.ProductForm_Load);
   }

   // ✅ CORRECT - Dynamic logic in Load event
   private void ProductForm_Load(object sender, EventArgs e)
   {
       this.cboCategory.SelectedIndex = 0;
       LoadDataAsync();
   }
   ```

### Trade-offs

| Aspect | Helper Methods | Designer-Compatible |
|--------|---------------|---------------------|
| **Code Length** | Short (~200 lines) | Long (~500+ lines) |
| **Maintainability** | High (DRY) | Medium (repetitive) |
| **Designer Support** | ❌ None | ✅ Full visual editing |
| **Flexibility** | ✅ Very flexible | ⚠️ More rigid |
| **Best For** | Code-only projects | Team projects with designers |

### When to Use Each Approach

**Use Designer-Compatible (InitializeComponent only):**
- ✅ Working in a team with UI designers
- ✅ Need to visually edit forms in Designer
- ✅ Building enterprise apps with many developers
- ✅ Client requires visual form editing capability

**Use Helper Methods (Dynamic code):**
- ✅ Solo developer, code-first approach
- ✅ Complex, programmatic UI generation
- ✅ Rapid prototyping
- ❌ But understand you lose Designer support!

### Default Recommendation

**⭐ DEFAULT**: Always use Designer-compatible code unless project explicitly requires dynamic UI generation.

📖 **See example**: [ProductForm.cs](d:\WORKSPACES\DemoApp\DemoApp\UI\Forms\ProductForm.cs) - Full Designer-compatible implementation

---

## 🧠 Context Loading Map

**⚡ CRITICAL**: This repository is designed to be used as a **Git Submodule** in C# WinForms projects.
When AI (Claude Code) starts working on a task, it should **load the appropriate guide** based on the task type.

📖 **Resource Index**: [.claude/INDEX.md](.claude/INDEX.md) - Quick reference for which resource to read based on task type

### 🔥 STEP 0: Load Context (ALWAYS DO THIS FIRST!)

**Before ANY task**, you **MUST**:

1. **Check `.claude/project-context.md`** - Project-specific config (UI framework, database, patterns)
2. **Check `.claude/INDEX.md`** - Find which guides/docs to read for the task

| Step | File | Purpose |
|------|------|---------|
| 1 | `.claude/project-context.md` | ⭐ Project config (UI framework, database, pattern choices) |
| 2 | `.claude/INDEX.md` | ⭐ Find relevant guides/docs/templates for task |

**Workflow**:
1. Check if `.claude/project-context.md` exists → Read for project config
2. **Read `.claude/INDEX.md`** → Find which resources to read for current task
3. Read only the relevant sections from guides/docs (not entire files)
4. Use appropriate templates

**⚠️ CRITICAL**: NEVER generate code from scratch - ALWAYS start with templates from INDEX.md!

---

## 🔧 Common Commands

```bash
# Build project
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Clean and rebuild
dotnet clean && dotnet build
```

---

## 📚 Full Resource Index

For complete list of all resources (guides, workflows, templates, docs, agents, commands) and task mappings:

📖 **See**: [.claude/INDEX.md](.claude/INDEX.md)

---

## 📞 Need Help?

1. **Resource Index** - [.claude/INDEX.md](.claude/INDEX.md) - Find what to read
2. **Practical examples** - [USAGE_GUIDE.md](USAGE_GUIDE.md)
3. **Full documentation** - [docs/00-overview.md](docs/00-overview.md)
4. **Working example** - [example-project/](example-project/)
5. **Issues?** - [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
