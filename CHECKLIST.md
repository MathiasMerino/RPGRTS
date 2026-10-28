# Checklist de Crónicas de Aethernia

Actualizado: 2026-10-28, America/La_Paz. Plan aprobado mediante la indicación del usuario «continua con el prompt».

## Resumen actual

- Terminadas: PRE-01 a PRE-05 y BAS-01 a BAS-03; 8 de 26 tareas verificadas.
- En curso: ninguna; 18 tareas pendientes.
- Pendiente inmediata: BAS-04, cámara isométrica con desplazamiento y zoom.
- Problemas: todavía no hay base jugable; reglas RPG, tres clases iniciales, guardado y tiempo compartido sin especificar.
- Validación Unity: 12/12 pruebas automáticas aprobadas, con entrada/salida de Play Mode, medidas, colisiones y render revisado; build pendiente para BAS-09.
- Siguiente actividad recomendada: implementar BAS-04 reutilizando TechnicalSandbox; la cámara actual es fija.

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
- Estado: [ ] Pendiente.
- Sistema / prioridad: Cámara/entrada / P0. Dependencias: BAS-03.
- Descripción: encuadre isométrico, desplazamiento y zoom con límites configurables.
- Aceptación: lectura clara de referencias, controles estables, zoom limitado y UI sin interferencias.
- Archivos: ninguno aún; previstos scripts en `03_Scripts/Camera/`, prefab cámara y acciones de entrada.
- Pruebas: compilar, Play Mode con ratón/teclado, extremos de zoom y repetición de entrada/salida.
- Validación: pendiente; proyección ortográfica es propuesta inicial.

### BAS-05 — Personaje provisional reutilizable
- Estado: [ ] Pendiente.
- Sistema / prioridad: Personajes / P0. Dependencias: BAS-03.
- Descripción: cápsula o modelo temporal a escala, sin clase ni atributos RPG inventados.
- Aceptación: prefab con pivote/collider coherentes, visible en cámara y sin referencias perdidas.
- Archivos: ninguno aún; previstos prefab en `02_Prefabs/Characters/` y escena de pruebas.
- Pruebas: inspección a escala, colisiones y Play Mode con BAS-04.
- Validación: pendiente de implementación y Unity.

### BAS-06 — Selección y movimiento por ratón
- Estado: [ ] Pendiente.
- Sistema / prioridad: Exploración/navegación / P0. Dependencias: BAS-04, BAS-05.
- Descripción: seleccionar personaje y ordenar destino; probar AI Navigation existente.
- Aceptación: llega a destino accesible, evita obstáculo, no atraviesa puerta incompatible, ignora destinos inválidos y clics UI.
- Archivos: ninguno aún; previstos `03_Scripts/Selection/`, `03_Scripts/Movement/`, acciones y datos de navegación de sandbox.
- Pruebas: compilación, suelo/obstáculo/paso, órdenes repetidas, pérdida de selección y clic fuera del terreno.
- Validación: pendiente; collider y agente deben compartir escala coherente.

### BAS-07 — Interacción básica
- Estado: [ ] Pendiente.
- Sistema / prioridad: Interacción / P0. Dependencias: BAS-06.
- Descripción: un objeto provisional seleccionable/interactuable con resultado visible, sin inventario completo.
- Aceptación: interacción dentro de alcance, rechazo controlado fuera de alcance y referencias seguras tras desactivar objeto.
- Archivos: ninguno aún; previstos `03_Scripts/Interaction/`, prefab objeto y sandbox.
- Pruebas: compilar, casos de distancia, clic repetido y desactivación en Play Mode.
- Validación: pendiente de implementación y Unity.

### BAS-08 — UI medieval reutilizable
- Estado: [ ] Pendiente.
- Sistema / prioridad: Presentación / P0. Dependencias: BAS-07.
- Descripción: panel de selección e interacción, botón base y estilo compartido; validar propuesta uGUI.
- Aceptación: dos usos de componentes compartidos sin duplicar lógica; texto legible, cambios de resolución y clic UI sin ordenar movimiento.
- Archivos: ninguno aún; previstos prefabs UI, estilos/configuración y presentadores en `03_Scripts/UI/`.
- Pruebas: compilación, Play Mode en varias resoluciones, navegación y alta/baja de suscripciones.
- Validación: pendiente; apariencia final y recursos artísticos no aprobados todavía.

### BAS-09 — Verificar base jugable
- Estado: [ ] Pendiente.
- Sistema / prioridad: Integración / P0. Dependencias: BAS-08.
- Descripción: cerrar primer hito con recorrido cámara → selección → movimiento → interacción → feedback.
- Aceptación: recorrido reproducible, sin errores de Console ni referencias rotas; build de prueba arranca; evidencia de cada criterio.
- Archivos: ninguno aún; previstos ajustes de integración y evidencia en documentos; builds excluidos de Git.
- Pruebas: compilación Unity, Play Mode, build local y perfilado básico sin presupuesto inventado.
- Validación: pendiente; solo entonces declarar la base jugable completada.

