# Phase 4: Enhance init-project.ps1

**Status**: 📋 High-Level Plan
**Duration**: 1 day
**Priority**: 🟡 High
**Prerequisites**: Phases 1, 2, 3 complete

---

## 🎯 Goals

Enhance the existing `init-project.ps1` script to automatically set up a complete WinForms project with:

1. **Auto-install agents** from Phase 2
2. **Auto-install workflows** from Phase 1
3. **Auto-install plan templates** from Phase 3
4. **Integrate scaffolding wizard** from Phase 3
5. **One-command setup**: Complete project in <5 minutes

---

## 📦 Current init-project.ps1 Features

**Existing capabilities**:
- ✅ Create project structure (Forms, Services, Repositories, etc.)
- ✅ Setup Git repository with .gitignore
- ✅ Add .editorconfig for consistent formatting
- ✅ Configure pre-commit hooks
- ✅ Setup Claude Code integration (.claude/ folder)
- ✅ Add NuGet packages (EF Core, Serilog, xUnit, etc.)
- ✅ Create initial Program.cs with DI container
- ✅ Setup VS Code tasks and launch config
- ✅ Git Submodule support for standards integration

---

## 📊 Enhancements to Add

### Enhancement 1: Auto-Install Agents (Phase 2)

**Current**: Agents not included in init
**Enhanced**: Copy agents to new project

```powershell
# New parameter
param(
    ...
    [switch]$IncludeAgents = $true
)

if ($IncludeAgents) {
    Write-Host "Installing WinForms agents..." -ForegroundColor Cyan
    Copy-Item "$PSScriptRoot/../.claude/agents/*" ".claude/agents/" -Recurse
    Write-Host "  ✅ winforms-reviewer" -ForegroundColor Green
    Write-Host "  ✅ test-generator" -ForegroundColor Green
    Write-Host "  ✅ docs-manager" -ForegroundColor Green
    Write-Host "  ✅ mvp-validator" -ForegroundColor Green
}
```

### Enhancement 2: Auto-Install Workflows (Phase 1)

**Current**: CLAUDE.md is monolithic
**Enhanced**: Install modular workflows

```powershell
if ($IncludeWorkflows) {
    Copy-Item "$PSScriptRoot/../.claude/workflows/*" ".claude/workflows/" -Recurse
    Write-Host "  ✅ Development workflow" -ForegroundColor Green
    Write-Host "  ✅ Testing workflow" -ForegroundColor Green
    Write-Host "  ✅ Code review checklist" -ForegroundColor Green
    Write-Host "  ✅ Expert behavior guide" -ForegroundColor Green
}
```

### Enhancement 3: Auto-Install Plan Templates (Phase 3)

**Current**: No planning support
**Enhanced**: Install plan templates

```powershell
if ($IncludePlanTemplates) {
    New-Item -ItemType Directory -Path "plans/templates" -Force
    New-Item -ItemType Directory -Path "plans/reports" -Force
    Copy-Item "$PSScriptRoot/../plans/templates/*" "plans/templates/" -Recurse
    Write-Host "  ✅ Form implementation template" -ForegroundColor Green
    Write-Host "  ✅ Service implementation template" -ForegroundColor Green
    Write-Host "  ✅ Repository implementation template" -ForegroundColor Green
    Write-Host "  ✅ Refactoring template" -ForegroundColor Green
    Write-Host "  ✅ Testing template" -ForegroundColor Green
}
```

### Enhancement 4: Integrate Scaffolding Wizard

**Current**: Manual file creation
**Enhanced**: Offer to scaffold first feature

```powershell
# After project creation
Write-Host ""
Write-Host "Project created successfully!" -ForegroundColor Green
Write-Host ""
$scaffold = Read-Host "Would you like to scaffold your first feature? (y/N)"

if ($scaffold -eq 'y') {
    $featureName = Read-Host "Feature name (e.g., Customer, Product)"
    $pattern = Read-Host "Pattern (MVP/MVVM) [MVP]"
    if ([string]::IsNullOrWhiteSpace($pattern)) { $pattern = "MVP" }

    & "$PSScriptRoot/scaffold-feature.ps1" -FeatureName $featureName -Pattern $pattern -OutputPath "."
}
```

