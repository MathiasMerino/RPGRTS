param(
    [ValidateSet('Baseline', 'GenerateSandbox', 'Foundation')]
    [string]$Mode = 'Baseline'
)

$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
$taskProject = Join-Path $taskRoot 'ProyectoRPGRTS'
$taskVersionText = Get-Content (Join-Path $taskProject 'ProjectSettings/ProjectVersion.txt') -Raw
$taskVersion = [regex]::Match($taskVersionText, 'm_EditorVersion: (\S+)').Groups[1].Value
$taskEditor = "C:\Program Files\Unity\Hub\Editor\$taskVersion\Editor\Unity.exe"
if (-not (Test-Path -LiteralPath $taskEditor)) { throw "Unity $taskVersion no esta instalado." }
$taskLock = Join-Path $taskProject 'Temp/UnityLockfile'
if (Test-Path -LiteralPath $taskLock) {
    # A failed compilation can leave the file behind; an exclusive OS lock proves use.
    try {
        $taskLockProbe = [System.IO.File]::Open($taskLock, 'Open', 'ReadWrite', 'None')
        $taskLockProbe.Dispose()
    } catch {
        throw 'Unity mantiene bloqueado este proyecto. Cierra ese editor antes de iniciar otra instancia.'
    }
}

$taskLogs = Join-Path $taskProject 'Logs'
New-Item -ItemType Directory -Path $taskLogs -Force | Out-Null
$taskLog = Join-Path $taskLogs "$Mode.log"
$taskResult = Join-Path $taskLogs "$Mode-results.xml"
$taskArguments = @('-batchmode', '-projectPath', ('"' + $taskProject + '"'), '-logFile', ('"' + $taskLog + '"'))
if ($Mode -eq 'GenerateSandbox') {
    $taskArguments += @('-nographics', '-quit', '-executeMethod', 'Aethernia.Editor.TechnicalSandboxBuilder.Create')
} else {
    # Let the test runner exit: -quit could stop asynchronous tests prematurely.
    $taskArguments += @('-runTests', '-testPlatform', 'EditMode', '-assemblyNames', 'Aethernia.Editor.Tests', '-testResults', ('"' + $taskResult + '"'))
    if ($Mode -eq 'Baseline') {
        $taskArguments += @('-nographics', '-testFilter', 'Aethernia.Tests.Editor.EnvironmentValidationTests')
    }
    # Foundation uses the graphics device to capture the sandbox off screen.
}

$taskStarted = [DateTime]::UtcNow
$taskProcess = Start-Process -FilePath $taskEditor -ArgumentList $taskArguments -WindowStyle Hidden -PassThru
Write-Output "Unity $taskVersion; modo $Mode; PID $($taskProcess.Id)."
if (-not $taskProcess.WaitForExit(600000)) {
    throw "Unity sigue activo despues de 10 minutos (PID $($taskProcess.Id)). No se ha forzado su cierre. Revisa $taskLog."
}
$taskProcess.Refresh()
if ($taskProcess.ExitCode -ne 0) { throw "Unity termino con codigo $($taskProcess.ExitCode). Revisa $taskLog." }
if ($Mode -ne 'GenerateSandbox') {
    if (-not (Test-Path -LiteralPath $taskResult)) { throw 'Unity no genero resultados; no se considera una prueba correcta.' }
    if ((Get-Item -LiteralPath $taskResult).LastWriteTimeUtc -lt $taskStarted) { throw 'El XML de resultados es anterior a esta ejecucion.' }
    [xml]$taskXml = Get-Content -LiteralPath $taskResult -Raw
    $taskRun = $taskXml.'test-run'
    if ([int]$taskRun.total -eq 0 -or [int]$taskRun.failed -gt 0 -or $taskRun.result -ne 'Passed') {
        throw "Pruebas sin validar: total=$($taskRun.total), fallos=$($taskRun.failed), resultado=$($taskRun.result)."
    }
    Write-Output "PASS: $($taskRun.passed)/$($taskRun.total) pruebas; $taskResult"
}
Write-Output "Finalizado con codigo 0. Log: $taskLog"
