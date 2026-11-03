# E18 — candidata de progresión para revisión

E18 queda preparada técnicamente como candidata, con la incidencia táctica RTS abierta y aprobación humana pendiente. **La regresión general no está completamente aprobada.** No se inicia E19. Rama `feature/e18-progresion-balance`, apilada sobre E17/PR8 sin fusionar. Unity6000.5.8f1, URP17.5.0, escena `Assets/01_Scenes/AetherniaProgressionDemo.unity`.

## Comportamiento integrado

- XP por hechos reales y ledger único; niveles1..4 a0/12/32/60XP. Crecen salud máxima, ataque/daño y puntos de mejora; guardar/cargar o equipar no acumula bonos ni cura. La ficha muestra el atributo principal aplicado y valores derivados; los demás conceptos del GDD no se presentan como estadísticas funcionales completas.
- Guerrero: Golpe poderoso/resistencia; Monje: Golpe concentrado/precisión/movilidad; Mago: Proyectil arcano/alcance. Nivel2, foco2 por encuentro, coste1/acción RPG o recargaRTS3s e intervalo de ataque. Impacto o Disciplina hasta2rangos, requisitos/puntos guardados, identidad compartida en ambos modos.
- Catálogo9tipos: equipo básico, armas avanzadas por clase, chaleco, talismán, mineral y consumible. Botín único configurable de ruinas y entrega pendiente si no caben ambas piezas. Forja/venta/recursos conectan expedición y asentamiento; equipo actualiza cálculos y usa representaciones existentes.
- Tres compañeros RPG separados de seis reclutas RTS. Espadachín resiste, luchador aporta daño/movilidad y arquero alcance. Compañeros ganan4XP por guardianía participada, nivel2 a8XP. Ejército se entrena con8oro/3madera, cuartel/taller2; conserva identidad, gana máximos/daño sin curación instantánea. El descanso existente4PV/10s en campamento alcanza también a los militares supervivientes E18.
- Producción, mejoras de edificios/capacidad y territorios reutilizadas. Venta de madera financia equipo/entrenamiento, sin arbitraje ni producción fuera del juego. Guardianías ajustables e IA que usa recuperación/guardia como acciones normales.
- Reintento tras derrota separado del cambio de clase, órdenes de grupo sin duplicar, objetivos con nombres y avisosXP. GuardadoVersion1 ampliado de forma aditiva y migración con defaults/créditos únicos. Escenas anteriores, GDD y arte conservados; sin Meshy, modelos o animaciones finales nuevos.

Parámetros/fórmulas y límites: [progresión](E18_PROGRESION.md), [balance](E18_BALANCE.md). [Guía para jugar y abrir Unity](GUIA_E18.md).

## Pruebas ejecutadas

| Evidencia | Resultado y alcance |
|---|---|
| [Regresión general](Validacion/E18/REGRESION_GENERAL.xml) |174/176, source60d01b8. Dos fallos registrados, sin omitir/excluir pruebas.|
| [Cierre dirigido final](Validacion/E18/CIERRE_DIRIGIDO_4.xml) |4/4: regresión visual histórica corregida sin modificar su aserción, equipo nuevo, ficha tresclases720/1080 y entrenamiento/descanso/HPguardado. Posterior a la general.|
| [Ciclo nativo](Validacion/E18/CICLO_NATIVO.json) |6/6procesos Windows: Nueva partida→misión→grupo→ruinas→XP/botín→forja/entrenamiento→conquista/producción→guardar, más nuevo proceso/Continuar por cada clase. Source60d01b8.|
| [Procesos Windows](Validacion/E18/WINDOWS.json) |8/8 incluyendo2procesos desde ZIP extraído con candidata finalae0051b: recorrido completoGuerrero y reinicio. Sin HP/fondos/recompensas inyectados.|
| [Simulación](Validacion/E18/BALANCE_SIMULADO.json) |720semillas sin geometría: grupo preparado360/360; solosGuerrero89/120,Monje86/120,Mago93/120. No equivale a jugabilidad física ni revisión humana.|
| [Integridad portable](Validacion/E18/PORTABLE.json) |CRC y SHA por archivoPASS;18ZIPanteriores intactos; carpetasDoNotShip excluidas.|
| [Auditoría de fuente](Validacion/E18/SOURCE_AUDIT.json) |Versiones/GDD/escenas previas conservadas; trespiezas privadas mantienen hash y no están trackeadas; sin patrones de credenciales ni archivos nuevos≥95MiB.|

Los bloques dirigidos intermedios y sus fallos/correcciones están conservados en Validacion/E18; no se suman repeticiones como cobertura distinta. La primera ejecuciónWindows falló tras conquista por watchdog de producción durante grabación; se amplió solo esperaQA E18, manteniendo ciclo10s e incremento exacto por ciclo real. La regresión visual general provenía de perder IDs destinados solo a bindings visuales: se restauró esa compatibilidad, sin admitirlos como objetos de inventario.

**Pendiente conocido:** `ThreeClassesWithLegalBacklineOrdersAt30` volvió a perder conGuerrero. E17-A-RTS sigue abierto: carga o reserva automática no garantiza supervivencia. No se alteró la expectativa para conseguir verde. No se repitió toda la batería general después del ajuste final; se ejecutaron4dirigidas y2procesos portables. No declarar176/176 ni resolución de la incidencia.

## Evidencia y build

[49capturas reales](Images/E18), con [manifiesto/origen/hash](Validacion/E18/CAPTURAS.json). [Combate RPG](Videos/E18/E18-Combate-RPG.mp4) y [patrulla RTS](Videos/E18/E18-Patrulla-RTS.mp4): clips continuos del player, audioDSP, decodificación verificada. Paso de captura12Hz; no es una medición de FPS. Native/PlayMode corresponden a estados y revisiones documentados, no a composiciones de arte.

ZIP Windows x64 local `ProyectoRPGRTS/Builds/Aethernia-E18-Progresion-Windows.zip`:87.842.896bytes, SHA256 `bc6828e2fe78c0f3f1115aa020ab7a5cc4913cd81b15dcd71aef3a8963d5d72f`. Build separada `Builds/ProgressionE18Windows`; ZIP local ignorado porGit, sin sustituir entregas previas. Incluye guía y trespartidas de ejemplo provenientes de recorridos reales. Para evaluación completa usar Nueva partida.

## Revisión humana y Git

[Hoja humana pendiente](Validacion/E18/REVISION_HUMANA.md): duración25–45min como meta, diversión, dificultad, legibilidad,×/Escape y dirección artística. Las duraciones automáticas no miden una sesión humana. No se agregaron esperas para inflar duración. Arte E08 provisional, sin inventario individual de compañeros, narrativa definitiva, árbol extenso o múltiples dificultades.

Commits por bloques desdee6ca1a1 hasta fuente finalae0051b; historial previo preservado. [PR#9 en borrador](https://github.com/MathiasMerino/RPGRTS/pull/9) publicado sobre `feature/e17-art-pipeline-alpha-polish`, abierto y sin merge. Main/dev yPR1..8 conservan estado/SHA; sin force/merge/configuración global ni publicación de recursos privados.

Siguiente actividad recomendada: revisión humana guiada y priorizar problemas concretos observados, especialmente táctica/claridad RTS. Cualquier fase nueva requiere autorización; E19 no iniciada.
