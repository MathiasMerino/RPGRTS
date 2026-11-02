# Revisar el pulido visual E16

## Abrir y jugar

1. Elegir la rama `feature/e16-pulido-visual-ui`. Añadir `ProyectoRPGRTS/` en Unity Hub y abrir con **Unity 6000.5.8f1** (URP 17.5.0).
2. Esperar importación, abrir `Assets/01_Scenes/AetherniaVisualPolish.unity`, revisar Console y pulsar **Play**. No ejecutar generadores: los recursos ya existen. `AetherniaExpeditions` y las escenas anteriores se conservan.
3. **Comenzar una aventura** muestra Guerrero, Mago y Monje. Elegir tarjeta, nombre y linaje; iniciar el prólogo. La selección usa ilustraciones temporales, no renders del modelo jugable. Continuar/Cargar conservan el flujo previo.
4. WASD mueve al héroe; flechas/botón medio/rueda controlan cámara. E interactúa cerca de los objetos. Abrir y cerrar Personaje, Inventario, Partida, Ayuda y Configuración; comprobar también ×/ESC. F5 guarda y F9 carga fuera de combate.
5. Para compañeros, ruinas y conquista seguir [la guía de expediciones](GUIA_EXPEDICIONES.md). E16 conserva costes, acciones, producción y guardado; no entrega recursos para facilitar la demostración.

## Build Windows

Ejecutar `ProyectoRPGRTS/Builds/VisualPolishWindows/Aethernia.exe` o extraer el ZIP E16 completo y ejecutar `Aethernia.exe`. Mantener juntos ejecutable, DLL y carpeta Data.

Los lanzadores `Revisar-warrior.cmd`, `Revisar-monk.cmd` y `Revisar-mage.cmd` usan copias E13 reales separadas de la partida personal. Guardar modifica esas copias: respaldar `PartidasEjemplo/` para repetir. No son accesos de QA ni omiten combates. Los archivos de build se excluyen de Git; para regenerarlos usar `BuildVisualPolish`.

## Revisión humana pendiente

Probar a **1280×720**, **1920×1080** y **1024×768**. Revisar nitidez, márgenes, recortes, lectura del inventario/turnos, selección, proporción de retratos y comodidad del ratón. En ruinas observar ambiente frío, antorchas cálidas y visibilidad del grupo; salir debe recuperar la luz exterior. En RTS comprobar barras, panel de órdenes y selección de tropas. Registrar observaciones: la automatización y la inspección técnica de capturas no aprueban belleza, diversión o balance.

## Sustituir arte después

- `Assets/12_VisualPolish/Presentation.asset`: **Figure** por ClassId alimenta menú y tarjetas. **Portrait** conserva una referencia auxiliar al recorte existente.
- HUD/ficha: siguen el `CharacterVisualCatalog` E08; cambiar **ReferenceArt/ReferenceUv/PortraitSprite** desde Unity. Cambiar solo Portrait del perfil E16 no actualiza ese HUD.
- Modelos/rigs: siguen el catálogo E08. Sustituir su representación sin cambiar controlador, colliders, estado o daño autoritativo. Conservar `.meta`/GUID y proporciones. El FBX Meshy ajeno no se integró.
- `Assets/12_VisualPolish/Prefabs`: nueve componentes de botón, tarjeta, cuero, pergamino, retrato, tooltip y aviso; atlas y fuente compartidos. DemoWidgets instancia el botón base y selección instancia ClassCard. Otros componentes están disponibles, sin funcionalidades ficticias.

Las tres hojas originales de `ProductionProvisional/Source` permanecen intactas y provisionales.

## Repetir pruebas

Desde la raíz, con el editor cerrado sobre este proyecto:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical -TimeoutMinutes 45
.\Tools\Invoke-UnityValidation.ps1 -Mode BuildVisualPolish
.\Tools\Invoke-DemoSmoke.ps1 -WithGraphics -Executable "$PWD/ProyectoRPGRTS/Builds/VisualPolishWindows/Aethernia.exe"
.\Tools\Invoke-ExpeditionSmoke.ps1 -WithGraphics -Executable "$PWD/ProyectoRPGRTS/Builds/VisualPolishWindows/Aethernia.exe"
```

Los probes requieren flags explícitos y usan partidas aisladas. Resultados locales en Logs; evidencia seleccionada en `Docs/Validacion/E16`. Las capturas no miden FPS ni GC. No repetir VisualPolish/Create sobre recursos existentes.
