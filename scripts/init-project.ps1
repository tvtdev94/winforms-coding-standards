# WinForms Project Initialization Script
# Creates a new WinForms project following coding standards

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,

    [Parameter(Mandatory=$false)]
    [ValidateSet('net8.0', 'net6.0', 'net48')]
    [string]$Framework = 'net8.0',

    [Parameter(Mandatory=$false)]
    [switch]$IncludeTests = $true,

    [Parameter(Mandatory=$false)]
    [switch]$IncludeExampleCode = $false,

    [Parameter(Mandatory=$false)]
    [switch]$IntegrateStandards = $true,

    [Parameter(Mandatory=$false)]
    [string]$StandardsRepo = ""
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "WinForms Project Initialization" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Project Name: $ProjectName"
Write-Host "Framework: $Framework"
Write-Host "Include Tests: $IncludeTests"
Write-Host "Include Example Code: $IncludeExampleCode"
Write-Host "Integrate Standards: $IntegrateStandards"
Write-Host ""

# ============================================================================
# Step 1: Create Solution
# ============================================================================
Write-Host "[1] Creating solution..." -ForegroundColor Cyan

if (Test-Path $ProjectName) {
    Write-Host "[ERROR] Directory '$ProjectName' already exists!" -ForegroundColor Red
    exit 1
}

dotnet new sln -n $ProjectName -o $ProjectName
Set-Location $ProjectName

Write-Host "  [OK] Solution created" -ForegroundColor Green

# ============================================================================
# Step 2: Create Main Project
# ============================================================================
Write-Host ""
Write-Host "[2] Creating WinForms project..." -ForegroundColor Cyan

dotnet new winforms -n $ProjectName -f $Framework
dotnet sln add "$ProjectName/$ProjectName.csproj"

Write-Host "  [OK] WinForms project created" -ForegroundColor Green

# ============================================================================
# Step 3: Create Folder Structure
# ============================================================================
Write-Host ""
Write-Host "[3] Creating folder structure..." -ForegroundColor Cyan

$folders = @(
    @{Name="Models"; Template="// Place your data models here`n// Example: Customer.cs, Order.cs`n"},
    @{Name="Services"; Template="// Place your business logic services here`n// Example: CustomerService.cs, OrderService.cs`n"},
    @{Name="Repositories"; Template="// Place your data access repositories here`n// Example: CustomerRepository.cs, OrderRepository.cs`n"},
    @{Name="Forms"; Template=$null},  # MainForm will be moved here
    @{Name="Views"; Template="// Place your view interfaces here (for MVP pattern)`n// Example: ICustomerView.cs, IOrderView.cs`n"},
    @{Name="Presenters"; Template="// Place your presenters here (for MVP pattern)`n// Example: CustomerPresenter.cs, OrderPresenter.cs`n"},
    @{Name="Data"; Template="// Place your DbContext and configurations here`n// Example: AppDbContext.cs, EntityConfigurations/`n"},
    @{Name="Utils"; Template="// Place your utility classes and extensions here`n// Example: StringExtensions.cs, DateHelper.cs`n"},
    @{Name="Resources"; Template="// Place your resources here`n// Example: Icons/, Images/, Strings.resx`n"}
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Path "$ProjectName/$($folder.Name)" -Force | Out-Null

    # Add README.md to help Rider/VS show the folder
    if ($folder.Template) {
        $readmeContent = "# $($folder.Name)`n`n$($folder.Template)"
        $readmeContent | Out-File -FilePath "$ProjectName/$($folder.Name)/README.md" -Encoding UTF8 -Force
    }

    Write-Host "  [OK] Created $($folder.Name)/" -ForegroundColor Green
}

# Move Form1 to Forms folder and rename to MainForm
Move-Item -Path "$ProjectName/Form1.cs" -Destination "$ProjectName/Forms/MainForm.cs" -Force
Move-Item -Path "$ProjectName/Form1.Designer.cs" -Destination "$ProjectName/Forms/MainForm.Designer.cs" -Force
if (Test-Path "$ProjectName/Form1.resx") {
    Move-Item -Path "$ProjectName/Form1.resx" -Destination "$ProjectName/Forms/MainForm.resx" -Force
}

