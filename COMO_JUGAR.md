# Crónicas de Aethernia Prototipo técnico

Este prototipo implementa el recorrido jugable y los sistemas mínimos acordados para comprobar la visión del GDD. Las reglas y los recursos artísticos son provisionales; la hoja de ruta de expansión está en `Docs/PLAN_EXPANSION.md`.

La segunda etapa audita y corrige la integración existente. El estado real por sistema, pruebas automáticas y límites están en `Docs/AUDITORIA_FASE_2.md`. La sesión humana sigue pendiente; usar `Docs/PRUEBA_MANUAL_FASE_2.md` para registrar resultados.

La entrega actual añade la primera iteración de fase 3 (A/B/C1): auditoría visual, dirección artística documentada y UI contextual. En exploración, el panel derecho abre Resumen, Personaje, Inventario y Partida. La ayuda cambia por modo; Interactuar aparece solo en exploración. Escenario y apariencias todavía son los provisionales anteriores. Véanse `Docs/FASE3_ITERACION_1.md` y `Documentacion/DIRECCION_ARTISTICA.md`.

## Abrir el juego

Ejecutar `ProyectoRPGRTS/Builds/Windows/Aethernia.exe`. Mantener junto al ejecutable todos los archivos de la carpeta Windows. Para editarlo, abrir `ProyectoRPGRTS/` en Unity 6000.5.8f1 y cargar `Assets/01_Scenes/PlayablePrototype.unity`.

En Unity: esperar a que termine la importación y compilación, revisar Console, seleccionar Game a 1280×720 o 1920×1080 y pulsar Play. Para detener, pulsar Play otra vez. Revisar cualquier error nuevo en Console y anotarlo con los pasos realizados. Cerrar ese editor antes de usar los lanzadores de validación en segundo plano.

El NavMeshAgent del héroe de la escena está desactivado deliberadamente en edición y se activa al iniciar Play, después de cargar el NavMesh. Mantener ese ajuste; evita un aviso nativo de agente sin superficie durante el arranque de la build.

Entrega E01: `ProyectoRPGRTS/Builds/Aethernia-Fase3-E01-Windows.zip`: descomprimir todo y abrir Aethernia.exe. Los ZIP anteriores conservan las entregas históricas. Al iniciar se crea una partida de ensayo nueva; cargar una partida previa requiere F9 o Partida → Cargar. No hay guardado automático.

La casa medieval, el camino y los decorados se encuentran en la misma escena jugable, bajo `D01 Medieval Quarter`. La biblioteca reutilizable está en `Assets/02_Prefabs/Environment/Medieval`; medidas, capturas antes/después y límites en [informe D01](Docs/D01_ESCENARIO_MEDIEVAL.md). El héroe muestra la clase seleccionada mediante figuras geométricas provisionales; conserva la cápsula como física. Fichas e importación futura en [informe E01](Docs/E01_PERSONAJES.md). La cubierta se recorta al interponerse ante el héroe y se restaura al alejarse. La rueda quieta ya no debe provocar acercamiento automático.

## Controles

| Acción | Control |
| --- | --- |
| Seleccionar héroe, unidad RTS u objeto | Clic izquierdo |
| Caminar, ordenar un grupo o colocar un taller | Clic derecho en el terreno |
| Desplazar la cámara | WASD, flechas o arrastrar con botón central |
| Acercar/alejar | Rueda del ratón |
| Activar un objeto seleccionado cercano | E o botón Interactuar |
| Guardar/cargar fuera de un encuentro | F5/F9 o botones correspondientes |

Los paneles y la escritura del nombre bloquean los controles del mundo. Abrir Personaje para elegir nombre/clase; el campo de nombre se oculta al cerrar esa sección o cambiar de modo. Cambiar clase durante esta prueba restablece la salud. Es una facilidad del prototipo, no una regla definitiva de progresión.

## Recorrido recomendado

