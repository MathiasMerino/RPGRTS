# E09 — Combate táctico y respuesta audiovisual

Inicio 2026-11-01. Base a7acaca/E08; PR #1/#2/#3 abiertos, borrador y sin fusionar. Rama feature/e09-combate-rpg-ia; PR apilado hacia E08. Motor 6000.5.8f1/URP 17.5.0; GDD v0.2 intacto por SHA dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661.

## Auditoría y decisiones

GDD §§3.2, clases y progresión: conceptual, reglas exactas pendientes. La especificación provisional autorizada y TurnBattle son la autoridad: d20+2 contra12 (+túnica), iniciativa d20 (+2 Monje), una acción/4m, enemigo18PV/3daño, guardia2 daño recibido, recuperación4 una vez. Guerrero24PV/6daño/1,8m/3,2m-s; Monje20/5/1,8/4; Mago16/8/6/3,2. No modificar estas cifras, RNG u orden de tiradas. Sin mana/hechizos individuales/progresión implementados: presentar el ataque a distancia existente del Mago como ataque arcano provisional, sin inventar un sistema mágico.

Ya existen reglas, navegación/LOS, ataque/guardia/recuperación, iniciativa, misión y siete clips Generic E08. Faltan fases observables, decisión enemiga separada y verificable, presentación del enemigo, resultados estructurados, selección/HP contextual y VFX/audio. No existen audios locales. Reutilizar visual Guerrero E08 para el rival provisional y crear efectos/PCM originales sin descargar recursos. No Humanoid/arte final.

## Plan secuencial

1. E09-01 auditoría y rama; E09-02 fases/resultados/IA local conservando reglas; E09-03 animación enemiga/sincronización/selección/UI; E09-04 VFX/sonidos originales e integración; E09-05 pruebas incrementales, regresión, capturas/video y revisión de escritura; E09-06 build/recorridos/ZIP/documentación/commits/PR. Humanos y recursos artísticos definitivos pendientes.

TurnBattle se extiende, no se duplica. EndTurn conserva la vía síncrona para simulación determinista y compatibilidad. Los comandos de la demo usan una secuencia corta de turno enemigo con fases reales y entradas bloqueadas; su avance depende de plazos finitos, nunca de eventos de animación. Daño calculado una vez por resultado aceptado. IA de un enemigo melee decide atacar/aproximarse/esperar con NavMesh, distancia y línea libre. Un segundo arquetipo a distancia queda propuesto: no hay perfil de reglas enemigo compatible aún, y añadir sus cifras alteraría el balance.

La presentación consume resultados con identidad y posiciones capturadas, no aplica daño. Fallback por clips ausentes; límites de partículas/voces explícitos, no optimización compleja sin perfilado. Una sola UI/gestor de ventanas C02. Audio original sintetizado, UI2D/mundo3D, pruebas de PCM y salida DSP cuando sea viable; escuchar/aprobar corresponde a humanos. Grabación de combate real en Unity, sin sustituirla por hojas conceptuales.

## Pendientes conservados

E08-H/E07-H y F2-07/X/ESC siguen pendientes. Modelos/ropa/rostros/UV y animaciones son pilotos; rigs Generic válidos, no Humanoid retargeteado. La causa de la incidencia transitoria de escritura E08 no está resuelta: repetir guardado y documentar LastWriteFailure si reaparece. Sin E10 ni merges de PR previos/main/dev.


## Implementación y responsabilidades