### Enhancement 5: Claude Code Permissions

**Current**: Manual permission setup
**Enhanced**: Auto-configure common permissions

```powershell
# Create .claude/settings.local.json with permissions
$permissions = @{
    permissions = @{
        allow = @(
            "Bash(dotnet build:*)",
            "Bash(dotnet test:*)",
            "Bash(git add:*)",
            "Bash(git commit:*)",
            "SlashCommand(/create/*)",
            "SlashCommand(/add/*)",
            "SlashCommand(/fix/*)",
            "SlashCommand(/refactor/*)"
        )
        deny = @()
        ask = @()
    }
}

$permissions | ConvertTo-Json -Depth 10 | Out-File ".claude/settings.local.json"
```

### Enhancement 6: Git Submodule Auto-Update

**Current**: Manual submodule commands
**Enhanced**: Auto-detect and update standards

```powershell
# Check if standards-repo is submodule
if (Test-Path ".gitmodules") {
    Write-Host "Standards repository detected as submodule" -ForegroundColor Cyan
    git submodule update --init --recursive
    Write-Host "  ✅ Standards updated to latest" -ForegroundColor Green
}
```

---

## 📊 Impact

### Before Phase 4

**Manual setup** (30-60 min):
1. Run init-project.ps1 (basic structure)
2. Manually copy agents from standards repo
3. Manually copy workflows
4. Manually copy plan templates
5. Manually create first feature files
6. Manually configure Claude Code permissions
7. Manual Git submodule commands

### After Phase 4

**One-command setup** (<5 min):
```powershell
.\init-project.ps1 `
    -ProjectName "MyWinFormsApp" `
    -IncludeAgents `
    -IncludeWorkflows `
    -IncludePlanTemplates `
    -ScaffoldFirstFeature "Customer"
