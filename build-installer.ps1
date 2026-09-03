param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x86",
    [switch]$SelfContained = $true,
    [switch]$SingleFile = $true
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $repoRoot "EmployeeAttestation\EmployeeAttestation.csproj"
$installerScript = Join-Path $repoRoot "installer\EmployeeAttestation.iss"
$publishDir = Join-Path $repoRoot "EmployeeAttestation\bin\$Configuration\net10.0-windows\$Runtime\publish"

$publishArgs = @(
    "publish",
    $projectPath,
    "-c", $Configuration,
    "-r", $Runtime,
    "--self-contained", $SelfContained.ToString().ToLowerInvariant(),
    "/p:PublishSingleFile=$($SingleFile.ToString().ToLowerInvariant())"
)

Write-Host "Publishing application to $publishDir"
dotnet @publishArgs

function Find-InnoSetupCompiler
{
    $command = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($null -ne $command)
    {
        return $command.Source
    }

    $registryPaths = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*",
        "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*"
    )

    foreach ($registryPath in $registryPaths)
    {
        $entry = Get-ItemProperty $registryPath -ErrorAction SilentlyContinue |
            Where-Object { $_.DisplayName -like "*Inno Setup*" } |
            Select-Object -First 1

        if ($null -ne $entry)
        {
            foreach ($candidateRoot in @($entry.InstallLocation, $entry.InnoSetup_Path))
            {
                if ([string]::IsNullOrWhiteSpace($candidateRoot))
                {
                    continue
                }

                $candidate = Join-Path $candidateRoot "ISCC.exe"
                if (Test-Path $candidate)
                {
                    return $candidate
                }
            }
        }
    }

    $wellKnownPaths = @(
        "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe"
    )

    foreach ($candidate in $wellKnownPaths)
    {
        if (Test-Path $candidate)
        {
            return $candidate
        }
    }

    return $null
}

$iscc = Find-InnoSetupCompiler
if ([string]::IsNullOrWhiteSpace($iscc))
{
    Write-Warning "Inno Setup Compiler (ISCC.exe) not found. Install Inno Setup and run:"
    Write-Warning "ISCC /DMyAppPublishDir=`"$publishDir`" `"$installerScript`""
    exit 0
}

Write-Host "Building installer from $installerScript"
& $iscc "/DMyAppPublishDir=$publishDir" $installerScript