# Update MainForm.cs namespace
$mainFormContent = Get-Content "$ProjectName/Forms/MainForm.cs" -Raw
$mainFormContent = $mainFormContent -replace "namespace $ProjectName", "namespace $ProjectName.Forms"
$mainFormContent = $mainFormContent -replace "partial class Form1", "partial class MainForm"
$mainFormContent = $mainFormContent -replace "public Form1\(\)", "public MainForm()"
$mainFormContent | Out-File -FilePath "$ProjectName/Forms/MainForm.cs" -Encoding UTF8 -Force

# Update MainForm.Designer.cs
$designerContent = Get-Content "$ProjectName/Forms/MainForm.Designer.cs" -Raw
$designerContent = $designerContent -replace "namespace $ProjectName", "namespace $ProjectName.Forms"
$designerContent = $designerContent -replace "partial class Form1", "partial class MainForm"
$designerContent = $designerContent -replace "Form1", "MainForm"
$designerContent | Out-File -FilePath "$ProjectName/Forms/MainForm.Designer.cs" -Encoding UTF8 -Force

Write-Host "  [OK] Moved and renamed Form1 to MainForm in Forms/" -ForegroundColor Green

# ============================================================================
# Step 4: Add NuGet Packages
# ============================================================================
Write-Host ""
Write-Host "[4] Adding NuGet packages..." -ForegroundColor Cyan

$packages = @(
    "Microsoft.Extensions.DependencyInjection",
    "Microsoft.Extensions.Configuration.Json",
    "Microsoft.Extensions.Logging",
    "Serilog.Extensions.Logging",
    "Serilog.Sinks.File",
    "Microsoft.EntityFrameworkCore.Sqlite",
    "Microsoft.EntityFrameworkCore.Design"
)

