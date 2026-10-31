# Crónicas de Aethernia

Prototipo académico de RPG isométrico y RTS desarrollado en Unity. Las mecánicas y el arte son provisionales; el historial de decisiones, cambios y asistencia de herramientas se conserva en la documentación.

## Abrir para evaluar

1. Descargar o clonar este repositorio. La rama de trabajo es `dev`; `main` contiene la última base estable publicada.
2. En Unity Hub, añadir **la carpeta `ProyectoRPGRTS/`**, situada dentro del repositorio.
3. Abrir con **Unity 6000.5.8f1** y esperar a que importe los recursos y restaure los paquetes. La primera apertura necesita conexión para descargar las dependencias declaradas en `Packages/`.
4. Abrir `Assets/01_Scenes/AetherniaVillageDemo.unity` (la escena anterior `PlayablePrototype.unity` se conserva), revisar Console y pulsar **Play**. La UI se puede revisar a 1280×720, 1600×900, 1920×1080 y 2560×1440.

Código, modelos, materiales, escenas, prefabs, archivos `.meta`, configuración de Unity y dependencias permanecen incluidos. Las carpetas `Library`, `Temp`, `Logs`, `UserSettings` y las compilaciones se omiten mediante `.gitignore`; Unity puede regenerar sus cachés. No hace falta recibir esas carpetas del autor.

## Material para el docente

- [Controles y recorrido jugable](COMO_JUGAR.md).
- [Checklist e historial de tareas](CHECKLIST.md) y [registro de cambios](REGISTRO_CAMBIOS.md).
- [GDD original](ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx) y [lectura consultable](Docs/GDD_LECTURA.md).
- [Arquitectura](Docs/ARQUITECTURA.md) y [informe E03–E06 con pruebas y capturas](Docs/E03_E06_PRODUCCION.md).
- [Flujo de GitHub y ramas](Docs/GITHUB_FLUJO.md).

La regresión E03–E06 del 2026-10-31 aprobó 62/62 pruebas; la entrega C02 histórica tenía 52. Son ejecuciones documentadas, no pruebas nuevas al descargar el repositorio. F2-07 tiene 7/8 comprobaciones humanas informadas por el usuario; falta confirmar el cierre X/ESC tras el ajuste. El arte definitivo sigue pendiente.

## Repetir las comprobaciones

En Windows, con el editor instalado y cerrado sobre este proyecto, ejecutar desde la raíz del repositorio:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical
.\Tools\Invoke-UnityValidation.ps1 -Mode BuildVillage
.\Tools\Invoke-PrototypeSmoke.ps1 -RequireCargo
.\Tools\Invoke-PrototypeSmoke.ps1 -WithGraphics -RequireCargo
```

Las pruebas/build generan resultados locales en `ProyectoRPGRTS/Logs/` y `ProyectoRPGRTS/Builds/`, excluidos de Git. Los informes y capturas seleccionadas sí se mantienen versionados. No ejecutar los modos de generación de escenarios/UI para abrir o evaluar la entrega existente.