## Prototipo vertical del GDD

### VER-01 — Aprobar especificaciones mínimas
- Estado: [ ] Pendiente.
- Sistema / prioridad: Diseño transversal / P1. Dependencias: BAS-09.
- Descripción: dividir decisiones sobre tres clases, atributos/turnos, objetos, tiempo compartido, guardado, región y límites de los sistemas.
- Aceptación: requisitos y ejemplos aprobados por sistema; hardware/presupuesto y alcance vertical fijados; no escribir historia completa.
- Archivos: ninguno aún; previstas especificaciones en `Docs/` y actualización del plan.
- Pruebas: revisión de consistencia con GDD §§2–11 y registro de decisiones.
- Validación: pendiente de diseño y aprobación; descomponer antes de implementar.

### VER-02 — Estado compartido y guardado mínimo
- Estado: [ ] Pendiente.
- Sistema / prioridad: Persistencia / P1. Dependencias: VER-01.
- Descripción: estado mutable, identificadores estables y formato versionado para los datos mínimos aprobados.
- Aceptación: guardar/cargar restaura personaje y un hecho de mundo; datos ausentes o incompatibles tienen comportamiento definido.
- Archivos: ninguno aún; previstos `03_Scripts/World/`, `03_Scripts/Persistence/` y pruebas pertinentes.
- Pruebas: compilación, ida y vuelta de datos y reinicio de partida; no mutar configuración ScriptableObject.
- Validación: pendiente de especificación e implementación.

### VER-03 — Creación básica y tres clases
- Estado: [ ] Pendiente.
- Sistema / prioridad: Personajes/progresión / P1. Dependencias: VER-02.
- Descripción: creador acotado y tres clases elegidas expresamente; contemplar catálogo sin implementar las doce.
- Aceptación: crear, cargar y visualizar personaje válido; reglas y clases corresponden a decisiones aprobadas.
- Archivos: ninguno aún; previstos datos de clases, lógica de personaje y UI reutilizada.
- Pruebas: compilación, selección válida/inválida y persistencia de cada clase.
- Validación: pendiente; las clases iniciales no están elegidas.

### VER-04 — Objetos, inventario y equipo mínimos
- Estado: [ ] Pendiente.
- Sistema / prioridad: Objetos / P1. Dependencias: VER-03.
- Descripción: subconjunto de objetos y reglas aprobado para exploración/combate.
- Aceptación: recoger, equipar y guardar sin duplicaciones ni pérdidas; UI reutiliza componentes.
- Archivos: ninguno aún; previstos configuración de objetos, inventario/equipo, presentadores y prefabs.
- Pruebas: compilación, cambios de equipo, límites definidos y guardar/cargar.
- Validación: pendiente; fabricación completa queda fuera del primer alcance.

### VER-05 — Encuentro RPG por turnos
- Estado: [ ] Pendiente.
- Sistema / prioridad: Combate RPG / P1. Dependencias: VER-04.
- Descripción: encuentro pequeño con iniciativa, acción/movimiento, dados y habilidades aprobadas.
- Aceptación: victoria/derrota y fin de turno coherentes; tiempo estratégico sigue la política acordada; datos separan lógica y presentación.
- Archivos: ninguno aún; previstos combate, habilidades, UI turnos y escena de encuentro.
- Pruebas: compilación, reglas con semillas controladas, Play Mode completo y persistencia según contrato.
- Validación: pendiente; descomponer y aprobar reglas antes de programar.

### VER-06 — Región mínima y consecuencia persistente
- Estado: [ ] Pendiente.
- Sistema / prioridad: Mundo/misiones/facciones / P1. Dependencias: VER-05.
- Descripción: integrar asentamiento, mazmorra, dos facciones y una misión aprobada en una región acotada.
- Aceptación: recorrido explorable; decisión modifica un hecho y su efecto permanece al recargar.
- Archivos: ninguno aún; previstos escenas de región, datos de facciones y misión, diálogo/eventos mínimos.
- Pruebas: compilación, recorridos alternativos, transición de escenas y guardar/cargar consecuencias.
- Validación: pendiente; macrohito por descomponer, sin crear narrativa ahora.

### VER-07 — Economía simplificada
- Estado: [ ] Pendiente.
- Sistema / prioridad: Economía/comercio / P1. Dependencias: VER-06.
- Descripción: producción/consumo y comercio mínimo con reglas aprobadas y conexión a estado de mundo.
- Aceptación: transacciones válidas, recursos conservados y paso del tiempo coherente; sin valores definitivos inventados.
- Archivos: ninguno aún; previstos economía, datos de mercancías/mercado y UI común.
- Pruebas: compilación, límites, transacciones repetidas, avance temporal y persistencia.
- Validación: pendiente de diseño y descomposición.

