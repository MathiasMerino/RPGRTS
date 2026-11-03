#Requires -Version 7.0
param([switch]$Apply)
$ErrorActionPreference='Stop'
$taskRoot=Split-Path -Parent $PSScriptRoot
$taskBuilds=[IO.Path]::GetFullPath((Join-Path $taskRoot 'ProyectoRPGRTS/Builds'))
$taskArchive=Join-Path $taskBuilds 'Archivo'
function Assert-LocalPath([string]$Path,[string]$Parent){
    $taskAbsolute=[IO.Path]::GetFullPath($Path)
    if(-not $taskAbsolute.StartsWith($Parent+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw "Ruta fuera del alcance: $taskAbsolute"}
    if((Test-Path -LiteralPath $taskAbsolute) -and ((Get-Item -LiteralPath $taskAbsolute -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)){throw "No mover enlaces: $taskAbsolute"}
}
function Get-TreeFingerprint([string]$Path){
    $taskLines=[Collections.Generic.List[string]]::new();$taskBytes=0L;$taskCount=0
    foreach($taskFile in (Get-ChildItem -LiteralPath $Path -File -Force -Recurse | Sort-Object FullName)){
        Assert-LocalPath $taskFile.FullName $Path
        $taskHash=(Get-FileHash -LiteralPath $taskFile.FullName -Algorithm SHA256).Hash
        $taskRelative=[IO.Path]::GetRelativePath($Path,$taskFile.FullName).Replace('\','/')
        $taskLines.Add($taskRelative+'|'+$taskHash);$taskBytes+=$taskFile.Length;$taskCount++
    }
    $taskDigest=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes(($taskLines -join "`n"))))
    [pscustomobject]@{files=$taskCount;bytes=$taskBytes;treeSHA256=$taskDigest}
}
$taskPlans=@(
    @{name='AlphaCandidateE17Windows-r1';category='Intermedias'},
    @{name='AlphaCandidateE17Windows-r2';category='Intermedias'},
    @{name='PortableE14E15-20261102-055044';category='Verificaciones'},
    @{name='PortableE16-20261102-075738';category='Verificaciones'},
    @{name='PortableE16r2-20261102-082827';category='Verificaciones'},
    @{name='PortableE16r3-20261102-090401';category='Verificaciones'},
    @{name='PortableE17-20261102-183406';category='Verificaciones'}
)
foreach($taskPath in @($taskBuilds,$taskArchive)){
    if((Test-Path -LiteralPath $taskPath) -and ((Get-Item -LiteralPath $taskPath -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)){throw 'Las carpetas de entrega no pueden ser enlaces.'}
}
if(-not $Apply){$taskPlans | ForEach-Object { [pscustomobject]@{from=$_.name;to=('Archivo/'+$_.category+'/'+$_.name)} };return}
if(Test-Path -LiteralPath (Join-Path $taskArchive 'MANIFIESTO.json')){Write-Output 'Archivo ya organizado; consulte Archivo/MANIFIESTO.json. Sin cambios.';return}
$taskZipBefore=@{};foreach($taskZip in Get-ChildItem -LiteralPath $taskBuilds -Filter '*.zip' -File){$taskZipBefore[$taskZip.Name]=(Get-FileHash -LiteralPath $taskZip.FullName -Algorithm SHA256).Hash}
$taskRecords=@()
foreach($taskPlan in $taskPlans){
    $taskSource=Join-Path $taskBuilds $taskPlan.name;$taskDestination=Join-Path (Join-Path $taskArchive $taskPlan.category) $taskPlan.name
    Assert-LocalPath $taskSource $taskBuilds;Assert-LocalPath $taskDestination $taskArchive
    if(-not(Test-Path -LiteralPath $taskSource)){if(Test-Path -LiteralPath $taskDestination){continue};throw "No existe origen: $taskSource"}
    if(Test-Path -LiteralPath $taskDestination){throw "Destino existente; no sobrescribir: $taskDestination"}
    $taskBefore=Get-TreeFingerprint $taskSource
    $taskParent=Split-Path -Parent $taskDestination;New-Item -ItemType Directory -Path $taskParent -Force | Out-Null
    Assert-LocalPath $taskSource $taskBuilds;Assert-LocalPath $taskDestination $taskArchive
    Move-Item -LiteralPath $taskSource -Destination $taskDestination
    $taskAfter=Get-TreeFingerprint $taskDestination
    if(($taskBefore | ConvertTo-Json -Compress) -ne ($taskAfter | ConvertTo-Json -Compress)){throw "Contenido distinto tras mover: $taskDestination"}
    $taskRecords+=@{from=('ProyectoRPGRTS/Builds/'+$taskPlan.name);to=('ProyectoRPGRTS/Builds/Archivo/'+$taskPlan.category+'/'+$taskPlan.name);files=$taskAfter.files;bytes=$taskAfter.bytes;treeSHA256=$taskAfter.treeSHA256;result='PASS'}
    Write-Output "PASS archivo: $($taskPlan.name)"
}
foreach($taskName in $taskZipBefore.Keys){if((Get-FileHash -LiteralPath (Join-Path $taskBuilds $taskName) -Algorithm SHA256).Hash -ne $taskZipBefore[$taskName]){throw "ZIP modificado: $taskName"}}
$taskReport=@{moved=$taskRecords;zipFilesPreserved=$taskZipBefore.Count;zipSHA256=$taskZipBefore;deletedFiles=0;scope='Solo carpetas intermedias/verificaciones; builds finales y ZIP conservados. Huellas por arbol antes/despues identicas.'}
$taskOutput=Join-Path $taskRoot 'Docs/Validacion/Organizacion';New-Item -ItemType Directory -Path $taskOutput -Force | Out-Null
$taskReport | ConvertTo-Json -Depth 7 | Set-Content -LiteralPath (Join-Path $taskOutput 'ENTREGAS_LOCALES.json') -Encoding utf8
$taskReport | ConvertTo-Json -Depth 7 | Set-Content -LiteralPath (Join-Path $taskArchive 'MANIFIESTO.json') -Encoding utf8
Write-Output "Conservados $($taskZipBefore.Count) ZIP; carpetas archivadas $($taskRecords.Count); archivos eliminados0."
