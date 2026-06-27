# ==========================================================
# bootstrap_02_dotnet.ps1
# Enterprise Code Intelligence Platform (ECIP)
# Sprint 0 - Part 2
# ==========================================================

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================="
Write-Host "ECIP Bootstrap 02"
Write-Host "Configure Enterprise Solution"
Write-Host "========================================="

if(!(Test-Path "ECIP.sln")){
    Write-Host "Run this script from the ECIP repository root."
    exit
}

# ----------------------------------------------------------
# Create Project References
# ----------------------------------------------------------

function Add-ReferenceIfMissing {

    param(
        [string]$Project,
        [string]$Reference
    )

    $projFile = Get-ChildItem -Recurse -Filter "$Project.csproj" | Select-Object -First 1
    $refFile  = Get-ChildItem -Recurse -Filter "$Reference.csproj" | Select-Object -First 1

    if($projFile -and $refFile){

        $content = Get-Content $projFile.FullName

        if($content -notmatch $Reference){

            dotnet add $projFile.FullName reference $refFile.FullName

            Write-Host "[Added] $Project -> $Reference"

        }
        else{

            Write-Host "[Skip] $Project -> $Reference"

        }

    }

}

Add-ReferenceIfMissing ECIP.Web ECIP.Core
Add-ReferenceIfMissing ECIP.Web ECIP.Infrastructure
Add-ReferenceIfMissing ECIP.Web ECIP.Shared

Add-ReferenceIfMissing ECIP.API ECIP.Core
Add-ReferenceIfMissing ECIP.API ECIP.Infrastructure
Add-ReferenceIfMissing ECIP.API ECIP.Shared

Add-ReferenceIfMissing ECIP.Infrastructure ECIP.Core
Add-ReferenceIfMissing ECIP.Infrastructure ECIP.Shared

Add-ReferenceIfMissing ECIP.RepositoryService ECIP.Core
Add-ReferenceIfMissing ECIP.RepositoryService ECIP.Shared

# ----------------------------------------------------------
# Folder Structure
# ----------------------------------------------------------

$structure = @{

"src/Frontend/ECIP.Web" = @(
"Controllers",
"Views",
"ViewModels",
"Services",
"Configuration",
"wwwroot"
)

"src/Backend/ECIP.API" = @(
"Controllers",
"DTOs",
"Services",
"Interfaces",
"Middlewares",
"Extensions",
"Configuration"
)

"src/Services/ECIP.RepositoryService" = @(
"Controllers",
"Git",
"Parser",
"Scanner",
"Services",
"Tools",
"Configuration"
)

"src/Libraries/ECIP.Core" = @(
"Entities",
"Models",
"Interfaces",
"Enums",
"Constants",
"DTOs",
"Common"
)

"src/Libraries/ECIP.Infrastructure" = @(
"Repositories",
"Persistence",
"Logging",
"Configuration",
"Extensions"
)

"src/Libraries/ECIP.Shared" = @(
"Responses",
"Exceptions",
"Utilities"
)

}

foreach($project in $structure.Keys){

    foreach($folder in $structure[$project]){

        $path = Join-Path $project $folder

        if(!(Test-Path $path)){

            New-Item -ItemType Directory -Force -Path $path | Out-Null

            Write-Host "[Created] $path"

        }

    }

}

# ----------------------------------------------------------
# NuGet Packages
# ----------------------------------------------------------

function Install-PackageIfMissing{

param(
[string]$Project,
[string]$Package
)

$proj = Get-ChildItem -Recurse -Filter "$Project.csproj" | Select-Object -First 1

if($proj){

$content = Get-Content $proj.FullName

if($content -notmatch $Package){

dotnet add $proj.FullName package $Package --no-restore

Write-Host "[Package] $Project -> $Package"

}

}

}

Install-PackageIfMissing ECIP.API Swashbuckle.AspNetCore
Install-PackageIfMissing ECIP.API Serilog.AspNetCore
Install-PackageIfMissing ECIP.API FluentValidation.AspNetCore

Install-PackageIfMissing ECIP.RepositoryService Serilog.AspNetCore

# ----------------------------------------------------------
# Restore
# ----------------------------------------------------------

dotnet restore ECIP.sln

Write-Host ""
Write-Host "====================================="
Write-Host "Bootstrap 02 completed successfully"
Write-Host "====================================="
Write-Host ""
Write-Host "Next:"
Write-Host "Run bootstrap_03_common.ps1"