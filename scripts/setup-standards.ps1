<#
.SYNOPSIS
    Setup WinForms Coding Standards for a new or existing project

.DESCRIPTION
    This script integrates the WinForms Coding Standards repository into your project
    using Git Submodule. It creates symlinks for Claude Code commands and templates.

.PARAMETER ProjectPath
    Path to your WinForms project (default: current directory)

.PARAMETER StandardsRepo
    Git repository URL for standards (default: this repo)

.PARAMETER SkipSymlinks
    Skip creating symlinks (use submodule directly)

.EXAMPLE
    .\setup-standards.ps1
    # Setup standards in current directory

.EXAMPLE
    .\setup-standards.ps1 -ProjectPath "D:\MyProjects\MyApp"
    # Setup standards in specific project

.EXAMPLE
    .\setup-standards.ps1 -StandardsRepo "https://github.com/yourorg/winforms-standards.git"
    # Use custom standards repository
#>

param(
    [string]$ProjectPath = ".",
    [string]$StandardsRepo = "",
    [switch]$SkipSymlinks
)

# Colors for output
function Write-Success { Write-Host "[OK] $args" -ForegroundColor Green }
function Write-Info { Write-Host "[INFO] $args" -ForegroundColor Cyan }
function Write-Warning { Write-Host "[WARN] $args" -ForegroundColor Yellow }
function Write-Error-Custom { Write-Host "[ERROR] $args" -ForegroundColor Red }

# Banner
Write-Host ""
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "║     WinForms Coding Standards - Project Setup            ║" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Get absolute path
$ProjectPath = Resolve-Path $ProjectPath -ErrorAction SilentlyContinue
if (-not $ProjectPath) {
    Write-Error-Custom "Project path does not exist!"
    exit 1
}

Write-Info "Target project: $ProjectPath"
Write-Host ""

# Check if Git is installed
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error-Custom "Git is not installed or not in PATH!"
    Write-Host "Please install Git: https://git-scm.com/downloads"
    exit 1
}

# Check if project is a Git repository
Push-Location $ProjectPath
$isGitRepo = Test-Path ".git"
if (-not $isGitRepo) {
    Write-Warning "Project is not a Git repository."
    $initGit = Read-Host "Initialize Git repository? (y/n)"
    if ($initGit -eq "y") {
        git init
        Write-Success "Git repository initialized"
    } else {
        Write-Error-Custom "Git repository required for submodule setup"
        Pop-Location
        exit 1
    }
}

# Auto-detect standards repo URL if not provided
if (-not $StandardsRepo) {
    # Try to get from current script location
    $scriptDir = Split-Path -Parent $PSCommandPath
    Push-Location $scriptDir
    Push-Location ".."
    $gitRemote = git remote get-url origin 2>$null
    Pop-Location
    Pop-Location

    if ($gitRemote) {
        $StandardsRepo = $gitRemote
        Write-Info "Detected standards repo: $StandardsRepo"
    } else {
        Write-Warning "Could not detect standards repository URL"
        $StandardsRepo = Read-Host "Enter standards repository URL (or path)"
    }
}

# Check if .standards submodule already exists
if (Test-Path ".standards") {
    Write-Warning "'.standards' directory already exists"
    $overwrite = Read-Host "Remove and reinstall? (y/n)"
    if ($overwrite -eq "y") {
        # Remove submodule
        git submodule deinit -f .standards 2>$null
        git rm -f .standards 2>$null
        Remove-Item -Path ".standards" -Recurse -Force -ErrorAction SilentlyContinue
        Remove-Item -Path ".git/modules/.standards" -Recurse -Force -ErrorAction SilentlyContinue
        Write-Success "Removed existing .standards"
    } else {
        Write-Info "Keeping existing setup"
        Pop-Location
        exit 0
    }
}

# Add submodule
Write-Info "Adding standards as Git submodule..."
Write-Host ""

try {
    git submodule add $StandardsRepo .standards
    git submodule update --init --recursive
    Write-Success "Standards submodule added successfully!"
} catch {
    Write-Error-Custom "Failed to add submodule: $_"
    Pop-Location
    exit 1
}

