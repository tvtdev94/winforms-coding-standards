# 🚀 Quick Start: Integrate Standards into Your Project

> **2-minute setup** to use WinForms Coding Standards in any project

---

## Option 1: New Project (Fastest) ⭐

Create a new project **with standards already integrated**:

```powershell
# Navigate to where you want the new project
cd D:\MyProjects

# Run the enhanced init script
D:\WORKSPACES\winforms-coding-standards\scripts\init-project.ps1 -ProjectName "MyApp"

# Done! Standards are automatically integrated
```

**What you get:**
- ✅ Complete WinForms project with DI, EF Core, Serilog
- ✅ Coding standards as Git submodule at `.standards/`
- ✅ Slash commands available in Claude Code
- ✅ Templates, docs, and examples ready to use

---

## Option 2: Existing Project (Simple)

Add standards to an **existing WinForms project**:

```powershell
# Navigate to your project
cd D:\MyProjects\MyExistingApp

# Run the setup script
D:\WORKSPACES\winforms-coding-standards\scripts\setup-standards.ps1

# Done!
```

**Manual alternative** (if you prefer):

```bash
# Add as submodule
git submodule add https://github.com/yourorg/winforms-standards.git .standards
git submodule update --init --recursive

# Create symlinks (Windows PowerShell as Admin)
New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude"
New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates"
```

---

## ✅ Verify Installation

```powershell
# Check structure
ls .standards          # Should see the standards repo
ls .standards/docs     # Should see documentation
ls .claude/commands    # Should see slash commands (if symlink worked)

# Open in VS Code with Claude Code
# Type "/" to see available commands
```

---

## 🎯 What You Can Do Now

### 1. Use Slash Commands

In Claude Code, type `/` to see:
- `/create-form` - Generate new form with MVP
- `/add-validation` - Add input validation
- `/setup-di` - Setup Dependency Injection
- `/refactor-to-mvp` - Refactor to MVP pattern
- ...and 7 more commands

### 2. Use Templates

```powershell
# Copy templates for quick start
cp templates/form-template.cs src/Forms/CustomerForm.cs
cp templates/service-template.cs src/Services/CustomerService.cs
```

### 3. Read Documentation

- **Practical examples**: `.standards/USAGE_GUIDE.md`
- **AI guide**: `.standards/CLAUDE.md`
- **Full docs**: `.standards/docs/00-overview.md`

### 4. Update Standards

```bash
cd .standards
git pull origin main
cd ..
```

---

## 📖 Full Documentation

For detailed setup options and troubleshooting:
- [INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md) - Complete integration guide
- [USAGE_GUIDE.md](USAGE_GUIDE.md) - Practical examples
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Common issues

---

**Last Updated**: 2025-11-07
