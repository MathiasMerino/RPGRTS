# Herramientas del proyecto

Ejecutar desde la raíz del repositorio con PowerShell7/Python según extensión. No son pasos obligatorios para abrir y jugar en Unity.

| Uso | Entrada |
|---|---|
| Validar Unity | `Invoke-UnityValidation.ps1 -Mode Vertical`; filtro `-TestFilter` para comprobaciones dirigidas. |
| Compilar candidata actual | `Invoke-UnityValidation.ps1 -Mode BuildCivilizations` (E19); `BuildProgression` conserva E18. |
| Probar build E19 | `Invoke-ProgressionSmoke.ps1 -Phase E19 -Classes warrior,mage -Races human,dwarf -WithGraphics` con slots aislados; E18 sigue como valor predeterminado. |
| Exportar evidencia E19 | `Export-ProgressionEvidence.py --phase E19`; consultar argumentos con `--help`. |
| Empaquetar build | `Package-VisualPolish.py --phase E19`; no sobrescribir ZIP existente. |
| Organizar archivos sueltos Unity | `Invoke-UnityValidation.ps1 -Mode ProjectOrganization`; mantiene GUID y audita dependencias. |
| Archivar verificaciones locales | `Organize-LocalDeliveries.ps1` muestra plan; `-Apply` conserva contenido y verifica hashes. |
| Etapas anteriores | `Invoke-DemoSmoke`, `Invoke-ExpeditionSmoke`, `Invoke-PrototypeSmoke`, exportadores/empaquetadores E08–E17. Conservar para regresión. |
| Autoría de arte/audio | `Generate-*` y modos de generación del lanzador; no repetir sobre assets existentes. |

Las rutas de scripts se mantienen para no romper instrucciones de validación existentes. [Mapa de carpetas](../Docs/ORGANIZACION_PROYECTO.md) y [guía E19](../Docs/GUIA_E19.md). No ejecutar dos editores sobre el mismo proyecto; no convertir scripts de autoría en una rutina de arranque.
