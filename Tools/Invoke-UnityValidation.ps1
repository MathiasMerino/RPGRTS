param(
    [ValidateSet('Baseline', 'GenerateSandbox', 'Foundation', 'GeneratePlayable', 'Playable', 'UpgradeVertical', 'Presentation', 'AgentStartup', 'ContextUi', 'MedievalQuarter', 'MedievalRefine', 'CharacterVisuals', 'CharacterPolish', 'HumanoidPipeline', 'MonkIdentity', 'MedievalHud', 'MedievalHudRefine', 'ProductionCharacters', 'ProductionPresentation', 'VillageDemo', 'VillagePolish', 'VillageE07Library', 'VillageE07Nature', 'VillageE07Transform', 'VillageE07Refine', 'VillageE07Visibility', 'AnimatedCharacters', 'AnimatedRefine', 'AnimatedFinalize', 'CombatE09', 'CombatE09Refine', 'BuildVillage', 'FirstDemo', 'BuildFirstDemo', 'Vertical', 'Build')]
    [string]$Mode = 'Baseline',
    [string]$TestFilter = '',
    [ValidateRange(1, 60)] [int]$TimeoutMinutes = 20
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
if ($Mode -in @('GenerateSandbox', 'GeneratePlayable', 'UpgradeVertical', 'Presentation', 'AgentStartup', 'ContextUi', 'MedievalQuarter', 'MedievalRefine', 'CharacterVisuals', 'CharacterPolish', 'HumanoidPipeline', 'MonkIdentity', 'MedievalHud', 'MedievalHudRefine', 'ProductionCharacters', 'ProductionPresentation', 'VillageDemo', 'VillagePolish', 'VillageE07Library', 'VillageE07Nature', 'VillageE07Transform', 'VillageE07Refine', 'VillageE07Visibility', 'AnimatedCharacters', 'AnimatedRefine', 'AnimatedFinalize', 'CombatE09', 'CombatE09Refine', 'BuildVillage', 'FirstDemo', 'BuildFirstDemo', 'Build')) {
    $taskMethod = switch ($Mode) {
        GenerateSandbox { 'Aethernia.Editor.TechnicalSandboxBuilder.Create' }
        GeneratePlayable { 'Aethernia.Editor.PlayablePrototypeBuilder.Create' }
        UpgradeVertical { 'Aethernia.Editor.VerticalSliceBuilder.Upgrade' }
        Presentation { 'Aethernia.Editor.VerticalSliceBuilder.FinalisePresentation' }
        AgentStartup { 'Aethernia.Editor.VerticalSliceBuilder.RepairNavigationStartup' }
        ContextUi { 'Aethernia.Editor.ContextUiBuilder.Upgrade' }
        MedievalQuarter { 'Aethernia.Editor.Environment.MedievalQuarterBuilder.Upgrade' }
        MedievalRefine { 'Aethernia.Editor.Environment.MedievalQuarterBuilder.Refine' }
        CharacterVisuals { 'Aethernia.Editor.Environment.CharacterVisualBuilder.Create' }
        CharacterPolish { 'Aethernia.Editor.Environment.CharacterVisualBuilder.Refine' }
        HumanoidPipeline { 'Aethernia.Editor.Characters.HumanoidPipelineBuilder.Create' }
        MonkIdentity { 'Aethernia.Editor.Characters.MonkIdentityBuilder.Create' }
        MedievalHud { 'Aethernia.Editor.C02HudBuilder.Upgrade' }
        MedievalHudRefine { 'Aethernia.Editor.C02HudBuilder.Refine' }
        ProductionCharacters { 'Aethernia.Editor.Characters.ProductionCharacterBuilder.Create' }
        ProductionPresentation { 'Aethernia.Editor.Characters.ProductionPresentationBuilder.Configure' }
        VillageDemo { 'Aethernia.Editor.ProductionVillageBuilder.Create' }
        VillagePolish { 'Aethernia.Editor.ProductionVillageBuilder.Polish' }
        VillageE07Library { 'Aethernia.Editor.Environment.VillageE07Architecture.Create' }
        VillageE07Nature { 'Aethernia.Editor.Environment.VillageE07Nature.Create' }
        VillageE07Transform { 'Aethernia.Editor.Environment.VillageE07SceneBuilder.Apply' }
        VillageE07Refine { 'Aethernia.Editor.Environment.VillageE07Refinement.Apply' }
        VillageE07Visibility { 'Aethernia.Editor.Environment.VillageE07Refinement.BindTreeVisibility' }
        CombatE09Refine { 'Aethernia.Editor.Characters.E09CombatBuilder.Refine' }
        CombatE09 { 'Aethernia.Editor.Characters.E09CombatBuilder.Create' }
        AnimatedFinalize { 'Aethernia.Editor.Characters.AnimatedCharacterBuilder.FinalizeAssets' }
        AnimatedRefine { 'Aethernia.Editor.Characters.AnimatedCharacterBuilder.RefineMeshes' }
        AnimatedCharacters { 'Aethernia.Editor.Characters.AnimatedCharacterBuilder.Create' }
        FirstDemo { 'Aethernia.Editor.Characters.FirstDemoBuilder.Create' }
        BuildFirstDemo { 'Aethernia.Editor.Characters.FirstDemoBuilder.Build' }
        BuildVillage { 'Aethernia.Editor.ProductionVillageBuilder.Build' }
        Build { 'Aethernia.Editor.PlayablePrototypeBuilder.Build' }
    }
    if ($Mode -ne 'MedievalHud') { $taskArguments += '-nographics' }
    $taskArguments += @('-quit', '-executeMethod', $taskMethod)
} else {
    # Let the test runner exit: -quit could stop asynchronous tests prematurely.
    $taskArguments += @('-runTests', '-testPlatform', 'EditMode', '-assemblyNames', 'Aethernia.Editor.Tests', '-testResults', ('"' + $taskResult + '"'))
    if ($Mode -eq 'Baseline') {
        $taskArguments += @('-nographics', '-testFilter', 'Aethernia.Tests.Editor.EnvironmentValidationTests')
    } elseif ($TestFilter) {
        $taskArguments += @('-testFilter', ('"' + $TestFilter + '"'))
    }
    # Foundation uses the graphics device to capture the sandbox off screen.
}

$taskStarted = [DateTime]::UtcNow
$taskProcess = Start-Process -FilePath $taskEditor -ArgumentList $taskArguments -WindowStyle Hidden -PassThru
Write-Output "Unity $taskVersion; modo $Mode; PID $($taskProcess.Id)."
if (-not $taskProcess.WaitForExit($TimeoutMinutes * 60000)) {
    throw "Unity sigue activo despues de $TimeoutMinutes minutos (PID $($taskProcess.Id)). No se ha forzado su cierre. Revisa $taskLog."
}
$taskProcess.Refresh()
if ($taskProcess.ExitCode -ne 0) { throw "Unity termino con codigo $($taskProcess.ExitCode). Revisa $taskLog." }
if ($Mode -notin @('GenerateSandbox', 'GeneratePlayable', 'UpgradeVertical', 'Presentation', 'AgentStartup', 'ContextUi', 'MedievalQuarter', 'MedievalRefine', 'CharacterVisuals', 'CharacterPolish', 'HumanoidPipeline', 'MonkIdentity', 'MedievalHud', 'MedievalHudRefine', 'ProductionCharacters', 'ProductionPresentation', 'VillageDemo', 'VillagePolish', 'VillageE07Library', 'VillageE07Nature', 'VillageE07Transform', 'VillageE07Refine', 'VillageE07Visibility', 'AnimatedCharacters', 'AnimatedRefine', 'AnimatedFinalize', 'CombatE09', 'CombatE09Refine', 'BuildVillage', 'FirstDemo', 'BuildFirstDemo', 'Build')) {
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
