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

- Unity 6000.5.8f1. **6/6 pruebas dirigidas**. Mara, los botones del duelo y los textos de iniciativa pasan a 1280×720, 1920×1080 y 2560×1440. Se probó una derrota real y la pantalla existente de reintento. Quedaron registrados los intentos anteriores de layout y del ensayo de derrota. [Resultados](Validacion/COREREFINE01/DIRIGIDAS.json)
- Windows x64 compilado desde `b64b12a`. El ZIP contiene la guía y los archivos del juego. Verificados CRC y SHA-256 de cada archivo extraído. [Integridad del ZIP](Validacion/COREREFINE01/PORTABLE.json)
- **18 combinaciones URP** ejecutadas con render verificado. La mediana de las seis muestras P95 por escenario fue 12,89 ms en aldea, 21,75 ms en menú y 20,62 ms en RTS. En la corrida ART de referencia fueron 11,23, 20,95 y 20,00 ms, respectivamente. Los tiempos incluyen el envío de render desde CPU en un player oculto, en escenas preparadas. Las corridas se hicieron por separado. El benchmark no mide FPS presentados, GPU ni RPG activo. [Medición](Validacion/COREREFINE01/RENDIMIENTO.json) · [Comparación](Validacion/COREREFINE01/COMPARATIVA_RENDIMIENTO.json)
- Capturas reales desde Unity. [Exploración](Images/COREREFINE01/Exploracion.png) · [Diálogo](Images/COREREFINE01/Dialogo-1280.png) · [Combate](Images/COREREFINE01/RPG-1280.png) · [Posada ART](Images/COREREFINE01/ART-Posada.png) · [Posada CORE](Images/COREREFINE01/CORE-Posada.png)

## Pendientes de alcance

La presentación de técnica marca los objetivos legales en la barra de iniciativa. Quedan para una iteración posterior la previsualización de ruta, los puntos de acción variables, los críticos y un sistema de cobertura. La mejora visual se limita al umbral de la posada, la plaza y el tramo que los conecta. La aprobación humana de balance, usabilidad y arte permanece abierta.

[Guía para jugar](GUIA_CORE_REFINE_01.md)
