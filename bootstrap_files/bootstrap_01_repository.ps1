# ==========================================================
# bootstrap_01_repository.ps1
# Enterprise Code Intelligence Platform (ECIP)
# Sprint 0 - Part 1
# Creates repository structure, solution and .NET projects
# ==========================================================

$ErrorActionPreference = "Stop"

$Root = "EnterpriseCodeIntelligencePlatform"

Write-Host "===== ECIP Bootstrap 01 ====="

# Check prerequisites
dotnet --version | Out-Null
git --version | Out-Null

if (!(Test-Path $Root)) {
    New-Item -ItemType Directory -Path $Root | Out-Null
}

Set-Location $Root

# Root folders
$folders = @(
"configuration",
"docs",
"prompt-registry",
"repositories",
"workspace",
"cache",
"logs",
"scripts",
"tests",
"src",
"src\\Frontend",
"src\\Backend",
"src\\AI",
"src\\Services",
"src\\Libraries"
)

foreach($folder in $folders){
    if(!(Test-Path $folder)){
        New-Item -ItemType Directory -Force -Path $folder | Out-Null
        Write-Host "[OK] $folder"
    }
}

# Solution
if(!(Test-Path "ECIP.sln")){
    dotnet new sln -n ECIP
}

Push-Location src

# Create projects
if(!(Test-Path "Frontend\\ECIP.Web")){
    dotnet new mvc -o Frontend/ECIP.Web -n ECIP.Web
}

if(!(Test-Path "Backend\\ECIP.API")){
    dotnet new webapi -o Backend/ECIP.API -n ECIP.API
}

if(!(Test-Path "Services\\ECIP.RepositoryService")){
    dotnet new webapi -o Services/ECIP.RepositoryService -n ECIP.RepositoryService
}

if(!(Test-Path "Libraries\\ECIP.Core")){
    dotnet new classlib -o Libraries/ECIP.Core -n ECIP.Core
}

if(!(Test-Path "Libraries\\ECIP.Infrastructure")){
    dotnet new classlib -o Libraries/ECIP.Infrastructure -n ECIP.Infrastructure
}

if(!(Test-Path "Libraries\\ECIP.Shared")){
    dotnet new classlib -o Libraries/ECIP.Shared -n ECIP.Shared
}

Pop-Location

# Add projects to solution
$projects = Get-ChildItem -Recurse -Filter *.csproj

foreach($proj in $projects){
    $exists = (dotnet sln ECIP.sln list) | Select-String $proj.Name
    if(-not $exists){
        dotnet sln ECIP.sln add $proj.FullName
    }
}

Write-Host ""
Write-Host "==================================="
Write-Host "Bootstrap 01 completed successfully"
Write-Host "==================================="
Write-Host ""
Write-Host "Next:"
Write-Host "Run bootstrap_02_dotnet.ps1"
