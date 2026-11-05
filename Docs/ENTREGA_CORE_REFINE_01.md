# CORE-REFINE-01 · Informe técnico

La candidata parte del PR de ART-ENV-01 y abre la campaña en **AetherniaCoreRefine01.unity**. E23, ART-ENV-01, el estudio cultural y las escenas UX históricas siguen disponibles. La escena usa las mismas referencias de campaña, guardado, construcción, tropas y navegación.

## Presentación y combate

El HUD medieval, la capa UX, el panel de expedición y el presentador del duelo evaluaban el modo por separado. Coincidían en el mismo Canvas y algunos consideraban una ventana propia como modal sin comunicarlo a los demás. CORE usa el modo de sesión, la pausa y la visibilidad modal para decidir qué superficie se activa. El duelo y el grupo comparten un presentador táctico. El RTS conserva su panel y el diálogo de un NPC sin retrato usa iniciales.

El turno conserva **una acción y hasta cuatro metros**. El desplazamiento táctico descuenta la longitud de la ruta navegable. Un destino inválido, un objetivo incapacitado o un ataque fuera de alcance dejan intacto el presupuesto. La barra destaca a quien actúa y muestra vida, bando, iniciativa y selección. Cuando el Merodeador gana iniciativa recorre su ruta de apertura antes de ceder el control. Los acompañantes usan el orden individual que ya aplica el grupo. La IA intenta un ataque legal y conserva una salida acotada si no puede desplazarse.

Las tiradas de ataques y técnicas muestran d20, modificador, defensa y resultado. La técnica existente consume una acción y foco, con selección y confirmación. En grupo, un aliado vivo en el lado opuesto del rival, a distancia de cuerpo a cuerpo, con línea libre y en el mismo nivel permite **+2** al ataque básico. La regla se aplica a ambos bandos. El prototipo conserva las clases, armas, defensas y desbloqueos existentes.

## RTS, escenas y guardado

El acceso a la patrulla, la expedición y la conquista sigue las condiciones de la campaña. El RTS comparte el estado de mundo y utiliza sus propios controles para selección, órdenes, formaciones y cámara. El mismo flujo regresa a exploración y construcción. La ranura v1 conserva la identidad, recursos, edificios, ejército, capítulos y contratos. Las pruebas usan archivos de guardado separados.

## Arte

El foco de CORE es la entrada de la posada y su continuidad con la plaza. Las mallas de autoría son estáticas y comparten pizarra, madera, piedra, hierro y vidrio de E07. La marquesina y el farol se conectan al recorte contextual de la posada. Las losas usan la superficie navegable existente. La biblioteca conserva dos prefabs reutilizables. La marquesina mide 3,14 m de ancho y 1,25 m de fondo. Su malla y farol suman 1.644 triángulos. El módulo de losa tiene 980 triángulos y se instancia tres veces. No se detectaron cambios en la firma de colisiones, la malla de navegación ni los puntos de interacción.

## Validación y artefactos

- Unity 6000.5.8f1. **6/6 pruebas dirigidas**. Mara, los botones del duelo y los textos de iniciativa pasan a 1280×720, 1920×1080 y 2560×1440. Se probó una derrota real, la pantalla de reintento y la entrada al panel de construcción desde el menú. R2 evita repetir la vida del héroe durante RPG y los recursos en construcción. Quedaron registrados los intentos anteriores de layout y del ensayo de derrota. [Resultados](Validacion/COREREFINE01/DIRIGIDAS.json)
- Windows x64 R2 compilado desde `f805fcc`. El ZIP contiene la guía y los archivos del juego. Verificados CRC y SHA-256 de cada archivo extraído. [Integridad del ZIP](Validacion/COREREFINE01/PORTABLE.json)
- **18 combinaciones URP** ejecutadas con render verificado. La mediana de las seis muestras P95 por escenario fue 11,32 ms en aldea, 21,30 ms en menú y 18,98 ms en RTS. En la corrida ART de referencia fueron 11,23, 20,95 y 20,00 ms, respectivamente. Los tiempos incluyen el envío de render desde CPU en un player oculto, en escenas preparadas. Las corridas se hicieron por separado. El benchmark no mide FPS presentados, GPU ni RPG activo. [Medición](Validacion/COREREFINE01/RENDIMIENTO.json) · [Comparación](Validacion/COREREFINE01/COMPARATIVA_RENDIMIENTO.json)
- **Regresión general R1**. Unity registró 242 aprobadas, cero fallos y una omitida de Meshy privado, sobre 243 casos. El runner devuelve `Skipped:Ignored` por la omisión. Se usó el archivo histórico de física con los bytes de Git. [Resultados](Validacion/COREREFINE01/GENERAL.json)
- **6/6 procesos Windows en R2**, desde el ZIP extraído. La revisión anterior también obtuvo 6/6. Guerrero humano, Mago enano y Monje elfo completaron su ruta y cargaron la partida en un proceso nuevo. El Guerrero conquistó, el Mago cerró por pacto y el Monje continuó sin conquistar. La ruta llevó tres acompañantes a la guardianía y reutilizó el estado para construir y guardar. [Evidencia Windows R2](Validacion/COREREFINE01/WINDOWS.json) · [R1](Validacion/COREREFINE01/WINDOWS-R1.json)
- Capturas reales desde Unity. [Exploración](Images/COREREFINE01/Exploracion.png) · [Diálogo](Images/COREREFINE01/Dialogo-1280.png) · [Combate](Images/COREREFINE01/RPG-1280.png) · [Posada ART](Images/COREREFINE01/ART-Posada.png) · [Posada CORE](Images/COREREFINE01/CORE-Posada.png) · [Grupo RPG](Images/COREREFINE01/Grupo-RPG.png) · [Preparación RTS](Images/COREREFINE01/RTS-Preparacion.png) · [Conquista](Images/COREREFINE01/RTS-Conquista.png) · [Construcción](Images/COREREFINE01/Construccion.png)

## Pendientes de alcance

La presentación de técnica marca los objetivos legales en la barra de iniciativa. Quedan para una iteración posterior la previsualización de ruta, los puntos de acción variables, los críticos y un sistema de cobertura. La mejora visual se limita al umbral de la posada, la plaza y el tramo que los conecta. La comprobación geométrica a tres resoluciones cubre Mara y el duelo. Queda abierta para otros diálogos, los paneles RTS y el inventario. La aprobación humana de balance, usabilidad y arte permanece abierta.

La siguiente revisión humana debe probar la lectura del grupo, la elección de técnica y el cambio de cámara entre RPG y RTS en una partida nueva. Después conviene decidir qué problema de claridad o balance merece la próxima iteración.

[Guía para jugar](GUIA_CORE_REFINE_01.md) · [PR #19, borrador sobre ART-ENV-01](https://github.com/MathiasMerino/RPGRTS/pull/19)
