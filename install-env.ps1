function Check-Command([String]$command) {
    try {
        $cont = Invoke-Expression $command
    }
    catch {
        return $false
    }

    return $true
}

# Check Node.js
if (-not (Check-Command 'node --version')) {
    Write-Host 'Missing Node.js, downloading from Pomelo Cloud.'
    Invoke-WebRequest 'https://resource.pomelo.cloud/third-party/node-v12.16.1-x64.msi' -OutFile 'node-v12.16.1-x64.msi'
    $InstallNodeJsCommand = 'msiexec.exe /i node-v12.16.1-x64.msi /qn'
    Remove-Item -Path 'node-v12.16.1-x64.msi' -Force
} else {
    Write-Host 'Node.js is already installed. Skipped the installation step.'
}

# Check .NET Core SDK 3.1
if (-not (Check-Command 'dotnet') -or -not((Invoke-Expression 'dotnet --version') -contains '3.1')) {
    Write-Host 'Missing .NET Core 3.1 or higher, downloading from Pomelo Cloud.'
    Invoke-WebRequest 'https://resource.pomelo.cloud/third-party/dotnet-sdk-3.1.102-win-x64.exe' -OutFile 'dotnet-sdk-3.1.102-win-x64.exe'
    $InstallNodeJsCommand = '.\dotnet-sdk-3.1.102-win-x64.exe /install /norestart /quiet'
    Remove-Item -Path 'dotnet-sdk-3.1.102-win-x64.exe' -Force
} else {
    Write-Host '.NET Core SDK 3.1 is already installed. Skipped the installation step.'
}

# Download Electron
$RepoPath = Get-Location
$ElectronPath = Join-Path $RepoPath 'Pomelo-Explorer'
$ElectronPath = Join-Path $ElectronPath 'obj'
if (-not (Test-Path $ElectronPath)) {
    New-Item -Path $ElectronPath -ItemType Directory
}

$ElectronPath = Join-Path $ElectronPath 'Host'
if (-not (Test-Path $ElectronPath)) {
    New-Item -Path $ElectronPath -ItemType Directory
}

$ElectronPath = Join-Path $ElectronPath 'node_modules'
if (-not (Test-Path $ElectronPath)) {
    New-Item -Path $ElectronPath -ItemType Directory
}

$ElectronPath = Join-Path $ElectronPath 'electron'
if (-not (Test-Path $ElectronPath)) {
    Write-Host 'Electron node module not found, downloading from Pomelo Cloud...'
    Invoke-WebRequest 'https://resource.pomelo.cloud/third-party/electron.zip' -OutFile 'electron.zip'
    Expand-Archive 'electron.zip' -DestinationPath $ElectronPath
} else {
    Write-Host 'Electron node module is already installed. Skipped the installation step.'
}
# Check Electronize CLI
if (-not (Check-Command 'electronize')) {
    Write-Host 'Electronize CLI is not found, downloading...'
    $InstallElectronizeCommand = 'dotnet tool install --global ElectronNET.CLI'
    Invoke-Expression $InstallNodeJsCommand
}

Write-Host 'The environment setup finished!'