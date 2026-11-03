# Herramientas del proyecto

Ejecutar desde la raíz del repositorio con PowerShell7/Python según extensión. No son pasos obligatorios para abrir y jugar en Unity.

| Uso | Entrada |
|---|---|
| Validar Unity | `Invoke-UnityValidation.ps1 -Mode Vertical`; filtro `-TestFilter` para comprobaciones dirigidas. |
| Compilar candidata actual | `Invoke-UnityValidation.ps1 -Mode BuildProgression`. |
| Probar build E18 | `Invoke-ProgressionSmoke.ps1 -WithGraphics` con slots aislados. |
| Exportar evidencia E18 | `Export-ProgressionEvidence.py`; consultar argumentos con `--help`. |
| Empaquetar build | `Package-VisualPolish.py --phase E18`; no sobrescribir ZIP existente. |
| Organizar archivos sueltos Unity | `Invoke-UnityValidation.ps1 -Mode ProjectOrganization`; mantiene GUID y audita dependencias. |
| Archivar verificaciones locales | `Organize-LocalDeliveries.ps1` muestra plan; `-Apply` conserva contenido y verifica hashes. |
| Etapas anteriores | `Invoke-DemoSmoke`, `Invoke-ExpeditionSmoke`, `Invoke-PrototypeSmoke`, exportadores/empaquetadores E08–E17. Conservar para regresión. |
| Autoría de arte/audio | `Generate-*` y modos de generación del lanzador; no repetir sobre assets existentes. |

Las rutas de scripts se mantienen para no romper instrucciones de validación existentes. [Mapa de carpetas](../Docs/ORGANIZACION_PROYECTO.md) y [guía E18](../Docs/GUIA_E18.md). No ejecutar dos editores sobre el mismo proyecto; no convertir scripts de autoría en una rutina de arranque.
