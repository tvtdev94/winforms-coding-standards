# Add Coding Standards to Existing Project
# Copies coding standards into an existing WinForms project

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Add Coding Standards to Project" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# Check if we're in a valid project directory
# ============================================================================

# Check for .csproj or .sln file
$hasCsproj = Get-ChildItem -Path "." -Filter "*.csproj" -Recurse -Depth 1 | Select-Object -First 1
$hasSln = Get-ChildItem -Path "." -Filter "*.sln" | Select-Object -First 1

if (-not $hasCsproj -and -not $hasSln) {
    Write-Host "[ERROR] No .csproj or .sln file found in current directory!" -ForegroundColor Red
    Write-Host "   Please run this script from your project root." -ForegroundColor Yellow
    exit 1
}

$projectName = if ($hasSln) { $hasSln.BaseName } else { $hasCsproj.BaseName }
Write-Host "Project detected: $projectName" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Find standards repository
# ============================================================================

Write-Host "Looking for coding standards..." -ForegroundColor Cyan

$standardsPath = $null

# Option 1: Standards already exist as submodule
if (Test-Path ".standards") {
    $standardsPath = ".standards"
    Write-Host "  Found: .standards (submodule)" -ForegroundColor Green
}
# Option 2: Script is run from standards repo itself
elseif (Test-Path "scripts\add-standards.ps1") {
    $standardsPath = "."
    Write-Host "  Found: Current directory is standards repo" -ForegroundColor Green
}
# Option 3: Parent directory contains standards
elseif (Test-Path "..\winforms-coding-standards") {
    $standardsPath = "..\winforms-coding-standards"
    Write-Host "  Found: ..\winforms-coding-standards" -ForegroundColor Green
}
# Option 4: Ask user
else {
    Write-Host "  Standards not found automatically." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Options:" -ForegroundColor Yellow
    Write-Host "  [1] Clone from GitHub (recommended)" -ForegroundColor Gray
    Write-Host "  [2] Specify local path" -ForegroundColor Gray
    Write-Host "  [3] Cancel" -ForegroundColor Gray
    $choice = Read-Host "Select option (1-3)"

    switch ($choice) {
        "1" {
            Write-Host ""
            Write-Host "Cloning standards repository..." -ForegroundColor Cyan
            $repoUrl = Read-Host "Enter Git URL (or press Enter for default)"
            if ([string]::IsNullOrWhiteSpace($repoUrl)) {
                $repoUrl = "https://github.com/your-org/winforms-coding-standards.git"
            }

            git clone $repoUrl ".standards" 2>&1 | Out-Null
            if (Test-Path ".standards") {
                $standardsPath = ".standards"
                Write-Host "  [OK] Cloned to .standards" -ForegroundColor Green
            } else {
                Write-Host "  [ERROR] Failed to clone repository" -ForegroundColor Red
                exit 1
            }
        }
        "2" {
            $standardsPath = Read-Host "Enter path to standards repository"
            if (-not (Test-Path $standardsPath)) {
                Write-Host "  [ERROR] Path not found: $standardsPath" -ForegroundColor Red
                exit 1
            }
        }
        "3" {
            Write-Host "Cancelled." -ForegroundColor Yellow
            exit 0
        }
        default {
            Write-Host "Invalid option." -ForegroundColor Red
            exit 1
        }
    }
}

Write-Host ""

# ============================================================================
# Verify standards repository
# ============================================================================

