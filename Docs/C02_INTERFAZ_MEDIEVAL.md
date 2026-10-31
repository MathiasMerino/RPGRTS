# C02 — Interfaz medieval compacta

Estado: **C02.1..6 completadas y verificadas técnicamente**, 2026-10-30, America/La_Paz. Alcance autorizado: rediseñar la presentación uGUI existente sin reglas o sistemas nuevos. Checkpoint `codex/c02-base-25c1a10`; rama `codex/c02-medieval-ui`. Unity 6000.5.8f1/URP 17.5.0, GDD original y hash de lectura conservados; §12.5 confirma estilo PC medieval y disposición por validar. F2-07 humana sigue pendiente.

## C02.1 — Auditoría y decisiones

Las tres capturas del usuario se conservan en Documentacion/UI/C02 como referencias privadas fuera de Assets: sus textos, marcas y navegador no son instrucciones técnicas ni assets del juego. La primera organiza ficha/retrato sobre pergamino; la segunda separa inventario, figura y estadísticas con marcos oscuros; la tercera reserva el centro al mundo y agrupa acciones abajo. Ninguna es una captura de nuestro Unity ni prueba de sus sistemas; no hay referencia RTS independiente adicional. Reutilizamos jerarquía, agrupación y contraste, sin copiar caras, marcos, iconos o layouts literalmente.

UI real: uGUI, CanvasScaler 1280×720, MedievalTheme, MedievalPanel/MedievalButton, VerticalSliceHud/HudBinding, ExplorationHud, UiInputGuard y ClassReferenceCard. E02-B mantiene 49 pruebas previas. Problemas: cabecera/menú/recursos de gran altura, ayuda permanente, panel derecho de 420 px, campo de nombre separado, inventario en una línea y exceso de texto repetido. Materiales UI anteriores son colores planos, aunque el mundo ya usa URP.

Implementación prevista: cabecera compacta, dock inferior contextual, ventanas cerrables para ficha/inventario/menú/registro, ayuda opcional; una plantilla de ficha para las tres clases con retrato conceptual del Monje y retratos de geometría propia para Guerrero/Mago. Inventario seleccionable con acciones existentes. Solo taller disponible y cancelación del modo; RTS con selección y órdenes reales, RPG con iniciativa/acciones/objetivo. No inventar maná, niveles, atributos, habilidades, compañeros, edificios, formaciones o minimapa. El registro presenta hechos/reputación ya guardados, no una nueva lógica de misiones.

## C02.2 — Sistema visual

Paleta: madera #30251B/#60432B, bronce #A17B45, pergamino #D8C5A0, carbón #242424 y dorado base #C1A36B; se aclara el texto/dorado hasta #F1E7D0/#E1C288 para lectura sobre madera. Tinta oscura sobre pergamino. Ornamento discreto, selección por negrita/texto además de color y salud numérica junto a barra. Se mantiene fuente Unity disponible por legibilidad/licencia, sin instalar fuentes.

Skill **imagegen**, herramienta integrada image_gen.imagegen: un atlas original de cuatro superficies en `Assets/08_UI/Medieval/Textures/MedievalSkinAtlas.png`; [prompt exacto](../Documentacion/UI/C02/MedievalSkinAtlas.prompt.txt). Resultado 1254×1254, cuatro sprites persistentes sobre la misma textura y nine-slice en Unity. El PNG se conserva íntegro; sin edición raster, copias de marcos comerciales o dependencias nuevas. Retratos Guerrero/Mago en Portraits son renders nativos de los modelos propios E01, sin cámaras de retrato en ejecución. Monje mantiene el PNG/UV E02-B sin alterarlo.

## Implementación por bloques