Write-Host ""

# Create symlinks for easy access (optional)
if (-not $SkipSymlinks) {
    Write-Info "Creating symlinks for convenience..."

    # Check for admin rights on Windows
    $isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

    if ($isAdmin) {
        # Symlink .claude commands
        if (-not (Test-Path ".claude")) {
            try {
                New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude" -ErrorAction Stop | Out-Null
                Write-Success "Created symlink: .claude -> .standards\.claude"
            } catch {
                Write-Warning "Could not create .claude symlink: $_"
            }
        }

        # Symlink templates
        if (-not (Test-Path "templates")) {
            try {
                New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates" -ErrorAction Stop | Out-Null
                Write-Success "Created symlink: templates -> .standards\templates"
            } catch {
                Write-Warning "Could not create templates symlink: $_"
            }
        }
    } else {
        Write-Warning "Symlinks require Administrator privileges"
        Write-Info "To create symlinks, run this script as Administrator, or manually access:"
        Write-Host "  - Commands: .standards\.claude\commands\"
        Write-Host "  - Templates: .standards\templates\"
    }
}

Write-Host ""

# Create .clauderc to load CLAUDE.md from submodule
Write-Info "Configuring Claude Code integration..."

$clauderc = @"
# Claude Code Configuration
# Auto-generated by setup-standards.ps1

# Load coding standards from submodule
# This allows Claude Code to access standards even though they're in .standards/
"@

Set-Content -Path ".clauderc" -Value $clauderc
Write-Success "Created .clauderc configuration"

# Update .gitignore
if (Test-Path ".gitignore") {
    $gitignoreContent = Get-Content ".gitignore" -Raw
    if ($gitignoreContent -notmatch "\.clauderc") {
        Add-Content -Path ".gitignore" -Value "`n# Claude Code config (optional)`n.clauderc"
        Write-Success "Updated .gitignore"
    }
} else {
    Set-Content -Path ".gitignore" -Value "# Claude Code config (optional)`n.clauderc"
    Write-Success "Created .gitignore"
}

Write-Host ""
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "║                   Setup Complete!                         ║" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

# Summary
Write-Host "[Standards Location]" -ForegroundColor Cyan
Write-Host "   $ProjectPath\.standards" -ForegroundColor White
Write-Host ""

Write-Host "[Documentation]" -ForegroundColor Cyan
Write-Host "   .standards\README.md          - Overview"
Write-Host "   .standards\USAGE_GUIDE.md     - Practical examples"
Write-Host "   .standards\CLAUDE.md          - AI assistant guide"
Write-Host "   .standards\docs\              - Full documentation"
Write-Host ""

Write-Host "[Available Resources]" -ForegroundColor Cyan
if (Test-Path ".claude") {
    Write-Host "   [OK] .claude\commands\          - Slash commands (symlinked)"
} else {
    Write-Host "   [DIR] .standards\.claude\commands\ - Slash commands"
}

if (Test-Path "templates") {
    Write-Host "   [OK] templates\                 - Code templates (symlinked)"
} else {
    Write-Host "   [DIR] .standards\templates\      - Code templates"
}
Write-Host ""

Write-Host "[Quick Start]" -ForegroundColor Cyan
Write-Host "   1. Read: .standards\USAGE_GUIDE.md"
Write-Host "   2. Use Claude Code slash commands (type / to see list)"
Write-Host "   3. Copy templates from .standards\templates\"
Write-Host "   4. Follow conventions in .standards\docs\"
Write-Host ""

Write-Host "[Update Standards]" -ForegroundColor Cyan
Write-Host "   cd .standards"
Write-Host "   git pull origin main"
Write-Host "   cd .."
Write-Host ""

Write-Host "[Pro Tips]" -ForegroundColor Yellow
Write-Host "   - Commit .gitmodules and .standards to your repo"
Write-Host "   - Team members: run 'git submodule update --init' after clone"
Write-Host "   - Standards update automatically when you pull latest"
Write-Host ""

Pop-Location

Write-Success "Ready to code!"
