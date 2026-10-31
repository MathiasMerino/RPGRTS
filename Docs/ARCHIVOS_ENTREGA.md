# Inventario de archivos de la entrega

La primera tabla conserva el inventario de la primera etapa. Al final se registra la segunda etapa. Cada asset nuevo incluye su `.meta`, aunque la tabla omite esos acompañantes para facilitar la lectura. Los binarios y registros quedan fuera de Git.

## Adición E02

Entrega posterior E02-B: `Docs/E02B_MONJE_PROVISIONAL.md` y siete capturas `Docs/Images/E02B-*`; prefab `Assets/02_Prefabs/Characters/MonkProvisional/Monk_ConceptProxy.prefab`; dos materiales/seis mallas/una ilustración en `Assets/07_Characters/MonkProvisional/`; `Assets/02_Prefabs/UI/ClassReferenceCard.prefab`. Autoría: MonkIdentityBuilder/MonkProxyGeometry; runtime: ClassReferenceCard y adiciones pequeñas a catálogo/HUD; pruebas MonkIdentityTests y talla actualizada en CharacterVisualTests; modo MonkIdentity en lanzador y referencia uGUI en assembly de editor existente. Catálogo/escena cambian presentación únicamente. Fuentes anteriores, guía E02, candidato vacío y estados pendientes se conservan. Build local: `ProyectoRPGRTS/Builds/Aethernia-Fase3-E02B-Windows.zip`; resultados exactos en el informe E02-B.

Entrega artística posterior E02-A: `Documentacion/Personajes/MONJE_DISENO_BASE.md`, captura de referencia del usuario en Referencias y hoja PNG/prompt en Conceptos. Documentación fuera de Assets, estado PROPUESTO. No son modelos, texturas PBR o animaciones del juego; se conserva el historial técnico E02 y sus pendientes. Cambios de seguimiento en checklist, registro, dirección artística y enlaces de escala/importación; sin cambios al proyecto Unity.

- Guía única: `Docs/E02_IMPORTACION_HUMANOID.md` (uso, contrato, manifiesto exacto de recursos faltantes, pruebas y límites).
- Plantilla vacía: `Assets/02_Prefabs/Characters/Humanoid/Monk_ImportTemplate.prefab`; perfil/controller y carpetas en `Assets/07_Characters/Humanoid/`. Carpetas sin recursos se preservan con .gitkeep ignorados por Unity.
- Runtime: HumanoidVisualRig/AnimationProfile/AnimationDriver; extensiones pequeñas de Catalog/Model/Presenter. Editor: `Assets/03_Scripts/Editor/Characters/`, assembly propia y ventana nativa.
- Pruebas: HumanoidPipelineTests, referencia de assembly y modo de generación HumanoidPipeline en lanzador. Catálogo existente preserva GUID/fallback.
- Checklist, registro, arquitectura, plan y dirección actualizados; no se generan modelos o clips artísticos ficticios. Recursos y animaciones reales quedan pendientes.

## Adición E01

`Docs/E01_PERSONAJES.md` y siete capturas `Docs/Images/E01-*`; cinco prefabs en `Assets/02_Prefabs/Characters/Visuals/`, catálogo/ocho materiales/veintiuna mallas en `Assets/07_Characters/Provisional/`, cuatro componentes en `Runtime/Presentation/`, CharacterVisualBuilder, CharacterVisualTests y cambios acotados en escena/lanzador/consolidación de autoría. Guías, arquitectura, medidas, checklist y registro actualizados. La biblioteca D01 y el prefab técnico del héroe se preservan. Build/paquete y resultados exactos en el informe E01.

## Adición D01

- Escena modificada: `ProyectoRPGRTS/Assets/01_Scenes/PlayablePrototype.unity`; conserva los bloques originales, colliders y NavMesh.
- Once prefabs nuevos: `ProyectoRPGRTS/Assets/02_Prefabs/Environment/Medieval/`.
- Ocho materiales, dieciséis texturas, veintitrés mallas y dos firmas de referencia: `ProyectoRPGRTS/Assets/06_Environment/Medieval/`.
- Autoría de editor: `ProyectoRPGRTS/Assets/03_Scripts/Editor/Environment/` (cinco scripts y assembly exclusiva de editor).
- Pruebas nuevas: `MedievalEnvironmentTests.cs`; referencia de assembly de pruebas actualizada. Corrección pequeña en `Runtime/Camera/IsometricCameraRig.cs`.
- Lanzador actualizado: `Tools/Invoke-UnityValidation.ps1`, modos de autoría D01 sin regenerar el resto del prototipo.
- Evidencia/instrucciones: `Docs/D01_ESCENARIO_MEDIEVAL.md`, medidas, dirección artística, checklist, registro, plan y `Docs/Images/D01-*.png`.
- Artefacto local nuevo: `ProyectoRPGRTS/Builds/Aethernia-Fase3-D01-Windows.zip`; los ZIP históricos se conservan. Resultados exactos en el informe D01.

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

## Fase 3 — iteración A/B/C1

Dirección artística en Documentacion/DIRECCION_ARTISTICA.md; auditoría/validación y ocho capturas en Docs/FASE3_ITERACION_1.md e Images/Fase3*. Escena PlayablePrototype y MedievalTheme actualizados para navegación contextual, reutilizando cuatro instancias del botón compartido y acciones originales. Tres scripts UI modificados, editor ContextUiBuilder nuevo, prueba ContextUiValidationTests nueva con .meta, captura parametrizada y acceso de prueba al nombre adaptado. Tools/Invoke-UnityValidation.ps1 añade ContextUi (aplicación única); arquitectura, instrucciones, plan/checklist/registro y AGENTS actualizados.

Build actual en Windows/Aethernia.exe y paquete Aethernia-Fase3-UI-C1-Windows.zip; anteriores ZIP conservados. D/E/C2 son trabajo pendiente, no archivos artísticos implementados. No se modificó el GDD ni se descargaron recursos externos.
