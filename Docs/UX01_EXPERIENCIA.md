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
