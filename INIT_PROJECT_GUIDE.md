# 🚀 Init Project Script - Complete Guide

Quick guide for using `init-project.ps1` to create WinForms projects with coding standards integrated.

---

## ⚡ Quick Start

### **Option 1: With Administrator Rights (Recommended)**

```powershell
# Run PowerShell as Administrator
# Right-click PowerShell -> Run as Administrator

cd D:\WORKSPACES
.\winforms-coding-standards\scripts\init-project.ps1

# Select:
# - Integrate standards: Y
# Result: ✅ Symlinks created (auto-updating slash commands)
```

### **Option 2: Without Administrator Rights**

```powershell
# Run normal PowerShell

cd D:\WORKSPACES
.\winforms-coding-standards\scripts\init-project.ps1

# Select:
# - Integrate standards: Y
# Result: ✅ Files copied (manual update needed)
```

---

## 🔍 What Gets Created

### **With Admin Rights (Symlinks):**

```
YourProject/
├── .standards/              # Git Submodule
│   ├── .claude/
│   │   ├── commands/        # 19 slash commands
│   │   ├── agents/          # 4 AI agents
│   │   └── guides/          # Documentation
│   └── templates/           # Code templates
│
├── .claude/                 # ⭐ Symlink -> .standards/.claude/
│   └── commands/            # Slash commands visible to Claude Code!
│
├── templates/               # ⭐ Symlink -> .standards/templates/
│
└── [Your project files...]
```

**Benefits:**
- ✅ Slash commands available immediately
- ✅ Auto-update when you `git pull` in `.standards/`
- ✅ No manual copying needed

---

### **Without Admin Rights (Copied Files):**

```
YourProject/
├── .standards/              # Git Submodule
│   ├── .claude/
│   └── templates/
│
├── .claude/                 # ⭐ Copied from .standards/.claude/
│   └── commands/            # Slash commands available!
│
├── templates/               # ⭐ Copied from .standards/templates/
│
└── [Your project files...]
```

**Benefits:**
- ✅ Slash commands available
- ⚠️ Need manual update: `Copy-Item .standards\.claude .claude -Recurse -Force`

---

## 📋 Script Features

### **1. Auto-Detects Admin Rights**

```powershell
# Script automatically checks:
if (IsAdmin) {
    # Create symlinks (auto-updating)
} else {
    # Copy files (manual update)
    # Show tip about running as Admin
}
```

### **2. Correct EF Core Versions**

Fixed in v5.5.1! No more NU1202 errors.

| .NET Version | EF Core Version |
|--------------|-----------------|
| .NET 8.0     | 8.0.11          |
| .NET 6.0     | 6.0.36          |
| .NET Framework 4.8 | 6.0.36    |

### **3. UI Framework Support**

- **Standard WinForms** - Free, default controls
- **DevExpress** - Commercial, professional UI
- **ReaLTaiizor** - Free, modern themes (Material/Metro)

### **4. Claude Code Integration**

After script completes:

```
[Claude Code Integration]
  [OK] Slash commands available! Type / to see all commands
```

### **5. Project Context (NEW in v2.0!)** 🎉

Script creates `.claude/project-context.md` with your configuration:

```markdown
# Project Context

## Configuration

### UI Framework: DevExpress  ← Your choice from Step 4!
### Framework: net8.0          ← Your choice from Step 2!
### Database: SQLite            ← Your choice from Step 3!
### Pattern: MVP                ← Your choice from Step 5!

## AI Instructions
- Use DevExpress templates
- Use SQLite for database
- Follow MVP pattern
```

**Why this matters:**
- ✅ AI reads this file automatically
- ✅ AI knows which UI framework to use
- ✅ AI uses correct templates
- ✅ NO MORE "Which UI framework?" questions! 🎯

**Example:**
```
User: /auto-implement "CRUD for Customer"
AI: Reads .claude/project-context.md
AI: Sees "UI Framework: DevExpress"
AI: Uses DevExpress templates automatically! ← No questions asked! ✅
```

---

## 🎯 Usage Examples

### **Example 1: Simple SQLite App with ReaLTaiizor**

```powershell
# Run as Admin
.\scripts\init-project.ps1

# Answers:
# 1. Project Name: CustomerApp
# 2. Framework: [1] .NET 8.0
# 3. Database: [1] SQLite
# 4. UI Framework: [3] ReaLTaiizor
# 5. Pattern: [1] MVP
# 6. Tests: Y
# 7. Example Code: n
# 8. Standards: Y

# Result:
# ✅ Symlinks created
# ✅ 19 slash commands available
# ✅ Ready to use /auto-implement
```

