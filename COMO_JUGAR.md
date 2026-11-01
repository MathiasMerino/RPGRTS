# Crónicas de Aethernia Prototipo técnico

Este prototipo implementa el recorrido jugable y los sistemas mínimos acordados para comprobar la visión del GDD. Las reglas y los recursos artísticos son provisionales; la hoja de ruta de expansión está en `Docs/PLAN_EXPANSION.md`.

La segunda etapa audita y corrige la integración existente. El estado real por sistema, pruebas automáticas y límites están en `Docs/AUDITORIA_FASE_2.md`. La sesión humana sigue pendiente; usar `Docs/PRUEBA_MANUAL_FASE_2.md` para registrar resultados.

La interfaz C02 incorpora un HUD medieval compacto sobre D01/E01/E02/E02-B. Arriba aparecen salud/recursos, Ayuda y Menú; abajo, Personaje, Inventario, Registro y acciones contextuales. Las ventanas se cierran con × o ESC. Menú reúne guardado/carga y cambio de actividad; la ayuda es opcional. [Informe C02](Docs/C02_INTERFAZ_MEDIEVAL.md) y dirección artística documentan apariencia y límites.

## Entrega E09 para revisión

Paquete local verificado: `ProyectoRPGRTS/Builds/Aethernia-E09-Combate-Windows.zip`. 89/89 pruebas y recorridos Windows sin gráficos/con GPU aprobados; no sustituyen una sesión humana. Abrir la misma demo con Unity6000.5.8f1. Informe, evidencia audiovisual y guía humana: [E09_COMBATE_TACTICO](Docs/E09_COMBATE_TACTICO.md).

El combate conserva sus reglas: Guerrero con espada, Monje marcial, Mago con ataque arcano a distancia. Seleccionar rival por clic o botón del panel contextual; Atacar, Guardia o Recuperar consumen la acción. Fin de turno muestra ENEMIGO ACTUANDO durante una secuencia breve; esperar a TU TURNO. La IA ataca si dispone de alcance/línea libre, se aproxima por ruta legal o espera. PV/dados son reales, con partículas y sonidos originales provisionales. La misión y recompensa única no cambian; guardar/cargar fuera del encuentro. No hay mana ni hechizos nuevos.

Se conservan los once ZIP anteriores y las escenas originales. Generic/arte/sonidos son pilotos; la sesión humana y X/ESC siguen pendientes de confirmación. E08 y anteriores, descritos abajo, forman el historial.

## Entrega E08

Paquete local actual: `ProyectoRPGRTS/Builds/Aethernia-E08-Personajes-Windows.zip`; descomprimir completo y abrir Aethernia.exe. Los diez ZIP anteriores se conservan.

La demo actual conserva la aldea E07 y añade Monje, Guerrero y Mago con mallas skinned, rigs Generic y clips originales Idle/Walk/Run/Attack/Defend/Hit/Defeat. La apariencia y las animaciones son provisionales, con aprobación artística/humana pendiente. Fuente, límites y pasos detallados en [E08_PERSONAJES_ANIMADOS](Docs/E08_PERSONAJES_ANIMADOS.md). Las notas de E01–E07 posteriores conservan el historial; sus afirmaciones de figuras estáticas no describen los modelos E08 activos.

Para revisar: Personaje → clase fuera de combate, clic derecho para caminar; recoger/equipar espada en el cofre, acercarse al adversario y usar Atacar/Defender/Finalizar turno. La animación no aplica daño ni altera turnos. F5/F9 conservan la partida v1. Run depende de la velocidad existente del agente, sin tecla nueva de sprint.

## Abrir el juego

Ejecutar `ProyectoRPGRTS/Builds/Windows/Aethernia.exe`. Mantener junto al ejecutable todos los archivos de la carpeta Windows. Para editarlo, abrir `ProyectoRPGRTS/` en Unity 6000.5.8f1 y cargar `Assets/01_Scenes/AetherniaVillageDemo.unity`. La escena anterior `PlayablePrototype.unity` se conserva.

En Unity: esperar a que termine la importación y compilación, revisar Console, seleccionar Game a 1280×720 o 1920×1080 y pulsar Play. Para detener, pulsar Play otra vez. Revisar cualquier error nuevo en Console y anotarlo con los pasos realizados. Cerrar ese editor antes de usar los lanzadores de validación en segundo plano.

El NavMeshAgent del héroe de la escena está desactivado deliberadamente en edición y se activa al iniciar Play, después de cargar el NavMesh. Mantener ese ajuste; evita un aviso nativo de agente sin superficie durante el arranque de la build.

Entrega E07: `ProyectoRPGRTS/Builds/Aethernia-E07-Aldea-Windows.zip`: descomprimir todo y abrir Aethernia.exe. Los ZIP anteriores, incluido Aethernia-Fase3-C02-Windows.zip, conservan las entregas históricas. Al iniciar se crea una partida de ensayo nueva; cargar una partida previa requiere F9 o Menú → Cargar. No hay guardado automático.

