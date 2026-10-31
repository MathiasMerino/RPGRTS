# Crónicas de Aethernia

Prototipo académico de RPG isométrico y RTS desarrollado en Unity. Las mecánicas y el arte son provisionales; el historial de decisiones, cambios y asistencia de herramientas se conserva en la documentación.

## Abrir para evaluar

1. Descargar o clonar este repositorio. La rama de trabajo es `dev`; `main` contiene la última base estable publicada.
2. En Unity Hub, añadir **la carpeta `ProyectoRPGRTS/`**, situada dentro del repositorio.
3. Abrir con **Unity 6000.5.8f1** y esperar a que importe los recursos y restaure los paquetes. La primera apertura necesita conexión para descargar las dependencias declaradas en `Packages/`.
4. Abrir `Assets/01_Scenes/PlayablePrototype.unity`, revisar Console y pulsar **Play**. La UI se puede revisar a 1280×720, 1600×900, 1920×1080 y 2560×1440.

Código, modelos, materiales, escenas, prefabs, archivos `.meta`, configuración de Unity y dependencias permanecen incluidos. Las carpetas `Library`, `Temp`, `Logs`, `UserSettings` y las compilaciones se omiten mediante `.gitignore`; Unity puede regenerar sus cachés. No hace falta recibir esas carpetas del autor.

## Material para el docente

- [Controles y recorrido jugable](COMO_JUGAR.md).
- [Checklist e historial de tareas](CHECKLIST.md) y [registro de cambios](REGISTRO_CAMBIOS.md).
- [GDD original](ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx) y [lectura consultable](Docs/GDD_LECTURA.md).
- [Arquitectura](Docs/ARQUITECTURA.md) y [último informe de interfaz con capturas](Docs/C02_INTERFAZ_MEDIEVAL.md).
- [Flujo de GitHub y ramas](Docs/GITHUB_FLUJO.md).

Las 52 pruebas aprobadas corresponden a la entrega C02 documentada; no son una ejecución nueva por descargar el repositorio. La evaluación humana F2-07 y el arte definitivo siguen pendientes.

## Repetir las comprobaciones

En Windows, con el editor instalado y cerrado sobre este proyecto, ejecutar desde la raíz del repositorio:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical
.\Tools\Invoke-UnityValidation.ps1 -Mode Build
.\Tools\Invoke-PrototypeSmoke.ps1
.\Tools\Invoke-PrototypeSmoke.ps1 -WithGraphics
```

Las pruebas/build generan resultados locales en `ProyectoRPGRTS/Logs/` y `ProyectoRPGRTS/Builds/`, excluidos de Git. Los informes y capturas seleccionadas sí se mantienen versionados. No ejecutar los modos de generación de escenarios/UI para abrir o evaluar la entrega existente.
