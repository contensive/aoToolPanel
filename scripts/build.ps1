#Requires -Version 5.1
[CmdletBinding()]
param(
    [string]   $LocalDeployTarget  = '',
    [hashtable]$RemoteDeployTarget = $null
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot '..\..\Contensive5\scripts\build-addon-collection.psm1') -Force

$projectRoot = (Resolve-Path "$PSScriptRoot\..").Path

Invoke-ContensiveBuild `
    -CollectionName    'aoToolPanel' `
    -CollectionPath    "$projectRoot\collections\aoToolPanel" `
    -SolutionPath      "$projectRoot\source\aoToolPanel.sln" `
    -BinPath           "$projectRoot\source\aoToolPanel\bin\Release\netstandard2.0" `
    -DeploymentRoot    'C:\Deployments\aoToolPanel' `
    -CleanFolders      @(
                           "$projectRoot\source\aoToolPanel\bin"
                           "$projectRoot\source\aoToolPanel\obj"
                       ) `
    -UiPath            "$projectRoot\ui" `
    -LocalDeployTarget  $LocalDeployTarget `
    -RemoteDeployTarget $RemoteDeployTarget