La demo E03–E06 añade tres identidades visuales estáticas, retratos conceptuales y la misión «El cargamento perdido»; seguir el recorrido al final de este documento y consultar Docs/E03_E06_PRODUCCION.md.

La casa medieval, el camino y los decorados se encuentran en la misma escena jugable, bajo `D01 Medieval Quarter`. La biblioteca reutilizable está en `Assets/02_Prefabs/Environment/Medieval`; medidas, capturas antes/después y límites en [informe D01](Docs/D01_ESCENARIO_MEDIEVAL.md). El héroe muestra la clase seleccionada mediante figuras geométricas provisionales; conserva la cápsula como física. Fichas e importación futura en [informe E01](Docs/E01_PERSONAJES.md). La cubierta se recorta al interponerse ante el héroe y se restaura al alejarse. La rueda quieta ya no debe provocar acercamiento automático.

E02 prepara importación Humanoid, equipo configurable y siete estados sin clips. E02-B añade un Monje estático provisional de 1,78 m basado en el concepto: cabeza rapada, tela naranja asimétrica, torso parcialmente descubierto, pantalón oscuro y vendas. En Personaje → Monje aparece su referencia 2D y estadísticas base; el modelo del mundo permite revisar la clase seleccionada. Equipo E01 conservado, sin animaciones. La alternativa E01 y el candidato E02 siguen disponibles. [Informe/capturas E02-B](Docs/E02B_MONJE_PROVISIONAL.md); uso del workbench y recursos finales faltantes en [guía E02](Docs/E02_IMPORTACION_HUMANOID.md).

## Controles

| Acción | Control |
| --- | --- |
| Seleccionar héroe, unidad RTS u objeto | Clic izquierdo |
| Caminar, ordenar un grupo o colocar un taller | Clic derecho en el terreno |
| Desplazar la cámara | WASD, flechas o arrastrar con botón central |
| Acercar/alejar | Rueda del ratón |
| Activar un objeto seleccionado cercano | E o botón Interactuar |
| Guardar/cargar fuera de un encuentro | F5/F9 o botones correspondientes |
| Cerrar ventanas y ayuda | × o ESC |

Los paneles y la escritura del nombre bloquean los controles del mundo. Abrir Personaje para elegir nombre/clase; el campo de nombre se oculta al cerrar esa sección o cambiar de modo. Cambiar clase durante esta prueba restablece la salud. Es una facilidad del prototipo, no una regla definitiva de progresión.

## Recorrido recomendado

1. Abrir Personaje, elegir Guerrero, Mago o Monje y escribir un nombre. Seleccionar el personaje y caminar con clic derecho.
2. Seleccionar el cofre marrón cercano, acercarse y pulsar E. Entrega equipo y recursos una sola vez. Abrir Inventario para equipar espada/túnica o usar ración.
3. En Inventario, probar Comercio: 5 oro por 2 madera. El mercado de la Facción A también permite esa operación al interactuar cerca.
4. Menú → Construir y hacer clic derecho en terreno despejado. Cada taller cuesta 10 oro y 5 madera; su huella es 3 × 3 m. No se permite solapar objetos ni colocar sobre el héroe. Consume alimento y produce madera cada 10 segundos simulados. Volver a explorar abandona la colocación.
5. Volver a Explorar. Buscar el portal gris a la izquierda del asentamiento, seleccionarlo y activarlo cerca. En la mazmorra, activar el hito marrón: la Facción A gana +10 de reputación y B cambia -5. El portal verde permite regresar.
6. Abrir Menú, guardar, cambiar una condición de prueba y cargar para comprobar que se restauran personaje, recursos, equipo, construcciones y consecuencias. F5/F9 conservan sus atajos. Registro muestra hechos/reputación existentes; no hay mapa o compañeros implementados.
7. Menú → Combate RPG: la iniciativa determina quién actúa primero. Usar Acercarse o clic derecho para el movimiento limitado, esperar a llegar, luego Atacar, Guardia o Recuperar y finalizar turno. El ataque necesita una línea libre de obstáculos; el rival avanza hasta 3 m por ruta entre turnos, con representación animada en la demo E09. Mago tiene alcance mayor; Monje obtiene bonificación de iniciativa. Recuperar se usa una vez por encuentro. Si hay derrota, volver a Explorar y elegir clase para reiniciar la prueba.
8. Tras una victoria RPG, Escalar a RTS prueba una transición opcional: los seis aliados empiezan con 14 de salud en lugar de 10. Menú → Batalla RTS inicia una prueba independiente de seis contra seis.
9. En RTS, seleccionar una unidad o Todo el grupo y dar órdenes con clic derecho. Avanzar ordena una formación corta; los soldados atacan automáticamente a los enemigos próximos. El campo de despliegue debe estar libre: si un taller ocupa esa zona, el juego rechaza iniciar sin cambiar la partida. Para este ensayo usar una partida con ese extremo del asentamiento libre; todavía no hay demolición. Al concluir, volver a Explorar y guardar el hecho de victoria; los soldados transitorios se retiran. Aún no existe un mapa de conquista territorial.