- `TurnBattle`: fases Player/Enemy/Finished, una acción por participante, ronda y resultados inmutables con secuencia. Rechaza duplicados, movimiento/ataque durante la fase contraria y distancias no finitas. Iniciativa, tiradas, PV, daño, equipo, recuperación y versión1 de guardado se conservan. La apertura por iniciativa sigue la regla anterior: ataque inicial únicamente si el rival gana y está en alcance/línea libre.
- `TurnEncounter`: administra el encuentro existente. `RequestEndTurn` es la vía del HUD en la demo: .65s antes de decidir, .35s para representar el paso y .45s de preparación antes de resolver. Bloquea entradas durante Enemy, revalida alcance/LOS al resolver y finaliza sin depender de un clip. La vía síncrona `EndTurn` queda para compatibilidad y pruebas existentes. Clear/desactivación cancela la espera sin dejar una fase bloqueada.
- `MeleeEnemyPolicy`: reglas locales puras y comprobables. Sin turno/vida/objetivo válido espera; en alcance y LOS ataca; con camino completo se aproxima; de lo contrario espera. Un objetivo actual, sin conocimiento de otros enemigos o atributos nuevos. Paso físico por NavMesh <=3m, parada1.3m; tolerancias previas1.8/1.9m conservadas. La representación recorre esa ruta durante .35s. Un paso rechazado se comunica como espera.
- `CombatResolution`/`CombatFeedback`: identidad de encuentro/secuencia y cantidades auténticas, aceptadas una vez. Los presentadores y el HUD las leen; no aplican daño ni cambian recompensas. `PrototypeDirector` mantiene los comandos, misión, mundo compartido y separación de modos.
- `CharacterCombatAnimationPresenter.MotionStarted` informa del inicio efectivo de Attack E08. El VFX del ataque del héroe se presenta al45% del clip; hay un plazo máximo1.6s y fallback inmediato cuando falta Attack/perfil. El daño autoritativo y la barra se actualizan al aceptar la acción; animación/VFX no vuelven a calcularlo. La sincronización es temporal y provisional, no un evento físico en el filo ni retargeting.
- `EnemyCombatVisual`: reutiliza visual Guerrero E08 y su Animator Generic, arma de entrenamiento cosmética, selección, aproximación, Attack/Hit/Defeat. Independiente del agente y sin colliders/root motion propios. Conserva el último golpe/muerte al volver la misión a explorar. Desactivar el presentador recupera la cápsula física; si falta rig/perfil enemigo se conserva la cápsula y las reglas siguen funcionando.
- `CombatHudPanel`: un panel contextual dentro de C02, objetivo seleccionable por botón/collider real, PV18, ronda/fase/acción/movimiento y resultado/dado. El footer también distingue Enemy. Se asignó el sprite compartido a las barras: uGUI necesitaba ese recurso para mostrar el relleno parcial; se comprueba la geometría visible, no solo fillAmount. No hay gestor de ventanas nuevo.

## Diferencias entre clases y recursos

| Clase | Acción existente presentada | Identidad preservada |
| --- | --- | --- |
| Guerrero | Attack de espada E08, Defend, Hit, Defeat; impacto metálico/partículas ocre | 24PV, arma anclada al rig y equipo real |
| Monje | Attack marcial E08 sin espada visible, defensa corporal, golpe seco breve |20PV, mayor movilidad/bonificación de iniciativa |
| Mago | Attack a6m, gesto E08 y traza arcana azul violácea discreta; daño real |16PV, ataque8; sin mana/hechizos individuales nuevos |

Recuperar mantiene la acción real (+4 hasta máximo, una vez); no hay clip específico, por lo que conserva Idle y presenta la cantidad real/tono/partículas. Los siete estados existentes y los trece clips E08 siguen siendo pilotos Generic. Equipo, colliders y lógica no se duplican. El bono previo de espada equipada (+1daño) sigue siendo global, también si Monje/Mago muestran otra presentación; las pruebas equipan ese objeto legalmente. Restricciones de armas/bonos por clase no existen aún y requerirían una decisión de reglas, no ocultar el bono ni cambiar el balance en E09.

`Assets/09_Feedback/CombatE09` contiene un Particle System compartido (`Impact.prefab`), material URP Particles/Unlit y máscara suave original32px. Impactos acertados, bloqueo parcial, guardia, recuperación y resultado final usan colores discretos; los fallos no muestran impacto acertado. Anillo de selección32segmentos, traza visual sin proyectil físico y cifras auténticas flotantes. Máximo8 objetos transitorios activos; cada emisor admite24partículas y emite12; cola visual acotada8. No se añadió VFX Graph ni dependencias.

Catorce WAV originales mono48kHz/16bit: Select, PanelOpen/Close, Confirm, Sword, Punch, Block, Arcane, Hurt, Miss, Victory, Defeat, Quest, Recover. Generados una vez por `Tools/Generate-E09Audio.py` con DSP determinista, sin muestras/música comercial. `Audio/PROVENANCE.json` conserva procedencia, hashes y medidas PCM. Son sonidos piloto sintetizados; no grabaciones realistas finales ni aprobación de mezcla. Banco ScriptableObject compartido, seis voces fijas (2UI2D y4mundo3D), volúmenes .18/.30; se reciclan sin crear AudioSources por golpe. Eventos reales, suscripciones liberadas; finalizar misión se detecta una vez sobre el mismo estado, cargar una partida completada no fabrica ese evento.

