﻿param(
    [String] $GitUser,
    [String] $GitEmail,
    [String] $GitPAT,
    [String] $WorkingPath
)

Write-Host 'Starting Sync GitHub Script Repository From Azure Repos...'

if ($GitPAT -eq $null) {
    Throw 'Git PAT is invalid'
    return
}

if ($WorkingPath -eq $null) {
    Throw 'Working path is invalid'
    return
}

if (-not (Test-Path $WorkingPath)) {
    New-Item -Path $WorkingPath -ItemType Directory
}

$AzureRepoPath = Get-Location
Set-Location $WorkingPath
# Build Clone Command
$CloneCommand = 'git clone https://'+ $GitUser + ':' + $GitPAT +'@github.com/yukozh/Pomelo-Explorer.git'
Invoke-Expression $CloneCommand
$GitHubRepoPath = Join-Path $WorkingPath 'Pomelo-Explorer'

Get-ChildItem $AzureRepoPath | Where { !(($_ -is [System.IO.DirectoryInfo]) -and ($_.Name -eq ".git")) } | Copy-Item -Destination $GitHubRepoPath -Recurse -Force

Set-Location $GitHubRepoPath
$ConfigPath = Join-Path $GitHubRepoPath '.git'
$ConfigPath = Join-Path $ConfigPath 'config'
$CommitCommand = 'git -c user.name="' + $GitUser + '" -c user.email="' + $GitEmail + '" commit -a -m "Auto sync from Azure DevOps"';

git add -A
Invoke-Expression $CommitCommand
git push

Write-Host 'The changes has been pushed to GitHub.'

Remove-Item -Path $WorkingPath -Recurse

Write-Host 'Removed temp folder.'

Set-Location $AzureRepoPath