- C02.3: cabecera de 66 px y dock de 106 px a referencia 1280×720, márgenes 16 px. Ocupación geométrica permanente 23,29 %; 76,71 % queda fuera de esas franjas, excluyendo avisos temporales y ventanas abiertas. Menú arriba; Personaje, Inventario y Registro abajo en exploración; Ayuda opcional y cierre por botón/ESC. Las ventanas no pausan la simulación. No se añaden atajos que interfieran con WASD, E o F5/F9.
- C02.4: ficha común con nombre editable, clase, PV actuales, daño **base**, alcance, velocidad, diferencia de clase y equipo real. Monje conserva concepto provisional; Guerrero/Mago muestran sus modelos actuales como tales. No se muestra maná/nivel/experiencia inventados. Inventario separa las tres categorías existentes, cantidades, selección, descripción/estado equipado y acciones de equipar/usar/comerciar; piezas ausentes se identifican y deshabilitan, sin fingir posesión.
- C02.5: el dock cambia automáticamente. RPG: iniciativa de ambos actores, orden, salud rival, movimiento restante, acción/guardia y comandos existentes. Construcción: único taller seleccionado, coste/huella/ciclo reales, instrucción de colocación y retorno que cancela ese modo. RTS: seleccionados/aliados/rivales, selección de grupo, avance y ataque cercano automático. No se crean edificios, habilidades, formaciones o suministros nuevos. El retorno está deshabilitado durante un encuentro activo.

Arquitectura: VerticalSliceHud conserva las suscripciones/HudBinding/comandos y soporta la vista compacta opcional; la ruta anterior sigue disponible para otras escenas. MedievalHudView posee presentación y estado de ventanas; InventoryPanelView posee solo selección UI. ClassReferenceCard/catálogo reciben retratos/captions opcionales. TurnBattle expone dos lecturas de estado (guardia/recuperación) y RTS una cuenta seleccionada; no cambia reglas o cálculos. El sondeo de 0,5 s existente permite observar movimiento/selección RTS y expiración de avisos; no se añade reconstrucción por frame. Tema/sprites y controles previos se reutilizan. Prefabs C02 MedievalWindow/ActionSlot son plantillas reutilizables, junto a ClassReferenceCard y componentes nativos de barra/grid/ventana.

Se desacoplaron las instancias UI de **esta escena** del prefab raíz heredado para serializar el nuevo layout de forma explícita: el primer ensayo detectó overrides anidados que perdían tamaño al recargar. Los prefabs históricos se conservan y no se cambia la organización del mundo. Botones/clases/acciones originales se reutilizan; nuevos botones solo presentan ventanas, ayuda, selección de objeto y retorno a la acción existente. Física/NavMesh comprobados sin cambios durante autoría.

## Validación

| Comprobación ejecutada | Resultado |
| --- | --- |
| Suite final (49 anteriores + 3 C02) | **52/52**, cero fallos/omisiones, salida 0; 7,57 s de casos, excluyendo arranque/recargas |
| Layout y captura nativa GPU | 11 estados × 4 resoluciones = **44 layouts/capturas**, 1280×720, 1600×900, 1920×1080 y 2560×1440 |
| Entrada UI | Botones dentro de pantalla, sin solapes, tamaño mínimo, textos completos y raycast superior de cada botón; centro del mundo accesible con ventanas cerradas |
| Funcional C02 | Tres clases/retratos/estadísticas, nombre, abrir/cerrar/ayuda, selección de objeto, equipo/ración/comercio, un comando por clic tras reactivar HUD, guardado aislado/carga y estados RPG/construcción/RTS |
| Regresión | Movimiento/cámara/colisiones, D01/E01/E02/E02-B, mazmorra/interacciones, inventario/guardado v1, economía/construcción/reputación, RPG y RTS conservados |
| Build Windows | Succeeded, salida 0; 183.099.535 bytes, 39,44 s |
| Ejecutable sin gráficos / con GPU | Ambos PASS/salida 0; recorridos 7,06 / 7,31 s, GPU RTX 3050 Laptop con Direct3D 12 |
| Logs finales | Sin errores/avisos C#, excepciones o fallos de agente detectados |
| Integridad | 278 .meta/GUID sin faltantes/huérfanos/duplicados; GDD, mundo, física/NavMesh, modelos/materiales D01/E01/E02/E02-B, prefabs UI históricos, paquetes y ProjectSettings intactos; historial del checklist conservado |

Las pruebas ejecutan Play Mode, comandos/botones y raycasts del EventSystem automáticamente; no son una sesión humana de ratón/teclado. Se revisaron visualmente las once capturas seleccionadas. Los selectores de pruebas históricas se adaptaron a las ventanas cerrables sin retirar aceptación de sistemas. Los ensayos iniciales fallidos y las correcciones de serialización, UTF-8, capas de UI y test de callback quedan en REGISTRO_CAMBIOS.md y Logs/C02-first/second/full-first/third. No se declara éxito a partir de solo compilar.

