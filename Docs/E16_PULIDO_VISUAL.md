# E16 — pulido visual y presentación

Entrega visual implementada y comprobada sobre E14/E15; conserva una incidencia de QA RTS y la valoración humana pendientes. Unity **6000.5.8f1**, URP **17.5.0**. Nueva escena: `Assets/01_Scenes/AetherniaVisualPolish.unity`. Las escenas y mecánicas anteriores se conservan. No comienza E17.

## Cambios

| Área | Resultado | Estado del arte |
| --- | --- | --- |
| Menú | Título, acciones alineadas, aviso separado, ilustración Monje y realce de botones | Temporal; ilustración identificada como tal |
| Selección | Tres tarjetas centradas, proporción conservada, rol y selección por borde/texto; nombre/linaje existentes | Monje disciplinado, Guerrero marcial y Mago reservado proceden de hojas existentes |
| HUD y ventanas | Tema común para exploración, personaje, inventario, partida, campamento, RPG/grupo y RTS; captions adaptables; vida por color | Sin sustituir comandos o datos por decoración |
| Tipografía | LiberationSans SDF existente/OFL; sin doble dibujo del Text legado, márgenes y tamaños corregidos | Sin regenerar fuente ni instalar plugins |
| Biblioteca | Nueve prefabs: botón normal/primario/secundario, tarjeta, cuero, pergamino, marco, tooltip y aviso; atlas de cinco regiones, 320×64 | Botón base y tarjeta usados en runtime; componentes restantes preparados sin funcionalidades ficticias |
| Aldea | Vegetación moderada, césped cálido y props reutilizados; caminos y escala de 1 unidad = 1 m conservados | Sin ampliar mapa ni añadir obstáculos |
| Ruinas | Piedra fría, antorchas cálidas, marcadores, relleno suave y luz regional reversible | Geometría/animaciones provisionales |
| RTS | Mismo lenguaje material, props en márgenes, barras por equipo y nombres reducidos; HUD delante de etiquetas del mundo | Navegación, unidades, órdenes y reglas conservadas |
| URP | Tres variantes Lit con texturas compartidas, smoothness 0.08–0.13; FXAA; saturación −6, contraste 7, exposición −0.05, bloom 0.06 y viñeta 0.12 | Sin ShaderGraph, agua o VFX complejos sin necesidad |

Iluminación: sol exterior 1.08 cálido/interior 0.70 frío; cuatro antorchas existentes con pulso ±4.5% y un relleno interior de intensidad 1.4/rango 8. Sin sombras puntuales nuevas. El ambiente y el sol se restauran al desactivar el componente; no cambia el reloj.

## Recursos y conservación

`Assets/12_VisualPolish/` contiene perfil, prefabs, sprites, materiales y Volume. Las hojas **GUERRERO_HOJA_PROPUESTA_v01**, **MAGO_HOJA_PROPUESTA_v01** y **MONJE_HOJA_PROPUESTA_v02**, en `ProductionProvisional/Source`, se reutilizan mediante recortes Sprite/UV; ningún PNG original se editó o duplicó. No se descargaron recursos ni se generaron imágenes IA como evidencia.

`Presentation.asset → Classes/Figure` controla las tarjetas/menú. Portrait es auxiliar; el HUD/ficha conserva el catálogo E08 y sus campos ReferenceArt/ReferenceUv/PortraitSprite. Los modelos/rigs siguen desacoplados de controlador y estado. [Guía de sustitución](GUIA_E16.md).

GDD, paquetes/versiones, escenas históricas, NavMesh y fuentes de gameplay permanecen intactos. La prueba compara todos los colliders entre E14 y E16, incluidos inactivos, y la misma referencia de reglas. Decoración nueva sin colliders ni rebake. El FBX Meshy y sus metadatos ajenos se conservan fuera de estos commits. Evidencia: [conservación](Validacion/E16/CONSERVACION.json), [PNG originales](Validacion/E16/ART_SOURCE.json).

## Pruebas y hallazgos

