# UX-02 — Mundo inicial explorable

Base UX-01 d56fa59 / PR11, candidata sin evaluación humana. Rama feature/ux02-world-exploration, sin fusiones. GDD0.2 y dirección artística preservados; nombres, diálogos y distribución provisionales. Referencias adjuntas orientan composición y lectura; no se copian recursos.

## Secuencia y aceptación

| ID | Depende | Aceptación técnica | Estado |
|---|---|---|---|
| UX2-A | UX01 | Auditar incidencias y PR; proteger calidad local, privados, originales y21ZIP | Completado y verificado |
| UX2-B | A | Una candidata de escena sobre UX01; aldea, sendero, bifurcación, ruinas/campamento/puesto conectados; rutas completas | Completado y verificado |
| UX2-C | B | Cámara cercana única, límites y transición; oclusión reversible de techos/árboles/muros | Completado y verificado |
| UX2-D | B,C | NPC con actividad corta, diálogo modal y funciones reales; E con proximidad/foco/línea de visión; indicador temporal | Completado y verificado |
| UX2-E | D | Descubrimientos, alijo, materiales, señales/almacén; hechos y recompensas únicos persistentes; objetivos coherentes | Completado y verificado |
| UX2-F | B,D | Preparación segura, selección simple/múltiple/grupos, formación real, feedback y ataque explícito; órdenes inválidas rechazadas | Completado y verificado |
| UX2-G | C–F | HUD contextual y diálogos en720/1080/1440; guardado/Continuar/legado/ruinas y regreso | Completado y verificado |
| UX2-H | B–G | Dirigidas + general conservadas, Windows real, capturas/vídeos, build/ZIP, informe/guía/commits/PR apilado | Completado y verificado |
| UX2-HUM | H | Evaluación humana de comodidad, ritmo, diversión, balance y arte | Pendiente |

## Arquitectura prevista

Reutilizar GameSession/PrototypeState.Facts, DemoFlow, LostCargo, PartyWorld/DungeonExpedition, PlayerCommands, IsometricCameraRig y RtsBattleController. Configuración optativa WorldExploration; escenas anteriores no activan UX02. Las interacciones de mundo observan el mismo estado y llaman a comercio, reclutamiento, misiones y viaje existentes. No nuevo gestor de partida ni copia de reglas de combate. Los prefabs públicos, materiales, rigs y animaciones se comparten. 1unidad=1metro; caminos3.5–4m. Nueva escena sólo para preservar recuperación/comparación de UX01; no duplicar niveles interiores ni bibliotecas.

## Incidencias heredadas

General UX01:202/206. Reserva RTS Guerrero y tres timeouts180s abiertos; los tres pases aislados no demuestran causa. Revisar trazas y reproducciones acotadas, conservar fallos; no alterar reglas/asserts legítimos para obtener verde. La evaluación humana sigue pendiente porque el usuario está ausente.

## Distribución implementada (metros)

| Lugar | Coordenadas X,Z | Relación y escala |
|---|---|---|
| Aparición segura / aldea |−10,1.4 / alrededor de0,0 |Arquitectura anterior6×8m y almacén4.2×5m, puertas anteriores1.4×2.4m |
| Camino principal |−12,1.5 →−47,−0.5 |Cinta continua3.8m con curvas suaves y UV2m |
| Cruce del bosque |−25,−2 |Oeste: patrulla; sur: ruinas; señal en el margen |
| Bucle / ruinas |−26,−8 →−16,−16 →−9,−11 |Ramal3.5m, desvíos cortos de alijo y madera; regreso a la aldea |
| Campamento militar |−47,−0.5 |Capitán abre conversación antes de preparar, campo físico40×30m heredado |
| Posición hostil |−28,1..5.5 |Defensores de la patrulla existente, cobertura visual al fondo y espacio frontal libre |
| Asentamiento |9,−10 |Construcción/producción/reclutamiento existentes, tres NPC nuevos1.8m |
| Interior de ruinas |80,0 |Mismo interior24×16m, acceso contextual; salida devuelve al ramal del bosque |

Los edificios/colisiones originales se conservan. Dos árboles heredados se reubican sólo en UX02 porque invadían el nuevo camino/acceso. El suelo físico del ramal añade34×24m y las dos copias de fondo se aplanan localmente; las mallas E07/UX01 originales se conservan. Se oculta únicamente la cinta visual UX01 que se superponía a la nueva ruta. Cajas0.8m, barriles1m, árboles7–8m y arbustos0.9m reutilizados. El escenario es una muestra provisional, no una ciudad definitiva ni una réplica de las referencias.

## Conexión y límites de contenido

WorldExploration reutiliza GameSession.AdditionalInteraction y PrototypeState.Facts; no posee otra campaña. Descubrimientos de sendero, entrada, campamento y puesto se notifican una vez. Alijo8oro, ramas3madera y provisión2alimento son valores provisionales de esta muestra. Comercio sigue5oro→2madera; compañero conserva costes/reglas del catálogo vigente. Señal y almacén informan; la puerta permite pedir provisiones, sin interior jugable. Guardia patrulla dos puntos y se detiene cerca del héroe; comerciante/enlace permanecen en su puesto. Conversaciones miran al jugador, usan retratos/modelos existentes y liberan entrada al cerrar/cargar/cambiar contexto. Recompensas, recursos, grupo, ejército y misión comparten la misma partida.

La misión del cargamento y su RPG conservan reglas/recompensa. La entrada a ruinas exige campamento asegurado, como antes; acercarse no genera combate. Los puntos interiores y sus dos encuentros se conservan. Se heredan compañeros, progresión, culturas, diplomacia, territorios y conquista; no se afirma haber ampliado todos esos sistemas.

## Presentación y materiales

Exploración5.4m de semialtura, RPG7, RTS9 y construcción10; seguimiento/anticipación/amortiguación UX01. Camera100 → UxView500 → World550 → RTSOverlay600 → captura10000 evita proyectar indicadores con la pose anterior. La ocultación usa RoofCutaway reversible, sin cambiar materiales o colliders. El resaltado es una sola etiqueta [E] cercana; no hay iconos permanentes en todo el mapa.

Caminos: Earth Lit existente porque el shader Alpha requiere UV normalizados para su máscara y las cintas usan UV métricos. Resto de recursos nuevos: materiales Alpha/URP/MedievalSurface compartidos y viento0.035 existente, con VisualWindBounds0.12m. No se añaden materiales, texturas, shaders, dependencias ni assets externos. Iluminación URP/atmósfera previa conservadas. No se declara perfilado FPS/GPU ni aprobación artística humana.
