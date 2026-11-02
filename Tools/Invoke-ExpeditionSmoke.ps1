param([switch]$WithGraphics,[switch]$Record,[ValidateSet('warrior','monk','mage')][string[]]$Classes=@('warrior','monk','mage'),[string]$Executable='')
$ErrorActionPreference='Stop'
$taskRoot=Split-Path -Parent $PSScriptRoot
$taskProject=Join-Path $taskRoot 'ProyectoRPGRTS'
$taskExe=Join-Path $taskProject 'Builds/ExpeditionsWindows/Aethernia.exe'
if($Executable){$taskExe=[IO.Path]::GetFullPath($Executable)}
if(-not(Test-Path -LiteralPath $taskExe)){throw 'Genera BuildExpeditions primero.'}
$taskLabel=if($WithGraphics){'GPU'}else{'Headless'}
$taskRun=Join-Path $taskProject ('Logs/ExpeditionQA/'+$taskLabel+'-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $taskRun -Force | Out-Null
foreach($taskClass in $Classes){
    $taskSlot=Join-Path $taskRun ($taskClass+'.json')
    # Partidas de una ejecución real del player E13; sin recursos ni salud añadidos.
    $taskBaseline=Join-Path $taskRoot ('Docs/Validacion/E14E15/EntradaE13/'+$taskClass+'.json')
    Copy-Item -LiteralPath $taskBaseline -Destination $taskSlot
    Get-FileHash -LiteralPath $taskBaseline -Algorithm SHA256 | Select-Object Path,Hash | ConvertTo-Json | Set-Content ($taskSlot+'.baseline.json')
    foreach($taskResume in @($false,$true)){
        $taskSuffix=if($taskResume){'resume'}else{'start'}
        $taskReceipt=$taskSlot+'.'+$taskSuffix+'.txt'
        $taskLog=Join-Path $taskRun ($taskClass+'-'+$taskSuffix+'.log')
        $taskArgs=@('-batchmode','-aetherniaExpeditionSmoke',('"'+$taskSlot+'"'),'-screen-width','1280','-screen-height','720','-logFile',('"'+$taskLog+'"'))
        if(-not $WithGraphics){$taskArgs+='-nographics'}
        if($taskResume){$taskArgs+='-demoResume'}
        if($Record -and $WithGraphics -and -not $taskResume -and $taskClass -eq $Classes[0]){$taskArgs+='-demoRecord'}
        $taskStarted=[DateTime]::UtcNow
        $taskProcess=Start-Process -FilePath $taskExe -ArgumentList $taskArgs -WindowStyle Hidden -PassThru
        Write-Output "Expedition $taskClass $taskSuffix $taskLabel PID=$($taskProcess.Id)"
        $taskWatch=[Diagnostics.Stopwatch]::StartNew();$taskPeak=0L
        while(-not $taskProcess.WaitForExit(500)){
            $taskProcess.Refresh();if(-not $taskProcess.HasExited){$taskPeak=[Math]::Max($taskPeak,$taskProcess.WorkingSet64)}
            if($taskWatch.Elapsed.TotalSeconds -gt 600){throw "Player sigue activo PID=$($taskProcess.Id); revisa $taskLog. No se fuerza cierre."}
        }
        $taskProcess.Refresh()
        if($taskProcess.ExitCode -ne 0){throw "Player exit=$($taskProcess.ExitCode). Revisa $taskLog y $taskReceipt"}
        if(-not(Test-Path -LiteralPath $taskReceipt) -or (Get-Item $taskReceipt).LastWriteTimeUtc -lt $taskStarted){throw 'No existe comprobante nuevo.'}
        $taskText=Get-Content -LiteralPath $taskReceipt -Raw
        if(-not $taskText.StartsWith('PASS:')){throw $taskText}
        if(Select-String -LiteralPath $taskLog -Pattern 'Failed to create agent|Exception:|NullReferenceException|Assertion failed' -Quiet){throw "Error en log $taskLog"}
        @{class=$taskClass;resume=$taskResume;elapsedSeconds=$taskWatch.Elapsed.TotalSeconds;sampledPeakWorkingSetBytes=$taskPeak;measuredFps=$false;measuredGc=$false}|ConvertTo-Json|Set-Content ($taskReceipt+'.metrics.json')
        Write-Output $taskText
    }
}
Write-Output "Resultados: $taskRun"

