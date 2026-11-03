# UX-01 — Entrega técnica de la candidata

Candidata con regresión general202/206 y cuatro fallos conservados; evaluación humana pendiente. Base E19 conservada; rama `feature/ux01-onboarding-camera-world`, [PR11 en borrador](https://github.com/MathiasMerino/RPGRTS/pull/11) sobre E19/PR10. Sin fusiones ni cambios main/dev.

La nueva escena es `Assets/01_Scenes/AetherniaUx01Demo.unity`. Reutiliza las cuatro razas, tres clases, estado del mundo y sistemas de E19. Su configuración es optativa: las escenas anteriores conservan su entrada habitual.

## Cambios implementados

- Menú, creación e introducción breve → exploración segura → habitante/cargamento → RPG real → recompensa única → patrulla segura → selección/movimiento/reagrupación → ataque ordenado → campamento, producción y reclutamiento → mundo libre existente.
- Seguimiento isométrico suave, anticipación pequeña, zoom limitado y encuadres por modo. Un solo controlador; WASD de exploración usa la velocidad derivada de raza/clase.
- Un candidato E cercano con histéresis y objetivo asociado al objeto real. Modalidad de ficha/ayuda/diálogo bloquea motor, cámara e interacción. El teclado funciona independientemente de la presencia de ratón.
- HUD contextual con recursos cuando son necesarios, ventanas anteriores conservadas, aviso temporal único, orientación ocultable y controles RTS compactos.
- Preparación RTS sin daño; hitos requieren llegada real y el ataque requiere orden explícita. Las órdenes de movimiento no caducan arbitrariamente a los tres segundos en UX. Se conservan salud, bajas y resolución auténticas.
- Persistencia aditiva de hitos, posiciones y selección de preparación. Continuar reconstruye la formación sin duplicar tropas; partidas avanzadas E19 conservan su progreso y ejército cultural.
- Corregido el contrato de rig para proporciones raciales registradas, manteniendo rechazo de escalas accidentales y escala física del actor.
- Escenario occidental con camino de4m y recursos compartidos. Una copia de Grass E16 aplana sólo campo/camino para impedir que colinas decorativas oculten cuerpos sobre navegación plana; original y escenas anteriores intactos. Campo físico40×30m, fondo visual64×60m.

## Pruebas y resultados

| Método | Resultado | Alcance y límites |
|---|---|---|
| Play Mode / datos dirigidos |9/9 en DIRIGIDA_10 |Tres clases;12variantes visuales dentro de un caso; bloqueo modal; legado E13/E19; preparación, guardado y carga; alturas de ambas mallas |
| Refinamiento posterior |3/3 en DIRIGIDA_11 |Superficies, guardado E19 avanzado con ejército cultural y bloqueo modal; no se suma como regresión global |
| Windows con gráficos, build final5468c38 |4/4procesos |Humano–Guerrero/Enano–Mago: menú, creación, intro, WASD/E reales, misión, RPG, RTS dirigido, construcción, producción, reclutamiento y reinicio/Continuar |
| ZIP extraído, Windows con gráficos |2/2procesos |Enano–Mago: recorrido completo y nuevo proceso/Continuar; mismo estado sin duplicar recompensas |
| Paquete |CRC/SHA/extracciónPASS |87864571bytes; veinte ZIP previos intactos; DoNotShip excluido |
| Regresión general |202/206;0omisiones |Falla la expectativa RTS histórica y hay tres timeouts de180s; XML y mensajes completos conservados |
| Aislamiento de los tres timeouts |3/3PASS |Mismo código/límites en editor nuevo; incidencia del pase largo permanece abierta |
| Prueba humana |Pendiente |Comodidad/diversión/balance/arte; clic/arrastre/Shift físicos y oclusión en juego manual |

Los ensayos del editor utilizan las API del motor, interactor y órdenes. Windows verifica teclas WASD/E mediante InputSystem y ejecuta los mismos controladores reales para selección/órdenes y botones de interfaz. No equivale a una sesión humana con ratón. Se generaron capturas a1280×720,1920×1080 y2560×1440; la inspección de imágenes por el agente tampoco constituye aprobación humana.

Derrotas conservadas: el Monje pierde el primer RTS en el pase DIRIGIDA_10 y vence en un segundo intento usando el botón Reintentar. Se mantienen ambos resultados; se consumen sólo raciones existentes y se usan Guardia/ataques/órdenes legales. El reintento restaura salud conforme a la regla previa del prototipo; no se inyecta HP ni dinero en el recorrido. Un segundo fracaso hace fallar el ensayo. Los registros están en IntentosDirigidos. No garantiza ganar con cualquier estrategia ni corrige la incidencia histórica de reserva RTS.

## Compilación y revisión

- Juego: `ProyectoRPGRTS/Builds/Ux01Windows/Aethernia.exe`.
- Portable: `ProyectoRPGRTS/Builds/Aethernia-UX01-Experiencia-Windows.zip`.
- SHA256ZIP: `689f162ecb1ebd24d7d9609beee3dea7a95bd17161ffb284d17fc7f0bf4c3483`.
- Código/escena/mallas de la build:5468c38; huellas en SOURCE_BUILD.json. El ajuste local previo QualitySettings se conserva aparte, SHA62494399222dd3d2c992d4c9c41deecdeffd5e50fde962be46b7f38a0a09b916; no forma parte de los commits. La única diferencia de recurso respecto a la huella de construcción es la caché generada de AetherniaBodySDF: se restauró el asset tipográfico versionado tras las pruebas; GUID y fuente originales se conservan. INTEGRIDAD_FINAL.json distingue esta caché de cambios inesperados.
- En Unity6000.5.8f1 abre Assets/01_Scenes/AetherniaUx01Demo.unity y pulsa Play. El ejecutable inicia directamente el menú. Consulta GUIA_UX01.md para el circuito y los controles.

## Evidencias y Git

37PNG finales/comparativas: Docs/Images/UX01;1280×720 nativos y1920/2560 en Play Mode. Vídeos: Docs/Videos/UX01/UX01-Combate-RPG.mp4 y UX01-Patrulla-RTS.mp4. Son frames del player con audio DSP original;12Hz de captura no mide FPS ni tiempo de juego humano. No se generaron poses o victorias sintéticas.

Se conservan fallos iniciales, XML, recibos, intentos y diagnóstico de superficie en Docs/Validacion/UX01. SurfaceIsolation es una fixture de presentación aislada y está rotulada como tal, no como una partida completa.

Commits de implementación:1030e63 (flujo/cámara/interacción/preparación),42ea494 (modalidad/rig/persistencia verificados),5468c38 (doble superficie, UV, teclado y reintento). PR11draft sobre E19/PR10; revisar PR1→…→10→11. Sin merges, force push, cambios main/dev ni publicación de originales/derivados privados. Escenas y mallas originales conservadas.

## Incidencias de la regresión general

- E18RtsScenarioTests.ThreeClassesWithLegalBacklineOrdersAt30: Guerrero no gana la estrategia de reserva. Aserción histórica intacta; sigue abierta.
- HumanoidPipelineTests.MissingAssetsLeaveMovementAndMonkFallbackFunctional: timeout180s; rutina anterior de fallback/movimiento, sin fallo de aserción.
- UxExperienceTests.MageFullRoute: timeout180s en el pase general; el mismo recorrido tiene comprobantes Windows completos.
- UxExperienceTests.ModalWindowsBlockMotorCameraAndInteractor: timeout180s en el pase general; pasó en las dirigidas anteriores.

Los tiempos de los tres casos exceden ampliamente el límite al quedar el proceso sin avanzar durante intervalos largos. No se confirmó la causa externa ni se atribuye al código sin reproducción. La única comprobación aislada de esos tres casos pasó3/3, sin ampliar timeouts o modificar código. Se registra en TIMEOUTS_AISLADOS.xml/json; no reproduce la interrupción del pase largo y no confirma su causa ni sustituye GENERAL.xml. No se repite toda la regresión para ocultar fallos, ni se mezclan los pases para declarar206/206. Los agregados temporales XML atraviesan recargas de dominio y no miden rendimiento ni duración real del juego.

## Pendientes y siguiente trabajo recomendado

Revisión humana de ritmo, legibilidad, cámara/oclusión, ratón y dificultad, especialmente coordinación del héroe cuerpo a cuerpo en RTS. La prueba histórica de reserva RTS y su aserción siguen conservadas; sus fallos anteriores no se borran por este pase dirigido. Arte/modelos, relato y balance permanecen provisionales. No se midieron FPS/GPU/GC; los recibos Windows sólo registran duración automatizada y memoria muestreada.

Recomendación: realizar primero la guía de revisión humana, registrar decisiones de UX y balance, y revisar la cadena de PR antes de promover una versión. No se inicia E20 ni se convierte esta candidata en una versión estable aprobada.


Comparación visual real (estados/tiempos distintos): [RPG anterior](Images/UX01/Antes/human-warrior-Rpg.png) → [RPG UX](Images/UX01/Windows/warrior-11-FirstRpg.png); [RTS anterior](Images/UX01/Antes/human-warrior-Rts.png) → [preparación UX](Images/UX01/Windows/warrior-18-Positioned.png); [vista general](Images/UX01/Windows/warrior-24-WorldOverview.png).
