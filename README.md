# Crónicas de Aethernia

Prototipo académico de RPG isométrico y RTS desarrollado en Unity. Las mecánicas y el arte son provisionales; el historial de decisiones, cambios y asistencia de herramientas se conserva en la documentación.

## Abrir para evaluar

1. Descargar o clonar este repositorio. Para revisar esta entrega, seleccionar `feature/e16-pulido-visual-ui`; depende de E14–E15 (PR #6), sin presumir los PR anteriores fusionados. `dev` integra desarrollo y `main` contiene la última base estable aprobada.
2. En Unity Hub, añadir **la carpeta `ProyectoRPGRTS/`**, situada dentro del repositorio.
3. Abrir con **Unity 6000.5.8f1** y esperar a que importe los recursos y restaure los paquetes. La primera apertura necesita conexión para descargar las dependencias declaradas en `Packages/`.
4. Abrir `Assets/01_Scenes/AetherniaVisualPolish.unity`, revisar Console y pulsar **Play**. Se conservan AetherniaExpeditions, AetherniaFirstDemo, AetherniaVillageDemo y PlayablePrototype. El pulido E16 se comprueba a 1280×720, 1920×1080 y 1024×768; ver resultados y límites en el informe.

Código, modelos, materiales, escenas, prefabs, archivos `.meta`, configuración de Unity y dependencias permanecen incluidos. Las carpetas `Library`, `Temp`, `Logs`, `UserSettings` y las compilaciones se omiten mediante `.gitignore`; Unity puede regenerar sus cachés. No hace falta recibir esas carpetas del autor.

## Material para el docente

- [Pulido E16: cambios, capturas y pruebas](Docs/E16_PULIDO_VISUAL.md).
- [Guía E16: Unity, build y sustitución del arte](Docs/GUIA_E16.md).
- [Entrega E14–E15 con capturas, vídeos y pruebas](Docs/ENTREGA_E14_E15.md).
- [Guía actual de compañeros, mazmorra y conquista](Docs/GUIA_EXPEDICIONES.md). [Controles históricos](COMO_JUGAR.md) se conservan.
- [Checklist e historial de tareas](CHECKLIST.md) y [registro de cambios](REGISTRO_CAMBIOS.md).
- [GDD original](ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx) y [lectura consultable](Docs/GDD_LECTURA.md).
- [Arquitectura](Docs/ARQUITECTURA.md) y [informe E07 con comparación y pruebas](Docs/E07_ALDEA_MEDIEVAL.md).
- [Flujo de GitHub y ramas](Docs/GITHUB_FLUJO.md).

E16 aprobó la regresión global125/125 y verificó presentación a tres resoluciones. Un recorrido dirigido posterior tuvo una derrota del Mago en RTS; el caso aislado volvió a pasar sin cambios. Esa variación y la evaluación humana siguen pendientes; ver evidencia y límites en el informe E16.

La entrega E14–E15 documenta121/121 regresión,11/11 focales,1/1UI final y20procesos Windows aprobados, incluidos reinicios y ZIP extraído. Evaluación humana/arte/balance pendientes. La regresión histórica E07 del 2026-10-31 aprobó67/67 pruebas; E03–E06 tenía 62 y C02 52. Son ejecuciones documentadas, no pruebas nuevas al descargar el repositorio. F2-07 tiene 7/8 comprobaciones humanas informadas por el usuario; falta confirmar el cierre X/ESC tras el ajuste. El arte definitivo sigue pendiente.

## Repetir las comprobaciones

En Windows, con el editor instalado y cerrado sobre este proyecto, ejecutar desde la raíz del repositorio:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical -TimeoutMinutes 45
.\Tools\Invoke-UnityValidation.ps1 -Mode BuildVisualPolish
.\Tools\Invoke-DemoSmoke.ps1 -WithGraphics -Executable "$PWD/ProyectoRPGRTS/Builds/VisualPolishWindows/Aethernia.exe"
.\Tools\Invoke-ExpeditionSmoke.ps1 -WithGraphics -Executable "$PWD/ProyectoRPGRTS/Builds/VisualPolishWindows/Aethernia.exe"
```

Las pruebas/build generan resultados locales en `ProyectoRPGRTS/Logs/` y `ProyectoRPGRTS/Builds/`, excluidos de Git. Los informes y capturas seleccionadas sí se mantienen versionados. No ejecutar los modos de generación de escenarios/UI para abrir o evaluar la entrega existente.