1. Abrir Personaje, elegir Guerrero, Mago o Monje y escribir un nombre. Seleccionar el personaje y caminar con clic derecho.
2. Seleccionar el cofre marrón cercano, acercarse y pulsar E. Entrega equipo y recursos una sola vez. Abrir Inventario para equipar espada/túnica o usar ración.
3. En Inventario, probar Comercio: 5 oro por 2 madera. El mercado de la Facción A también permite esa operación al interactuar cerca.
4. Entrar en Construir y hacer clic derecho en terreno despejado. Cada taller cuesta 10 oro y 5 madera; su huella es 3 × 3 m. No se permite solapar objetos ni colocar sobre el héroe. Consume alimento y produce madera cada 10 segundos simulados.
5. Volver a Explorar. Buscar el portal gris a la izquierda del asentamiento, seleccionarlo y activarlo cerca. En la mazmorra, activar el hito marrón: la Facción A gana +10 de reputación y B cambia -5. El portal verde permite regresar.
6. Abrir Partida, guardar, cambiar una condición de prueba y cargar para comprobar que se restauran personaje, recursos, equipo, construcciones y consecuencias. F5/F9 conservan sus atajos.
7. Probar Combate RPG: la iniciativa determina quién actúa primero. Usar Acercarse o clic derecho para el movimiento limitado, esperar a llegar, luego Atacar, Guardia o Recuperar y finalizar turno. El ataque necesita una línea libre de obstáculos; el rival avanza hasta 3 m por ruta entre turnos, aún con presentación instantánea. Mago tiene alcance mayor; Monje obtiene bonificación de iniciativa. Recuperar se usa una vez por encuentro. Si hay derrota, volver a Explorar y elegir clase para reiniciar la prueba.
8. Tras una victoria RPG, Escalar a RTS prueba una transición opcional: los seis aliados empiezan con 14 de salud en lugar de 10. Batalla RTS desde la barra superior inicia una prueba independiente de seis contra seis.
9. En RTS, seleccionar una unidad o Todo el grupo y dar órdenes con clic derecho. Avanzar ordena una formación corta; los soldados atacan automáticamente a los enemigos próximos. El campo de despliegue debe estar libre: si un taller ocupa esa zona, el juego rechaza iniciar sin cambiar la partida. Para este ensayo usar una partida con ese extremo del asentamiento libre; todavía no hay demolición. Al concluir, volver a Explorar y guardar el hecho de victoria; los soldados transitorios se retiran. Aún no existe un mapa de conquista territorial.

El calendario económico se pausa durante RPG y RTS, según la política provisional aprobada. Los encuentros deben terminar antes de cambiar de actividad o guardar; la interacción de exploración se desactiva durante combate.

El panel muestra arma y armadura equipadas. Una carga ausente/corrupta/incompatible o con posición inválida explica el rechazo y conserva la partida. Los guardados v1 válidos del prototipo se mantienen compatibles; datos con cantidades imposibles o reloj/ciclo incoherente se rechazan.

## Repetir la validación en segundo plano

Con Unity cerrado sobre este proyecto, ejecutar desde PowerShell en esta carpeta:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical
.\Tools\Invoke-UnityValidation.ps1 -Mode Build
.\Tools\Invoke-PrototypeSmoke.ps1
.\Tools\Invoke-PrototypeSmoke.ps1 -WithGraphics
```

El primer comando ejecuta 42 pruebas de editor, geometría, UI, navegación, persistencia y sistemas integrados; incluye catorce layouts de la UI contextual, cuatro casos D01, tres casos E01 y capturas con GPU en modo batch. El segundo compila la build de Windows. El tercero inicia esa build sin gráficos, recorre los sistemas automáticamente y escribe un comprobante en Logs. El cuarto repite con dispositivo gráfico en segundo plano. Los lanzadores rechazan fallos de agente o excepciones del log aunque el recorrido devuelva 0. Las pruebas no utilizan la partida guardada del jugador ni sustituyen una sesión humana.

No repetir GenerateSandbox, GeneratePlayable, UpgradeVertical, ContextUi, MedievalQuarter o CharacterVisuals sobre assets ya preparados: las herramientas rechazan sobrescrituras/duplicados. MedievalRefine es una herramienta de autoría, no un paso necesario para abrir o probar la entrega. Las escenas de muestra y referencia se conservan como evidencia de las etapas anteriores.

## Alcance y siguientes fases

Una sección del asentamiento tiene la muestra visual medieval D01; el resto, la mazmorra, unidades y talleres conservan geometría de prueba y nombres técnicos. No incluyen el continente completo, narrativa final, todas las clases, compañeros, diplomacia avanzada, asedios, arte final ni una edición comercial terminada. La evaluación humana de diversión, accesibilidad y equilibrio sigue siendo necesaria antes de convertir las hipótesis del prototipo en decisiones finales.

Consultar `CHECKLIST.md`, `Docs/ESPECIFICACION_PROTOTIPO.md`, `Docs/VALIDACION_PROTOTIPO.md` y `REGISTRO_CAMBIOS.md` para alcance aprobado, resultados e historial.
