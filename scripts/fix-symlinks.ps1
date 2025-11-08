# Fix Symlinks for Claude Code Commands
# Run this script as Administrator in your project directory

param(
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "."
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "Fixing Symlinks for Claude Code Integration" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "[ERROR] This script must be run as Administrator!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Steps to run as Admin:" -ForegroundColor Yellow
    Write-Host "  1. Close this PowerShell window"
    Write-Host "  2. Right-click PowerShell -> 'Run as Administrator'"
    Write-Host "  3. cd to your project folder"
    Write-Host "  4. Run this script again"
    Write-Host ""
    exit 1
}

# Navigate to project directory
Set-Location $ProjectPath
$currentPath = Get-Location

Write-Host "Project Path: $currentPath" -ForegroundColor Green
Write-Host ""

# Check if .standards exists
if (-not (Test-Path ".standards")) {
    Write-Host "[ERROR] .standards directory not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "This script requires the coding standards submodule." -ForegroundColor Yellow
    Write-Host "Please run setup-standards.ps1 first." -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "[OK] Found .standards directory" -ForegroundColor Green

# Create .claude symlink
Write-Host ""
Write-Host "Creating .claude symlink..." -NoNewline

if (Test-Path ".claude") {
    # Check if it's already a symlink
    $item = Get-Item ".claude" -Force
    if ($item.LinkType -eq "SymbolicLink") {
        Write-Host " [SKIP] Already exists" -ForegroundColor Yellow
    } else {
        Write-Host ""
        Write-Host "[WARN] .claude exists but is not a symlink" -ForegroundColor Yellow
        $response = Read-Host "Remove and recreate as symlink? (y/n)"
        if ($response -eq "y") {
            Remove-Item ".claude" -Recurse -Force
            New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude" -Force | Out-Null
            Write-Host "[OK] .claude symlink created" -ForegroundColor Green
        }
    }
} else {
    New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude" -Force | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

# Create templates symlink
Write-Host "Creating templates symlink..." -NoNewline

if (Test-Path "templates") {
    # Check if it's already a symlink
    $item = Get-Item "templates" -Force
    if ($item.LinkType -eq "SymbolicLink") {
        Write-Host " [SKIP] Already exists" -ForegroundColor Yellow
    } else {
        Write-Host ""
        Write-Host "[WARN] templates exists but is not a symlink" -ForegroundColor Yellow
        $response = Read-Host "Remove and recreate as symlink? (y/n)"
        if ($response -eq "y") {
            Remove-Item "templates" -Recurse -Force
            New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates" -Force | Out-Null
            Write-Host "[OK] templates symlink created" -ForegroundColor Green
        }
    }
} else {
    New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates" -Force | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

# Verify
Write-Host ""
Write-Host "Verifying setup..." -ForegroundColor Cyan

$allGood = $true

if (Test-Path ".claude\commands") {
    $commandCount = (Get-ChildItem ".claude\commands" -Filter "*.md").Count
    Write-Host "[OK] .claude\commands found ($commandCount commands)" -ForegroundColor Green
} else {
    Write-Host "[ERROR] .claude\commands not accessible" -ForegroundColor Red
    $allGood = $false
}

if (Test-Path "templates") {
    $templateCount = (Get-ChildItem "templates" -Filter "*.cs").Count
    Write-Host "[OK] templates found ($templateCount templates)" -ForegroundColor Green
} else {
    Write-Host "[ERROR] templates not accessible" -ForegroundColor Red
    $allGood = $false
}

Write-Host ""

if ($allGood) {
    Write-Host "================================================" -ForegroundColor Green
    Write-Host "Success! Claude Code integration is ready!" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "[Next Steps]" -ForegroundColor Yellow
    Write-Host "  1. Open this project in VS Code with Claude Code"
    Write-Host "  2. Type / to see available slash commands"
    Write-Host "  3. Start coding with standards!"
    Write-Host ""
} else {
    Write-Host "================================================" -ForegroundColor Red
    Write-Host "Setup incomplete - please check errors above" -ForegroundColor Red
    Write-Host "================================================" -ForegroundColor Red
    Write-Host ""
}