El calendario económico se pausa durante RPG y RTS, según la política provisional aprobada. Los encuentros deben terminar antes de cambiar de actividad o guardar; la interacción de exploración se desactiva durante combate.

El panel muestra arma y armadura equipadas. Una carga ausente/corrupta/incompatible o con posición inválida explica el rechazo y conserva la partida. Los guardados v1 válidos del prototipo se mantienen compatibles; datos con cantidades imposibles o reloj/ciclo incoherente se rechazan.

## Repetir la validación en segundo plano

Con Unity cerrado sobre este proyecto, ejecutar desde PowerShell en esta carpeta:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical -TimeoutMinutes 20
.\Tools\Invoke-UnityValidation.ps1 -Mode BuildVillage -TimeoutMinutes 15
.\Tools\Invoke-PrototypeSmoke.ps1 -RequireCargo -RequireVillageE07 -RequireAnimatedE08 -RequireTacticalE09
.\Tools\Invoke-PrototypeSmoke.ps1 -WithGraphics -RequireCargo -RequireVillageE07 -RequireAnimatedE08 -RequireTacticalE09
```

La batería actual reúne 89 pruebas de editor, geometría, UI, navegación, persistencia y sistemas integrados; incluye pruebas D01/E01/E02/E02-B y tres nuevas C02, con 44 layouts/capturas C02 a cuatro resoluciones, comprobación de textos/botones y raycast de cada control para detectar paneles que lo tapen. El segundo compila la build de Windows. El tercero inicia esa build sin gráficos, recorre los sistemas automáticamente y escribe un comprobante en Logs. El cuarto repite con dispositivo gráfico en segundo plano. Los lanzadores rechazan fallos de agente o excepciones del log aunque el recorrido devuelva 0. Las pruebas no utilizan la partida guardada del jugador ni sustituyen una sesión humana.

No repetir GenerateSandbox, GeneratePlayable, UpgradeVertical, ContextUi, MedievalQuarter, CharacterVisuals, HumanoidPipeline, MonkIdentity o MedievalHud sobre assets ya preparados: las herramientas rechazan sobrescrituras/duplicados. MedievalRefine/MedievalHudRefine son herramientas de autoría, no pasos para abrir o probar la entrega. Tampoco repetir CombatE09 ni Generate-E09Audio sobre los recursos existentes. Las escenas de muestra y referencia se conservan como evidencia de las etapas anteriores.

## Alcance y siguientes fases

Una sección del asentamiento tiene la muestra visual medieval D01; el resto, la mazmorra, unidades y talleres conservan geometría de prueba y nombres técnicos. No incluyen el continente completo, narrativa final, todas las clases, compañeros, diplomacia avanzada, asedios, arte final ni una edición comercial terminada. La evaluación humana de diversión, accesibilidad y equilibrio sigue siendo necesaria antes de convertir las hipótesis del prototipo en decisiones finales.

Consultar `CHECKLIST.md`, `Docs/ESPECIFICACION_PROTOTIPO.md`, `Docs/VALIDACION_PROTOTIPO.md` y `REGISTRO_CAMBIOS.md` para alcance aprobado, resultados e historial.


## Demo E03–E06 — El cargamento perdido

Abrir `Assets/01_Scenes/AetherniaVillageDemo.unity`. Elegir una clase en Personaje, cerrar la ficha con × o ESC y seleccionar al héroe. Clic derecho para caminar; E para interactuar cerca del objetivo. Acercarse al Habitante de la aldea (−7,5; −3 en X/Z), leer y aceptar su encargo. Seguir el objetivo hasta el arcón (−10; 3), interactuar e iniciar el combate RPG. Usar Acercarse, Atacar y Fin de turno; Recuperar cuando sea necesario. Tras vencer, regresar al habitante para entregar el cargamento: 20 oro y una ración si hay espacio, una sola vez. F5/F9 guardan/cargan fuera de combate y conservan la etapa.

El menú mantiene las actividades anteriores. El bastón del Mago es un accesorio visual, no un objeto equipable; cambiar clase no crea equipo. Las tres figuras son representaciones geométricas provisionales sin rig ni animaciones finales. Guía de prueba detallada: `Docs/E05_CARGAMENTO_PERDIDO.md`.


## Entorno E07

La misma demo incorpora entrada, plaza con pozo, cuatro edificios/cobertizo, camino al bosque, vegetación y utilería, con iluminación URP de tarde y recorte reversible de estructuras/árboles. El recorrido de misión, clases/equipo e inventario conserva los controles descritos. Para revisar la entrega usar feature/e07-aldea-medieval; es dependiente de E03–E06 mientras PR #1 no se integre. Comparación, medidas, pruebas y protocolo de cámara en Docs/E07_ALDEA_MEDIEVAL.md. La revisión humana de arte y X/ESC sigue pendiente; la geometría de personaje no tiene rigs/clips finales.