La grabación usa [AudioRenderer de Unity](https://docs.unity3d.com/ja/current/ScriptReference/AudioRenderer.html): fotogramas Play Mode y muestras del DSP nativo, no una mezcla de WAV inventada después. El MP4 añade esas muestras a los frames originales. El audio no silencioso confirma salida técnica; escucha, timbre y mezcla requieren evaluación humana.

## Verificación y límites

Pruebas focales iniciales:11/11 reglas;16/16 con presentación, recursos, misión y grabación. Se corrigieron fixtures (collider hijo y estado local capturado que no sobrevivía EnterPlayMode), sin alterar reglas para forzar resultados. En pruebas de misión se recoge el cofre, equipa legalmente, acepta el encargo, combate con dados/turnos reales, regresa y obtiene20oro una sola vez con cada clase. Se guarda/carga tras aceptación, recuperación del cargamento y entrega; no se fuerza vida/victoria.

Primera regresión88/89: la prueba de layout integrada Monje detectó un botón nuevo26px bajo el mínimo32px. Se corrige el tamaño del botón/texto, conservando el criterio y el XML intermedio. El wrapper alcanzó15min; Unity terminó después y produjo el XML. Repetición con límite20min. La primera toma tenía oclusión por el entorno. Al repetir en el claro se detectó que el guion atacaba en fotogramas fijos mientras seguía Enemy y saltaba acciones: ahora espera fase/acción reales y exige daño de ambos lados. La toma corregida pasó con salida0:120fotogramas, DSP stereo48kHz RMS0.005859/peak0.076390; no captura de poses. Build, comprobantes y SHA finales constan en la entrega al final de este informe. Las capturas automáticas/inspección técnica de píxeles y el recorrido del ejecutable no son una sesión manual con ratón/teclado. No se mide FPS a partir de la captura fijada a12Hz. Sin perfil temporal detallado de GC o benchmark de rendimiento general; límites de emisores/voces comprobados, sin optimización compleja.

### Evaluación humana pendiente (E09-H)

1. Abrir `ProyectoRPGRTS/` con Unity6000.5.8f1, escena `Assets/01_Scenes/AetherniaVillageDemo.unity`, Play. No repetir generadores. Elegir una clase fuera de combate, recoger cofre cercano y equipar espada/túnica desde Inventario.
2. Ir al Habitante(-7.5,-3 X/Z), aceptar, caminar al arcón(-10,3), interactuar. Seleccionar rival por clic o botón contextual, acercarse con movimiento legal y esperar a llegar.
3. Probar Atacar/Guardia/Recuperar/Fin de turno. Durante ENEMIGO ACTUANDO las acciones deben quedar bloqueadas. Revisar lectura de PV/dado/mensajes, golpe marcial, espada anclada, ataque arcano real, daño/defensa y cámara/oclusión.
4. Tras vencer, volver al habitante y cobrar una sola vez. Guardar/cargar fuera del encuentro con F5/F9; repetir desde partida previa para Monje/Mago. La semilla de la sesión humana es libre: los ensayos automáticos con semilla42 no garantizan ganar todas las tiradas futuras.
5. Escuchar UI, impactos, resultados y entrega; revisar volumen y espacialización. Comprobar X/ESC, abrir inventario, construir fuera de rutas y probar RTS. Registrar incidencias/clase/acción; confirmar por separado F2-07, E07-H y E08-H.

Siguiente actividad recomendada: esa evaluación, correcciones acotadas según resultados y recursos finales aprobados. Enemigo ranged, habilidades/mana, nuevo balance y retargeting Humanoid requieren propuesta posterior. No se inicia E10.

## Resultados finales y entrega local — 2026-11-01

**Técnica E09 verificada; evaluación humana pendiente.** 89/89 pruebas, cero fallos/omisiones y salida0. Se conservan los73 casos anteriores y16 nuevos:12 reglas/IA/dados/guardia/duplicados,3 recursos/integración/misión y1 grabación real. Compilación/editor exacto6000.5.8f1. El XML informa32.93s del runner; las recargas/entrada de Play Mode hacen que el proceso completo tarde más de quince minutos. Límite del lanzador20min; no se aceleró ni deshabilitó la regresión para obtener PASS.

| Comprobación | Resultado comprobado |
| --- | --- |
| Tres clases y misión | NPC, diálogo, aceptación, ruta, dados, turnos escalonados, victoria, regreso y20oro una sola vez; save/load v1 en cada etapa |
| IA y entradas | Aproximación <=3m y espera fuera de rango; LOS física impide daño; End/Attack duplicados rechazados; fase enemiga termina sin evento de clip |
| Presentación | Attack/Guard/Hit/Defeat E08, diferencias por clase; falta de Attack/perfil y perfil enemigo nulo sin bloqueo; barras por malla visible y botón32px |
| Sistemas previos | Movimiento/cámara/clases/inventario/equipo/construcción/economía/mazmorra/reputación/RTS/persistencia cubiertos por la regresión y sondas |
| Windows sin gráficos | PASS, recorrido90.81s; misión/animación de las tres clases y sonda E09; salida0 |
| Windows con RTX3050 | PASS, recorrido90.60s; mismos sistemas, captura nativa y salida0 |
| Audio |14PCM originales importados, salida DSP nativa grabada y pico0.076364 en ambos players; escucha humana pendiente |
| Escritura E08 | No reapareció en regresión final/players/guardados de misión; causa raíz no confirmada, operación atómica y esquema intactos |

Memoria: instantánea nativa `Profiler.GetTotalAllocatedMemoryLong` al final del recorrido125212006bytes sin gráficos y163487282 con GPU; no es GC por frame. Pico de working set **muestreado** por Windows cada0.5s:224141312/555999232bytes; incluye proceso completo y captura, no solo combate. Duración de proceso111.23/94.52s incluyendo arranque. No benchmark de FPS, GC temporal ni comparación de rendimiento controlada; no se declara optimización cuantificada. Emisores<=24partículas por sistema, hasta8transitorios y6voces comprobados.

**Evidencia:** [19 capturas originales](../../Images/E09) (18de misión/acciones por clase y1del player), [vídeo de combate real con audio](../../Images/E09/E09-Combate-Real.mp4), [manifest/hash y DSP](../../Validacion/E09_EVIDENCIA.json), [resultados/sondas](../../Validacion/E09_RESULTADOS.json), [regresión89/89](../../Validacion/E09_REGRESION.xml). El vídeo tiene120frames a paso de captura12Hz, diez segundos; H264/AAC decodificados, audio nativo estéreo48kHz, RMS0.005859/pico0.076390. Esa tasa fija no mide FPS. Las acciones se ejecutan con estado/dados reales, no muestras de poses. Las tomas Ready/Attack/EnemyTurn/Damage/Victory/Completed proceden de la misión real. Player-Tactical es un encuentro de prueba posterior a la entrega, no otra recompensa de misión. Inspección técnica de originales/contacto/frames realizada; ninguna sesión humana se marca aprobada.

**Paquete local Windows x64 Development:** `ProyectoRPGRTS/Builds/Aethernia-E09-Combate-Windows.zip`,86756445bytes/306archivos. SHA256 `11d3db3e940f286ef79ff7c2f52dcee3919f2a9accee4a7226b68d9cf2c17a79`. CRC y hash de cada archivo contra Windows verificados; los once ZIP históricos conservan su tamaño/SHA. Build203799042bytes; compilación85.00s. [Manifiesto del paquete](../../Validacion/E09_PAQUETE.json). Descomprimir completo y ejecutar Aethernia.exe; no se publica una release estable.

GDD/hash, escena PlayablePrototype, recursos E07 y mallas/rigs/clips/prefabs E08, persistencia y versiones preservados: [auditoría](../../Validacion/E09_AUDITORIA.json). Rama feature/e09-combate-rpg-ia; commits4b21d4f (reglas),c545386 (presentación/recursos),a07dbd5 (pruebas/sondas), más informe/evidencia. [PR#4](https://github.com/MathiasMerino/RPGRTS/pull/4) abierto en borrador hacia E08; no merges ni modificaciones de main/dev. Pendientes E09-H/E08-H/E07-H/F2-07 y recursos definitivos conservados; no E10.

Para repetir: con Unity cerrado ejecutar `Tools/Invoke-UnityValidation.ps1 -Mode Vertical -TimeoutMinutes 20`, después `-Mode BuildVillage`. Ejecutar `Tools/Invoke-PrototypeSmoke.ps1 -RequireCargo -RequireVillageE07 -RequireAnimatedE08 -RequireTacticalE09`; repetir con `-WithGraphics`. Son recorridos automáticos, no sustituyen los pasos humanos anteriores. Regeneración de evidencia: `python Tools/Pack-E09Evidence.py` después de las pruebas/player; no repetir generadores de assets.



Publicación para revisión: rama sincronizada con origin, PR#4 abierto/borrador, sin merges. Main/dev conservan SHA inicial y#1/#2/#3 continúan abiertos, borrador y sin fusionar. [Comprobante GitHub](../../Validacion/E09_GITHUB.json) registra el SHA de creación del PR; el cierre documental posterior conserva el historial. Git limpio y HEAD/origin/PR comprobados al concluir. La próxima actividad es E09-H; no queda una tarea técnica de E09 bloqueada por recursos, pero el arte/audio final y la aprobación humana permanecen fuera de este cierre técnico.
