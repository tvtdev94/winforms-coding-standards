# 🔗 Integration Guide: Using WinForms Coding Standards in Your Projects

> How to integrate this coding standards repository into your WinForms projects using Git Submodule

---

## 🎯 Overview

This guide shows you how to **integrate coding standards into ANY WinForms project** (new or existing) so that:

✅ Claude Code can see all slash commands
✅ You have access to templates and documentation
✅ Standards stay synced across all your projects
✅ Your team gets the same standards automatically

---

## 🚀 Quick Start (TL;DR)

```powershell
# Windows (PowerShell)
cd D:\MyProjects\MyWinFormsApp
curl -o setup-standards.ps1 https://raw.githubusercontent.com/yourorg/winforms-standards/main/scripts/setup-standards.ps1
.\setup-standards.ps1

# Linux/Mac (Bash)
cd ~/Projects/MyWinFormsApp
curl -o setup-standards.sh https://raw.githubusercontent.com/yourorg/winforms-standards/main/scripts/setup-standards.sh
chmod +x setup-standards.sh
./setup-standards.sh
```

**Done!** 🎉 Your project now has access to all standards, commands, and templates.

---

## 📋 Prerequisites

- ✅ **Git** installed ([download](https://git-scm.com/downloads))
- ✅ Your project is a **Git repository** (or willing to initialize one)
- ✅ **(Windows only)** Administrator privileges for creating symlinks (optional)

---

## 🛠️ Method 1: Automated Setup (Recommended)

### Step 1: Download Setup Script

**Option A: Clone this repository first** (recommended)

```powershell
# Clone standards repo to a central location
cd D:\WORKSPACES
git clone https://github.com/yourorg/winforms-coding-standards.git

# Now use the script for any project
cd D:\MyProjects\MyApp
D:\WORKSPACES\winforms-coding-standards\scripts\setup-standards.ps1
```

**Option B: Download script directly**

```powershell
# Windows
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/yourorg/winforms-standards/main/scripts/setup-standards.ps1" -OutFile "setup-standards.ps1"
.\setup-standards.ps1

# Linux/Mac
curl -o setup-standards.sh https://raw.githubusercontent.com/yourorg/winforms-standards/main/scripts/setup-standards.sh
chmod +x setup-standards.sh
./setup-standards.sh
```

### Step 2: What the Script Does

The script automatically:

1. ✅ Adds standards as **Git submodule** at `.standards/`
2. ✅ Creates **symlinks** for `.claude/` and `templates/` (Windows: needs Admin)
3. ✅ Configures `.clauderc` for Claude Code integration
4. ✅ Updates `.gitignore` if needed

### Step 3: Verify Installation

```powershell
# Check structure
ls .standards          # Should see the standards repo
ls .claude/commands    # Should see slash commands (if symlink worked)

# Test with Claude Code
# Open project in VS Code with Claude Code extension
# Type "/" to see available commands
```

---

## 🔧 Method 2: Manual Setup

If you prefer manual control:

### Step 1: Add Git Submodule

```bash
cd /path/to/your/project

# Add standards as submodule
git submodule add https://github.com/yourorg/winforms-coding-standards.git .standards

# Initialize and update
git submodule update --init --recursive
```

### Step 2: Create Symlinks (Optional but Recommended)

**Windows (PowerShell as Administrator):**

```powershell
# Symlink Claude commands
New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude"

# Symlink templates
New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates"
```

**Linux/Mac:**

```bash
# Symlink Claude commands
ln -s .standards/.claude .claude

# Symlink templates
ln -s .standards/templates templates
```

### Step 3: Configure Claude Code (Optional)

Create `.clauderc`:

```bash
# Claude Code Configuration
# Load coding standards from submodule
```

Update `.gitignore`:

```
# Claude Code config (optional)
.clauderc
```

### Step 4: Commit Changes

```bash
git add .gitmodules .standards .gitignore
git commit -m "Add WinForms coding standards as submodule"
```

---

## 📁 Result: Project Structure

After integration, your project will look like:

```
/MyWinFormsApp
  ├── .git/
  ├── .gitmodules              # Submodule configuration
  ├── .gitignore
  ├── .clauderc                # Claude Code config
  │
  ├── .standards/              # ⭐ Standards submodule
  │   ├── .claude/
  │   ├── docs/
  │   ├── templates/
  │   ├── CLAUDE.md
  │   ├── README.md
  │   └── USAGE_GUIDE.md
  │
  ├── .claude/                 # ⭐ Symlink → .standards/.claude/
  │   └── commands/            # Slash commands visible to Claude Code
  │
  ├── templates/               # ⭐ Symlink → .standards/templates/
  │   ├── form-template.cs
  │   ├── service-template.cs
  │   └── ...
  │
  ├── /src                     # Your application code
  ├── /tests
  └── MyApp.sln
```

---

## 🎓 Using Standards in Your Project

### 1. Access Documentation

```powershell
# Read the guides
.standards/README.md           # Overview
.standards/USAGE_GUIDE.md      # Practical examples
.standards/CLAUDE.md           # AI assistant guide

# Browse full docs
.standards/docs/architecture/
.standards/docs/best-practices/
.standards/docs/ui-ux/
```

### 2. Use Slash Commands (Claude Code)

Open your project in VS Code with Claude Code extension:

```
Type "/" to see all available commands:

/create-form          - Generate new form with MVP pattern
/add-validation       - Add input validation to form
/add-data-binding     - Setup data binding for controls
/fix-threading        - Fix cross-thread UI issues
/refactor-to-mvp      - Refactor code to MVP pattern
/setup-di             - Setup Dependency Injection
/add-error-handling   - Add comprehensive error handling
/optimize-performance - Optimize WinForms performance
/review-code          - Review code against standards
/check-standards      - Quick compliance check
/add-test             - Generate unit tests
```

### 3. Use Code Templates

```powershell
# Copy templates to your project
cp templates/form-template.cs src/Forms/MyNewForm.cs
cp templates/service-template.cs src/Services/MyService.cs
cp templates/repository-template.cs src/Repositories/MyRepository.cs
cp templates/test-template.cs tests/MyServiceTests.cs

# Edit and adapt to your needs
```

### 4. Follow Coding Standards

When writing code, reference:

- **Architecture**: `.standards/docs/architecture/mvp-pattern.md`
- **Naming**: `.standards/docs/conventions/naming-conventions.md`
- **Best Practices**: `.standards/docs/best-practices/`
- **Examples**: `.standards/docs/examples/`

---

## 🔄 Updating Standards

### Keep Standards Up-to-Date

```bash
# Update to latest standards
cd .standards
git pull origin main
cd ..

# Commit the update
git add .standards
git commit -m "Update coding standards to latest version"
```

### Check Current Version

```bash
cd .standards
git log -1 --oneline
cd ..
```

---

## 👥 Team Collaboration

### For Team Members Cloning Your Project

When someone clones your project:

```bash
# Clone project
git clone https://github.com/yourorg/MyWinFormsApp.git
cd MyWinFormsApp

# Initialize submodules
git submodule update --init --recursive

# Recreate symlinks (Windows - as Admin, optional)
New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude"
New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates"

# Or use the setup script
.standards/scripts/setup-standards.ps1
```

**Pro Tip:** Add this to your project's README:

```markdown
## Setup

git clone https://github.com/yourorg/MyWinFormsApp.git
cd MyWinFormsApp
git submodule update --init --recursive
```

---

## 🔍 Troubleshooting

### Issue: Symlinks Not Working on Windows

**Cause:** Insufficient permissions

**Solution:**
1. Run PowerShell as **Administrator**
2. Run setup script again, OR
3. Access standards directly: `.standards/.claude/commands/`

### Issue: Claude Code Doesn't See Commands

**Cause:** Symlink not created or Claude Code not detecting `.claude/`

**Solution:**

**Quick Fix (Windows):**
```powershell
# Run PowerShell as Administrator, then:
cd path\to\your\project
.\scripts\fix-symlinks.ps1
```

**Manual Fix (Windows):**
```powershell
# In PowerShell as Administrator:
cd MyApp
New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude"
New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates"
```

**Manual Fix (Linux/Mac):**
```bash
cd MyApp
ln -s .standards/.claude .claude
ln -s .standards/templates templates
```

**Verification:**
1. Check if `.claude/commands/` exists
2. Restart VS Code / Claude Code
3. Type `/` to see available commands

**Alternative (No Symlink):**
Access commands directly from `.standards/.claude/commands/` in Claude Code

### Issue: Submodule Not Updating

**Cause:** Forgot to pull submodule changes

**Solution:**
```bash
git submodule update --remote --merge
```

### Issue: "Submodule already exists" Error

**Cause:** Trying to add submodule that's already there

**Solution:**
```bash
# Remove existing submodule
git submodule deinit -f .standards
git rm -f .standards
rm -rf .git/modules/.standards

# Add again
git submodule add <repo-url> .standards
```

---

## 🆚 Comparison: Different Integration Methods

| Method | Portable | Auto-Update | Easy Setup | Team-Friendly |
|--------|----------|-------------|------------|---------------|
| **Git Submodule** (Recommended) | ✅ Yes | ✅ Yes | ✅ Easy | ✅ Yes |
| **Copy Files** | ✅ Yes | ❌ Manual | ✅ Easiest | ⚠️ Out of sync |
| **Symlink Only** | ❌ No | ✅ Yes | ⚠️ Medium | ❌ No |
| **NuGet Package** | ✅ Yes | ✅ Yes | ⚠️ Complex | ✅ Yes |

**Verdict:** Git Submodule is the **best balance** of all factors.

---

## 📝 Best Practices

### DO ✅

- ✅ Commit `.gitmodules` and `.standards/` to your repo
- ✅ Update standards regularly (`git submodule update --remote`)
- ✅ Document setup steps in your project's README
- ✅ Use symlinks for convenience (if possible)
- ✅ Test setup on fresh clone before sharing with team

### DON'T ❌

- ❌ Modify files inside `.standards/` directly (changes will be lost)
- ❌ Commit symlinks to Git (they're machine-specific)
- ❌ Forget to run `git submodule update --init` after clone
- ❌ Mix standards versions across team members

---

## 🎯 Next Steps

After integration:

1. **Read** `.standards/USAGE_GUIDE.md` for practical examples
2. **Review** your existing code with `/review-code` command
3. **Refactor** to follow standards using templates
4. **Train** team on slash commands and standards
5. **Share** this guide with team members

---

## 📞 Support

**Having issues?**

1. Check [TROUBLESHOOTING.md](.standards/TROUBLESHOOTING.md)
2. Review this guide's troubleshooting section
3. Open an issue on the standards repository
4. Ask Claude Code: "Help me setup WinForms coding standards"

---

## 📄 Additional Resources

- **Standards Documentation**: `.standards/docs/00-overview.md`
- **Usage Examples**: `.standards/USAGE_GUIDE.md`
- **Claude Code Guide**: `.standards/CLAUDE.md`
- **Example Project**: `.standards/example-project/`
- **Pre-commit Hooks**: `.standards/.githooks/`
- **Code Snippets**: `.standards/snippets/`

---

**Last Updated**: 2025-11-07
**Version**: 1.0
**Status**: ✅ Ready for production use