if (-not (Test-Path "$standardsPath\.claude")) {
    Write-Host "[ERROR] Invalid standards repository - .claude folder not found!" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path "$standardsPath\templates")) {
    Write-Host "[ERROR] Invalid standards repository - templates folder not found!" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Standards repository validated" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Collect project information
# ============================================================================

Write-Host "Project Configuration" -ForegroundColor Yellow
Write-Host "=====================" -ForegroundColor Yellow
Write-Host ""

# UI Framework
Write-Host "1. UI Framework" -ForegroundColor Yellow
Write-Host "   [1] Standard WinForms (default controls)" -ForegroundColor Gray
Write-Host "   [2] DevExpress (commercial)" -ForegroundColor Gray
Write-Host "   [3] ReaLTaiizor (free, modern themes)" -ForegroundColor Gray
$uiChoice = Read-Host "   Select (1-3)"
$UIFramework = switch ($uiChoice) {
    "1" { "Standard" }
    "2" { "DevExpress" }
    "3" { "ReaLTaiizor" }
    default { "Standard" }
}
Write-Host "   Selected: $UIFramework" -ForegroundColor Green
Write-Host ""

# Database
Write-Host "2. Database Provider" -ForegroundColor Yellow
Write-Host "   [1] SQLite" -ForegroundColor Gray
Write-Host "   [2] SQL Server" -ForegroundColor Gray
Write-Host "   [3] PostgreSQL" -ForegroundColor Gray
Write-Host "   [4] MySQL" -ForegroundColor Gray
Write-Host "   [5] None" -ForegroundColor Gray
$dbChoice = Read-Host "   Select (1-5)"
$Database = switch ($dbChoice) {
    "1" { "SQLite" }
    "2" { "SQLServer" }
    "3" { "PostgreSQL" }
    "4" { "MySQL" }
    "5" { "None" }
    default { "SQLite" }
}
Write-Host "   Selected: $Database" -ForegroundColor Green
Write-Host ""

# Pattern
Write-Host "3. Architecture Pattern" -ForegroundColor Yellow
Write-Host "   [1] MVP (recommended for WinForms)" -ForegroundColor Gray
Write-Host "   [2] MVVM" -ForegroundColor Gray
Write-Host "   [3] Simple (no pattern)" -ForegroundColor Gray
$patternChoice = Read-Host "   Select (1-3)"
$Pattern = switch ($patternChoice) {
    "1" { "MVP" }
    "2" { "MVVM" }
    "3" { "Simple" }
    default { "MVP" }
}
Write-Host "   Selected: $Pattern" -ForegroundColor Green
Write-Host ""

# Framework
Write-Host "4. Target Framework" -ForegroundColor Yellow
Write-Host "   [1] .NET 8.0" -ForegroundColor Gray
Write-Host "   [2] .NET 6.0" -ForegroundColor Gray
Write-Host "   [3] .NET Framework 4.8" -ForegroundColor Gray
$fwChoice = Read-Host "   Select (1-3)"
$Framework = switch ($fwChoice) {
    "1" { "net8.0" }
    "2" { "net6.0" }
    "3" { "net48" }
    default { "net8.0" }
}
Write-Host "   Selected: $Framework" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Confirmation
# ============================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Project      : $projectName"
Write-Host "UI Framework : $UIFramework"
Write-Host "Database     : $Database"
Write-Host "Pattern      : $Pattern"
Write-Host "Framework    : $Framework"
Write-Host "Standards    : $standardsPath"
Write-Host ""

$confirm = Read-Host "Proceed? (Y/n)"
if ($confirm -eq "n" -or $confirm -eq "N") {
    Write-Host "Cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# ============================================================================
# Copy standards files
# ============================================================================

Write-Host "Copying standards files..." -ForegroundColor Cyan

# Backup existing files if they exist
if (Test-Path ".claude") {
    $backupName = ".claude.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Move-Item ".claude" $backupName
    Write-Host "  [INFO] Existing .claude backed up to $backupName" -ForegroundColor Yellow
}

if (Test-Path "templates") {
    $backupName = "templates.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Move-Item "templates" $backupName
    Write-Host "  [INFO] Existing templates backed up to $backupName" -ForegroundColor Yellow
}

# Create .claude folder
New-Item -ItemType Directory -Path ".claude" -Force | Out-Null

# Copy .claude subdirectories
$claudeSubdirs = @("agents", "commands", "guides", "workflows")
foreach ($subdir in $claudeSubdirs) {
    if (Test-Path "$standardsPath\.claude\$subdir") {
        Copy-Item -Recurse "$standardsPath\.claude\$subdir" -Destination ".claude\$subdir" -Force
    }
}
Write-Host "  [OK] Copied .claude directory" -ForegroundColor Green

# Copy templates
if (Test-Path "$standardsPath\templates") {
    Copy-Item -Recurse "$standardsPath\templates" -Destination "templates" -Force
    Write-Host "  [OK] Copied templates directory" -ForegroundColor Green
}

# Copy CLAUDE.md
if (Test-Path "$standardsPath\CLAUDE.md") {
    Copy-Item "$standardsPath\CLAUDE.md" -Destination "CLAUDE.md" -Force
    Write-Host "  [OK] Copied CLAUDE.md" -ForegroundColor Green
}

# Copy INDEX.md
if (Test-Path "$standardsPath\.claude\INDEX.md") {
    Copy-Item "$standardsPath\.claude\INDEX.md" -Destination ".claude\INDEX.md" -Force
    Write-Host "  [OK] Copied .claude\INDEX.md" -ForegroundColor Green
}

# Create plans directory
if (-not (Test-Path "plans")) {
    New-Item -ItemType Directory -Path "plans" -Force | Out-Null
}
if (Test-Path "$standardsPath\plans\templates") {
    Copy-Item -Recurse "$standardsPath\plans\templates" -Destination "plans\templates" -Force
    Write-Host "  [OK] Copied plans\templates" -ForegroundColor Green
}
if (-not (Test-Path "plans\research")) {
    New-Item -ItemType Directory -Path "plans\research" -Force | Out-Null
}
if (-not (Test-Path "plans\reports")) {
    New-Item -ItemType Directory -Path "plans\reports" -Force | Out-Null
}
Write-Host "  [OK] Created plans directory structure" -ForegroundColor Green

Write-Host ""

# ============================================================================
# Create project-context.md
# ============================================================================

Write-Host "Creating project context..." -ForegroundColor Cyan

$uiFrameworkDesc = switch ($UIFramework) {
    "Standard" { "Use **standard WinForms controls**: Button, TextBox, DataGridView, Label, etc." }
    "DevExpress" { "Use **DevExpress controls**: XtraGrid, LookUpEdit, LayoutControl, SimpleButton, etc." }
    "ReaLTaiizor" { "Use **ReaLTaiizor controls**: MaterialButton, MaterialTextBox, MaterialListView, etc." }
    default { "Use standard WinForms controls" }
}

$patternDesc = switch ($Pattern) {
    "MVP" { "**Model-View-Presenter**: Forms implement View interfaces, Presenters contain UI logic." }
    "MVVM" { "**Model-View-ViewModel**: Use ViewModels with INotifyPropertyChanged." }
    "Simple" { "**Simple pattern**: Direct form-to-service calls." }
    default { "MVP pattern" }
}

$projectContext = @"
# Project Context

> **Auto-generated by add-standards.ps1**
> **Generated**: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## Configuration

| Setting | Value |
|---------|-------|
| **Project Name** | $projectName |
| **UI Framework** | $UIFramework |
| **Database** | $Database |
| **Pattern** | $Pattern |
| **Framework** | $Framework |

## UI Framework Rules

$uiFrameworkDesc

## Architecture Pattern

$patternDesc

## Templates to Use

| Component | Template |
|-----------|----------|
| Form | ``templates/$( if ($UIFramework -eq "DevExpress") { "dx-form-template.cs" } elseif ($UIFramework -eq "ReaLTaiizor") { "rt-material-form-template.cs" } else { "form-template.cs" })`` |
| Service | ``templates/service-template.cs`` |
| Repository | ``templates/repository-template.cs`` |
| Presenter | ``templates/presenter-template.cs`` |
| Validator | ``templates/validator-template.cs`` |
| Test | ``templates/test-template.cs`` |

## Project-Specific Rules

<!-- Add your project-specific rules here -->

- Follow existing code patterns in this project
- Use existing naming conventions
- Check existing services/repositories for reference

## DO NOT

- Do not mix UI frameworks
- Do not add new dependencies without approval
- Do not change existing architecture patterns
"@

$projectContext | Out-File -FilePath ".claude\project-context.md" -Encoding UTF8
Write-Host "  [OK] Created .claude\project-context.md" -ForegroundColor Green

Write-Host ""

# ============================================================================
# Add to .gitignore
# ============================================================================

Write-Host "Updating .gitignore..." -ForegroundColor Cyan

$gitignoreEntries = @"

# ============================================================================
# Coding Standards (copied from .standards - do not commit)
# ============================================================================
# These files are copied from the standards repository and should not be
# committed to your project. Run update-standards.ps1 to update them.

# Claude Code standards (copied)
.claude/agents/
.claude/commands/
.claude/guides/
.claude/workflows/
.claude/INDEX.md
CLAUDE.md

# Templates (copied)
templates/

# Plan templates (copied)
plans/templates/

# Plan working files (generated)
plans/research/
plans/reports/

# Keep project-specific config (NOT ignored):
# .claude/project-context.md
"@

if (Test-Path ".gitignore") {
    $gitignoreContent = Get-Content ".gitignore" -Raw
    if ($gitignoreContent -notmatch "Coding Standards \(copied") {
        Add-Content ".gitignore" $gitignoreEntries
        Write-Host "  [OK] Updated .gitignore with standards exclusions" -ForegroundColor Green
    } else {
        Write-Host "  [SKIP] .gitignore already has standards exclusions" -ForegroundColor Yellow
    }
} else {
    $gitignoreEntries | Out-File -FilePath ".gitignore" -Encoding UTF8
    Write-Host "  [OK] Created .gitignore with standards exclusions" -ForegroundColor Green
}

# ============================================================================
# Done
# ============================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Standards Added Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Files added:" -ForegroundColor Yellow
Write-Host "  .claude/           - Slash commands, agents, guides"
Write-Host "  .claude/project-context.md - Your project config"
Write-Host "  templates/         - Code templates"
Write-Host "  plans/             - Planning templates"
Write-Host "  CLAUDE.md          - AI assistant guide"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review .claude/project-context.md"
Write-Host "  2. Type / in Claude Code to see slash commands"
Write-Host "  3. Try: /create:form, /create:service, /cook"
Write-Host ""
Write-Host "Update standards later:" -ForegroundColor Yellow
Write-Host "  .\scripts\update-standards.ps1"
Write-Host "  -- OR --"
Write-Host "  Copy-Item <standards-path>\.claude\* .claude -Recurse -Force"
Write-Host "  Copy-Item <standards-path>\templates\* templates -Recurse -Force"
Write-Host ""
