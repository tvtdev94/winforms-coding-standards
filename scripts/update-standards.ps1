<#
.SYNOPSIS
    Update coding standards and sync configuration files

.DESCRIPTION
    This script updates the .standards submodule and syncs configuration files
    (.editorconfig, .gitignore) from the standards repository to the current project.

    Use this when:
    - Standards repository has been updated
    - You want to get the latest slash commands, templates, and documentation
    - You don't have symlinks (non-Admin) and need to manually sync config files

.PARAMETER Force
    Force update even if there are uncommitted changes

.EXAMPLE
    .\update-standards.ps1

.EXAMPLE
    .\update-standards.ps1 -Force
#>

param(
    [switch]$Force
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Update Coding Standards" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .standards submodule exists
if (-not (Test-Path ".standards/.git")) {
    Write-Host "[ERROR] .standards submodule not found" -ForegroundColor Red
    Write-Host "        This project doesn't have coding standards integrated" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To integrate standards, run:" -ForegroundColor Yellow
    Write-Host "  ..\winforms-coding-standards\scripts\setup-standards.ps1" -ForegroundColor White
    Write-Host ""
    exit 1
}

# Check for uncommitted changes
$gitStatus = git status --porcelain 2>&1
if ($gitStatus -and -not $Force) {
    Write-Host "[WARN] You have uncommitted changes" -ForegroundColor Yellow
    Write-Host ""
    git status --short
    Write-Host ""
    Write-Host "Commit your changes first, or run with -Force to proceed anyway" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

# Step 1: Update submodule
Write-Host "[1] Updating .standards submodule..." -ForegroundColor Cyan
Write-Host ""

Push-Location .standards
$beforeHash = git rev-parse HEAD
Pop-Location

git submodule update --remote --merge .standards *>&1 | Out-Null

if ($LASTEXITCODE -eq 0) {
    Push-Location .standards
    $afterHash = git rev-parse HEAD
    Pop-Location

    if ($beforeHash -eq $afterHash) {
        Write-Host "  [OK] Already up to date" -ForegroundColor Green
    } else {
        Write-Host "  [OK] Updated from $($beforeHash.Substring(0,7)) to $($afterHash.Substring(0,7))" -ForegroundColor Green
    }
} else {
    Write-Host "  [ERROR] Failed to update submodule" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 2: Check if using symlinks or copies
Write-Host "[2] Checking configuration files..." -ForegroundColor Cyan
Write-Host ""

$usingSymlinks = $false
$configFiles = @(".editorconfig", ".gitignore")

# Check if files are symlinks
foreach ($file in $configFiles) {
    if (Test-Path $file) {
        $item = Get-Item $file
        if ($item.LinkType -eq "SymbolicLink") {
            $usingSymlinks = $true
            Write-Host "  [OK] $file is symlinked (auto-updates)" -ForegroundColor Green
        }
    }
}

if ($usingSymlinks) {
    Write-Host ""
    Write-Host "  Config files are symlinked - they update automatically!" -ForegroundColor Green
    Write-Host "  No manual sync needed." -ForegroundColor Green
} else {
    # Step 3: Sync config files
    Write-Host ""
    Write-Host "[3] Syncing configuration files..." -ForegroundColor Cyan
    Write-Host ""

    $syncedCount = 0
    foreach ($file in $configFiles) {
        if (Test-Path ".standards/$file") {
            Copy-Item ".standards/$file" -Destination "." -Force
            Write-Host "  [OK] $file synced" -ForegroundColor Green
            $syncedCount++
        } else {
            Write-Host "  [SKIP] $file not found in standards" -ForegroundColor Yellow
        }
    }

    if ($syncedCount -gt 0) {
        Write-Host ""
        Write-Host "  [TIP] Run as Admin to create symlinks for auto-update:" -ForegroundColor Cyan
        Write-Host "        ..\winforms-coding-standards\scripts\setup-standards.ps1 -CreateSymlinks" -ForegroundColor White
    }
}

# Step 4: Show what's new
Write-Host ""
Write-Host "[4] Checking for changes..." -ForegroundColor Cyan
Write-Host ""

Push-Location .standards
$changes = git log $beforeHash..$afterHash --oneline 2>&1
Pop-Location

if ($changes) {
    Write-Host "  Recent updates:" -ForegroundColor Yellow
    Write-Host ""
    $changes | ForEach-Object {
        Write-Host "    $_" -ForegroundColor White
    }
} else {
    Write-Host "  No new updates" -ForegroundColor Green
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Update Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Standards location: .standards/" -ForegroundColor Cyan
Write-Host "Slash commands: .standards/.claude/commands/" -ForegroundColor Cyan
Write-Host "Templates: .standards/templates/" -ForegroundColor Cyan
Write-Host "Documentation: .standards/docs/" -ForegroundColor Cyan
Write-Host ""
