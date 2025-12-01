<#
.SYNOPSIS
    Update coding standards in current project

.DESCRIPTION
    This script updates the coding standards files in your project from:
    - .standards submodule (if exists)
    - Or a specified standards repository path

    Files updated:
    - .claude/agents, commands, guides, workflows
    - .claude/INDEX.md
    - templates/
    - plans/templates/
    - CLAUDE.md

    Files preserved:
    - .claude/project-context.md (your project config)

.PARAMETER StandardsPath
    Path to standards repository. If not specified, will auto-detect.

.EXAMPLE
    .\update-standards.ps1

.EXAMPLE
    .\update-standards.ps1 -StandardsPath "C:\repos\winforms-coding-standards"
#>

param(
    [string]$StandardsPath
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Update Coding Standards" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# Check if standards exist in current project
# ============================================================================

if (-not (Test-Path ".claude")) {
    Write-Host "[ERROR] .claude folder not found!" -ForegroundColor Red
    Write-Host "   Run add-standards.ps1 first to add standards to this project." -ForegroundColor Yellow
    exit 1
}

Write-Host "Current project has standards installed." -ForegroundColor Green
Write-Host ""

# ============================================================================
# Find standards source
# ============================================================================

Write-Host "Looking for standards source..." -ForegroundColor Cyan

if ($StandardsPath) {
    # User specified path
    if (-not (Test-Path $StandardsPath)) {
        Write-Host "[ERROR] Path not found: $StandardsPath" -ForegroundColor Red
        exit 1
    }
    Write-Host "  Using: $StandardsPath" -ForegroundColor Green
}
elseif (Test-Path ".standards") {
    # Option 1: .standards submodule exists
    Write-Host "  Found: .standards (submodule)" -ForegroundColor Green
    Write-Host ""
    Write-Host "Updating submodule..." -ForegroundColor Cyan

    Push-Location ".standards"
    git pull origin main 2>&1 | Out-Null
    Pop-Location

    $StandardsPath = ".standards"
    Write-Host "  [OK] Submodule updated" -ForegroundColor Green
}
elseif (Test-Path "..\winforms-coding-standards") {
    $StandardsPath = "..\winforms-coding-standards"
    Write-Host "  Found: $StandardsPath" -ForegroundColor Green
}
elseif (Test-Path "..\..\winforms-coding-standards") {
    $StandardsPath = "..\..\winforms-coding-standards"
    Write-Host "  Found: $StandardsPath" -ForegroundColor Green
}
else {
    Write-Host "  Standards source not found automatically." -ForegroundColor Yellow
    Write-Host ""
    $StandardsPath = Read-Host "Enter path to standards repository"

    if (-not (Test-Path $StandardsPath)) {
        Write-Host "[ERROR] Path not found: $StandardsPath" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""

# ============================================================================
# Verify standards repository
# ============================================================================

if (-not (Test-Path "$StandardsPath\.claude")) {
    Write-Host "[ERROR] Invalid standards repository - .claude folder not found!" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Standards source validated" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Show what will be updated
# ============================================================================

Write-Host "Files to update:" -ForegroundColor Yellow
Write-Host "  .claude/agents/"
Write-Host "  .claude/commands/"
Write-Host "  .claude/guides/"
Write-Host "  .claude/workflows/"
Write-Host "  .claude/INDEX.md"
Write-Host "  templates/"
Write-Host "  plans/templates/"
Write-Host "  CLAUDE.md"
Write-Host ""
Write-Host "[NOTE] .claude/project-context.md will NOT be overwritten" -ForegroundColor Cyan
Write-Host ""

$confirm = Read-Host "Proceed with update? (Y/n)"
if ($confirm -eq "n" -or $confirm -eq "N") {
    Write-Host "Cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# ============================================================================
# Update files
# ============================================================================

Write-Host "Updating standards..." -ForegroundColor Cyan

# Backup project-context.md
$projectContextBackup = $null
if (Test-Path ".claude\project-context.md") {
    $projectContextBackup = Get-Content ".claude\project-context.md" -Raw
}

# Update .claude subdirectories
$claudeSubdirs = @("agents", "commands", "guides", "workflows")
foreach ($subdir in $claudeSubdirs) {
    if (Test-Path ".claude\$subdir") {
        Remove-Item ".claude\$subdir" -Recurse -Force
    }
    if (Test-Path "$StandardsPath\.claude\$subdir") {
        Copy-Item -Recurse "$StandardsPath\.claude\$subdir" -Destination ".claude\$subdir" -Force
        Write-Host "  [OK] Updated .claude\$subdir" -ForegroundColor Green
    }
}

# Restore project-context.md
if ($projectContextBackup) {
    $projectContextBackup | Out-File -FilePath ".claude\project-context.md" -Encoding UTF8 -NoNewline
    Write-Host "  [OK] Preserved .claude\project-context.md" -ForegroundColor Green
}

# Update INDEX.md
if (Test-Path "$StandardsPath\.claude\INDEX.md") {
    Copy-Item "$StandardsPath\.claude\INDEX.md" -Destination ".claude\INDEX.md" -Force
    Write-Host "  [OK] Updated .claude\INDEX.md" -ForegroundColor Green
}

# Update templates
if (Test-Path "templates") {
    Remove-Item "templates" -Recurse -Force
}
if (Test-Path "$StandardsPath\templates") {
    Copy-Item -Recurse "$StandardsPath\templates" -Destination "templates" -Force
    Write-Host "  [OK] Updated templates" -ForegroundColor Green
}

# Update CLAUDE.md
if (Test-Path "$StandardsPath\CLAUDE.md") {
    Copy-Item "$StandardsPath\CLAUDE.md" -Destination "CLAUDE.md" -Force
    Write-Host "  [OK] Updated CLAUDE.md" -ForegroundColor Green
}

# Update plans/templates
if (-not (Test-Path "plans")) {
    New-Item -ItemType Directory -Path "plans" -Force | Out-Null
}
if (Test-Path "plans\templates") {
    Remove-Item "plans\templates" -Recurse -Force
}
if (Test-Path "$StandardsPath\plans\templates") {
    Copy-Item -Recurse "$StandardsPath\plans\templates" -Destination "plans\templates" -Force
    Write-Host "  [OK] Updated plans\templates" -ForegroundColor Green
}

Write-Host ""

# ============================================================================
# Update .gitignore (ensure standards are excluded)
# ============================================================================

Write-Host "Checking .gitignore..." -ForegroundColor Cyan

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
        Write-Host "  [OK] .gitignore already has standards exclusions" -ForegroundColor Green
    }
} else {
    Write-Host "  [SKIP] No .gitignore found" -ForegroundColor Yellow
}

Write-Host ""

# ============================================================================
# Done
# ============================================================================

Write-Host "========================================" -ForegroundColor Green
Write-Host "  Standards Updated Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Updated from: $StandardsPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your project-context.md was preserved." -ForegroundColor Yellow
Write-Host ""
