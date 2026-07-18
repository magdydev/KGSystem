<#
.SYNOPSIS
    Starts (or restarts) the KGSystem backend API and/or Angular frontend.

.DESCRIPTION
    Tracks the PID of each process it starts under .run/*.pid at the repo root.
    On every invocation it first stops whatever is already running (matched by
    the saved PID, falling back to whatever process is bound to the expected
    port) and then starts a fresh instance. Output is redirected to log files
    under .run/ so the script itself can return immediately.

.PARAMETER BackendOnly
    Only (re)start the .NET API.

.PARAMETER FrontendOnly
    Only (re)start the Angular dev server.

.PARAMETER StopOnly
    Stop whatever is running and exit without starting anything new.

.EXAMPLE
    .\scripts\Run-App.ps1
    Restarts both backend and frontend.

.EXAMPLE
    .\scripts\Run-App.ps1 -BackendOnly
    Restarts only the API.

.EXAMPLE
    .\scripts\Run-App.ps1 -StopOnly
    Stops both processes without starting them again.
#>
[CmdletBinding()]
param(
    [switch]$BackendOnly,
    [switch]$FrontendOnly,
    [switch]$StopOnly
)

$ErrorActionPreference = 'Stop'

$RepoRoot    = Split-Path -Parent $PSScriptRoot
$BackendDir  = Join-Path $RepoRoot 'backend'
$FrontendDir = Join-Path $RepoRoot 'frontend'
$RunDir      = Join-Path $RepoRoot '.run'

New-Item -ItemType Directory -Path $RunDir -Force | Out-Null

$Targets = @{
    backend = @{
        Enabled  = -not $FrontendOnly
        PidFile  = Join-Path $RunDir 'backend.pid'
        LogFile  = Join-Path $RunDir 'backend.out.log'
        ErrFile  = Join-Path $RunDir 'backend.err.log'
        Port     = 5100
        WorkDir  = $BackendDir
        FilePath = 'dotnet'
        Args     = @('run', '--project', 'src/KGSystem.API')
    }
    frontend = @{
        Enabled  = -not $BackendOnly
        PidFile  = Join-Path $RunDir 'frontend.pid'
        LogFile  = Join-Path $RunDir 'frontend.out.log'
        ErrFile  = Join-Path $RunDir 'frontend.err.log'
        Port     = 4200
        WorkDir  = $FrontendDir
        FilePath = 'npm.cmd'
        Args     = @('start')
    }
}

function Get-PidsOnPort([int]$Port) {
    Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue |
        Select-Object -ExpandProperty OwningProcess -Unique
}

function Stop-Target([string]$Name, [hashtable]$Target) {
    $stoppedAny = $false
    $pidsToStop = [System.Collections.Generic.HashSet[int]]::new()

    if (Test-Path $Target.PidFile) {
        $savedPid = Get-Content $Target.PidFile -ErrorAction SilentlyContinue
        if ($savedPid -and (Get-Process -Id $savedPid -ErrorAction SilentlyContinue)) {
            [void]$pidsToStop.Add([int]$savedPid)
        }
        Remove-Item $Target.PidFile -Force -ErrorAction SilentlyContinue
    }

    foreach ($procId in Get-PidsOnPort $Target.Port) {
        [void]$pidsToStop.Add([int]$procId)
    }

    foreach ($procId in $pidsToStop) {
        # A process already reaped by an earlier tree-kill in this same loop
        # is not an error - just skip it.
        if (-not (Get-Process -Id $procId -ErrorAction SilentlyContinue)) { continue }
        Write-Host "  Stopping $Name process (PID $procId)..." -ForegroundColor Yellow
        try {
            # /T kills the whole process tree (dotnet/ng spawn child processes).
            # taskkill writes to stderr on failure, which PowerShell promotes to
            # a terminating error under $ErrorActionPreference = 'Stop' - a lost
            # race (process already gone) is not a real failure, so swallow it.
            taskkill /PID $procId /T /F *> $null
        } catch {
        }
        $stoppedAny = $true
    }

    if (-not $stoppedAny) {
        Write-Host "  $Name is not currently running." -ForegroundColor DarkGray
    }
}

function Start-Target([string]$Name, [hashtable]$Target) {
    Write-Host "  Starting $Name (logs: $($Target.LogFile))..." -ForegroundColor Green
    $proc = Start-Process -FilePath $Target.FilePath `
        -ArgumentList $Target.Args `
        -WorkingDirectory $Target.WorkDir `
        -RedirectStandardOutput $Target.LogFile `
        -RedirectStandardError $Target.ErrFile `
        -WindowStyle Hidden `
        -PassThru

    Set-Content -Path $Target.PidFile -Value $proc.Id
    Write-Host "  $Name started (PID $($proc.Id)), listening on port $($Target.Port)." -ForegroundColor Green
}

foreach ($name in $Targets.Keys) {
    $target = $Targets[$name]
    if (-not $target.Enabled) { continue }

    Write-Host "== $name ==" -ForegroundColor Cyan
    Stop-Target -Name $name -Target $target

    if (-not $StopOnly) {
        Start-Target -Name $name -Target $target
    }
}

if (-not $StopOnly) {
    Write-Host "`nDone. Tail logs with: Get-Content .run\backend.out.log -Wait" -ForegroundColor Cyan
}
