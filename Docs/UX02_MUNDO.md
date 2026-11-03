# UX-02 — Mundo inicial explorable

Base UX-01 d56fa59 / PR11, candidata sin evaluación humana. Rama feature/ux02-world-exploration, sin fusiones. GDD0.2 y dirección artística preservados; nombres, diálogos y distribución provisionales. Referencias adjuntas orientan composición y lectura; no se copian recursos.

## Secuencia y aceptación

| ID | Depende | Aceptación técnica | Estado |
|---|---|---|---|
| UX2-A | UX01 | Auditar incidencias y PR; proteger calidad local, privados, originales y21ZIP | En desarrollo |
| UX2-B | A | Una candidata de escena sobre UX01; aldea, sendero, bifurcación, ruinas/campamento/puesto conectados; rutas completas | Pendiente |
| UX2-C | B | Cámara cercana única, límites y transición; oclusión reversible de techos/árboles/muros | Pendiente |
| UX2-D | B,C | NPC con actividad corta, diálogo modal y funciones reales; E con proximidad/foco/línea de visión; indicador temporal | Pendiente |
| UX2-E | D | Descubrimientos, alijo, materiales, señales/almacén; hechos y recompensas únicos persistentes; objetivos coherentes | Pendiente |
| UX2-F | B,D | Preparación segura, selección simple/múltiple/grupos, formación real, feedback y ataque explícito; órdenes inválidas rechazadas | Pendiente |
| UX2-G | C–F | HUD contextual y diálogos en720/1080/1440; guardado/Continuar/legado/ruinas y regreso | Pendiente |
| UX2-H | B–G | Dirigidas + general conservadas, Windows real, capturas/vídeos, build/ZIP, informe/guía/commits/PR apilado | Pendiente |
| UX2-HUM | H | Evaluación humana de comodidad, ritmo, diversión, balance y arte | Pendiente |

## Arquitectura prevista

Reutilizar GameSession/PrototypeState.Facts, DemoFlow, LostCargo, PartyWorld/DungeonExpedition, PlayerCommands, IsometricCameraRig y RtsBattleController. Configuración optativa WorldExploration; escenas anteriores no activan UX02. Las interacciones de mundo observan el mismo estado y llaman a comercio, reclutamiento, misiones y viaje existentes. No nuevo gestor de partida ni copia de reglas de combate. Los prefabs públicos, materiales, rigs y animaciones se comparten. 1unidad=1metro; caminos3.5–4m. Nueva escena sólo para preservar recuperación/comparación de UX01; no duplicar niveles interiores ni bibliotecas.

## Incidencias heredadas

General UX01:202/206. Reserva RTS Guerrero y tres timeouts180s abiertos; los tres pases aislados no demuestran causa. Revisar trazas y reproducciones acotadas, conservar fallos; no alterar reglas/asserts legítimos para obtener verde. La evaluación humana sigue pendiente porque el usuario está ausente.
