param([Parameter(Mandatory=$true)][string]$Executable)
$ErrorActionPreference='Stop'
$taskRoot=Split-Path -Parent $PSScriptRoot
$taskProject=Join-Path $taskRoot 'ProyectoRPGRTS'
$taskExe=[IO.Path]::GetFullPath($Executable)
if(-not(Test-Path -LiteralPath $taskExe)){throw 'No existe la build indicada.'}
$taskEditors=Get-CimInstance Win32_Process -Filter "name='Unity.exe'" | Where-Object {$_.ExecutablePath -like '*6000.5.8f1*' -and $_.CommandLine -like '*ProyectoRPGRTS*'}
if($taskEditors){throw 'Cerrar la validacion del editor antes de medir rendimiento.'}
$taskRun=Join-Path $taskProject ('Logs/E17/Benchmark-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $taskRun | Out-Null
$taskOutput=Join-Path $taskRun 'performance.json'
$taskLog=Join-Path $taskRun 'player.log'
$taskBaseline=Join-Path $taskRoot 'Docs/Validacion/E14E15/EntradaE13/warrior.json'
$taskArgs=@('-batchmode','-aetherniaAlphaBenchmark',('"'+$taskOutput+'"'),'-benchmarkBaseline',('"'+$taskBaseline+'"'),'-screen-width','1280','-screen-height','720','-logFile',('"'+$taskLog+'"'))
$taskProcess=Start-Process -FilePath $taskExe -ArgumentList $taskArgs -WindowStyle Hidden -PassThru
Write-Output "Benchmark nativo PID=$($taskProcess.Id). Sin editor, grabacion ni otros players de QA."
if(-not $taskProcess.WaitForExit(360000)){throw "Proceso sigue activo PID=$($taskProcess.Id); revisar $taskLog. No se fuerza cierre."}
$taskProcess.Refresh()
if($taskProcess.ExitCode -ne 0){throw "Benchmark exit=$($taskProcess.ExitCode). $taskLog"}
if(-not(Test-Path -LiteralPath $taskOutput)){throw 'No existe informe de rendimiento.'}
$taskReport=Get-Content -LiteralPath $taskOutput -Raw | ConvertFrom-Json
if($taskReport.status -ne 'PASS' -or $taskReport.rows.Count -ne 18){throw 'Medicion incompleta.'}
if(Select-String -LiteralPath $taskLog -Pattern 'Exception:|NullReferenceException|Assertion failed' -Quiet){throw 'Error en el registro del benchmark.'}
foreach($taskRow in $taskReport.rows){if($taskRow.renderSubmissions -lt $taskRow.frames-1 -or -not($taskRow.counters | Where-Object {$_.available -and $_.unit -eq 'count' -and $_.mean -gt 0})){throw 'Fila sin render verificado.'}}
Write-Output "PASS 18 escenarios medidos: $taskOutput"
