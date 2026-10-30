# Inventario de archivos de la entrega

La primera tabla conserva el inventario de la primera etapa. Al final se registra la segunda etapa. Cada asset nuevo incluye su `.meta`, aunque la tabla omite esos acompañantes para facilitar la lectura. Los binarios y registros quedan fuera de Git.

| Estado | Archivo |
| --- | --- |
| Modificado | `AGENTS.md` |
| Modificado | `CHECKLIST.md` |
| Nuevo | `COMO_JUGAR.md` |
| Nuevo | `Docs/ARQUITECTURA.md` |
| Nuevo | `Docs/ESPECIFICACION_PROTOTIPO.md` |
| Nuevo | `Docs/Images/Dungeon.png` |
| Nuevo | `Docs/Images/PlayablePrototype.png` |
| Nuevo | `Docs/Images/RtsBattle.png` |
| Nuevo | `Docs/Images/TurnCombat.png` |
| Nuevo | `Docs/PLAN_EXPANSION.md` |
| Nuevo | `Docs/VALIDACION_PROTOTIPO.md` |
| Modificado | `PLAN_DESARROLLO.md` |
| Nuevo | `ProyectoRPGRTS/Assets/01_Scenes/PlayablePrototype.unity` |
| Nuevo | `ProyectoRPGRTS/Assets/02_Prefabs/Camera/IsometricCamera.prefab` |
| Nuevo | `ProyectoRPGRTS/Assets/02_Prefabs/Characters/PrototypeHero.prefab` |
| Nuevo | `ProyectoRPGRTS/Assets/02_Prefabs/Characters/PrototypeWorkshop.prefab` |
| Nuevo | `ProyectoRPGRTS/Assets/02_Prefabs/UI/ExplorationHud.prefab` |
| Nuevo | `ProyectoRPGRTS/Assets/02_Prefabs/UI/MedievalButton.prefab` |
| Nuevo | `ProyectoRPGRTS/Assets/02_Prefabs/UI/MedievalPanel.prefab` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Editor/PlayablePrototypeBuilder.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Editor/VerticalSliceBuilder.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Aethernia.Runtime.asmdef` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Camera/IsometricCameraRig.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Combat/TurnBattle.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Combat/TurnEncounter.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Construction/WorldConstruction.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Exploration/ExplorationUnit.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Exploration/PlayerCommands.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Interaction/InteractionPoint.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Persistence/PrototypeSaveStore.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Rts/RtsBattleController.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/UI/ExplorationHud.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/UI/MedievalUiTheme.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/UI/UiInputGuard.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/UI/VerticalSliceHud.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/Validation/BuildSmokeProbe.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/World/GameSession.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/World/PrototypeDirector.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/World/PrototypeRules.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/World/PrototypeState.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Runtime/World/WorldOperations.cs` |
| Modificado | `ProyectoRPGRTS/Assets/03_Scripts/Tests/Editor/Aethernia.Editor.Tests.asmdef` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Tests/Editor/PlayableValidationTests.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Tests/Editor/SceneCapture.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Tests/Editor/VerticalIntegrationTests.cs` |
| Nuevo | `ProyectoRPGRTS/Assets/03_Scripts/Tests/Editor/VerticalModelTests.cs` |
| Modificado | `ProyectoRPGRTS/Assets/Settings/DefaultVolumeProfile.asset` |
| Modificado | `ProyectoRPGRTS/Assets/Settings/PC_RPAsset.asset` |
| Nuevo | `ProyectoRPGRTS/Assets/Settings/Prototype/ApprovedRules.asset` |
| Nuevo | `ProyectoRPGRTS/Assets/Settings/Prototype/Hostile.mat` |
| Nuevo | `ProyectoRPGRTS/Assets/Settings/Prototype/MedievalTheme.asset` |
| Nuevo | `ProyectoRPGRTS/Assets/Settings/Prototype/PrototypeNavMesh.asset` |
| Nuevo | `ProyectoRPGRTS/Assets/Settings/Prototype/VerticalNavMesh.asset` |
| Modificado | `ProyectoRPGRTS/Assets/Settings/UniversalRenderPipelineGlobalSettings.asset` |
| Modificado | `ProyectoRPGRTS/ProjectSettings/EditorBuildSettings.asset` |
| Modificado | `ProyectoRPGRTS/ProjectSettings/NavMeshAreas.asset` |
| Modificado | `ProyectoRPGRTS/ProjectSettings/ProjectSettings.asset` |
| Modificado | `REGISTRO_CAMBIOS.md` |
| Nuevo | `Tools/Invoke-PrototypeSmoke.ps1` |
| Modificado | `Tools/Invoke-UnityValidation.ps1` |

Artefacto local: `ProyectoRPGRTS/Builds/Aethernia-Prototipo-Windows.zip`.
Ejecutable: `ProyectoRPGRTS/Builds/Windows/Aethernia.exe`.
Los archivos originales de diseño y las escenas de referencia se preservan.

## Segunda etapa — 2026-10-30

| Cambio | Archivos / propósito |
| --- | --- |
| Documentación | CHECKLIST.md, PLAN_DESARROLLO.md, REGISTRO_CAMBIOS.md, COMO_JUGAR.md: secuencia, evidencia e instrucciones actuales; conserva las 26 tareas previas |
| Informes | Docs/AUDITORIA_FASE_2.md y Docs/PRUEBA_MANUAL_FASE_2.md nuevos; Docs/ARQUITECTURA.md actualizado; Docs/VALIDACION_PROTOTIPO.md identificado como histórico; este inventario |
| Evidencia visual | Docs/Images/Fase2*: capturas propias del render de Unity, sin sustituir imágenes históricas |
| Navegación/táctica | Runtime/Exploration/ExplorationUnit.cs, Runtime/Combat/TurnBattle.cs y TurnEncounter.cs: parada, trayectoria, tránsito y geometría de ataque |
| RTS/integración | Runtime/Rts/RtsBattleController.cs y Runtime/World/PrototypeDirector.cs: participantes, validación del despliegue, modos y carga |
| Estado/guardado | Runtime/World/GameSession.cs, WorldOperations.cs y Runtime/Persistence/PrototypeSaveStore.cs: IDs/modos, cantidades y coherencia temporal |
| UI | Runtime/UI/ExplorationHud.cs y VerticalSliceHud.cs: mensajes específicos, equipo visible y acciones/modos disponibles |
| Validación | Tests/Editor/PhaseTwoValidationTests.cs nuevo con .meta; Runtime/Validation/BuildSmokeProbe.cs reforzado; Tools/Invoke-UnityValidation.ps1 admite filtro dirigido |
| Arranque de navegación | Instancia del héroe en PlayablePrototype.unity: agente desactivado en edición y activado en Start por ExplorationUnit; método puntual RepairNavigationStartup en Editor/VerticalSliceBuilder.cs; modo AgentStartup y rechazo de avisos/excepciones en Tools/Invoke-PrototypeSmoke.ps1 |

Las rutas Runtime/Tests de la tabla parten de `ProyectoRPGRTS/Assets/03_Scripts/`. No se regeneraron escenas/prefabs ni se actualizaron paquetes. La build Windows local se reemplaza al compilar; el ZIP original sigue siendo la entrega histórica. La segunda entrega utiliza `ProyectoRPGRTS/Builds/Aethernia-Fase2-Windows.zip`. Resultados y checksum en AUDITORIA_FASE_2.md.
