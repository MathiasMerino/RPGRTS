# Crónicas de Aethernia

Prototipo académico de RPG isométrico y RTS desarrollado en Unity. Las mecánicas y el arte son provisionales; el historial de decisiones, cambios y asistencia de herramientas se conserva en la documentación.

## Abrir para evaluar

1. Descargar o clonar este repositorio. Para revisar esta entrega, seleccionar `feature/e18-progresion-balance`; [PR#9 en borrador](https://github.com/MathiasMerino/RPGRTS/pull/9), apilado sobre [E17/PR#8 en borrador](https://github.com/MathiasMerino/RPGRTS/pull/8), sin presumir los PR anteriores fusionados. `dev` integra desarrollo y `main` contiene la última base estable aprobada.
2. En Unity Hub, añadir **la carpeta `ProyectoRPGRTS/`**, situada dentro del repositorio.
3. Abrir con **Unity 6000.5.8f1** y esperar a que importe los recursos y restaure los paquetes. La primera apertura necesita conexión para descargar las dependencias declaradas en `Packages/`.
4. Abrir `Assets/01_Scenes/AetherniaProgressionDemo.unity`, revisar Console y pulsar **Play**. Se conservan AetherniaAlphaCandidate, AetherniaVisualPolish, AetherniaExpeditions, AetherniaFirstDemo, AetherniaVillageDemo y PlayablePrototype. La progresión E18 se comprueba con las tres clases y UI a720p/1080p; ver resultados y límites en el informe.

Código, modelos, materiales, escenas, prefabs, archivos `.meta`, configuración de Unity y dependencias permanecen incluidos. Las carpetas `Library`, `Temp`, `Logs`, `UserSettings` y las compilaciones se omiten mediante `.gitignore`; Unity puede regenerar sus cachés. No hace falta recibir esas carpetas del autor.

## Organización

[Mapa de carpetas y conservación de recursos](Docs/ORGANIZACION_PROYECTO.md) · [Índice de documentación](Docs/README.md) · [Historial por etapas](Docs/Historial/README.md). Los recursos provisionales y las escenas anteriores siguen disponibles.

## Material para el docente

- [E18: progresión, resultados y límites](Docs/ENTREGA_E18.md).
- [Guía E18 para Unity/Windows](Docs/GUIA_E18.md), [parámetros de balance](Docs/E18_BALANCE.md) y [hoja de revisión humana pendiente](Docs/Validacion/E18/REVISION_HUMANA.md).
- [Capturas E18](Docs/Images/E18) y [vídeos reales con audio](Docs/Videos/E18).

- [E17: estado técnico y problemas conocidos](Docs/Historial/Etapas/E17_ALFA.md).
- [Guía de revisión E17](Docs/GUIA_ALFA_E17.md) y [comparación de12tipos de capturas reales](Docs/Historial/Etapas/COMPARATIVA_E17.md).
- [Pipeline para sustituir los modelos provisionales](Docs/E17_PIPELINE_PERSONAJES.md).
- [Pulido E16: cambios, capturas y pruebas](Docs/Historial/Etapas/E16_PULIDO_VISUAL.md).
- [Guía E16: Unity, build y sustitución del arte](Docs/GUIA_E16.md).
- [Entrega E14–E15 con capturas, vídeos y pruebas](Docs/Historial/Etapas/ENTREGA_E14_E15.md).
- [Guía actual de compañeros, mazmorra y conquista](Docs/GUIA_EXPEDICIONES.md). [Controles históricos](COMO_JUGAR.md) se conservan.
- [Checklist e historial de tareas](CHECKLIST.md) y [registro de cambios](REGISTRO_CAMBIOS.md).
- [GDD original](ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx) y [lectura consultable](Docs/GDD_LECTURA.md).
- [Arquitectura](Docs/ARQUITECTURA.md) y [informe E07 con comparación y pruebas](Docs/Historial/Etapas/E07_ALDEA_MEDIEVAL.md).
- [Flujo de GitHub y ramas](Docs/GITHUB_FLUJO.md).

E18 conserva el registro de regresión174/176: un fallo visual corregido mediante4/4dirigidas posteriores y la tácticaRTS de reserva todavía pendiente. Ocho procesos Windows PASS (seis por las tres clases y dos desde ZIP extraído); ZIP nuevo CRC/SHA y18ZIPanteriores conservados. No es una regresión totalmente verde ni aprobación humana de balance/arte.

E16 aprobó la regresión global125/125 y verificó presentación a tres resoluciones. Un recorrido dirigido posterior tuvo una derrota del Mago en RTS; el caso aislado volvió a pasar sin cambios. Esa variación y la evaluación humana siguen pendientes; ver evidencia y límites en el informe E16.

La entrega E14–E15 documenta121/121 regresión,11/11 focales,1/1UI final y20procesos Windows aprobados, incluidos reinicios y ZIP extraído. Evaluación humana/arte/balance pendientes. La regresión histórica E07 del 2026-10-31 aprobó67/67 pruebas; E03–E06 tenía 62 y C02 52. Son ejecuciones documentadas, no pruebas nuevas al descargar el repositorio. F2-07 tiene 7/8 comprobaciones humanas informadas por el usuario; falta confirmar el cierre X/ESC tras el ajuste. El arte definitivo sigue pendiente.

## Repetir las comprobaciones

En Windows, con el editor instalado y cerrado sobre este proyecto, ejecutar desde la raíz del repositorio:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical -TimeoutMinutes 45
.\Tools\Invoke-UnityValidation.ps1 -Mode BuildAlpha
.\Tools\Invoke-DemoSmoke.ps1 -WithGraphics -Executable "$PWD/ProyectoRPGRTS/Builds/AlphaCandidateE17Windows/Aethernia.exe"
.\Tools\Invoke-ExpeditionSmoke.ps1 -WithGraphics -Executable "$PWD/ProyectoRPGRTS/Builds/AlphaCandidateE17Windows/Aethernia.exe"
```

Las pruebas/build generan resultados locales en `ProyectoRPGRTS/Logs/` y `ProyectoRPGRTS/Builds/`, excluidos de Git. Los informes y capturas seleccionadas sí se mantienen versionados. No ejecutar los modos de generación de escenarios/UI para abrir o evaluar la entrega existente.