foreach ($package in $packages) {
    Write-Host "  Adding $package..." -NoNewline
    dotnet add $ProjectName package $package --no-restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

Write-Host "  Restoring packages..." -NoNewline
dotnet restore $ProjectName | Out-Null
Write-Host " [OK]" -ForegroundColor Green

# Add README.md files to .csproj so they show in Rider/VS
$csprojPath = "$ProjectName/$ProjectName.csproj"
$csprojContent = Get-Content $csprojPath -Raw

# Add ItemGroup for README files if not already present
if (-not $csprojContent.Contains("<None Include=")) {
    $readmeItemGroup = @"

  <ItemGroup>
    <None Include="Models\README.md" />
    <None Include="Services\README.md" />
    <None Include="Repositories\README.md" />
    <None Include="Views\README.md" />
    <None Include="Presenters\README.md" />
    <None Include="Data\README.md" />
    <None Include="Utils\README.md" />
    <None Include="Resources\README.md" />
  </ItemGroup>
"@
    $endProjectTag = '</Project>'
    $csprojContent = $csprojContent.Replace($endProjectTag, $readmeItemGroup + [Environment]::NewLine + $endProjectTag)
    $csprojContent | Out-File -FilePath $csprojPath -Encoding UTF8 -Force
}

# ============================================================================
# Step 5: Create appsettings.json
# ============================================================================
Write-Host ""
Write-Host "[5] Creating appsettings.json..." -ForegroundColor Cyan

$appsettingsHash = @{
    ConnectionStrings = @{
        DefaultConnection = "Data Source=$ProjectName.db"
    }
    Logging = @{
        LogLevel = @{
            Default = "Information"
            Microsoft = "Warning"
            "Microsoft.EntityFrameworkCore" = "Information"
        }
    }
}

$appsettingsHash | ConvertTo-Json -Depth 10 | Out-File -FilePath "$ProjectName/appsettings.json" -Encoding UTF8

# Update .csproj to copy appsettings.json
$csprojPath = "$ProjectName/$ProjectName.csproj"
$csprojContent = Get-Content $csprojPath -Raw
$itemGroupXml = @'
  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
'@

$endProjectTag = '</Project>'
$csprojContent = $csprojContent.Replace($endProjectTag, $itemGroupXml + [Environment]::NewLine + $endProjectTag)
$csprojContent | Out-File -FilePath $csprojPath -Encoding UTF8

Write-Host "  [OK] appsettings.json created" -ForegroundColor Green

# ============================================================================
# Step 6: Create Program.cs with DI
# ============================================================================
Write-Host ""
Write-Host "[6] Creating Program.cs with DI..." -ForegroundColor Cyan

$programCs = @'
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using {0}.Forms;

namespace {0}
{{
    internal static class Program
    {{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {{
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {{
                ApplicationConfiguration.Initialize();

                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // Build DI container
                var services = new ServiceCollection();
                ConfigureServices(services, configuration);
                var serviceProvider = services.BuildServiceProvider();

                // Run application
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }}
            catch (Exception ex)
            {{
                Log.Fatal(ex, "Application terminated unexpectedly");
                MessageBox.Show(
                    $"Fatal error: {{ex.Message}}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }}
            finally
            {{
                Log.CloseAndFlush();
            }}
        }}

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {{
            // Configuration
            services.AddSingleton(configuration);

            // Logging
            services.AddLogging(builder =>
            {{
                builder.ClearProviders();
                builder.AddSerilog();
            }});

            // TODO: Add your services here
            // services.AddDbContext<AppDbContext>(options =>
            //     options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // services.AddScoped<IYourRepository, YourRepository>();
            // services.AddScoped<IYourService, YourService>();

            // Forms
            services.AddTransient<MainForm>();
        }}
    }}
}}
'@

$programCsFinal = $programCs -f $ProjectName
$programCsFinal | Out-File -FilePath "$ProjectName/Program.cs" -Encoding UTF8 -Force

Write-Host "  [OK] Program.cs created with DI" -ForegroundColor Green

# ============================================================================
# Step 7: Create Test Project (if requested)
# ============================================================================
if ($IncludeTests) {
    Write-Host ""
    Write-Host "[7] Creating test projects..." -ForegroundColor Cyan

    # Unit tests
    dotnet new xunit -n "$ProjectName.Tests" -f $Framework
    dotnet sln add "$ProjectName.Tests/$ProjectName.Tests.csproj"
    dotnet add "$ProjectName.Tests" reference $ProjectName
    dotnet add "$ProjectName.Tests" package Moq --no-restore
    dotnet add "$ProjectName.Tests" package FluentAssertions --no-restore

    Write-Host "  [OK] Unit test project created" -ForegroundColor Green

    # Integration tests
    dotnet new xunit -n "$ProjectName.IntegrationTests" -f $Framework
    dotnet sln add "$ProjectName.IntegrationTests/$ProjectName.IntegrationTests.csproj"
    dotnet add "$ProjectName.IntegrationTests" reference $ProjectName
    dotnet add "$ProjectName.IntegrationTests" package Microsoft.EntityFrameworkCore.Sqlite --no-restore

    Write-Host "  [OK] Integration test project created" -ForegroundColor Green

    # Restore packages
    Write-Host "  Restoring test packages..." -NoNewline
    dotnet restore | Out-Null
    Write-Host " [OK]" -ForegroundColor Green
}

# ============================================================================
# Step 8: Setup configuration files (.editorconfig and .gitignore)
# ============================================================================
Write-Host ""
Write-Host "[8] Setting up configuration files..." -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $PSCommandPath
$repoRoot = Split-Path -Parent $scriptPath

# These files will be symlinked if standards are integrated (auto-update)
# Otherwise they will be copied (one-time)
$configFiles = @(".editorconfig", ".gitignore")

foreach ($file in $configFiles) {
    if (Test-Path "$repoRoot/$file") {
        # Just copy for now - will be converted to symlink in step 10 if integrating standards
        Copy-Item "$repoRoot/$file" -Destination "." -Force
        Write-Host "  [OK] $file copied" -ForegroundColor Green
    }
}

# ============================================================================
# Step 9: Create VS Code configuration
# ============================================================================
Write-Host ""
Write-Host "[9] Creating VS Code tasks and launch config..." -ForegroundColor Cyan

New-Item -ItemType Directory -Path ".vscode" -Force | Out-Null

# Create tasks.json (using string template to preserve ${workspaceFolder})
$tasksJson = @"
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "`${workspaceFolder}/$ProjectName.sln",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "`$msCompile",
      "group": {
        "kind": "build",
        "isDefault": true
      }
    },
    {
      "label": "clean",
      "command": "dotnet",
      "type": "process",
      "args": ["clean", "`${workspaceFolder}/$ProjectName.sln"],
      "problemMatcher": "`$msCompile"
    },
    {
      "label": "rebuild",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "`${workspaceFolder}/$ProjectName.sln",
        "--no-incremental",
        "/property:GenerateFullPaths=true"
      ],
      "problemMatcher": "`$msCompile",
      "dependsOn": "clean"
    },
    {
      "label": "run",
      "command": "dotnet",
      "type": "process",
      "args": ["run", "--project", "`${workspaceFolder}/$ProjectName/$ProjectName.csproj"],
      "problemMatcher": "`$msCompile",
      "dependsOn": "build"
    },
    {
      "label": "test",
      "command": "dotnet",
      "type": "process",
      "args": ["test", "`${workspaceFolder}/$ProjectName.sln"],
      "problemMatcher": "`$msCompile",
      "group": {
        "kind": "test",
        "isDefault": true
      }
    },
    {
      "label": "test-verbose",
      "command": "dotnet",
      "type": "process",
      "args": ["test", "`${workspaceFolder}/$ProjectName.sln", "--verbosity", "normal"],
      "problemMatcher": "`$msCompile"
    },
    {
      "label": "watch",
      "command": "dotnet",
      "type": "process",
      "args": ["watch", "run", "--project", "`${workspaceFolder}/$ProjectName/$ProjectName.csproj"],
      "problemMatcher": "`$msCompile"
    }
  ]
}
"@

