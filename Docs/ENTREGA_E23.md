# E23 · Primera alfa integrada

Escena `Assets/01_Scenes/AetherniaE23IntegratedAlpha.unity`. Build Windows x64 `AETHERNIA_ALPHA_E23_Windows.zip`. La candidata usa la campaña, el mundo libre y los datos de E22 sobre el mismo estado. Se conservaron razas, clases, retratos, modelos, navegación, materiales URP y escenas anteriores. Unity 6000.5.8f1.

## Cambios jugables

- El cartel inferior muestra el verbo del objetivo válido de **E**. Consulta el mismo foco de la entrada de jugador y sólo se muestra al acercarse. Las confirmaciones del cargamento comprueban alcance y línea de visión. Cobrar guarda inmediatamente.
- La guía de exploración libre prioriza entregar los encargos listos y luego orienta a los aceptados. Si el destino está en otra zona, señala la entrada o salida de las ruinas. La campaña conserva la prioridad hasta su cierre.
- El centinela presenta un solo objetivo. La cámara encuadra al héroe y al primer rival de una guardianía con margen para el panel derecho. El panel genérico deja espacio al de turnos durante estos combates.
- El Mago recibe una pista para atacar desde lejos y retirarse antes de terminar el turno. Se mantuvieron estadísticas y reglas. El comercio y los encargos reutilizan sonidos del proyecto.
- Elia conserva la bolsa real, el inventario y los servicios que dependen del taller. Las recompensas siguen alimentando la misma economía de mejora, producción y reclutamiento. RTS conserva despliegue seguro, órdenes, técnica, conquista y producción territorial.

## Evidencia y límites

- E22 cerró 230/230 en regresión y 8/8 procesos Windows, con tres cierres y recarga. El fallo visual del segundo botón pasó a E23. [Informe E22](ENTREGA_E22.md)
- E23 pasó 5/5 dirigidas después de corregir el alcance de la prueba de interfaz. El primer intento obtuvo 4/5 porque el validador del tablón también inspeccionó el menú oculto. Se verificaron los botones y etiquetas del tablón a 1280×720, 1920×1080 y 2560×1440. Esta prueba cubre el tablón. [Prueba dirigida](Validacion/E23/Dirigidas.xml)
- ZIP comprobado con CRC y SHA‑256 por archivo y verificado de nuevo tras extraerlo fuera del proyecto. Se conservaron los 26 ZIP históricos. [Manifiesto](Validacion/E23/PORTABLE.json)
- 6/6 procesos del ZIP externo. Guerrero humano conquistó, Mago enano pactó y Monje elfo cerró por cuenta propia. Cada uno completó los tres encargos, compró y vendió, mejoró la granja y continuó desde un nuevo proceso. [Recorridos](Validacion/E23/WINDOWS.json)
- El centinela se ganó con cero, uno y dos acompañantes y su panel mostró un solo objetivo. En una prueba dirigida posterior, el Guerrero ganó ambas guardianías originales con uno, dos y tres acompañantes y cobró el hallazgo. En solitario perdió la primera guardianía y pudo retirarse al campamento. La prueba observó ambos resultados con la semilla 42 y una estrategia fija. [Composiciones](Validacion/E23/GUARDIANIAS.csv)
- El Mago volvió a terminar el primer combate con 16/18 PV después de cuatro retiradas con semilla 42. Quedan pendientes más semillas, estrategias y evaluación humana.
- 224 capturas de los tres recorridos principales. La selección incluye inventario, ficha y diario, y el antes y después del centinela. [Capturas Windows](Images/E23/Windows) · [Tablón en tres resoluciones](Images/E23/Layout)
- El recorrido grabado del Guerrero añadió 2/2 procesos Windows. El vídeo conserva 151 fotogramas de la cámara del juego durante la operación RTS y audio DSP. [Vídeo](Videos/E23/Puesto-del-Umbral.mp4) · [Registro](Validacion/E23/GRABACION.txt)
- Regresión general 232/232, sin fallos, omitidas ni inconclusas. La prueba adicional de composiciones aprobó 1/1 después de la general y se registra aparte. [General](Validacion/E23/GENERAL.json) · [Dirigida de composiciones](Validacion/E23/GuardianComposition.xml)

## Rendimiento medido

Perfilado en un i5‑12450H con RTX 3050 Laptop GPU, desde la extracción externa y con un guardado real de E22. Se completaron 18 combinaciones de calidad y resolución con 9.705 solicitudes de render URP fuera de pantalla. El rótulo E13 del JSON pertenece a la sonda reutilizada.

| Carga · Alto 1080p | Intervalo medio de frame del proceso | P95 | P95 de asignación GC |
|---|---:|---:|---:|
| Aldea en pausa | 4,43 ms | 6,43 ms | 1.156 B |
| Menú con vista previa | 7,83 ms | 14,01 ms | 1.618 B |
| Preparación RTS en pausa | 7,27 ms | 10,59 ms | 11.044 B |

La medición incluye solicitudes de render. Quedan pendientes GPU frame time, FPS presentados, mazmorra en movimiento, combate activo y construcción. La evaluación humana de balance, oclusión, ritmo, nitidez y fluidez presentada queda pendiente. [Datos](Validacion/E23/PERFORMANCE.json)

La próxima revisión debe empezar con una sesión humana de la candidata. Prioriza el primer RPG del Mago con otras semillas, la dificultad de las guardianías en solitario y el rendimiento en una batalla activa.

El guardado v1 de E22 se conserva. La compilación usa la configuración local existente de antialiasing de calidad media, que queda fuera de los commits. No se añadieron dependencias de arte.

[Guía](GUIA_E23.md) · [Notas](NOTAS_E23.md) · [Integración](INTEGRACION_E23.md) · [PR #17](https://github.com/MathiasMerino/RPGRTS/pull/17)