---

### **Example 2: Enterprise SQL Server App**

```powershell
.\scripts\init-project.ps1

# Answers:
# 1. Project Name: EnterpriseERP
# 2. Framework: [1] .NET 8.0
# 3. Database: [2] SQL Server
# 4. UI Framework: [2] DevExpress
# 5. Pattern: [1] MVP
# 6. Tests: Y
# 7. Example Code: n
# 8. Standards: Y
```

---

## 🔧 Troubleshooting

### **Issue 1: Slash commands not showing**

**Check:**
```powershell
cd YourProject
ls .claude
# Should see: commands/, agents/, guides/, workflows/

ls .claude/commands
# Should see 19 .md files
```

**Fix:**
```powershell
# If .claude doesn't exist:
Copy-Item -Recurse .standards\.claude -Destination .claude

# Reload VS Code / Claude Code
```

---

### **Issue 2: "Not running as Administrator" warning**

**To get symlinks:**
```powershell
# Close current PowerShell
# Right-click PowerShell icon -> Run as Administrator
# Re-run script (delete old project first if needed)
```

**Or keep using copied files:**
- Slash commands still work
- Update manually when standards change

---

### **Issue 3: EF Core version errors (NU1202)**

**Fixed in latest version!**

If you still get errors:
```powershell
# Check script version
git pull  # Update standards repo

# Or manually fix:
dotnet remove package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.11
```

---

## 📊 Comparison: Symlink vs Copy

| Feature | Symlink (Admin) | Copy (No Admin) |
|---------|-----------------|-----------------|
| Slash commands | ✅ Available | ✅ Available |
| Auto-update | ✅ Yes | ❌ Manual |
| Setup time | Same | Same |
| Maintenance | ✅ Easy | ⚠️ Manual |
| Recommended | ✅ Yes | If no Admin |

---

## 🎓 Next Steps After Init

### **1. Open in Claude Code**

```powershell
cd YourProject
code .  # Open in VS Code

# In Claude Code chat:
Type /
# You'll see all 19 slash commands!
```

### **2. Create Your First Feature**

```
# In Claude Code:
/auto-implement "CRUD for Customer with Name, Email, Phone"

# Wait for plan... approve it...
# ✅ Complete feature created in ~8 minutes!
```

### **3. Run & Test**

```powershell
dotnet build
dotnet test
dotnet run --project YourProject
```

---

## 🔄 Updating Standards

### **With Symlinks (Auto):**

```powershell
cd YourProject/.standards
git pull
cd ../..

# ✅ Slash commands auto-updated!
# No need to copy anything
```

### **With Copied Files (Manual):**

```powershell
cd YourProject/.standards
git pull
cd ..

# Copy updated files
Copy-Item -Recurse .standards\.claude -Destination .claude -Force
Copy-Item -Recurse .standards\templates -Destination templates -Force
```

---

## 📝 Summary

**What the script does:**
1. ✅ Creates WinForms project structure
2. ✅ Installs correct NuGet packages (with proper versions!)
3. ✅ Sets up DI, logging, configuration
4. ✅ Adds coding standards as Git Submodule
5. ✅ Creates symlinks (Admin) or copies files (No Admin)
6. ✅ Makes 19 slash commands available to Claude Code
7. ✅ Includes `/auto-implement` for rapid development

**You get:**
- Production-ready project structure
- Best practices built-in
- 19 powerful slash commands
- Auto-implement feature creation
- Complete documentation

---

**Happy coding!** 🎉

---

## 📋 Version History

### **v2.0 (2025-11-18)** - Project Context Update 🎉
- ✅ **NEW**: Creates `.claude/project-context.md` automatically
- ✅ **NEW**: AI reads project config (UI framework, database, pattern)
- ✅ **NEW**: No more repeated "Which UI framework?" questions
- ✅ **FIX**: AI uses correct templates based on init choices

**What changed:**
- Added Step 12: "Creating project context for AI assistants"
- AI now knows project configuration without asking
- `/auto-implement` and other commands use project context

### **v1.0 (2025-11-08)** - Initial Release
- ✅ Basic project initialization
- ✅ Git Submodule integration
- ✅ Symlink/copy support
- ✅ 19 slash commands
- ❌ AI asked "Which UI framework?" every time (fixed in v2.0)

---

For more details, see:
- [USAGE_GUIDE.md](USAGE_GUIDE.md) - Practical examples
- [CLAUDE.md](CLAUDE.md) - AI assistant guide
- [INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md) - Git Submodule details