$tasksJson | Out-File -FilePath ".vscode/tasks.json" -Encoding UTF8 -Force
Write-Host "  [OK] .vscode/tasks.json created" -ForegroundColor Green

# Create launch.json (using string template to preserve ${workspaceFolder})
$launchJson = @"
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Launch (WinForms)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "`${workspaceFolder}/$ProjectName/bin/Debug/$Framework-windows/$ProjectName.dll",
      "args": [],
      "cwd": "`${workspaceFolder}/$ProjectName",
      "console": "internalConsole",
      "stopAtEntry": false
    },
    {
      "name": ".NET Attach",
      "type": "coreclr",
      "request": "attach"
    }
  ]
}
"@

$launchJson | Out-File -FilePath ".vscode/launch.json" -Encoding UTF8 -Force
Write-Host "  [OK] .vscode/launch.json created" -ForegroundColor Green

# ============================================================================
# Step 10: Initialize Git
# ============================================================================
Write-Host ""
Write-Host "[10] Initializing git repository..." -ForegroundColor Cyan

git init | Out-Null
git add . | Out-Null
git commit -m "Initial commit: Project structure created by init-project.ps1" | Out-Null

Write-Host "  [OK] Git repository initialized" -ForegroundColor Green

# ============================================================================
# Step 11: Integrate Coding Standards (if requested)
# ============================================================================
if ($IntegrateStandards) {
    Write-Host ""
    Write-Host "[11] Integrating coding standards..." -ForegroundColor Cyan

    # Auto-detect standards repo URL
    if (-not $StandardsRepo) {
        Push-Location $repoRoot
        $StandardsRepo = git remote get-url origin 2>$null
        Pop-Location
    }

    if ($StandardsRepo) {
        Write-Host "  Using standards repo: $StandardsRepo"

        # Add as submodule (suppress all output)
        Start-Process git -ArgumentList "submodule","add",$StandardsRepo,".standards" -NoNewWindow -Wait -RedirectStandardError "$env:TEMP\git_stderr.txt" -RedirectStandardOutput "$env:TEMP\git_stdout.txt"

        # Check if submodule was actually added
        if (Test-Path ".standards/.git") {
            git submodule update --init --recursive *>&1 | Out-Null
            Write-Host "  [OK] Standards added as submodule" -ForegroundColor Green

            # Try to create symlinks (requires Admin on Windows)
            $isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

            if ($isAdmin) {
                # Symlink for .claude (slash commands)
                try {
                    New-Item -ItemType SymbolicLink -Path ".claude" -Target ".standards\.claude" -ErrorAction Stop | Out-Null
                    Write-Host "  [OK] Symlink created: .claude" -ForegroundColor Green
                } catch {
                    Write-Host "  [WARN]  Could not create .claude symlink" -ForegroundColor Yellow
                }

                # Symlink for templates
                try {
                    New-Item -ItemType SymbolicLink -Path "templates" -Target ".standards\templates" -ErrorAction Stop | Out-Null
                    Write-Host "  [OK] Symlink created: templates" -ForegroundColor Green
                } catch {
                    Write-Host "  [WARN]  Could not create templates symlink" -ForegroundColor Yellow
                }

                # Symlink for config files (auto-update when standards update)
                $configFiles = @(".editorconfig", ".gitignore")
                foreach ($file in $configFiles) {
                    if (Test-Path ".standards/$file") {
                        try {
                            # Remove copied file first
                            if (Test-Path $file) {
                                Remove-Item $file -Force
                            }
                            # Create symlink
                            New-Item -ItemType SymbolicLink -Path $file -Target ".standards\$file" -ErrorAction Stop | Out-Null
                            Write-Host "  [OK] Symlink created: $file (auto-updates)" -ForegroundColor Green
                        } catch {
                            Write-Host "  [WARN]  Could not create $file symlink, using copy" -ForegroundColor Yellow
                            # Restore copied version if symlink fails
                            Copy-Item ".standards/$file" -Destination "." -Force
                        }
                    }
                }
            } else {
                Write-Host "  [WARN]  Run as Admin to create symlinks for auto-update" -ForegroundColor Yellow
                Write-Host "         Config files (.editorconfig, .gitignore) were copied" -ForegroundColor Yellow
                Write-Host "         Use update-standards.ps1 script to sync them manually" -ForegroundColor Yellow
            }

            # Commit submodule
            git add .gitmodules .standards
            git commit -m "Add coding standards as submodule" | Out-Null
            Write-Host "  [OK] Standards integration complete" -ForegroundColor Green

            # Create .claude/settings.local.json with pre-approved permissions
            Write-Host ""
            Write-Host "  Creating .claude/settings.local.json with permissions..." -ForegroundColor Cyan

            # Get current directory path for permissions
            $currentPath = (Get-Location).Path -replace '\\', '/'

            $settingsJson = @"
{
  "permissions": {
    "allow": [
      "Bash(dotnet build:*)",
      "Bash(dotnet run:*)",
      "Bash(dotnet test:*)",
      "Bash(dotnet clean:*)",
      "Bash(dotnet restore:*)",
      "Bash(dotnet add:*)",
      "Bash(dotnet new:*)",
      "Bash(dotnet publish:*)",
      "Bash(dotnet pack:*)",
      "Bash(git status:*)",
      "Bash(git log:*)",
      "Bash(git diff:*)",
      "Bash(git add:*)",
      "Bash(git commit:*)",
      "Bash(git submodule:*)",
      "Bash(powershell.exe:*)",
      "Bash(pwsh:*)",
      "Bash(cat:*)",
      "Bash(ls:*)",
      "Bash(find:*)",
      "Bash(grep:*)",
      "Bash(tree:*)",
      "Bash(tail:*)",
      "Bash(head:*)",
      "Bash(wc:*)",
      "Read($currentPath/**)",
      "Edit($currentPath/**)",
      "Write($currentPath/**)",
      "Glob($currentPath/**)",
      "Grep($currentPath/**)",
      "SlashCommand(/create-form)",
      "SlashCommand(/create-service)",
      "SlashCommand(/create-repository)",
      "SlashCommand(/create-dialog)",
      "SlashCommand(/create-custom-control)",
      "SlashCommand(/add-validation)",
      "SlashCommand(/add-error-handling)",
      "SlashCommand(/add-logging)",
      "SlashCommand(/add-settings)",
      "SlashCommand(/add-data-binding)",
      "SlashCommand(/setup-di)",
      "SlashCommand(/refactor-to-mvp)",
      "SlashCommand(/fix-threading)",
      "SlashCommand(/optimize-performance)",
      "SlashCommand(/check-standards)",
      "SlashCommand(/add-test)",
      "SlashCommand(/auto-implement)"
    ],
    "deny": [
      "Bash(git push:*)",
      "Bash(git pull:*)",
      "Bash(git checkout:*)",
      "Bash(git branch -D:*)",
      "Bash(git merge:*)",
      "Bash(git rebase:*)",
      "Bash(git reset --hard:*)",
      "Bash(rm -rf:*)",
      "Bash(chmod:*)"
    ],
    "ask": []
  }
}
"@

            New-Item -ItemType Directory -Path ".claude" -Force | Out-Null
            $settingsJson | Out-File -FilePath ".claude/settings.local.json" -Encoding UTF8 -Force
            Write-Host "  [OK] Claude permissions configured" -ForegroundColor Green
        } else {
            Write-Host "  [WARN]  Could not add standards submodule" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [WARN]  Standards repo URL not detected, skipping" -ForegroundColor Yellow
    }
}

# ============================================================================
# Step 12: Install Git Hooks (if available)
# ============================================================================
if (Test-Path "$repoRoot/.githooks") {
    Write-Host ""
    Write-Host "[12] Installing git hooks..." -ForegroundColor Cyan

    Copy-Item "$repoRoot/.githooks" -Destination ".githooks" -Recurse -Force
    git config core.hooksPath .githooks

    Write-Host "  [OK] Git hooks installed" -ForegroundColor Green
}

# ============================================================================
# Summary
# ============================================================================
Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "Project created successfully!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "[Location] $(Get-Location)" -ForegroundColor Cyan
Write-Host ""
Write-Host "[Next steps]" -ForegroundColor Yellow
Write-Host "  1. cd $ProjectName"
Write-Host "  2. Open $ProjectName.sln in Visual Studio"
Write-Host "  3. Start coding with MVP pattern!"
Write-Host ""
Write-Host "[Useful commands]" -ForegroundColor Yellow
Write-Host "  dotnet build              # Build project"
Write-Host "  dotnet run --project $ProjectName  # Run application"
if ($IncludeTests) {
    Write-Host "  dotnet test               # Run all tests"
}
Write-Host ""

if ($IntegrateStandards -and (Test-Path ".standards")) {
    Write-Host "[Coding Standards]" -ForegroundColor Yellow
    Write-Host "  .standards/USAGE_GUIDE.md     # Practical examples"
    Write-Host "  .standards/CLAUDE.md          # AI assistant guide"
    Write-Host "  .standards/docs/              # Full documentation"
    if (Test-Path ".claude") {
        Write-Host "  Type / in Claude Code         # See slash commands"
    } else {
        Write-Host "  .standards/.claude/commands/  # Slash commands"
    }
    Write-Host ""
    Write-Host "[Update standards]" -ForegroundColor Yellow
    Write-Host "  cd .standards && git pull && cd .."
} else {
    Write-Host "[Documentation]" -ForegroundColor Yellow
    Write-Host "  See USAGE_GUIDE.md for practical examples"
    Write-Host "  See docs/ folder for detailed guidelines"
}
Write-Host ""
