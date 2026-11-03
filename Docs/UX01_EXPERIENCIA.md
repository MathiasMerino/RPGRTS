# UX-01 — Experiencia progresiva

Base: E19 `3e53508`, escena E19 conservada. Nueva escena optativa UX-01; las reglas anteriores conservan su comportamiento. Narrativa y arte provisionales, GDD 0.2 sin modificaciones.

## Plan y aceptación

| ID | Dependencia | Alcance y aceptación | Estado |
|---|---|---|---|
| UX-A | E19 | Inicio sin enemigos activos; misión existente introduce el primer combate, regreso real a exploración | En desarrollo |
| UX-B | A | Una cámara, seguimiento suave, zoom y perfiles por modo; WASD y colisiones preservados | Pendiente |
| UX-C | A | Una interacción cercana válida, E y objetivo coherentes, sin repetir recompensas | Pendiente |
| UX-D | B,C | HUD contextual sin duplicación; ventanas X/ESC y tres resoluciones | Pendiente |
| UX-E | A | Preparación RTS sin daño; selección, movimiento y reagrupación reales antes del ataque | Pendiente |
| UX-F | B,E | Circulación y arena legibles con recursos públicos existentes, navegación propia | Pendiente |
| UX-G | A,E | Progreso aditivo, Continuar y partidas antiguas sin reiniciar avances | Pendiente |
| UX-H | A–G | Pruebas, capturas, recorrido Windows, build, informe y PR apilado sobre E19 | Pendiente |
| UX-HUM | H | Evaluación humana de comodidad, claridad, diversión y balance | Pendiente |

Archivos y resultados se incorporarán al checklist tras cada bloque. La incidencia histórica RTS sigue abierta: la prueba de reserva no constituye una estrategia de victoria garantizada; no se modifica para obtener verde.

## Decisiones técnicas

Se reutilizan DemoFlow, DemoProgress, LostCargoQuest, PlayerCommands, IsometricCameraRig, GameSession y RtsBattleController. El perfil UX es optativo y no crea otra partida ni otro gestor de modos. La misión del cargamento conserva sus hechos y recompensas. La preparación RTS reutiliza las mismas tropas y órdenes del encuentro real. El escenario y navegación nuevos se guardan aparte, sin regenerar E19.

GitHub verificado: PR1–10 abiertos en borrador, sin fusionar; main/dev intactas. Rama UX sobre E19/PR10. QualitySettings es un cambio local anterior ajeno, SHA `62494399222dd3d2c992d4c9c41deecdeffd5e50fde962be46b7f38a0a09b916`; se conserva sin incluirlo en commits. Recursos privados excluidos.

## Integración y QA en curso

La escala racial provisional hacía que el contrato de rig rechazara modelos válidos al cambiar sus proporciones. Se admite únicamente la escala registrada por RaceVisualVariant; se siguen rechazando escalas accidentales y no cambian actores, colisiones ni assets originales.

El editor comprueba el motor y el interactor mediante sus API: el teclado sintético no llega a su bucle de entrada en esta configuración. El player Windows verificará WASD/E reales por separado. Se conservan todos los resultados fallidos; el pase dirigido anterior fue6/7 con muerte legítima del Monje, aun con aliados supervivientes. El recorrido actual consume sólo raciones existentes, prioriza arqueros y utiliza Guardia/ataques legales, sin modificar estadísticas o condiciones de victoria.

Cámara: exploración6.2m de semialtura, RPG7, RTS8.5, construcción10; zoom acotado, amortiguación0.18s, reposo tras2s de desplazamiento manual. Distancia de cámara45m. Un controlador escribe su transformación.

Escala: actor/cápsula1.8m, modelos raciales proporcionales; caminos4m, campo transitable64×60m, corredor de despliegue sin obstáculos, edificios y piezas anteriores en metros. Superficie de navegación UX con GUID propio y volumen limitado para no duplicar las ruinas E14. Materiales Grass/Earth y decoraciones públicas compartidas.