Métricas de alcance: un Canvas, 95 Graphic totales incluyendo controles inactivos, atlas ~11,21 MB de recurso nativo en editor y una sola cámara de juego conservada. Cuatro sprites comparten textura; retratos no se renderizan cada frame. No son mediciones de FPS, GC/draw calls o una mejora de rendimiento demostrada. Contraste de colores base calculado: texto/madera ≈12,15:1 y pergamino/tinta ≈9,55:1; no certifica cada píxel texturado o toda accesibilidad. No se aplicaron optimizaciones complejas sin datos.

Comprobantes locales: `ProyectoRPGRTS/Logs/C02-final-results.xml`/log, C02-build-summary/log, C02-headless/graphics-smoke/player, C02-ui-metrics.txt, C02-integrity.json y C02-package.json.

Entrega portable: `ProyectoRPGRTS/Builds/Aethernia-Fase3-C02-Windows.zip`, **77.005.206 bytes, 305 entradas**, CRC correcto; SHA-256 `2629012e4853500e12fbd495dbd1129452f2d80500f1c4b08d8ef2e00a8e734d`. Exe, DLL runtime y level0 coinciden por hash con la build probada; siete ZIP históricos conservan su hash. DoNotShip excluido. Descomprimir todo y ejecutar Aethernia.exe.

## Capturas antes/después

| Evidencia real | Enlace |
| --- | --- |
| Antes: HUD y ficha E02-B | [Exploración 720p](Images/C02-Before-Exploration-1280.png) · [Personaje 1080p](Images/C02-Before-Character-1920.png) |
| Después: HUD compacto | [Exploración 720p](Images/C02-Exploration-1280.png) |
| Plantilla común | [Guerrero](Images/C02-Guerrero-1280.png) · [Mago](Images/C02-Mago-1280.png) · [Monje 1080p](Images/C02-Monje-1920.png) |
| Ventanas y datos existentes | [Inventario 900p](Images/C02-Inventory-1600.png) · [Registro](Images/C02-Journal-1280.png) · [Menú](Images/C02-Menu-1280.png) · [Ayuda](Images/C02-Help-1280.png) |
| Dock por modalidad | [Construcción](Images/C02-Construction-1280.png) · [RPG](Images/C02-RPG-1280.png) · [RTS 1440p](Images/C02-RTS-2560.png) |

Before proviene de la validación E02-B del checkpoint. After son capturas Play Mode de la suite C02 final, no imágenes generadas del videojuego. La cámara de pruebas encuadra el asentamiento; se comparan distribución/ocupación de UI, no cambios geométricos del mundo. Las 44 imágenes completas quedan en Logs; trece representativas (dos previas/once actuales) se conservan en Docs/Images.

## Revisar en Unity y límites

1. Abrir `ProyectoRPGRTS/` en **Unity 6000.5.8f1**, cargar `Assets/01_Scenes/PlayablePrototype.unity`, esperar importación y revisar Console.
2. Game a una de las cuatro resoluciones, Play. Abrir Personaje, alternar clases y editar nombre; cerrar con ×/ESC. Caminar/seleccionar en el mundo; activar cofre cercano con E. Inventario → seleccionar objeto → equipar/usar/comerciar.
3. Menú → Guardar/Cargar; Registro muestra hechos/reputación. Menú → Construir; clic derecho coloca taller y Volver a explorar sale del modo. Menú → Combate RPG/Batalla RTS; comprobar acciones y retorno al terminar. Ayuda muestra controles del modo.
4. Para repetir automáticamente, cerrar ese editor y usar los cuatro comandos de [COMO_JUGAR](../COMO_JUGAR.md). No repetir MedievalHud ni los generadores históricos sobre assets existentes; MedievalHudRefine es autoría, no requisito de arranque.

Pendientes: F2-07 (evaluación humana de lectura/comodidad/diversión), aprobación estética final, iconografía específica y retratos/modelos finales de Guerrero/Mago. Fuente Unity y modelos geométricos siguen provisionales; Monje usa arte de propuesta. No se añade minimapa, grupo de compañeros, nuevo sistema de diario/misiones, niveles/maná/habilidades, edificios o formaciones. El mapa previsto en F3-C02 histórico no se declara implementado: el nuevo brief permite conservarlo fuera del alcance actual. Siguiente actividad recomendada: sesión humana C02 antes de ampliar HUD o producir arte final. No se inicia otra fase.