### VER-08 — Construcción básica
- Estado: [ ] Pendiente.
- Sistema / prioridad: Construcción/propiedad / P1. Dependencias: VER-07.
- Descripción: edificio y terreno acotados, costes aprobados, colocación a escala y vínculo económico.
- Aceptación: respetar propiedad, colisiones, recursos y acceso navegable; edificio persiste.
- Archivos: ninguno aún; previstos lógica de construcción, configuración, prefab edificio y UI.
- Pruebas: compilación, colocación inválida, coste insuficiente, navegación tras construir y guardar/cargar.
- Validación: pendiente de diseño y descomposición.

### VER-09 — Batalla RTS acotada
- Estado: [ ] Pendiente.
- Sistema / prioridad: Guerra RTS / P1. Dependencias: VER-08.
- Descripción: especificar y crear una batalla con selección de grupos, órdenes y estadísticas simplificadas.
- Aceptación: órdenes y resultado verificables; número de unidades y rendimiento acordados; consecuencias guardadas.
- Archivos: ninguno aún; previstos unidades/escuadrones, órdenes, UI y escena RTS.
- Pruebas: compilación, grupos/obstáculos, victoria/derrota, perfilado con volumen definido y persistencia.
- Validación: pendiente; descomponer reclutamiento, formaciones y demás alcance militar antes de implementar.

### VER-10 — Validar prototipo vertical completo
- Estado: [ ] Pendiente.
- Sistema / prioridad: Integración/calidad / P1. Dependencias: VER-09.
- Descripción: verificar elementos del GDD §10 en un recorrido y evaluar viabilidad de expansión.
- Aceptación: región, tres clases, asentamiento, mazmorra, turnos, dos facciones, economía, construcción, RTS y misión persistente comprobados o alcance renegociado explícitamente.
- Archivos: ninguno aún; previstos informe de validación, ajustes y documentación de build.
- Pruebas: compilación, build, regresión funcional, guardado, legibilidad y perfilado en hardware acordado.
- Validación: pendiente; no basta demostrar cada sistema de forma aislada.

## Experimentos y expansión

### EXP-01 — Evaluar transición RPG a RTS
- Estado: [ ] Pendiente.
- Sistema / prioridad: Experimento de modos / P2. Dependencias: VER-10 y autorización específica del experimento.
- Descripción: probar un conflicto que escala con refuerzos, según GDD §12.3.
- Aceptación: protocolo de tiempo/estado definido, efecto RPG sobre RTS legible y conclusión conservar/modificar/descartar.
- Archivos: ninguno aún; previstos prototipo aislado e informe; no modificar encuentros generales automáticamente.
- Pruebas: compilación, transición y casos de cancelación/fallo; evaluación de jugabilidad.
- Validación: pendiente; propuesta experimental, no obligación del prototipo.

### FUT-01 — Descomponer expansión posterior
- Estado: [ ] Pendiente.
- Sistema / prioridad: Producción futura / P2. Dependencias: VER-10.
- Descripción: priorizar compañeros, origen, clases restantes, magia, fabricación, diplomacia/gobierno, simulación regional, mapa continental, caravanas, asedios, narrativa y arte/audio definitivos.
- Aceptación: plan nuevo aprobado con dependencias y presupuesto; ninguna propuesta provisional convertida en definitiva sin revisión.
- Archivos: ninguno aún; previstos nuevas tareas y especificaciones, preservando este historial.
- Pruebas: revisión del GDD completo y resultados del prototipo; pruebas técnicas se definirán por subtarea.
- Validación: pendiente; es un hito de planificación, no autorización de implementar todos los sistemas.

## Historial de seguimiento

- 2026-10-28: inspección inicial y lectura completa del GDD; PRE-01/PRE-02 verificadas; documentación en elaboración. No se implementaron mecánicas.
- 2026-10-28: PRE-03 verificada con cobertura textual completa, 60 hashes originales intactos y 26 tareas consistentes; Git local inicializado y registro en curso.
- 2026-10-28: PRE-04 verificada mediante commit base `7e009d8`, índice de 64 archivos y estado limpio; preparación terminada. PRE-05 pendiente de aprobación; Unity y mecánicas sin ejecutar.
- 2026-10-28: plan aprobado por el usuario; BAS-01 completada con 3/3 pruebas en segundo plano; BAS-02/BAS-03 en preparación y validación incremental.
- 2026-10-28: BAS-02 y BAS-03 verificadas con suite final 12/12 y render GPU revisado; 8/26 tareas completas. Siguiente: BAS-04. No se requiere una nueva aprobación del plan para continuar la base autorizada.
