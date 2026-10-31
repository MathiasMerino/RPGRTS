param([switch]$WithGraphics, [switch]$RequireCargo)

$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
$taskProject = Join-Path $taskRoot 'ProyectoRPGRTS'
$taskExe = Join-Path $taskProject 'Builds/Windows/Aethernia.exe'
$taskLabel = if ($WithGraphics) { 'Graphics' } else { 'Final' }
$taskReceipt = Join-Path $taskProject "Logs/$($taskLabel)-build-smoke.txt"
$taskLog = Join-Path $taskProject "Logs/$($taskLabel)-player.log"
if (-not (Test-Path -LiteralPath $taskExe)) { throw 'Primero genera la build con Invoke-UnityValidation.ps1 -Mode Build.' }
$taskStarted = [DateTime]::UtcNow
$taskArgs = @('-batchmode', '-aetherniaSmoke', ('"' + $taskReceipt + '"'), '-logFile', ('"' + $taskLog + '"'))
if (-not $WithGraphics) { $taskArgs += '-nographics' }
$taskProcess = Start-Process -FilePath $taskExe -ArgumentList $taskArgs -WindowStyle Hidden -PassThru
Write-Output ('Player smoke PID: ' + $taskProcess.Id)
if (-not $taskProcess.WaitForExit(180000)) { throw "El player sigue activo (PID $($taskProcess.Id)); consulta el log. No se ha forzado su cierre." }
$taskProcess.Refresh()
if ($taskProcess.ExitCode -ne 0) { throw "El player termino con codigo $($taskProcess.ExitCode). Consulta $taskLog." }
if (-not (Test-Path -LiteralPath $taskReceipt) -or (Get-Item -LiteralPath $taskReceipt).LastWriteTimeUtc -lt $taskStarted) { throw 'Falta un comprobante nuevo del recorrido.' }
$taskText = Get-Content -LiteralPath $taskReceipt -Raw
if (-not $taskText.StartsWith('PASS:')) { throw $taskText }
if (Select-String -LiteralPath $taskLog -Pattern 'Failed to create agent|Exception:|NullReferenceException|Assertion failed' -Quiet) {
    throw "El recorrido termino, pero el player registro un fallo de navegacion o una excepcion. Consulta $taskLog."
}
Write-Output $taskText
if ($RequireCargo) {
    $taskCargoReceipt = $taskReceipt + '.cargo.txt'
    if (-not (Test-Path -LiteralPath $taskCargoReceipt) -or (Get-Item -LiteralPath $taskCargoReceipt).LastWriteTimeUtc -lt $taskStarted) {
        throw 'Falta un comprobante nuevo de la mision con las tres clases. Verifica que compilaste con BuildVillage.'
    }
    $taskCargoText = Get-Content -LiteralPath $taskCargoReceipt -Raw
    if (-not $taskCargoText.StartsWith('PASS:')) { throw $taskCargoText }
    Write-Output $taskCargoText
}
