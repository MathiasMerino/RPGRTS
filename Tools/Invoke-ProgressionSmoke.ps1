param([ValidateSet('E18','E19','UX01','UX02','E20','E201')][string]$Phase='E18',[string[]]$Races=@(),[switch]$WithGraphics,[switch]$Record,[ValidateSet('warrior','monk','mage')][string[]]$Classes=@('warrior','monk','mage'),[string]$Executable='')
$ErrorActionPreference='Stop'
$taskRoot=Split-Path -Parent $PSScriptRoot
$taskProject=Join-Path $taskRoot 'ProyectoRPGRTS'
$taskBuild=if($Phase -eq 'E201'){'Builds/E201Windows/Aethernia.exe'}elseif($Phase -eq 'E20'){'Builds/E20Windows/Aethernia.exe'}elseif($Phase -eq 'UX02'){'Builds/Ux02Windows/Aethernia.exe'}elseif($Phase -eq 'UX01'){'Builds/Ux01Windows/Aethernia.exe'}elseif($Phase -eq 'E19'){'Builds/CivilizationsE19Windows/Aethernia.exe'}else{'Builds/ProgressionE18Windows/Aethernia.exe'}
$taskFlag=if($Phase -in @('E20','E201')){'-aetherniaE20Smoke'}elseif($Phase -eq 'UX02'){'-aetherniaUx02Smoke'}elseif($Phase -eq 'UX01'){'-aetherniaUx01Smoke'}elseif($Phase -eq 'E19'){'-aetherniaCivilizationSmoke'}else{'-aetherniaProgressionSmoke'}
if($Phase -in @('E19','UX01','UX02','E20','E201') -and $Races.Count -ne $Classes.Count){throw 'E19 requiere una raza por clase, en el mismo orden.'}
$taskExe=Join-Path $taskProject $taskBuild
if($Executable){$taskExe=[IO.Path]::GetFullPath($Executable)}
if(-not(Test-Path -LiteralPath $taskExe)){throw "Falta la build de $Phase en $taskExe."}
$taskLabel=if($WithGraphics){'GPU'}else{'Headless'}
$taskRun=Join-Path $taskProject ('Logs/'+$(if($Phase -eq 'E201'){'E201QA'}elseif($Phase -eq 'E20'){'E20QA'}elseif($Phase -eq 'UX02'){'Ux02QA'}elseif($Phase -eq 'UX01'){'Ux01QA'}elseif($Phase -eq 'E19'){'CivilizationQA'}else{'ProgressionQA'})+'/'+$taskLabel+'-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $taskRun -Force | Out-Null
for($taskIndex=0;$taskIndex -lt $Classes.Count;$taskIndex++){
    $taskClass=$Classes[$taskIndex]
    $taskRace=if($Phase -in @('E19','UX01','UX02','E20','E201')){$Races[$taskIndex]}else{''}
    $taskName=if($taskRace){$taskRace+'-'+$taskClass}else{$taskClass}
    $taskSlot=Join-Path $taskRun ($taskName+'.json')
    foreach($taskResume in @($false,$true)){
        $taskSuffix=if($taskResume){'resume'}else{'start'}
        $taskReceipt=$taskSlot+'.'+$taskSuffix+'.txt'
        $taskLog=Join-Path $taskRun ($taskName+'-'+$taskSuffix+'.log')
        $taskArgs=@('-batchmode',$taskFlag,('"'+$taskSlot+'"'),'-demoClass',$taskClass,'-screen-width','1280','-screen-height','720','-logFile',('"'+$taskLog+'"'))
        if($Phase -in @('E19','UX01','UX02','E20','E201')){$taskArgs+=@('-demoRace',$taskRace,'-demoCulture','stonehall')}
        if($Phase -eq 'E201'){$taskArgs+='-demoStabilization'}
        if(-not $WithGraphics){$taskArgs+='-nographics'}
        if($taskResume){$taskArgs+='-demoResume'}
        if($Record -and $WithGraphics -and -not $taskResume -and $taskClass -eq $Classes[0]){$taskArgs+='-demoRecord'}
        $taskStarted=[DateTime]::UtcNow
        $taskProcess=Start-Process -FilePath $taskExe -ArgumentList $taskArgs -WindowStyle Hidden -PassThru
        Write-Output "$Phase $taskName $taskSuffix $taskLabel PID=$($taskProcess.Id)"
        $taskWatch=[Diagnostics.Stopwatch]::StartNew();$taskPeak=0L
        while(-not $taskProcess.WaitForExit(500)){
            $taskProcess.Refresh();if(-not $taskProcess.HasExited){$taskPeak=[Math]::Max($taskPeak,$taskProcess.WorkingSet64)}
            if($taskWatch.Elapsed.TotalSeconds -gt 1200){throw "Player sigue activo PID=$($taskProcess.Id); revisa $taskLog. No se fuerza cierre."}
        }
        $taskProcess.Refresh()
        if($taskProcess.ExitCode -ne 0){throw "Player exit=$($taskProcess.ExitCode). Revisa $taskLog y $taskReceipt"}
        if(-not(Test-Path -LiteralPath $taskReceipt) -or (Get-Item $taskReceipt).LastWriteTimeUtc -lt $taskStarted){throw 'No existe comprobante nuevo.'}
        $taskText=Get-Content -LiteralPath $taskReceipt -Raw
        if(-not $taskText.StartsWith('PASS:')){throw $taskText}
        if(Select-String -LiteralPath $taskLog -Pattern 'Failed to create agent|Exception:|NullReferenceException|Assertion failed' -Quiet){throw "Error en log $taskLog"}
        @{phase=$Phase;race=$taskRace;class=$taskClass;resume=$taskResume;elapsedSeconds=$taskWatch.Elapsed.TotalSeconds;sampledPeakWorkingSetBytes=$taskPeak;measuredFps=$false;measuredGc=$false}|ConvertTo-Json|Set-Content ($taskReceipt+'.metrics.json')
        Write-Output $taskText
    }
}
Write-Output "Resultados: $taskRun"
