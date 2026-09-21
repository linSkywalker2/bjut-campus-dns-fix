[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourcePath = Join-Path $projectRoot 'src\CampusDnsFix.cs'
$manifestPath = Join-Path $projectRoot 'src\app.manifest'
$releasePath = Join-Path $projectRoot 'release'
$outputPath = Join-Path $releasePath '北工大校园网DNS修复.exe'

$compilerCandidates = @(
    (Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'),
    (Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe')
)
$compiler = $compilerCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1

if (-not $compiler) {
    throw '未找到 .NET Framework C# 编译器。请确认已安装 .NET Framework 4.x。'
}

New-Item -ItemType Directory -Force -Path $releasePath | Out-Null

& $compiler `
    /nologo `
    /target:winexe `
    /optimize+ `
    /platform:anycpu `
    "/win32manifest:$manifestPath" `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.Drawing.dll `
    /reference:System.Windows.Forms.dll `
    "/out:$outputPath" `
    $sourcePath

if ($LASTEXITCODE -ne 0) {
    throw "编译失败，csc.exe 退出代码：$LASTEXITCODE"
}

$hash = Get-FileHash -LiteralPath $outputPath -Algorithm SHA256
Write-Host "构建完成：$outputPath"
Write-Host "SHA-256：$($hash.Hash)"