| Ejecución | Resultado | Evidencia |
| --- | --- | --- |
| Base antes de editar | 6/6 | BASE_6.xml |
| Focal tras ajustes de UI/iluminación | 7/7 | FOCAL_FINAL_7.xml |
| Regresión general | **125/125**, antes del último ajuste de orden de dibujo | REGRESION_125.xml |
| Postajuste: UI, RTS y asentamientos | **9/10**; UI y asentamientos aprobados, derrota del Mago en patrulla | FOCAL_POST_CAPAS_INTERMEDIA.xml |
| Repetición aislada RTS, sin cambios | **1/1**, recorre las tres clases | RTS_REPETICION_1.xml |
| Windows r3 y ZIP extraído | **14/14 procesos PASS**: 12 GPU (tres clases, inicio/reinicio) + 2 desde ZIP extraído | WINDOWS.json y Windows/ |

Se comprobaron menú, tres clases, opciones, abrir/cerrar ventanas, guardar/cargar, viaje y combate grupal a **1280×720, 1920×1080 y 1024×768**: 29 capturas Play Mode, bounds/raycast y desbordamiento TMP en pantallas cubiertas. Eso no garantiza ausencia de problemas en cualquier texto, resolución o estado futuro.

Incidencias conservadas, sin ocultar fallos:

- Primer pase detectó recorte de «Personaje», pivote de imagen y aviso superpuesto: corregidos. Capturas nativas ampliadas detectaron captions de 216 px en botones de 88–148 px, barras oscuras y separadores dañados: corregidos; el HUD ahora tapa etiquetas del mundo, evitando que cubran órdenes RTS.
- Regresión intermedia 124/125: altura transformada 31.999939 frente a 32 px en un test de navegación. Se añadió tolerancia explícita de **0.001 px**, sin reducir el botón. XML intermedio conservado; repetición general 125/125.
- **Variación RTS pendiente:** el Mago perdió una patrulla del test histórico `AetherniaFirstDemo`, donde no se aplica el perfil E16. No fue timeout ni excepción UI. Pasó en la regresión general y en la repetición aislada, sin modificar salud, recursos, código o reglas entre intentos. No se declara determinismo por repetirlo: E16-QA-RTS registra revisión de condiciones/estrategia/balance. Los doce procesos GPU r2 también pasaron; sus comprobantes se identifican como intermedios.

La inspección técnica de capturas se realizó; **no hubo sesión manual humana nueva**. FPS y GC no se perfilaron; tiempos de recorrido y memoria muestreada no equivalen a un benchmark.

## Evidencia y entrega

[Comparación antes/después](Images/E16/COMPARACION.md). Capturas procedentes exclusivamente de Unity o del player, sin retoques. Encuadres/estados pueden diferir: comparación cualitativa. **57 PNG reales**: 29 Play Mode, 24 del player r3 y 4 anteriores. Manifiesto de origen, resolución y SHA en [CAPTURAS.json](Validacion/E16/CAPTURAS.json).

Build: `ProyectoRPGRTS/Builds/VisualPolishWindows/Aethernia.exe`. ZIP final: `Aethernia-E16-Pulido-Visual-r3-Windows.zip`, **87,478,209 bytes**, SHA-256 `8c3b39c6b2747d9ba9649740c6f862a604c2666b5ff272d10cb428c46ac29737`. CRC, extracción y SHA por archivo aprobados; **16 ZIP anteriores intactos**. [Manifiesto portable](Validacion/E16/PORTABLE.json). Las builds se excluyen de Git y se regeneran con BuildVisualPolish; el ZIP local contiene guía y tres partidas E13 reales separadas de la partida personal.

Rama `feature/e16-pulido-visual-ui`, desde `4efcd00` (PR #6). Commits de implementación: `2fb2843` menú/tarjetas, `a34026e` escena/atmósfera, `5747a01` biblioteca/arte, `07c1bb9` captions/vida, `c01bca7` QA y `43201c7` capas RTS y `5d93f5b` empaquetado/evidencia. Publicación en borrador pendiente del cierre Windows; main/dev y PR1..6 se verifican sin cambios, sin merges ni push forzado.

Pendientes: evaluación estética/uso humana, variación del recorrido automático RTS y arte/modelos/rigs/animaciones finales. Se recomienda probar personalmente las tres clases con [GUIA_E16](GUIA_E16.md), registrar hallazgos y sustituir arte mediante catálogos; no iniciar otra fase automáticamente.