```

**Result**:
- ✅ Complete project structure
- ✅ All agents installed
- ✅ All workflows installed
- ✅ All plan templates installed
- ✅ First feature scaffolded
- ✅ Claude Code configured
- ✅ Git repo initialized
- ✅ Ready to code immediately

**Time Savings**: 25-55 minutes per project

---

## 🔧 Implementation Approach

### Step 1: Read Current init-project.ps1 (30 min)

Understand existing structure:
- Parameters
- Functions
- Project creation logic
- Claude Code setup
- Git submodule handling

### Step 2: Add New Parameters (30 min)

```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,

    [Parameter(Mandatory=$false)]
    [ValidateSet("8.0", "4.8")]
    [string]$DotNetVersion = "8.0",

    # NEW PARAMETERS
    [switch]$IncludeAgents = $true,
    [switch]$IncludeWorkflows = $true,
    [switch]$IncludePlanTemplates = $true,
    [switch]$ScaffoldFirstFeature,
    [string]$FirstFeatureName,
    [ValidateSet("MVP", "MVVM")]
    [string]$Pattern = "MVP"
)
```

### Step 3: Add Agent Installation Function (1 hour)

```powershell
function Install-Agents {
    param([string]$ProjectPath)

    Write-Host "Installing WinForms agents..." -ForegroundColor Cyan

    $agentSource = "$PSScriptRoot/../.claude/agents"
    $agentDest = "$ProjectPath/.claude/agents"

    if (Test-Path $agentSource) {
        Copy-Item -Path "$agentSource/*" -Destination $agentDest -Recurse -Force
        Write-Host "  ✅ 4 agents installed" -ForegroundColor Green
    } else {
        Write-Warning "Agents not found. Run from winforms-coding-standards repo."
    }
}
```

### Step 4: Add Workflow Installation Function (1 hour)

Similar to Install-Agents, but for workflows

### Step 5: Add Plan Templates Installation (1 hour)

Similar pattern

### Step 6: Integrate Scaffolding Wizard (2 hours)

```powershell
# At end of script
if ($ScaffoldFirstFeature -or $FirstFeatureName) {
    $featureName = if ($FirstFeatureName) { $FirstFeatureName } else {
        Read-Host "Feature name (e.g., Customer, Product)"
    }

    Write-Host "Scaffolding first feature: $featureName..." -ForegroundColor Cyan
    & "$PSScriptRoot/scaffold-feature.ps1" `
        -FeatureName $featureName `
        -Pattern $Pattern `
        -OutputPath $ProjectPath
}
```

### Step 7: Add Summary Report (1 hour)

```powershell
# Final output
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Project Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Project: $ProjectName" -ForegroundColor Cyan
Write-Host "Location: $ProjectPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Installed Features:" -ForegroundColor Yellow
if ($IncludeAgents) { Write-Host "  ✅ WinForms Agents (4)" }
if ($IncludeWorkflows) { Write-Host "  ✅ Workflows (4)" }
if ($IncludePlanTemplates) { Write-Host "  ✅ Plan Templates (5)" }
if ($FirstFeatureName) { Write-Host "  ✅ First Feature: $FirstFeatureName" }
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. cd $ProjectName"
Write-Host "  2. code ."
Write-Host "  3. Start coding with Claude Code!"
Write-Host ""
Write-Host "Useful Commands:" -ForegroundColor Yellow
Write-Host "  dotnet build           - Build project"
Write-Host "  dotnet test            - Run tests"
Write-Host "  /create/form Customer  - Create new form"
Write-Host "  /review-code [file]    - Review code quality"
Write-Host ""
```

### Step 8: Update Documentation (1 hour)

- Update README.md with new parameters
- Update USAGE_GUIDE.md with enhanced workflow
- Update init-project.ps1 help comments

### Step 9: Testing (2 hours)

Test all combinations:
```powershell
# Test 1: Minimal setup
.\init-project.ps1 -ProjectName "Test1"

# Test 2: Full setup
.\init-project.ps1 `
    -ProjectName "Test2" `
    -IncludeAgents `
    -IncludeWorkflows `
    -IncludePlanTemplates

# Test 3: With scaffolding
.\init-project.ps1 `
    -ProjectName "Test3" `
    -IncludeAgents `
    -IncludeWorkflows `
    -IncludePlanTemplates `
    -FirstFeatureName "Customer" `
    -Pattern "MVP"

# Test 4: Submodule integration
.\init-project.ps1 `
    -ProjectName "Test4" `
    -IntegrateStandards `
    -StandardsRepo "https://github.com/user/winforms-coding-standards.git"
```

---

## ✅ Success Criteria

Phase 4 is complete when:

- [ ] All new parameters added and documented
- [ ] Agent installation function works
- [ ] Workflow installation function works
- [ ] Plan template installation function works
- [ ] Scaffolding wizard integration works
- [ ] Claude Code permissions auto-configured
- [ ] Git submodule auto-update works
- [ ] Summary report displays correctly
- [ ] All test scenarios pass
- [ ] Documentation updated
- [ ] Script produces working project in <5 min

---

## 🔗 Dependencies

**Requires**:
- Phase 1 workflows (to install)
- Phase 2 agents (to install)
- Phase 3 plan templates (to install)
- Phase 3 scaffold script (to integrate)

**Enables**:
- Complete one-command project setup
- Enterprise-grade developer experience

---

## 📝 Enhanced Parameter Examples

### Example 1: Quick Start

```powershell
# Minimal setup
.\init-project.ps1 -ProjectName "MyApp"
```

### Example 2: Full Featured

```powershell
# Everything included
.\init-project.ps1 `
    -ProjectName "MyApp" `
    -DotNetVersion "8.0" `
    -IncludeAgents `
    -IncludeWorkflows `
    -IncludePlanTemplates `
    -FirstFeatureName "Customer" `
    -Pattern "MVP"
```

### Example 3: With Standards Submodule

```powershell
# Integrate standards as submodule
.\init-project.ps1 `
    -ProjectName "MyApp" `
    -IntegrateStandards `
    -StandardsRepo "https://github.com/myorg/winforms-coding-standards.git" `
    -IncludeAgents `
    -IncludeWorkflows
```

---

**Next Phase**: [Phase 5: Skills & Auto-Documentation](../phase-5/OVERVIEW.md)

---

**Last Updated**: 2025-11-08
**Status**: High-Level Plan
