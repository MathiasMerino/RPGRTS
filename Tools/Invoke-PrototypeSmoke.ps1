$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
$taskProject = Join-Path $taskRoot 'ProyectoRPGRTS'
$taskExe = Join-Path $taskProject 'Builds/Windows/Aethernia.exe'
$taskReceipt = Join-Path $taskProject 'Logs/Final-build-smoke.txt'
$taskLog = Join-Path $taskProject 'Logs/Final-player.log'
if (-not (Test-Path -LiteralPath $taskExe)) { throw 'Primero genera la build con Invoke-UnityValidation.ps1 -Mode Build.' }
$taskStarted = [DateTime]::UtcNow
$taskArgs = @('-batchmode', '-nographics', '-aetherniaSmoke', ('"' + $taskReceipt + '"'), '-logFile', ('"' + $taskLog + '"'))
$taskProcess = Start-Process -FilePath $taskExe -ArgumentList $taskArgs -WindowStyle Hidden -PassThru
Write-Output ('Player smoke PID: ' + $taskProcess.Id)
if (-not $taskProcess.WaitForExit(180000)) { throw "El player sigue activo (PID $($taskProcess.Id)); consulta el log. No se ha forzado su cierre." }
$taskProcess.Refresh()
if ($taskProcess.ExitCode -ne 0) { throw "El player termino con codigo $($taskProcess.ExitCode). Consulta $taskLog." }
if (-not (Test-Path -LiteralPath $taskReceipt) -or (Get-Item -LiteralPath $taskReceipt).LastWriteTimeUtc -lt $taskStarted) { throw 'Falta un comprobante nuevo del recorrido.' }
$taskText = Get-Content -LiteralPath $taskReceipt -Raw
if (-not $taskText.StartsWith('PASS:')) { throw $taskText }
Write-Output $taskText
