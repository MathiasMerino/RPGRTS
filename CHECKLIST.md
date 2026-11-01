# Checklist de Crónicas de Aethernia

Actualizado: 2026-11-01, America/La_Paz. Fase 3 autorizada mediante el prompt «Dirección artística, rediseño visual, personajes e interfaces reutilizables»; A/B/C1, D01, E01, infraestructura E02, propuesta artística E02-A e identidad provisional E02-B, más rediseño de interfaz C02.

## Resumen actual

- E08: tres humanoides provisionales con rigs Generic/skin y siete estados, integrados en demo; **73/73 regresión**, Windows/headless/GPU PASS, 49 PNG/tres MP4 y ZIP verificado. [PR #3](https://github.com/MathiasMerino/RPGRTS/pull/3) abierto en borrador sobre E07; arte final/retargeting Humanoid y sesión humana pendientes. Informe Docs/E08_PERSONAJES_ANIMADOS.md.

- E07: aldea integrada, 33 prefabs, trece materiales nuevos y cinco albedos originales; **67/67 regresión**, 24 comparativas reales, build/recorridos headless-GPU PASS, captura del player y ZIP verificado. Informe Docs/E07_ALDEA_MEDIEVAL.md; aprobación humana/artística pendiente.

- E03–E06: tres apariencias estáticas/retratos/equipo compartidos y misión integrada en AetherniaVillageDemo. **62/62 pruebas**, build Windows y dos recorridos independientes con las tres clases PASS. Informe Docs/E03_E06_PRODUCCION.md. Arte/rig/clips finales y confirmación humana pendientes.

- Historial conservado: **26 de 26 tareas del alcance de prototipo y planificación aprobado**. La fase 2 se registra al final con estados independientes.
- Fase 2: **6 de 7 tareas técnicas/documentales completadas y verificadas**; F2-07: usuario informa 7/8 comprobaciones humanas; X/ESC pendiente de confirmación tras el ajuste.
- Evidencia de fase 2: 34/34 aprobadas, cero fallos/omisiones y salida 0; cinco capturas revisadas, incluidas dos resoluciones.
- Fase 3, A/B/C1/D01/E01: **7 de 8 tareas completadas y verificadas** (A/B/C1/D/E/F/G), **42/42 pruebas** y dos recorridos de la nueva build con salida 0. C2 pendiente; no es el cierre de toda la fase 3.
- Build Windows de fase 2: compilada, recorridos independientes sin gráficos y con GPU aprobados, salida 0 y sin aviso de agente inválido/excepciones; ZIP portable verificado.
- Reglas, clases y hoja de ruta fueron aprobadas por el usuario como prototipo. El juego comercial, continente, narrativa y arte finales no se declaran terminados.
- Pendiente: evaluación humana de controles/diversión/balance (F2-07). Expansión compleja y siguiente iteración de arte/UX sujetas a propuesta.
- Entrega previa D01: COMO_JUGAR.md, Docs/D01_ESCENARIO_MEDIEVAL.md, medidas y dirección artística; once prefabs, ocho materiales, ocho capturas reales y ZIP Windows D01. Informes y entregas anteriores conservados.

- Entrega previa E01: Docs/E01_PERSONAJES.md (tres fichas, importación y resultados), cinco prefabs, catálogo/ocho materiales compartidos, siete capturas y ZIP Windows E01. F2-07 sigue pendiente.

- E02: **infraestructura F3-E02 completada y verificada**, 46/46 pruebas y build/recorridos sin gráficos y con GPU aprobados. F3-E02-M (modelo/rig real) y F3-E02-A (clips/retargeting) pendientes. Guía/manifiesto: Docs/E02_IMPORTACION_HUMANOID.md. La alternativa E01 continúa activa.

- E02-A artístico: ficha/hoja conceptual entregadas y revisadas en Documentacion/Personajes/MONJE_DISENO_BASE.md; diseño **PROPUESTO, sin aprobación artística**. F3-E02-ARTE-01 cerrada como documentación; F3-E02-ARTE-AP pendiente. No modelo/rig/animaciones nuevos ni cambios al juego.

- E02-B posterior: **F3-E02-B completada y verificada**, nuevo proxy estático Monje 1,78 m y tarjeta de clase; alternativa original E01 conservada. **49/49 pruebas aprobadas**, incluidos tres casos de aceptación, build Windows y recorridos con/sin GPU aprobados. Siete capturas reales revisadas. Informe Docs/E02B_MONJE_PROVISIONAL.md. F3-E02-ARTE-AP, F3-E02-M, F3-E02-A de clips, F2-07 y C02 siguen pendientes. Los apartados anteriores describen sus entregas históricas; el permiso provisional E02-B no es aprobación artística final.

- C02 nuevo brief: **F3-C02.1..6 completadas y verificadas**, HUD compacto, ficha compartida, inventario seleccionable, registro de hechos y contextos por modalidad. **52/52 pruebas**, 44 layouts/capturas a cuatro resoluciones, build y recorridos con/sin GPU PASS; trece capturas representativas. Informe Docs/C02_INTERFAZ_MEDIEVAL.md. F3-C02 histórico se conserva; minimapa, arte final y F2-07 siguen pendientes.

Estados permitidos: `[ ] Pendiente`, `[ ] En desarrollo`, `[ ] Bloqueado`, `[x] Completado y verificado`.
Prioridades: P0 imprescindible para la siguiente fase; P1 prototipo vertical; P2 experimento/expansión.
Rutas relativas a esta carpeta; los assets pertenecen a `ProyectoRPGRTS/`. «Previstos» no significa que los archivos existan.
La aceptación de una tarea de código exige compilación y prueba funcional; conservar resultado, fecha y evidencia. En macrohitos, descomponer antes de programar.

## Preparación

### PRE-01 — Localizar y analizar el GDD
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Diseño / P0. Dependencias: ninguna.
- Descripción: leer el Word completo, distinguir definición conceptual, propuestas y pendientes.
- Aceptación: todas las secciones revisadas; fuente original preservada; decisiones abiertas visibles en el plan.
- Archivos: leído `ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx`; análisis en `PLAN_DESARROLLO.md`; copia consultable creada en `Docs/GDD_LECTURA.md`.
- Pruebas: lectura XML de cuerpo, tablas, notas y pie; revisión de propuestas y estados en sección 12.
- Validación: lectura íntegra realizada; 306 párrafos de cuerpo, sin imágenes ni revisiones controladas; notas/pie sin texto sustantivo. Unity no aplica al análisis.

### PRE-02 — Inventariar proyecto y herramientas
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Entorno / P0. Dependencias: PRE-01.
- Descripción: revisar carpetas, escenas, scripts, paquetes, editor e integración IDE.
- Aceptación: inventario con versiones y limitaciones, sin confundir paquete instalado con funcionalidad implementada.
- Archivos: leídos Assets, manifest/lock, ProjectSettings, `.vsconfig` y log histórico; resultados en `PLAN_DESARROLLO.md`.
- Pruebas: listado de assets, referencias YAML, manifest/lock y detección vswhere.
- Validación: 1 escena, 0 scripts propios, 0 prefabs; Unity 6000.5.8f1 instalado; herramientas de Unity para Visual Studio presentes. Compilación actual pendiente en BAS-01.

### PRE-03 — Documentación permanente y búsquedas acotadas
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Flujo de trabajo / P0. Dependencias: PRE-01, PRE-02.
- Descripción: crear instrucciones, plan, checklist, registro, copia de lectura y filtros de búsqueda/versionado.
- Aceptación: documentos coherentes, enlaces locales válidos, tareas con todos sus campos y GDD original intacto.
- Archivos: `AGENTS.md`, `CHECKLIST.md`, `PLAN_DESARROLLO.md`, `REGISTRO_CAMBIOS.md`, `Docs/GDD_LECTURA.md`, `.gitignore`, `.rgignore`.
- Pruebas: comprobar secciones/estados/dependencias, comparar texto del GDD y hashes de los 60 archivos originales inventariados.
- Validación: documentos UTF-8 y 26 tareas con campos/estados/dependencias válidos; cobertura de 303 párrafos con texto y dos tablas del GDD; 60 hashes originales idénticos. Los filtros excluyen cachés y generados. Unity no aplica a estos cambios documentales.

### PRE-04 — Crear historial Git inicial
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Control de versiones / P0. Dependencias: PRE-03.
- Descripción: iniciar Git local en esta carpeta, incluir fuentes y `.meta`, excluir cachés y guardar el estado inicial.
- Aceptación: commit local verificable, sin Library/Logs/generados ni publicación remota.
- Archivos: metadatos Git locales; `.gitignore`; fuentes existentes y documentos registrados.
- Pruebas: git status, check-ignore, listado de archivos registrados y log.
- Validación: commit inicial `7e009d8` comprobado, 64 archivos registrados, cachés/generados excluidos, estado limpio tras el commit y ningún remoto configurado. Identidad existente sin cambios globales. Se conserva el espaciado original de los YAML; los documentos nuevos pasan la revisión de whitespace.

### PRE-05 — Aprobar plan y primer hito
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Dirección / P0. Dependencias: PRE-03, PRE-04.
- Descripción: revisión del diagnóstico, secuencia y alcance de la base jugable por el usuario.
- Aceptación: aprobación expresa registrada antes de empezar la implementación.
- Archivos: `CHECKLIST.md`, `PLAN_DESARROLLO.md` y `REGISTRO_CAMBIOS.md` actualizados con la decisión.
- Pruebas: verificar autorización en la conversación.
- Validación: el usuario indicó «continua con el prompt» después de recibir el diagnóstico y el plan; se registra como aprobación para avanzar incrementalmente con la base técnica, sin ampliar sistemas definitivos pendientes.

## Base jugable

### BAS-01 — Validar entorno Unity
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Entorno / P0. Dependencias: PRE-05.
- Descripción: abrir el proyecto exacto, comprobar paquetes, Console, compilación y Play Mode de la escena existente.
- Aceptación: importación completa, sin errores impeditivos ni referencias perdidas; entrada/salida de Play Mode comprobada.
- Archivos: `Tools/Invoke-UnityValidation.ps1`, `ProyectoRPGRTS/Assets/03_Scripts/Tests/Editor/EnvironmentValidationTests.cs` y su asmdef; evidencia en `ProyectoRPGRTS/Logs/Baseline-results.xml` y log (locales, excluidos de Git).
- Pruebas: Unity 6000.5.8f1, Package Manager, Console, SampleScene y depurador si se necesita.
- Validación: Unity 6000.5.8f1 compiló y pasó 3/3 pruebas en batch: editor/paquetes exactos, escena sin scripts/referencias perdidos y entrada/salida de Play Mode. Código de salida 0. No se usó la ventana. Se corrigieron referencias duplicadas del asmdef y se adoptó EntityId, requerido por Unity 6.5.

### BAS-02 — Fijar referencias de escala
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Escala 3D / P0. Dependencias: BAS-01.
- Descripción: contrastar las medidas propuestas en el plan y preparar referencias reutilizables.
- Aceptación: 1 unidad = 1 m; humano, puerta, ventana, calle y edificio coherentes; colliders y paso documentados.
- Archivos: `Docs/ESCALA_REFERENCIA.md`, generador `ProyectoRPGRTS/Assets/03_Scripts/Editor/TechnicalSandboxBuilder.cs`, pruebas `FoundationValidationTests.cs`, seis prefabs en `02_Prefabs/ScaleReferences/`, materiales URP en `Assets/Settings/Development/` y sus `.meta`.
- Pruebas: inspección de transform/importación, comparación con cubo de 1 m y pruebas de huecos.
- Validación: dimensiones de los seis prefabs importados comprobadas; raíces a escala 1; collider humano de 1,80 m y diámetro 0,60 m, apoyado en Y=0; pasa por puerta y entrada de casa, pero queda bloqueado en hueco de 0,40 m. Referencias de ensayo documentadas; sin definir tamaños definitivos del GDD. Evidencia: `Docs/VALIDACION_BASE_TECNICA.md`.

### BAS-03 — Crear escena de pruebas independiente
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Escenarios / P0. Dependencias: BAS-02.
- Descripción: suelo, obstáculo y paso estrecho para ensayos; conservar SampleScene.
- Aceptación: escena abre con URP correcto, escala coherente, colisiones y ausencia de referencias perdidas.
- Archivos: `ProyectoRPGRTS/Assets/01_Scenes/TechnicalSandbox.unity`, sus prefabs/materiales y `.meta`, `ProjectSettings/SceneTemplateSettings.json` generado por Unity, informe `Docs/VALIDACION_BASE_TECNICA.md` y captura `Docs/Images/TechnicalSandbox.png`.
- Pruebas: apertura, guardado, Play Mode y revisión visual del entorno.
- Validación: escena creada/guardada por Unity; carga, Play Mode y salida comprobados, materiales URP soportados, suelo Y=0 y ausencia de referencias/scripts perdidos. Suite completa 12/12, cero fallos/omisiones, salida 0; captura GPU 1280×720 revisada visualmente sin materiales rosas ni recortes. SampleScene, GDD y paquetes conservados. Cámara fija de inspección; controles y navegación corresponden a tareas posteriores.

### BAS-04 — Cámara isométrica
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Cámara/entrada / P0. Dependencias: BAS-03.
- Descripción: encuadre isométrico, desplazamiento y zoom con límites configurables.
- Aceptación: lectura clara de referencias, controles estables, zoom limitado y UI sin interferencias.
- Archivos: rutas bajo `ProyectoRPGRTS/Assets/03_Scripts/` o `Assets/` según corresponde: `Runtime/Camera/IsometricCameraRig.cs` y `02_Prefabs/Camera/IsometricCamera.prefab`.
- Pruebas: compilar, Play Mode con ratón/teclado, extremos de zoom y repetición de entrada/salida.
- Validación: Cámara ortográfica 35,264°/45°, pan y zoom 7–25 comprobados en Play Mode; límites y bloqueo sobre UI verificados.

### BAS-05 — Personaje provisional reutilizable
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Personajes / P0. Dependencias: BAS-03.
- Descripción: cápsula o modelo temporal a escala, sin clase ni atributos RPG inventados.
- Aceptación: prefab con pivote/collider coherentes, visible en cámara y sin referencias perdidas.
- Archivos: rutas bajo `ProyectoRPGRTS/Assets/03_Scripts/` o `Assets/` según corresponde: `02_Prefabs/Characters/PrototypeHero.prefab`, `Runtime/Exploration/ExplorationUnit.cs` y sus .meta.
- Pruebas: inspección a escala, colisiones y Play Mode con BAS-04.
- Validación: Prefab reutilizable a escala, pivote en pies, cápsula 1,80 × 0,60 m, selección visible y presencia en NavMesh comprobados.

### BAS-06 — Selección y movimiento por ratón
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Exploración/navegación / P0. Dependencias: BAS-04, BAS-05.
- Descripción: seleccionar personaje y ordenar destino; probar AI Navigation existente.
- Aceptación: llega a destino accesible, evita obstáculo, no atraviesa puerta incompatible, ignora destinos inválidos y clics UI.
- Archivos: rutas bajo `ProyectoRPGRTS/Assets/03_Scripts/` o `Assets/` según corresponde: `Runtime/Exploration/PlayerCommands.cs`, `Settings/Prototype/PrototypeNavMesh.asset`, `ProjectSettings/NavMeshAreas.asset` y PlayablePrototype.unity.
- Pruebas: compilación, suelo/obstáculo/paso, órdenes repetidas, pérdida de selección y clic fuera del terreno.
- Validación: Rayos de selección por pantalla, órdenes bloqueadas por UI, rechazo de puntos fuera del terreno/interior de obstáculo y llegada por una ruta completa comprobados en Play Mode.

### BAS-07 — Interacción básica
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Interacción / P0. Dependencias: BAS-06.
- Descripción: un objeto provisional seleccionable/interactuable con resultado visible, sin inventario completo.
- Aceptación: interacción dentro de alcance, rechazo controlado fuera de alcance y referencias seguras tras desactivar objeto.
- Archivos: rutas bajo `ProyectoRPGRTS/Assets/03_Scripts/` o `Assets/` según corresponde: `Runtime/Interaction/InteractionPoint.cs`, `Runtime/World/GameSession.cs` y cofre en PlayablePrototype.unity.
- Pruebas: compilar, casos de distancia, clic repetido y desactivación en Play Mode.
- Validación: Interacción por alcance, resultado visible, rechazo de repetición y objeto desactivado comprobados sin errores.

### BAS-08 — UI medieval reutilizable
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Presentación / P0. Dependencias: BAS-07.
- Descripción: panel de selección e interacción, botón base y estilo compartido; validar propuesta uGUI.
- Aceptación: dos usos de componentes compartidos sin duplicar lógica; texto legible, cambios de resolución y clic UI sin ordenar movimiento.
- Archivos: rutas bajo `ProyectoRPGRTS/Assets/03_Scripts/` o `Assets/` según corresponde: `Runtime/UI/ExplorationHud.cs`, `MedievalUiTheme.cs`, `Settings/Prototype/MedievalTheme.asset` y prefabs UI/MedievalPanel, MedievalButton y ExplorationHud.
- Pruebas: compilación, Play Mode en varias resoluciones, navegación y alta/baja de suscripciones.
- Validación: Suite 15/15 aprobada; HUD renderizado y revisado a 1280×720 y 1920×1080, sin duplicar listeners al activar/desactivar; clics UI no ordenan movimiento.

### BAS-09 — Verificar base jugable
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Integración / P0. Dependencias: BAS-08.
- Descripción: cerrar primer hito con recorrido cámara → selección → movimiento → interacción → feedback.
- Aceptación: recorrido reproducible, sin errores de Console ni referencias rotas; build de prueba arranca; evidencia de cada criterio.
- Archivos: BuildSmokeProbe, PlayablePrototypeBuilder, Tools/Invoke-PrototypeSmoke.ps1, build Windows e informe Docs/VALIDACION_PROTOTIPO.md. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación Unity, Play Mode, build local y perfilado básico sin presupuesto inventado.
- Validación: La base pasó 15/15 pruebas y su primera build arrancó sin editor, caminó e interactuó con salida 0. La regresión del prototipo completo pasó 23/23; cierre de build ampliada en VER-10.

## Prototipo vertical del GDD

### VER-01 — Aprobar especificaciones mínimas
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Diseño transversal / P1. Dependencias: BAS-09.
- Descripción: dividir decisiones sobre tres clases, atributos/turnos, objetos, tiempo compartido, guardado, región y límites de los sistemas.
- Aceptación: especificación provisional aprobada, clases y política de tiempo fijadas para el ensayo; hardware de prueba identificado y presupuestos comerciales diferidos explícitamente según la aprobación.
- Archivos: Docs/ESPECIFICACION_PROTOTIPO.md, Settings/Prototype/ApprovedRules.asset y PrototypeRules.cs. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: revisión de consistencia con GDD §§2–11 y registro de decisiones.
- Validación: El usuario eligió Guerrero/Mago/Monje y aprobó las reglas como provisionales. Windows/PC actual y dos resoluciones son el entorno de ensayo; los presupuestos comerciales quedan explícitamente diferidos en la hoja de ruta aprobada.

### VER-02 — Estado compartido y guardado mínimo
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Persistencia / P1. Dependencias: VER-01.
- Descripción: estado mutable, identificadores estables y formato versionado para los datos mínimos aprobados.
- Aceptación: guardar/cargar restaura personaje y un hecho de mundo; datos ausentes o incompatibles tienen comportamiento definido.
- Archivos: Runtime/World/PrototypeState.cs, Runtime/Persistence/PrototypeSaveStore.cs y PrototypeDirector.cs. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, ida y vuelta de datos y reinicio de partida; no mutar configuración ScriptableObject.
- Validación: JSON versionado, guardado temporal/atómico, ida y vuelta de estado integrado y rechazo de corrupción/versiones desconocidas/datos no finitos comprobados sin alterar la partida actual.

### VER-03 — Creación básica y tres clases
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Personajes/progresión / P1. Dependencias: VER-02.
- Descripción: creador acotado y tres clases elegidas expresamente; contemplar catálogo sin implementar las doce.
- Aceptación: crear, cargar y visualizar personaje válido; reglas y clases corresponden a decisiones aprobadas.
- Archivos: PrototypeRules.cs, ApprovedRules.asset, VerticalSliceHud.cs y creación de nombre/clase en PlayablePrototype.unity. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, selección válida/inválida y persistencia de cada clase.
- Validación: Las tres clases aprobadas se seleccionan y aplican salud/velocidad; nombre y clase se guardan y cargan. Son valores ajustables, no un catálogo RPG definitivo.

### VER-04 — Objetos, inventario y equipo mínimos
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Objetos / P1. Dependencias: VER-03.
- Descripción: subconjunto de objetos y reglas aprobado para exploración/combate.
- Aceptación: recoger, equipar y guardar sin duplicaciones ni pérdidas; UI reutiliza componentes.
- Archivos: WorldOperations.cs, PrototypeState.cs, GameSession.cs y panel contextual de inventario/equipo. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, cambios de equipo, límites definidos y guardar/cargar.
- Validación: Capacidad 8, propiedad para equipar, consumo acotado de raciones, cofre de una sola recompensa y persistencia de arma/armadura verificados.

### VER-05 — Encuentro RPG por turnos
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Combate RPG / P1. Dependencias: VER-04.
- Descripción: encuentro pequeño con iniciativa, acción/movimiento, dados y habilidades aprobadas.
- Aceptación: victoria/derrota y fin de turno coherentes; tiempo estratégico sigue la política acordada; datos separan lógica y presentación.
- Archivos: Runtime/Combat/TurnBattle.cs, TurnEncounter.cs y UI de turnos. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, reglas con semillas controladas, Play Mode completo y persistencia según contrato.
- Validación: Combate completo con iniciativa, d20, acción, movimiento por longitud de ruta, alcance, guardia y resultado. Se verifican límites y victoria con semilla; el rival también debe acertar al ganar iniciativa. Calendario estratégico pausado.

### VER-06 — Región mínima y consecuencia persistente
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Mundo/misiones/facciones / P1. Dependencias: VER-05.
- Descripción: integrar asentamiento, mazmorra, dos facciones y una misión aprobada en una región acotada.
- Aceptación: recorrido explorable; decisión modifica un hecho y su efecto permanece al recargar.
- Archivos: PlayablePrototype.unity, InteractionPoint.cs, PrototypeDirector.cs, WorldOperations.cs y estado de reputación. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, recorridos alternativos, transición de escenas y guardar/cargar consecuencias.
- Validación: Asentamiento de ensayo, mazmorra accesible por portal, regreso visible, dos facciones y objetivo provisional. Reputación A +10/B -5 y hecho único persisten al guardar/cargar; sin crear historia definitiva.

### VER-07 — Economía simplificada
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Economía/comercio / P1. Dependencias: VER-06.
- Descripción: producción/consumo y comercio mínimo con reglas aprobadas y conexión a estado de mundo.
- Aceptación: transacciones válidas, recursos conservados y paso del tiempo coherente; sin valores definitivos inventados.
- Archivos: WorldOperations.cs, actualización temporal de GameSession.cs y paneles de recursos/comercio. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, límites, transacciones repetidas, avance temporal y persistencia.
- Validación: Intercambio conserva costes; talleres producen madera consumiendo alimento por ciclos. Recursos insuficientes rechazan operaciones; tiempo pausado en RPG/RTS y recursos persistentes comprobados.

### VER-08 — Construcción básica
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Construcción/propiedad / P1. Dependencias: VER-07.
- Descripción: edificio y terreno acotados, costes aprobados, colocación a escala y vínculo económico.
- Aceptación: respetar propiedad, colisiones, recursos y acceso navegable; edificio persiste.
- Archivos: Runtime/Construction/WorldConstruction.cs, PrototypeWorkshop.prefab y datos de construcciones guardables. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, colocación inválida, coste insuficiente, navegación tras construir y guardar/cargar.
- Validación: Colocación acotada, costes exactos, rechazo de solapamientos sin cobro, restauración de talleres y carving que bloquea su centro verificados. Edificio provisional de 3×3 m.

### VER-09 — Batalla RTS acotada
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Guerra RTS / P1. Dependencias: VER-08.
- Descripción: especificar y crear una batalla con selección de grupos, órdenes y estadísticas simplificadas.
- Aceptación: órdenes y resultado verificables; número de unidades y rendimiento acordados; consecuencias guardadas.
- Archivos: Runtime/Rts/RtsBattleController.cs, prefab de unidad y UI de grupo. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, grupos/obstáculos, victoria/derrota, perfilado con volumen definido y persistencia.
- Validación: Batalla 6×6, resultado persistente y pausa de calendario verificados en editor; la build definitiva ejecutó una orden de grupo y obtuvo victoria con salida 0.

### VER-10 — Validar prototipo vertical completo
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Integración/calidad / P1. Dependencias: VER-09.
- Descripción: verificar elementos del GDD §10 en un recorrido y evaluar viabilidad de expansión.
- Aceptación: región, tres clases, asentamiento, mazmorra, turnos, dos facciones, economía, construcción, RTS y misión persistente comprobados o alcance renegociado explícitamente.
- Archivos: Docs/VALIDACION_PROTOTIPO.md, suite Vertical, build Windows y comprobante final del player. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, build, regresión funcional, guardado, legibilidad y perfilado en hardware acordado.
- Validación: 23/23 pruebas, capturas revisadas, build Windows compilada y recorrido completo del player con salida 0. Paquete portable creado. Incluye todos los elementos del prototipo vertical aprobado, con reglas y arte provisionales; no equivale a producción comercial completa.

## Experimentos y expansión

### EXP-01 — Evaluar transición RPG a RTS
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Experimento de modos / P2. Dependencias: VER-10 y autorización específica del experimento.
- Descripción: probar un conflicto que escala con refuerzos, según GDD §12.3.
- Aceptación: protocolo de tiempo/estado definido, efecto RPG sobre RTS legible y conclusión conservar/modificar/descartar.
- Archivos: TurnEncounter.cs, RtsBattleController.cs, PrototypeDirector.cs y capturas/informe de transición. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: compilación, transición y casos de cancelación/fallo; evaluación de jugabilidad.
- Validación: Escalada optativa después de victoria RPG comprobada: 14 de salud aliada frente a 10 base, seis contra seis y calendario pausado. Conclusión: conservar como experimento opt-in; diversión y equilibrio pendientes de evaluación humana en expansión, sin imponerlo a todos los encuentros.

### FUT-01 — Descomponer expansión posterior
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Producción futura / P2. Dependencias: VER-10.
- Descripción: priorizar compañeros, origen, clases restantes, magia, fabricación, diplomacia/gobierno, simulación regional, mapa continental, caravanas, asedios, narrativa y arte/audio definitivos.
- Aceptación: hoja de ruta aprobada con prioridades/dependencias y definición explícita de las decisiones presupuestarias pendientes antes de producción; propuestas provisionales conservan su estado.
- Archivos: Docs/PLAN_EXPANSION.md. Rutas de código bajo ProyectoRPGRTS/Assets/03_Scripts/ y assets bajo Assets/.
- Pruebas: revisión del GDD completo y resultados del prototipo; pruebas técnicas se definirán por subtarea.
- Validación: El usuario aprobó la hoja de ruta con prioridades/dependencias. Su alcance aplaza presupuesto/calendario comercial hasta medir y aprobar cada fase; no autoriza iniciar automáticamente producción ni fija propuestas del GDD como definitivas.

## Historial de seguimiento

- 2026-10-28: inspección inicial y lectura completa del GDD; PRE-01/PRE-02 verificadas; documentación en elaboración. No se implementaron mecánicas.
- 2026-10-28: PRE-03 verificada con cobertura textual completa, 60 hashes originales intactos y 26 tareas consistentes; Git local inicializado y registro en curso.
- 2026-10-28: PRE-04 verificada mediante commit base `7e009d8`, índice de 64 archivos y estado limpio; preparación terminada. PRE-05 pendiente de aprobación; Unity y mecánicas sin ejecutar.
- 2026-10-28: plan aprobado por el usuario; BAS-01 completada con 3/3 pruebas en segundo plano; BAS-02/BAS-03 en preparación y validación incremental.
- 2026-10-28: BAS-02 y BAS-03 verificadas con suite final 12/12 y render GPU revisado; 8/26 tareas completas. Siguiente: BAS-04. No se requiere una nueva aprobación del plan para continuar la base autorizada.

- 2026-10-29: BAS-04–BAS-08 verificadas con 15/15 pruebas. Primera build en curso. El usuario aprobó las tres clases y las reglas provisionales del prototipo vertical.

- 2026-10-29: especificación y hoja de ruta aprobadas. Suite final 23/23; base jugable y módulos verticales verificados. VER-09/VER-10 pendientes únicamente del cierre del player. FUT-01 se acota expresamente al plan aprobado, con presupuesto/calendario comercial por definir antes de producción.

- 2026-10-29: cierre final 26/26 dentro del alcance aprobado. La build independiente verificó también órdenes de grupo y victoria RTS, guardado final y cuatro hechos persistentes. Paquete portable y guía de uso entregados; la expansión comercial continúa únicamente como hoja de ruta aprobada.

## FASE 2 — VALIDACIÓN E INTEGRACIÓN

Las 26 tareas anteriores son historial de la primera etapa y conservan sus resultados. Las siguientes pertenecen exclusivamente a la solicitud del 2026-10-30.

### F2-01 — Auditoría de la base existente
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Dirección y QA / P0. Dependencias: VER-10, FUT-01.
- Descripción: releer GDD, inspeccionar los doce sistemas, código, documentación y evidencia; repetir la suite base.
- Aceptación: hash original intacto; estado Git conocido; matriz distingue automatización, inspección y comprobación humana pendiente.
- Archivos: CHECKLIST.md, Docs/AUDITORIA_FASE_2.md, PLAN_DESARROLLO.md, REGISTRO_CAMBIOS.md.
- Pruebas: hash, Git y Unity Vertical con 23 casos previos.
- Resultado: fuentes/código revisados, hash y Git inicial comprobados. Base 23/23 Passed; cierre nativo -1073741819 registrado sin ocultarlo. Reproducción ampliada 23/32 con fallos esperados y un fallo del test identificado; evidencia y límites en AUDITORIA_FASE_2.md. La validación final con salida correcta corresponde a F2-05/F2-06.

### F2-02 — Cierre consistente de encuentros y navegación del héroe
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Integración RPG/RTS/mundo / P0. Dependencias: F2-01.
- Descripción: detener órdenes pendientes al entrar en gestión/RTS y retirar participantes transitorios al salir de sus encuentros o cargar.
- Aceptación: batalla finalizada detiene supervivientes; salir/cargar no deja soldados ni estado de combate anterior; victoria persistente intacta; encuentros activos no permiten cambiar/guardar/cargar; despliegue bloqueado se rechaza sin iniciar una batalla atascada.
- Archivos: Runtime/Exploration/ExplorationUnit.cs, Runtime/World/PrototypeDirector.cs, Runtime/Combat/TurnEncounter.cs, Runtime/Rts/RtsBattleController.cs y Tests/Editor/PhaseTwoValidationTests.cs, bajo ProyectoRPGRTS/Assets/03_Scripts/.
- Pruebas: reproducción antes de corregir; Play Mode, regresión vertical y smoke de build.
- Resultado: reproducción dirigida RTS 0/2 antes de corregir; aceptación posterior 3/3 con salida 0, evidencia Phase2-lifecycle-results.xml/log. Conserva victoria guardable y rechaza despliegue ocupado sin modificar edificios/modo. Regresión global/build en F2-05/F2-06.

### F2-03 — Obstáculos y movimiento táctico
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Combate RPG y navegación / P0. Dependencias: F2-02.
- Descripción: validar ruta y presupuesto del desplazamiento enemigo; impedir ataques a través de geometría y finalizar turno con el héroe aún en tránsito.
- Aceptación: recorrido enemigo máximo 3 m por turno, sin cruzar paredes; ataque bloqueado no consume acción; fin de turno en tránsito no repone movimiento; combate abierto conserva resultados.
- Archivos: Runtime/Exploration/ExplorationUnit.cs, Runtime/Combat/TurnBattle.cs y TurnEncounter.cs; Tests/Editor/PhaseTwoValidationTests.cs, bajo ProyectoRPGRTS/Assets/03_Scripts/.
- Pruebas: barrera controlada, recorrido con obstáculo, acción/movimiento y regresión RPG/RTS.
- Resultado: 8/8 pruebas dirigidas aprobadas, salida 0 (Phase2-tactical-results.xml/log); incluye dos casos de geometría/tránsito, cinco de reglas y victoria RPG/escalada/RTS. El rival avanza por ruta y no salta la barrera; pendiente valoración humana de ritmo/presentación.

### F2-04 — Persistencia e información de errores
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Guardado/UI/interacción / P1. Dependencias: F2-03.
- Descripción: reforzar rechazo seguro de datos y comunicar fallos/estado real sin ocultar mensajes útiles.
- Aceptación: carga inválida mantiene estado y posición; no desborda cantidades; reloj y ciclo no duplican producción; UI conserva motivo de interacción fallida y muestra equipo; interacciones fuera de exploración no mutan estado.
- Archivos: PrototypeSaveStore.cs, PrototypeDirector.cs, GameSession.cs, WorldOperations.cs, ExplorationHud.cs y VerticalSliceHud.cs en sus carpetas Runtime; Tests/Editor/PhaseTwoValidationTests.cs, bajo ProyectoRPGRTS/Assets/03_Scripts/.
- Pruebas: datos corruptos/extremos, carga válida previa compatible, mensajes y barreras de modo.
- Resultado: 8/8 dirigidas aprobadas, salida 0 (Phase2-persistence-ui-results.xml/log); rechazo de corrupción/reloj sin mutación, ida/vuelta válida, interacciones por modo/ID, mensajes de alcance y equipo en panel; render en dos resoluciones y bloqueo UI comprobados automáticamente. Revisión visual en F2-05.

### F2-05 — Integración, escala y evaluación visual
- Estado: [x] Completado y verificado.
- Sistema / prioridad: QA/arquitectura/arte / P1. Dependencias: F2-04.
- Descripción: verificar consecuencias, pausa, equipo, construcción, producción y guardado; revisar proporciones, URP y capturas; evaluar transiciones ocasionales sin ampliarlas.
- Aceptación: matriz de los doce sistemas con evidencia y límites; escenas conservadas, escala métrica; propuestas mayores separadas; capturas revisadas en dos resoluciones.
- Archivos: Docs/AUDITORIA_FASE_2.md, Docs/Images/Fase2*, pruebas de fase 2, CHECKLIST.md.
- Pruebas: suite ampliada, inspección arquitectónica/geométrica, render GPU batch.
- Resultado: regresión final 34/34, cero fallos/omisiones y salida 0 (Phase2-final-results.xml/log), incluido el ajuste de arranque. Cinco capturas revisadas; las tres que cambiaron tras repetir fueron inspeccionadas otra vez. Matriz de doce sistemas, escala/URP/referencias y propuestas verificadas dentro del alcance automático/visual. Sin sesión humana interactiva. Build/player corregidos se aceptan en F2-06.

### F2-06 — Build y entrega reproducible
- Estado: [x] Completado y verificado.
- Sistema / prioridad: Entrega / P1. Dependencias: F2-05.
- Descripción: compilar y ejecutar el recorrido independiente; empaquetar e informar el estado real con instrucciones Unity y commit local.
- Aceptación: suite aprobada, nueva build y smoke salida 0 sin aviso de agente inválido/excepciones; archivo portable íntegro, documentos coherentes y Git revisado; pendientes humanos explícitos.
- Archivos: COMO_JUGAR.md, PLAN_DESARROLLO.md, REGISTRO_CAMBIOS.md, CHECKLIST.md, informe/inventario de entrega; Tools/Invoke-UnityValidation.ps1 e Invoke-PrototypeSmoke.ps1; BuildSmokeProbe.cs, VerticalSliceBuilder.cs y override de arranque en PlayablePrototype.unity. Builds/Logs locales excluidos de Git.
- Pruebas: Build, smoke, integridad/hash ZIP, diff, metadatos/GUID y estado Git.
- Resultado: build de 170.919.383 bytes, 53,99 s, salida 0; smoke sin gráficos y con GPU aprobado, sin aviso NavMesh/excepciones. ZIP 69.566.729 bytes, 305 entradas, CRC correcto, SHA-256 en informe. Diff/metadatos y documentación revisados; commit local de entrega, sin publicación remota. F2-07 sigue pendiente.

### F2-07 — Evaluación manual con jugadores
- Estado: [ ] Pendiente.
- Sistema / prioridad: Experiencia jugable / P1. Dependencias: F2-06.
- Descripción: probar ratón/teclado reales y valorar comprensión, ritmo, accesibilidad, balance y diversión.
- Aceptación: registrar participante, hardware/resolución, recorrido, observaciones/fallos y decisión sobre siguiente iteración y escalada RPG/RTS.
- Archivos: Docs/PRUEBA_MANUAL_FASE_2.md creado como guion; registro real de sesión pendiente.
- Pruebas: sesión humana interactiva de los doce sistemas, sin sustituir por capturas o llamadas a métodos.
- Resultado: no realizada; no se marca como completada.

## FASE 3 — DIRECCIÓN ARTÍSTICA, UI Y VERTICAL SLICE

Iteración actual: bloques A, B y primera mejora C1, con integración/validación propias. D/E y ampliación C2 permanecen secuenciados; no se declara completa toda la fase 3 al entregar esta iteración. F2-07 sigue pendiente.

### F3-A01 — Auditoría visual y punto de control
- Estado: [x] Completado y verificado.
- Objetivo / descripción: contrastar fuentes, evidencia, herramientas y problemas visuales de la base.
- Sistema / prioridad: Dirección/QA / P0. Dependencias: F2-06.
- Aceptación: Git/evidencia conocidos, GDD intacto, captura real revisada y herramientas comprobadas.
- Archivos: Docs/FASE3_ITERACION_1.md, CHECKLIST.md; tag local codex/fase3-base-cc9691d.
- Pruebas: git status/log, hash GDD, XML 34/34, build/smokes de cc9691d, manifest/editor y catálogo de herramientas; inspección de captura.
- Resultado / problemas: base limpia; CLI disponible y ninguna herramienta Unity/MCP expuesta en esta sesión. Ruido de acciones y ayuda estática identificados. No equivale a una evaluación humana.

### F3-B01 — Dirección artística y dimensionamiento
- Estado: [x] Completado y verificado.
- Objetivo / descripción: documentar dirección, materiales, paletas, iluminación, arquitectura, cuatro conceptos de clase y sustitución visual.
- Sistema / prioridad: Arte/arquitectura / P0. Dependencias: F3-A01.
- Aceptación: confirmado/propuesto separados; Pícaro solo conceptual; medidas centralizadas y contrato VisualRoot sin alterar combate.
- Archivos: Documentacion/DIRECCION_ARTISTICA.md, Docs/ESCALA_REFERENCIA.md, PLAN_DESARROLLO.md, REGISTRO_CAMBIOS.md.
- Pruebas: cotejo con GDD §§12.1–12.6, medidas de autoría existente y revisión documental; no aplica compilación a conceptos.
- Resultado / problemas: propuestas reversibles documentadas; no son modelos/materiales finales implementados ni aprobación de arte final.

### F3-C01 — Primera UI contextual reutilizable
- Estado: [x] Completado y verificado.
- Objetivo / descripción: distribuir acciones existentes por secciones de exploración, ocultar nombre fuera de Personaje y cambiar ayuda/acciones por modo.
- Sistema / prioridad: UI/UX / P0. Dependencias: F3-B01.
- Aceptación: Resumen/Personaje/Inventario/Partida accesibles sin duplicar acciones; RPG/construcción/RTS coherentes; estilo compartido y sin reglas nuevas.
- Archivos: Runtime/UI/VerticalSliceHud.cs, ExplorationHud.cs y MedievalUiTheme.cs; Editor/ContextUiBuilder.cs; Tests/Editor/ContextUiValidationTests.cs, PlayableValidationTests.cs y SceneCapture.cs (bajo Assets/03_Scripts); PlayablePrototype.unity, MedievalTheme.asset y Tools/Invoke-UnityValidation.ps1.
- Pruebas: invocación programática de botones uGUI en Play Mode, modo/página, suscripciones, entrada y layout 720p/1080p; compilación exacta. No sustituye ratón/teclado humanos.
- Resultado / problemas: 35/35 en suite final, salida 0; catorce layouts comprobados y ocho capturas representativas inspeccionadas. Se corrigió Interactuar visible fuera de exploración. Nueve acciones simultáneas pasan a 0/3/4/2 por sección; mapa/diario/retratos en C02. Fuente provisional, evaluación humana pendiente.

### F3-C02 — Ampliación de información contextual
- Estado: [ ] Pendiente.
- Objetivo / descripción: preparar retrato del único héroe, mapa de región técnica y diario de hechos existentes; habilidades/categorías/formaciones solo con soporte real.
- Sistema / prioridad: UI/UX / P1. Dependencias: F3-C01, F3-E01.
- Aceptación: contenido procedente del estado real, sin compañeros o mecánicas simuladas por botones vacíos; criterios del siguiente bloque concretados antes de editar.
- Archivos: previstos presentadores/prefabs de mapa, diario y retrato; plan y pruebas específicas.
- Pruebas: pendientes de descomponer antes de implementar.
- Resultado / problemas: pendiente; no incluido en la primera entrega C1 solicitada.

### F3-D01 — Muestra de escenario medieval
- Estado: [x] Completado y verificado.
- Objetivo / descripción: mejorar suelo/camino, fachada y decorados reutilizables en una zona pequeña de la escena existente.
- Sistema / prioridad: Entorno/arte / P1. Dependencias: F3-B01, F3-C01.
- Aceptación: lenguaje material coherente, iluminación legible, huellas/escala conservadas, acceso a interacción y RTS; recuentos de geometría y revisión de render, sin afirmar FPS ni optimizar sin perfilado.
- Archivos: PlayablePrototype.unity; once prefabs en Assets/02_Prefabs/Environment/Medieval; materiales/mapas/mallas en Assets/06_Environment/Medieval; autoría Editor/Environment; MedievalEnvironmentTests.cs, IsometricCameraRig.cs y lanzador; informe Docs/D01_ESCENARIO_MEDIEVAL.md, medidas/dirección artística, ocho capturas Docs/Images/D01-*.
- Pruebas: biblioteca métrica y materiales URP; firmas físicas y hash NavMesh idénticos; recorrido de puerta/cofre/obstáculo; cámara quieta; regresión 39/39, salida 0 y ocho capturas comparativas revisadas. Build Windows 175.686.522 bytes, salida 0; smoke sin gráficos y con GPU aprobados (9,02 s / 7,16 s), sin excepciones ni agentes inválidos.
- Resultado / problemas: once prefabs, ocho materiales y dieciséis mapas propios, sin descargas. Corregido zoom involuntario con rueda cero tras reproducción fallida; juntas/camino refinados. Checkpoint codex/d01-base-8197b5f; cero bloques originales de escena eliminados. Biblioteca visual sin física propia, techo opaco sin corte contextual; límites documentados. E/C2 y sesión humana F2-07 permanecen pendientes.

### F3-E01 — Apariencias provisionales de tres clases
- Estado: [x] Completado y verificado.
- Objetivo / descripción: siluetas proporcionadas de Guerrero/Mago/Monje sustituibles por hijo visual; Pícaro conserva concepto documental.
- Sistema / prioridad: Personajes/presentación / P1. Dependencias: F3-B01, F3-D01.
- Aceptación: identidad por forma/material, lógica/collider/agente intactos, configuración compartida y reemplazo de malla sin editar daño/turnos.
- Archivos: cinco prefabs Characters/Visuals; Assets/07_Characters/Provisional (catálogo, ocho materiales y veintiuna mallas); Runtime/Presentation (Catalog/Presenter/Model/RoofCutaway), CharacterVisualBuilder/Tests, escena y lanzador; Docs/E01_PERSONAJES.md, siete capturas, medidas/dirección/arquitectura/guías.
- Pruebas: tres casos dirigidos aprobados; regresión final 42/42, cero fallos/omisiones, salida 0; dimensiones y fallback/reversibilidad, equipo real, clase/carga, paso/selección/colliders/combate y sistemas anteriores. Build 178.001.257 bytes/45,46 s, salida 0; player sin gráficos 7,50 s y con GPU 7,33 s, ambos PASS/salida 0.
- Resultado / problemas: tres figuras métricas genéricas y reemplazables, sin reglas/guardado/controlador nuevos. Oclusión D01 reproducida y recorte de cubierta reversible; física/NavMesh y biblioteca D01 preservados. Siete capturas reales revisadas; fichas/importación documentadas. Checkpoint codex/e01-base-ca5ba35. No son modelos/animaciones finales; paredes/interiores no resueltos de forma general. F2-07 y C02 siguen pendientes.

### F3-F01 — Integración de la iteración UI C1
- Estado: [x] Completado y verificado.
- Objetivo / descripción: comprobar compatibilidad de C1 con movimiento, interacción, clase, guardado, construcción y encuentros.
- Sistema / prioridad: Integración / P0. Dependencias: F3-C01.
- Aceptación: suite anterior conservada/adaptada al acceso contextual y nuevas pruebas funcionales; ausencia de referencias perdidas y estado guardado compatible.
- Archivos: pruebas de editor/Play Mode y Docs/FASE3_ITERACION_1.md; evidencia Phase3-final-results.xml/log (local, excluida de Git).
- Pruebas: suite completa con Unity 6000.5.8f1; snapshot de layout en cuatro modos y dos resoluciones.
- Resultado / problemas: repetición final 35/35, cero fallos/omisiones y salida 0 (8,33 s de casos, sin arranque/importación). Conserva 34 casos anteriores, con acceso a Personaje adaptado en la prueba de nombre. Mundo/reglas/guardado sin cambios; D/E requerirán integración propia.

### F3-G01 — Validación y entrega de esta iteración
- Estado: [x] Completado y verificado.
- Objetivo / descripción: revisar capturas reales, ejecutar build y recorrido independiente, registrar commits y guía.
- Sistema / prioridad: QA/entrega / P0. Dependencias: F3-F01.
- Aceptación: resultados nuevos con salida 0, imágenes inspeccionadas, build/ZIP verificables y documentación con pendientes reales.
- Archivos: ocho Docs/Images/Fase3*, informe, COMO_JUGAR.md, AGENTS.md, inventario/arquitectura, CHECKLIST.md, REGISTRO_CAMBIOS.md; Builds/Logs excluidos de Git.
- Pruebas: render, Build, smoke, metadatos/GUID, diff/Git e integridad del paquete.
- Resultado / problemas: 35/35, build Windows de 170.931.692 bytes, salida 0; player sin gráficos y con GPU aprobados sin fallos NavMesh/excepciones. ZIP 69.570.489 bytes/305 entradas, CRC correcto y SHA-256 en informe. Capturas antes/después y commits locales revisados. F2-07, D/E/C2 siguen pendientes; no se afirma arte final ni evaluación humana.


### F3-E02 — Infraestructura Humanoid y piloto Monje
- Estado: [x] Completado y verificado.
- Descripción / sistema: importación nativa FBX, candidato Monje sin malla inventada, Animator preparado y equipo visual configurable. Personajes/presentación.
- Prioridad / dependencias: P1; F3-E01 verificada, controlador y guardado existentes.
- Aceptación: flujo nativo configurable, validación de rig/escala/URP, fallback E01 intacto, siete estados sin clips falsos, equipo desde estado existente y regresión/build aprobadas. No equivale a integrar un modelo ausente.
- Archivos: Runtime/Presentation (Rig/Profile/Driver nuevos, Catalog/Model/Presenter ampliados); Editor/Characters y assembly propia; HumanoidPipelineTests y referencia; plantilla Monk_ImportTemplate, MonkAnimations.asset y HumanoidBase.controller; catálogo, lanzador, guía E02 y seguimiento.
- Pruebas: 4/4 dirigidas y 46/46 finales, cero fallos/omisiones, salida 0; FBX nativo de 1 m, flags Human sin afirmar rig real, rechazo de candidato/clip inválido, alternativa E01, lectura de movimiento, cinco tipos de bindings y round-trip v1. Build 178.013.219 bytes/64,97 s; player sin gráficos 7,40 s y con GPU 7,38 s, PASS/salida 0.
- Resultado / problemas: infraestructura preparada sin modelo/Avatar/clips reales; plantilla vacía y Animator apagado, grafo con siete motions nulas. Modelo provisional preservado; ventanas/métodos nativos preparados, pero creación/pose/retargeting con recurso real pendientes en E02-M/A. Ensayo ASCII corregido antes de cierre; fixture eliminado. No clips falsos, ítems/ranuras persistentes nuevas ni paquetes. Checkpoint codex/e02-base-2d95959; F2-07 sigue pendiente.

### F3-E02-M — Recurso real y rig del Monje
- Estado: [ ] Pendiente.
- Descripción / sistema: recibir/crear y aprobar FBX humanoide, texturas y equipo compatibles. Personajes/arte.
- Prioridad / dependencias: P1; F3-E02 y recurso con licencia/procedencia comprobada.
- Aceptación: modelo real aprobado, orientación/escala/pesos/avatar/URP verificados en editor y prefab candidato; sin sustituirlo por geometría de ensayo.
- Archivos: pendientes; manifiesto exacto en guía E02.
- Pruebas: pendientes con recurso real; la prueba de importación técnica no las completa.
- Resultado / problemas: falta el modelo; no iniciar diseño definitivo automáticamente.

### F3-E02-A — Clips reales y retargeting del Monje
- Estado: [ ] Pendiente.
- Descripción / sistema: validar Idle/Walk/Run/Attack/Defend/Hit/Defeat con el rig del piloto. Animación.
- Prioridad / dependencias: P1; F3-E02-M y clips compatibles.
- Aceptación: clips reales retargeteados y revisados, transiciones/pies/manos/sockets sin deteriorar navegación o combate; registrar limitaciones.
- Archivos: pendientes; estados/perfil preparados por E02 no son animaciones terminadas.
- Pruebas: pendientes con clips; no se declara animación funcional sin ejecución real.
- Resultado / problemas: faltan siete clips; F2-07 permanece independiente y pendiente.


### F3-E02-ARTE-01 — E02-A artístico: diseño propuesto del Monje
- Estado: [x] Completado y verificado.
- Descripción / sistema: ficha artística y técnica basada en dos referencias del usuario, especificación de hoja de personaje y propuesta visual 2D. Personajes/arte.
- Prioridad / dependencias: P1; infraestructura E02 y nuevo brief artístico E02-A.
- Aceptación: doce apartados de diseño, visible/inferido distinguido, medidas propuestas de 1,78 m, modularidad/rig/importación compatibles con E02 y estado PROPUESTO sin modificar gameplay/modelo actual.
- Archivos: Documentacion/Personajes/MONJE_DISENO_BASE.md; Referencias/MONJE_REFERENCIAS_USUARIO.png; Conceptos/MONJE_HOJA_PROPUESTA_v01.png y prompt exacto. Enlaces en dirección artística, escala, guía E02, plan/inventario; registro actualizado.
- Pruebas: doce secciones y tabla visible/brief/interpretado revisadas; lámina raster 1536x1024 con seis vistas/poses, detalles y paleta inspeccionada; enlaces/hashes y diff comprobados. Proyecto Unity sin cambios: no se repiten pruebas/build; 46/46 E02 es evidencia histórica, no una ejecución nueva.
- Resultado / problemas: entrega documental/2D completa, diseño PROPUESTO y aprobación pendiente. Talla artística 1,78 m sin aplicar al controlador de 1,80; dorso/cierres/rostro propuestos. Imagen generada no equivale a plano calibrado, FBX, UV/rig/texturas PBR o animación. F3-E02-A técnico continúa pendiente; no se modela ni integra el personaje.

### F3-E02-ARTE-AP — Aprobación artística de la propuesta del Monje
- Estado: [ ] Pendiente.
- Descripción / sistema: revisión del usuario de proporciones, vestido, paleta, variantes y zonas interpretadas. Arte/dirección.
- Prioridad / dependencias: P1; F3-E02-ARTE-01 entregada.
- Aceptación: aprobación explícita del usuario o correcciones registradas; no equivale a aprobación del rig/animaciones ni a F2-07.
- Archivos: ficha base y decisiones.
- Pruebas: revisión artística por el usuario pendiente.
- Resultado / problemas: no iniciar modelado definitivo, reemplazo del Monje ni animaciones finales.


### F3-E02-B — Identidad visual provisional del Monje
- Estado: [x] Completado y verificado.
- Descripción / sistema: prefab ligero de 1,78 m basado en concepto, materiales compartidos y referencia 2D en panel Personaje. Presentación/personajes/UI.
- Prioridad / dependencias: P1; E01/E02 y propuesta E02-A, uso provisional autorizado por nuevo prompt.
- Aceptación: silueta naranja asimétrica/rapada/pantalón/vendas legibles, física/estadísticas sin cambios, equipo y guardado compatibles, Guerrero/Mago intactos, UI 720p/1080p legible y capturas reales; sin FBX/rig/animaciones definitivos.
- Archivos: prefab Characters/MonkProvisional/Monk_ConceptProxy, dos materiales/seis mallas/ilustración en 07_Characters/MonkProvisional; UI/ClassReferenceCard; MonkIdentityBuilder/MonkProxyGeometry/ClassReferenceCard/MonkIdentityTests; catálogo/HUD/escena y assembly de editor con cambios acotados; talla esperada actualizada en CharacterVisualTests; lanzador, informe Docs/E02B_MONJE_PROVISIONAL.md, siete Docs/Images/E02B-*, guías/arquitectura/escala/plan/registro.
- Pruebas: dirigidas 3/3; regresión 49/49, cero fallos/omisiones, salida 0. Monje 1,78 m, 5.252 triángulos/10 renderers con equipo; URP, controles existentes/selección física, ruta real/cofre/inventario/equipo, guardado/carga, victoria RPG y UI 720p/1080p aprobados; Guerrero/Mago preservados. Siete capturas Play Mode GPU revisadas. Build Windows salida 0, recorridos nativos con/sin GPU PASS/salida 0. Logs/comprobantes E02B-* y ZIP Windows E02B verificados.
- Resultado / problemas: identidad provisional integrada sin cambios de física/NavMesh/reglas/guardado v1. Checkpoint codex/e02-b-base-afa1e7e y alternativa E01/candidato E02 conservados. Anatomía/uniones/manos/paños simples; equipo genérico con posibles intersecciones y postura estática, sin rig/animación. Aprobación artística final, modelo/rig/clips reales y evaluación humana F2-07 siguen pendientes; no se cierra C02 ni se inicia otra fase.


## FASE 3 — C02: rediseño de interfaz medieval

El nuevo brief redefine el alcance concreto de C02 en estas seis subtareas; F3-C02 anterior se conserva como historial. Sin minimapa funcional o sistemas nuevos; registro/retratos muestran datos existentes.

### F3-C02.1 — Auditoría visual
- Estado: [x] Completado y verificado.
- Descripción / sistema: Auditoría visual; UI/UX.
- Prioridad / dependencias: P1; ninguna.
- Aceptación: Identificar referencias y componentes existentes, sin auditoría ajena al UI.
- Archivos: Docs/C02_INTERFAZ_MEDIEVAL.md; referencias privadas en Documentacion/UI/C02.
- Pruebas: lectura de GDD §12.5 con hash intacto, inspección de uGUI/prefabs/scripts y comparación visual de las tres referencias con capturas E02-B.
- Resultado / problemas: auditoría de presentación completada; referencias no son evidencia Unity. Se conservan uGUI/controles y se concreta C02.2..6 sin mecánicas ficticias.

### F3-C02.2 — Sistema visual
- Estado: [x] Completado y verificado.
- Descripción / sistema: Sistema visual; UI/UX.
- Prioridad / dependencias: P1; C02.1.
- Aceptación: Atlas/sprites originales compartidos, texto legible y prefabs reutilizables sin dependencias nuevas.
- Archivos: Assets/08_UI/Medieval/Textures y Sprites; dos prefabs UI/C02; MedievalUiTheme; prompt en Documentacion/UI/C02.
- Pruebas: Atlas original 1254×1254, cuatro sprites sobre una textura; estilos/portraits verificados en Unity y capturas. Fuentes previas/prefabs intactos, sin dependencias nuevas.
- Resultado / problemas: verificado técnicamente. Correcciones de serialización, texto y orden de dibujo documentadas; fuentes/modelos/arte provisionales y evaluación humana F2-07 pendientes. No minimapa o mecánicas nuevas.

### F3-C02.3 — HUD de exploración
- Estado: [x] Completado y verificado.
- Descripción / sistema: HUD de exploración; UI/UX.
- Prioridad / dependencias: P1; C02.2.
- Aceptación: Mayoría de pantalla libre; ventanas cerrables, ayuda opcional, salud/recursos reales e input preservado.
- Archivos: C02HudBuilder, MedievalHudView, VerticalSliceHud, escena PlayablePrototype y Tools/Invoke-UnityValidation.ps1.
- Pruebas: Cerrar/abrir ventanas, ayuda, raycasts y centro del mundo libre; 23,29 % de ocupación de franjas persistentes. Layouts/UI guardados tras recarga y navegación conservada.
- Resultado / problemas: verificado técnicamente. Correcciones de serialización, texto y orden de dibujo documentadas; fuentes/modelos/arte provisionales y evaluación humana F2-07 pendientes. No minimapa o mecánicas nuevas.

### F3-C02.4 — Panel Personaje e inventario
- Estado: [x] Completado y verificado.
- Descripción / sistema: Panel Personaje e inventario; UI/UX.
- Prioridad / dependencias: P1; C02.3.
- Aceptación: Plantilla para tres clases, concepto Monje, datos/equipo reales, selección y acciones del inventario.
- Archivos: InventoryPanelView, ClassReferenceCard, catálogo ClassVisuals y retratos en 08_UI/Medieval/Portraits; escena y ficha común.
- Pruebas: Tres clases con datos/retratos reales, nombre y salud; inventario seleccionado, equipo/ración/comercio y carga aislada; un comando por clic tras reactivar HUD.
- Resultado / problemas: verificado técnicamente. Correcciones de serialización, texto y orden de dibujo documentadas; fuentes/modelos/arte provisionales y evaluación humana F2-07 pendientes. No minimapa o mecánicas nuevas.

### F3-C02.5 — Interfaces contextuales
- Estado: [x] Completado y verificado.
- Descripción / sistema: Interfaces contextuales; UI/UX.
- Prioridad / dependencias: P1; C02.3.
- Aceptación: RPG/construcción/RTS con comandos/datos existentes, estados diferenciados y retorno seguro.
- Archivos: MedievalHudView/VerticalSliceHud, getters de lectura en TurnBattle y RtsBattleController; escena/contextos.
- Pruebas: Cuatro contextos automáticos, iniciativa/acción/guardia RPG, taller/cancelación, selección/órdenes RTS y retorno bloqueado en encuentro; regresión preservada.
- Resultado / problemas: verificado técnicamente. Correcciones de serialización, texto y orden de dibujo documentadas; fuentes/modelos/arte provisionales y evaluación humana F2-07 pendientes. No minimapa o mecánicas nuevas.

### F3-C02.6 — Validación y entrega
- Estado: [x] Completado y verificado.
- Descripción / sistema: Validación y entrega; UI/UX.
- Prioridad / dependencias: P1; C02.4, C02.5.
- Aceptación: Compilar, probar cuatro resoluciones y regresión, capturar, build/recorridos, Git y docs; F2-07 pendiente.
- Archivos: MedievalHudTests; ContextUiValidationTests, MonkIdentityTests y PhaseTwoValidationTests adaptadas; SceneCapture; Docs/C02_INTERFAZ_MEDIEVAL.md, trece Images/C02-*, guías/plan/registro/AGENTS y ZIP C02.
- Pruebas: 52/52, cero fallos/omisiones, salida 0; 44 layouts/raycasts/textos a 720p/900p/1080p/1440p. Build salida 0; recorridos nativos con/sin GPU PASS/salida 0; ZIP CRC/hashes y siete archivos históricos preservados. 278 metadatos/GUID íntegros; historial/GDD/mundo conservados.
- Resultado / problemas: verificado técnicamente. Correcciones de serialización, texto y orden de dibujo documentadas; fuentes/modelos/arte provisionales y evaluación humana F2-07 pendientes. No minimapa o mecánicas nuevas.


## GITHUB — Conexión y conservación del historial

### GH-01 — Publicar repositorio existente y flujo main/dev
- Estado: [x] Completado y verificado.
- Descripción / sistema: conectar raíz Git existente con MathiasMerino/RPGRTS, conservar commits y documentar ramas/PR. Control de versiones.
- Prioridad / dependencias: P1; repositorio vacío creado por el usuario y publicación explícitamente autorizada el 2026-10-31.
- Aceptación: historial previo intacto, revisión de ignore/tamaño/credenciales, main predeterminada y dev de trabajo, ramas remotas sincronizadas y árbol limpio; sin jugabilidad nueva o push forzado.
- Archivos: AGENTS.md, .gitignore, Docs/GITHUB_FLUJO.md, CHECKLIST.md, REGISTRO_CAMBIOS.md y configuración Git local de origin/upstreams; sin editar Unity.
- Pruebas: remoto inicial sin referencias/tamaño 0; 757 blobs históricos, 682 versiones de texto examinadas sin credenciales candidatas, máximo 3.003.405 bytes; exclusiones Unity/secretos correctas. API GitHub confirma main/dev iguales y default main, 615 rutas/blob SHA idénticas en la base publicada; clonación independiente conserva los 14 commits originales y fsck PASS. Verificación final GitHub-* al publicar la documentación.
- Resultado / problemas: base inicial publicada por autorización de este encargo; flujo feature/*, fix/*, test/* → dev → PR aprobada a main. Ramas/checkpoints anteriores conservados localmente; no LFS/migración necesaria, instalaciones, configuración global o cambios de visibilidad/permisos/protecciones. La revisión por patrones no garantiza ausencia absoluta de secretos. F2-07 y arte final pendientes; las 52 pruebas C02 siguen siendo evidencia histórica, no se repiten por cambios documentales/Git.


### GH-02 — Gitignore e instrucciones para evaluación docente
- Estado: [x] Completado y verificado.
- Descripción / sistema: excluir archivos regenerables/locales conservando el proyecto y evidencias necesarias para evaluación académica. Entrega/Git.
- Prioridad / dependencias: P1; GH-01 y solicitud del usuario del 2026-10-31.
- Aceptación: no ocultar código, escenas, recursos, .meta, Packages, ProjectSettings, herramientas, GDD o documentación; cachés/builds/secretos excluidos y apertura descrita para docente.
- Archivos: .gitignore, README.md, CHECKLIST.md, REGISTRO_CAMBIOS.md.
- Pruebas: git check-ignore --no-index sobre 18 casos excluidos y 19 conservados; revisión de los 616 archivos versionados, cero ocultados por reglas; enlaces README válidos y git diff --check sin errores.
- Resultado / problemas: rutas de cachés ancladas al proyecto para no ignorar carpetas homónimas dentro de Assets; .dll/.fbx/.png y documentación conservadas. Sin eliminar archivos/commits o cambiar jugabilidad; no se repiten pruebas/build Unity por una configuración de Git y documentación. La primera importación regenera cachés y restaura paquetes; F2-07 pendiente.


### F3-E02-ARTE-02 — Nueva hoja conceptual del Monje v02
- Estado: [x] Completado y verificado.
- Descripción / sistema: generar nueva propuesta 2D original según brief del usuario; personajes/arte.
- Prioridad / dependencias: P1; ficha Monje y solicitud artística del 2026-10-31.
- Aceptación: cuatro vistas, pose neutral, guardia, detalles de vendas/calzado/tela/faja, coherencia de vestimenta, cuerpo completo y fondo neutro; sin integración automática en Unity.
- Archivos: Documentacion/Personajes/Conceptos/MONJE_HOJA_PROPUESTA_v02.png y prompt exacto; registro actualizado, v01 conservada.
- Pruebas: imagen 1536×1024 inspeccionada; seis figuras completas y cuatro recuadros de detalles/paleta, hombro izquierdo cubierto y ropa coherente; PNG/hash verificados. No aplica compilar o ejecutar Unity para este cambio artístico.
- Resultado / problemas: hoja conceptual 2D entregada, propuesta para revisión; no equivale a turnaround calibrado, FBX, rig o animaciones. Aprobación artística final y F2-07 siguen pendientes; apariencia/UI E02-B/C02 conservadas.


### F3-ARTE-GUE-01 — Hoja conceptual del Guerrero
- Estado: [x] Completado y verificado.
- Descripción / sistema: nueva propuesta 2D original de Guerrero medieval funcional, coherente con la presentación del Monje; personajes/arte.
- Prioridad / dependencias: P1; dirección artística y solicitud del usuario del 2026-10-31.
- Aceptación: frente/perfil/espalda/3/4, pose neutral y guardia de espada, detalles de armadura/cuero/brazal/bota/arma, figura completa y fondo neutro; sin ornamentación excesiva o integración Unity.
- Archivos: Documentacion/Personajes/Conceptos/GUERRERO_HOJA_PROPUESTA_v01.png y prompt exacto; registro actualizado.
- Pruebas: PNG 1536×1024 inspeccionado, seis figuras, cinco grupos de detalle y paleta; manos/espada, silueta, materiales y encuadre revisados; hash verificado. No se ejecuta Unity para una entrega raster/documental.
- Resultado / problemas: concepto propuesto para revisión, no turnaround calibrado o FBX/rig/animación. Escudo/capa opcionales omitidos para priorizar armadura y espada. Talla del prompt orientativa sin cambiar medidas E01 o controlador; F2-07 y aprobación artística final pendientes.


### F3-ARTE-MAG-01 — Hoja conceptual del Mago
- Estado: [x] Completado y verificado.
- Descripción / sistema: nueva propuesta 2D original de Mago medieval sobrio y coherente con Monje/Guerrero; personajes/arte.
- Prioridad / dependencias: P1; dirección artística y solicitud del usuario del 2026-10-31.
- Aceptación: frente/perfil/espalda/3/4, pose neutral y conjuración, detalles de bastón/grimorio/cinturones/telas/accesorios; silueta vertical, rostro/manos/pies visibles y efectos discretos.
- Archivos: Documentacion/Personajes/Conceptos/MAGO_HOJA_PROPUESTA_v01.png y prompt exacto; registro actualizado.
- Pruebas: PNG 1536×1024 inspeccionado: seis figuras, cinco recuadros y paleta; continuidad general de túnicas/equipo, silueta, encuadre y materiales revisados; hash verificado. No se ejecuta Unity para una entrega raster/documental.
- Resultado / problemas: hoja conceptual propuesta para revisión, no turnaround calibrado o modelo/rig/animación. Arte y medidas actuales E01 siguen intactos; aprobación artística final y F2-07 pendientes.


## PRODUCCIÓN — E03 a E06 (2026-10-31)

Diseños autorizados para uso provisional. F2-07: usuario informa 7/8, cierre X/ESC pendiente; no equivale a aprobación de arte final.

### PROD-E03 — Integración de tres apariencias
- Estado: [x] Completado y verificado.
- Sistema / prioridad: producción integrada / P1. Dependencias: hojas originales y base E02-B.
- Aceptación: criterios del encargo E03, implementación en Unity y pruebas; validación humana diferenciada.
- Archivos: ProductionCharacterBuilder, MonkProxyGeometry reutilizada con Box, tres prefabs en Characters/E03, mallas/materiales/Source en 07_Characters/ProductionProvisional, catálogo y pruebas ProductionVisualTests; informe y tres capturas.
- Pruebas: compilación/generación salida 0; 2/2 dirigidas metric/URP/recuperación y Play Mode con tres capturas reales inspeccionadas.
- Resultado / problemas: completada técnicamente, pendiente de validación humana. Representaciones estáticas 1,78/1,80 m sin colisión, 5.252–6.704 triángulos; originales preservados. No modelos/rig/animaciones finales.

### PROD-E04 — Catálogo, retratos y equipo compartidos
- Estado: [x] Completado y verificado.
- Sistema / prioridad: producción integrada / P1. Dependencias: E03.
- Aceptación: criterios del encargo E04, implementación en Unity y pruebas; validación humana diferenciada.
- Archivos: CharacterVisualCatalog/Model, ProductionPresentationBuilder, catálogo/prefabs E03, sprites Portraits y fuentes importadas, pruebas ProductionVisualTests y adaptación histórica MonkIdentityTests; informe/capturas.
- Pruebas: configuración/compilación salida 0; 4/4 dirigidas, tres clases/retratos/equipo repetido/retirada por estado/guardado/carga, layouts/raycast 720p/1080p.
- Resultado / problemas: completada técnicamente; tres retratos conceptuales 3:4 originales, configuración compartida sin estadísticas duplicadas. Bastón accesorio sin item/buff; modelos y validación humana pendientes.

### PROD-E05 — El cargamento perdido: misión integrada
- Estado: [x] Completado y verificado.
- Sistema / prioridad: producción integrada / P1. Dependencias: E04 y sistemas existentes.
- Aceptación: criterios del encargo E05, implementación en Unity y pruebas; validación humana diferenciada.
- Archivos: escena AetherniaVillageDemo, Quests/VillageResident/LostCargo/CargoDialogue, LostCargoQuest/Controller/View, extensión GameSession/TurnEncounter, ProductionVillageBuilder, LostCargoTests, lanzador y documentación/capturas E05.
- Pruebas: generación/compilación salida 0; 5/5 dirigidas antes y después de pulir etiquetas/semilla de prueba; recorridos reales con tres clases, guardado v1 por etapa, recompensa única/inventario lleno y victorias ajenas rechazadas; capturas 720p/1080p.
- Resultado / problemas: completada técnicamente; misión y NPC provisionales, diálogo/objetivo desacoplados de C02. Escena original preservada; revisión humana pendiente. No se simula victoria ni se cambia formato de guardado.

### PROD-E06 — Regresión, capturas, build y PR
- Estado: [x] Completado y verificado.
- Sistema / prioridad: producción integrada / P1. Dependencias: E03/E04/E05.
- Aceptación: criterios del encargo E06, implementación en Unity y pruebas; validación humana diferenciada.
- Archivos: VerticalSliceHud, GameSession.Rename, BuildSmokeProbe/CargoQuestSmoke, ProductionInputTests/MonkIdentityTests, lanzador smoke RequireCargo, documentación y capturas E06; Docs/Validacion/E03_E06_RESULTADOS.json.
- Pruebas: dirigida de entrada 1/1, regresión completa 62/62, build Windows x64 salida 0; recorridos headless/GPU con sistemas previos y misión con tres clases PASS; ZIP CRC/binarios/históricos verificados.
- Resultado / problemas: validación técnica aprobada y PR #1 abierto en borrador hacia dev (https://github.com/MathiasMerino/RPGRTS/pull/1). Rama publicada; main intacta. Fallo inicial 61/62 por nombre esperado de prefab corregido y toda la batería repetida. Humanos 7/8 informado, X/ESC pendiente; sin modelos/rig/clips finales.


## E07 — TRANSFORMACIÓN DE LA ALDEA (2026-10-31)

Dependencia Git: dab39c2/E03–E06, PR #1 sin fusionar. Escena integrada AetherniaVillageDemo. Plan y evidencia en Docs/E07_ALDEA_MEDIEVAL.md.

### E07-P0 — Preparación y línea base
- Estado: [x] Completado y verificado.
- Sistema / prioridad: entorno integrado / P1. Dependencias: E03–E06.
- Aceptación: PR revisado, GDD/versiones verificadas, encuadres comparables y snapshot físico/nav conservados.
- Archivos: plan E07, VillageE07Tests, Docs/Images/E07/Before (12 PNG), Docs/Validacion/E07_BASE_FISICA.txt y procedencia de texturas.
- Pruebas / resultado: 1/1 dirigida aprobada, compilación Unity salida 0, doce capturas reales; misión aceptada/ruta/combate real completados y snapshot físico/nav registrado. Primer intento de captura intentaba salir de combate activo: arnés corregido para vencer con las reglas existentes. Aprobación artística y humana pendientes.

### E07-P1 — Materiales y arquitectura modular
- Estado: [x] Completado y verificado.
- Sistema / prioridad: entorno integrado / P1. Dependencias: E07-P0.
- Aceptación: Materiales URP compartidos, modelos métricos, pendientes/aleros/cumbrera, módulos y variantes sin sobrescribir D01.
- Archivos: VillageE07Materials/Geometry/Architecture, cinco fuentes PNG y sus importadores, trece materiales nuevos y siete módulos/cinco edificios en VillageE07; lanzador y prueba de biblioteca.
- Pruebas / resultado: generación/compilación Unity salida 0 y dirigida 1/1 aprobada; raíz métrica, mallas/materiales soportados/compartidos, referencias, texturas/mipmaps y cuerpos con colisión diferenciada. Recursos D01 preservados. Revisión visual en escena y humana pendientes de P3/P4.

### E07-P2 — Terreno, vegetación y utilería
- Estado: [x] Completado y verificado.
- Sistema / prioridad: entorno integrado / P1. Dependencias: E07-P1.
- Aceptación: Caminos/plaza/entrada/bosque legibles y decorados coherentes, sin bloquear ruta ni zonas RTS/construcción.
- Archivos: VillageE07Nature/Props, 21 prefabs adicionales (6 naturales, 3 de suelo y 12 de utilería), mallas propias y pruebas/lanzador.
- Pruebas / resultado: generación/compilación salida 0, 2/2 dirigidas; mallas/materiales compartidos, troncos sólidos, vegetación blanda sin colisión y suelo transitable plano. Distribución de escena y ruta se validan en P3/P4; revisión humana pendiente.

### E07-P3 — Integración e iluminación
- Estado: [x] Completado y verificado.
- Sistema / prioridad: entorno integrado / P1. Dependencias: E07-P2.
- Aceptación: Escena original de demo mejorada, misión y UI intactas, sombras/atmósfera y oclusión reversible comprobadas.
- Archivos: AetherniaVillageDemo, VillageE07SceneBuilder/Refinement, Volume E07 y mallas de escena; assembly de autoría/lanzadores y VillageE07Tests.
- Pruebas / resultado: autoría/correcciones salida 0, 5/5 dirigidas y 67/67 regresión final, foto real revisada. Física original/NavMesh/IDs/misión conservados; navegación/recorte probados con triángulos y colisiones intactas. Arte y evaluación humana pendientes.

### E07-P4 — Capturas y regresión
- Estado: [x] Completado y verificado.
- Sistema / prioridad: entorno integrado / P1. Dependencias: E07-P3.
- Aceptación: Antes/después reales, diez vistas mínimas, navegación/cámara y pruebas existentes aprobadas sin falsear expectativas.
- Archivos: VillageE07Tests, Docs/Images/E07/Before/After y recorte, Docs/Validacion/E07_RESULTADOS.json/E07_BIBLIOTECA.txt, dirección artística/escala/informe.
- Pruebas / resultado: 67/67 regresión, cero fallos/omisiones; 24 comparativas 1080p y dos capturas de oclusión, inspeccionadas. URP/GUID/meta/referencias y originales comprobados; draw calls 1.292→1.079 en muestra equivalente. No FPS ni aprobación humana.

### E07-P5 — Build y publicación
- Estado: [x] Completado y verificado.
- Sistema / prioridad: entorno integrado / P1. Dependencias: E07-P4.
- Aceptación: Player Windows funciona con las tres clases/misión, ZIP verificado y PR apilado publicado sin fusionar #1/main.
- Archivos: BuildSmokeProbe/StandaloneGameCapture, referencias a URP instalado, lanzador RequireVillageE07, captura Standalone-1280, evidencia/informe y ZIP local excluido de Git.
- Pruebas / resultado: build salida 0; headless 36,05 s/GPU 35,59 s con tres clases/misión y sistemas anteriores PASS; captura real inspeccionada y ZIP CRC/binarios/históricos verificados. PR #2 publicado en borrador hacia feature/e03-e06-personajes-vertical-slice (https://github.com/MathiasMerino/RPGRTS/pull/2); #1 sigue abierto/borrador/sin fusionar. Humanos pendientes.


### E07-H — Evaluación humana y artística de la aldea
- Estado: [ ] Pendiente.
- Sistema / prioridad: revisión jugable/arte / P1. Dependencias: E07-P5.
- Aceptación: el usuario revisa cámara, oclusiones, lectura de clase/objetivos, movimiento y dirección visual; aprobación explícita.
- Archivos: informe E07 y protocolo humano previo; no se adivinan resultados de la sesión.
- Pruebas / resultado: pendiente de validación humana. F2-07 conserva 7/8 informado; X/ESC pendiente. Ninguna automatización aprueba el arte.


## E08 — PERSONAJES ARTICULADOS (2026-10-31)

Base E07/8f0108b, PR #1/#2 sin fusionar. Plan y evidencia en Docs/E08_PERSONAJES_ANIMADOS.md; aprobación artística/humana independiente.

### E08-01 — Auditoría y piloto de capacidad
- Estado: [x] Completado y verificado.
- Sistema / prioridad: personajes integrados / P1. Dependencias: E07.
- Aceptación: Herramientas/recursos/PR revisados, piloto con skin y esqueleto real y límites documentados.
- Archivos: AnimatedCharacterBuilder/NativeCharacterSkin, Docs/E08_PERSONAJES_ANIMADOS.md.
- Pruebas / resultado: Unity exacto compiló autoría, tres Avatares Generic válidos; casos de pesos/skin/BindPose y BakeMesh pasan. Sin Blender/bpy/modelos externos; FFmpeg local de Python verificado, sin instalación.

### E08-02 — Modelos, rigging y clips
- Estado: [x] Completado y verificado.
- Sistema / prioridad: personajes integrados / P1. Dependencias: E08-01.
- Aceptación: Tres modelos 3D originales articulados, pesos/bindposes válidos, clips reales y conceptos diferenciados.
- Archivos: Assets/07_Characters/AnimatedE08 y 02_Prefabs/Characters/E08; NativeCharacterSkin/Clips.
- Pruebas / resultado: Tres mallas originales, 24 huesos, dos influencias normalizadas, trece clips con siete estados por clase; normales/metros/deformación y ajuste de vendas guardado comprobados. Modelos provisionales; humano/artista pendiente.

### E08-03 — Integración modular de animación y combate
- Estado: [x] Completado y verificado.
- Sistema / prioridad: personajes integrados / P1. Dependencias: E08-02.
- Aceptación: Grafo compartido, locomoción real, acciones aceptadas/daño/derrota visuales y equipo en manos, sin cambiar reglas.
- Archivos: HumanoidVisualRig/AnimationProfile/AnimationDriver, CharacterCombatAnimationPresenter, TurnEncounter, PrototypeDirector y catálogo/escena demo.
- Pruebas / resultado: Prueba integrada de tres clases PASS: velocidades y estados Walk/Run/Idle reales, ataque aceptado/rechazado, Hit/Defend/Defeat, manos/equipo/v1; último golpe sobrevive limpieza de victoria. Reglas, turnos y daño no se mueven a clips.

### E08-04 — Escena, deformación y evidencia
- Estado: [x] Completado y verificado.
- Sistema / prioridad: personajes integrados / P1. Dependencias: E08-03.
- Aceptación: E07 conservada, poses/juntas/animación verificadas visualmente y capturas reales de estados.
- Archivos: AnimatedCharacterTests, Tools/Pack-E08Evidence.py, Docs/Images/E08, E08_EVIDENCIA.json.
- Pruebas / resultado: 45 PNG seleccionados: doce ángulos, 21 poses y doce tomas de acciones/resultados reales; 36 muestras alimentan tres MP4 decodificados de doce frames. Revisión técnica visual realizada con imágenes nativas; no aprobación humana/artística. E07/física/escena original conservadas en pruebas. Órdenes y poses muestreadas diferenciadas.

### E08-05 — Regresión y Windows
- Estado: [x] Completado y verificado.
- Sistema / prioridad: personajes integrados / P1. Dependencias: E08-04.
- Aceptación: Pruebas disponibles y recorridos de tres clases/misión/sistemas anteriores, build y ZIP verificables.
- Archivos: AnimatedCharacterSmoke/BuildSmokeProbe, PrototypeSaveStore (diagnóstico), lanzadores, E08_REGRESION.xml/E08_RESULTADOS.json/E08_PAQUETE.json y ZIP local.
- Pruebas / resultado: 73/73 final, salida 0; build 203.270.570 bytes; headless 54,77 s y GPU 55,46 s con misión/animación de tres clases y sistemas previos PASS. ZIP 86.334.027 bytes/305 archivos, CRC y todos los hashes verificados; diez ZIP previos intactos. Un fallo intermedio de escritura y recontrol 2/2 documentados; diagnóstico añadido, sin alterar operación/esquema. No aprobación humana.

### E08-06 — Documentación y publicación
- Estado: [x] Completado y verificado.
- Sistema / prioridad: personajes integrados / P1. Dependencias: E08-05.
- Aceptación: Guías/fichas/checklist/registro actualizados, commits y PR apilado, repo limpio y humano pendiente.
- Archivos: Docs/E08_PERSONAJES_ANIMADOS.md, E08_GITHUB.json, checklist/registro/plan/AGENTS/COMO_JUGAR/dirección/ficha Monje.
- Pruebas / resultado: commits por bloques, rama feature/e08-personajes-animados publicada; PR #3 abierto/borrador/base E07 (https://github.com/MathiasMerino/RPGRTS/pull/3). main/dev y PR #1/#2 sin fusionar ni modificar; comprobación de sincronía/limpieza al cierre. Fichas, licencias, Generic/clips, importación futura, pruebas/build/evidencia y límites documentados. Humano/arte final pendientes; no E09.


### E08-H — Revisión humana y artística
- Estado: [ ] Pendiente.
- Sistema / prioridad: revisión jugable/arte / P1. Dependencias: E08-06.
- Aceptación: el usuario revisa las tres clases en la aldea, lectura isométrica, proporciones, ropa, articulaciones, armas y acciones; aprobación explícita. F2-07/X/ESC y E07-H mantienen sus estados independientes.
- Archivos: Docs/E08_PERSONAJES_ANIMADOS.md y evidencia E08; no se inventan resultados humanos.
- Pruebas / resultado: pendiente; las pruebas automáticas y la inspección técnica de capturas no sustituyen esa sesión.
