# Organización conservadora del proyecto

Mantenimiento posterior a E18, no fase E19 ni cambio de mecánicas. Se conserva contenido útil, provisional, de respaldo y de validación. Los GUID de los assets movidos y la configuración gráfica local previa deben mantenerse.

Nota E19: se añade `Assets/15_Civilizations` con datos opt-in y dos prefabs derivados, más AetherniaCivilizationsDemo. El inventario y las cifras de mantenimiento siguientes conservan su fecha/base5507425; no son un inventario nuevo de E19.

## Mapa principal

| Carpeta | Función y conservación |
|---|---|
| `ProyectoRPGRTS/Assets/01_Scenes` | E19, E18 y escenas de regresión; no eliminar las anteriores. |
| `02_Prefabs`, `06_Environment` | Biblioteca reutilizable de personajes, entorno y construcción. |
| `03_Scripts/Runtime`, `Editor`, `Tests` | Lógica, autoría/mantenimiento y validación separadas. |
| `04_Sprites`, `07_Characters`, `08_UI`, `09_Feedback` | Ilustraciones/modelos/clips provisionales, UI y efectos; siguen teniendo utilidad. |
| `05_Docs` | GDD original; `PlantillaUnity/Readme.asset` conserva la referencia de plantilla. |
| `10_Demo` a `14_Progression` | Capas de configuración/recursos de etapas. E18 reutiliza dependencias anteriores: no son carpetas descartables. |
| `15_Civilizations` | Catálogo de razas/culturas E19, reglas compartidas y dos variantes de prefabs; modelos/materiales previos reutilizados. |
| `Settings` | URP y configuración; `Input/InputSystem_Actions.inputactions` agrupa el asset de entrada conservando su GUID. |
| `TextMesh Pro` | Recursos compartidos del paquete; se respeta su estructura. |
| `LocalOnly`, `07_Characters/MonkMeshy` | Experimentos privados; se mantienen excluidos de Git y de la entrega pública. |
| `Docs` | Guías y diseño aplicables; [índice](README.md). |
| `Docs/Historial` | Etapas, validaciones antiguas, seguimientos y contexto completo conservado. |
| `Docs/Validacion`, `Images`, `Videos` | Evidencia seleccionada, no cachés; se conserva en Git. |
| `Documentacion` | Dirección artística, conceptos y referencias de origen; no mover por parecido con arte importado. |
| `Tools` | Herramientas de validación, empaquetado, arte y mantenimiento. [Índice](../Tools/README.md). |

## Unity: qué sigue siendo útil

El inventario [RECURSOS_UNITY.csv](Validacion/Organizacion/RECURSOS_UNITY.csv) distingue dependencias actuales E18, código/herramientas, escenas de regresión, fuente de diseño y recursos históricos/provisionales reutilizables. No estar en el grafo de una escena no demuestra que un asset sea inútil: puede ser una carga dinámica, candidato de importación, alternativa o recurso de pruebas.

No se renombran las carpetas por etapa porque los generadores y validadores existentes utilizan rutas conocidas. Se evita duplicar modelos, materiales o texturas. Los dos movimientos internos son:

- `Assets/Readme.asset` → `Assets/05_Docs/PlantillaUnity/Readme.asset`.
- `Assets/InputSystem_Actions.inputactions` → `Assets/Settings/Input/InputSystem_Actions.inputactions`.

Se ejecutan con `AssetDatabase.MoveAsset`, manteniendo GUID y `.meta`. Las escenas y el GDD no se guardan ni regeneran. Método de mantenimiento: `Aethernia/Proyecto/Organizar archivos sueltos y auditar`; no es necesario ejecutarlo para jugar.

## Compilaciones locales

`ProyectoRPGRTS/Builds/CivilizationsE19Windows` contiene la candidata E19 en validación. `Builds/ProgressionE18Windows` y el ZIP E18 se conservan como entrega anterior. Todos los ZIP históricos y las builds finales permanecen en sus rutas previas. Solo las versiones intermedias E17-r1/r2 y cinco extracciones de verificación anteriores se archivan bajo `Builds/Archivo/Intermedias` y `Builds/Archivo/Verificaciones`.

[ENTREGAS_LOCALES.json](Validacion/Organizacion/ENTREGAS_LOCALES.json) contiene el mapa de rutas y huellas de contenido antes/después. Los informes antiguos conservan sus rutas de ejecución como evidencia histórica; utilizar este mapa para localizar las copias archivadas. No se elimina contenido ni se promete ahorro de disco al archivar.

`Tools/Organize-LocalDeliveries.ps1` permite revisar el plan; `-Apply` ejecuta el archivo con comprobación SHA y destinos dentro del proyecto. Requiere PowerShell7. No es un limpiador de cachés ni elimina builds.

## Archivos regenerables y validación

`Library`, `Temp`, `Logs`, `obj`, `UserSettings`, soluciones y proyectos C# generados siguen fuera de Git. No se editan ni se purgan durante esta organización. Las capturas/vídeos originales de QA en Logs se mantienen; la evidencia seleccionada ya está en Docs.

Evidencia del mantenimiento en [Validacion/Organizacion](Validacion/Organizacion). El cambio gráfico local en QualitySettings.asset existía antes de esta tarea y se conserva; no forma parte del commit de organización. La incidencia RTS y la evaluación humana de E18 permanecen pendientes.

## Resultado de esta organización

-27documentos movidos a Historial y referencias actualizadas; las evidencias originales JSON/XML permanecen en su ubicación.
-7carpetas locales archivadas,2185archivos/1.443.566.719bytes conservados con huellas idénticas;19ZIP intactos. No se elimina contenido ni se reduce ese volumen por moverlo.
-781assets públicos inventariados:538dependencias actuales E18,129de código/herramientas fuera de ese grafo,8escenas de regresión,1fuente de diseño y105recursos reutilizables/provisionales/históricos. Los7registros privados se omiten del catálogo público.
-578GUIDde dependencias antes/después idénticos,2assets y sus.meta con bytes idénticos; cargaE18 sin scripts perdidos.
-3/3pruebas dirigidas UnityPASS: escena/física/sin privados, RPGreal+movimiento+guardar/continuar por tresclases y técnicas en controladoresRPG/RTS por tresclases. No es una nueva batería general.
-9escenas, GDD, originales privados y cambio gráfico previo conservados.

Archivo de instrucciones raíz resumido; el contexto completo anterior está en Historial. .rgignore aparta historial/medios/evidencias de búsquedas generales; abrirlos por ruta o usar rg --no-ignore cuando sean necesarios. No se han ocultado de Git ni retirado de la entrega.
