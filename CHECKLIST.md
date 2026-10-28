# Checklist de Crónicas de Aethernia

Actualizado: 2026-10-28, America/La_Paz. Plan pendiente de aprobación.

## Resumen actual

- Terminadas: PRE-01 (GDD), PRE-02 (inventario) y PRE-03 (documentación).
- En curso: PRE-04 (registro Git).
- Pendientes inmediatas: PRE-05 (aprobación) y BAS-01 (abrir/validar Unity tras aprobar).
- Problemas: todavía no hay base jugable; reglas RPG, tres clases iniciales, guardado y tiempo compartido sin especificar.
- Validación Unity: pendiente. No se ejecutaron compilación, Play Mode ni build durante la fase documental.
- Siguiente actividad recomendada: revisar/aprobar el plan; después BAS-01. No programar mecánicas antes.

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
- Estado: [ ] En desarrollo.
- Sistema / prioridad: Control de versiones / P0. Dependencias: PRE-03.
- Descripción: iniciar Git local en esta carpeta, incluir fuentes y `.meta`, excluir cachés y guardar el estado inicial.
- Aceptación: commit local verificable, sin Library/Logs/generados ni publicación remota.
- Archivos: metadatos Git locales; `.gitignore`; fuentes existentes y documentos registrados.
- Pruebas: git status, check-ignore, listado de archivos registrados y log.
- Validación: repositorio inicializado; exclusiones comprobadas; pendiente commit y comprobación de estado final. Identidad Git existente disponible; no se modifica configuración global.

### PRE-05 — Aprobar plan y primer hito
- Estado: [ ] Pendiente.
- Sistema / prioridad: Dirección / P0. Dependencias: PRE-03, PRE-04.
- Descripción: revisión del diagnóstico, secuencia y alcance de la base jugable por el usuario.
- Aceptación: aprobación expresa registrada antes de empezar la implementación.
- Archivos: por actualizar `CHECKLIST.md` y `REGISTRO_CAMBIOS.md` con la decisión.
- Pruebas: verificar autorización en la conversación.
- Validación: pendiente de decisión del usuario; instalar herramientas no implica aprobar mecánicas.

## Base jugable

### BAS-01 — Validar entorno Unity
- Estado: [ ] Pendiente.
- Sistema / prioridad: Entorno / P0. Dependencias: PRE-05.
- Descripción: abrir el proyecto exacto, comprobar paquetes, Console, compilación y Play Mode de la escena existente.
- Aceptación: importación completa, sin errores impeditivos ni referencias perdidas; entrada/salida de Play Mode comprobada.
- Archivos: ninguno aún; previstos informe de evidencia en este checklist y registro.
- Pruebas: Unity 6000.5.8f1, Package Manager, Console, SampleScene y depurador si se necesita.
- Validación: pendiente de ejecución; revisar avisos históricos solo si reaparecen.

### BAS-02 — Fijar referencias de escala
- Estado: [ ] Pendiente.
- Sistema / prioridad: Escala 3D / P0. Dependencias: BAS-01.
- Descripción: contrastar las medidas propuestas en el plan y preparar referencias reutilizables.
- Aceptación: 1 unidad = 1 m; humano, puerta, ventana, calle y edificio coherentes; colliders y paso documentados.
- Archivos: ninguno aún; previstos guía de escala en `ProyectoRPGRTS/Assets/05_Docs/` y prefabs patrón en `02_Prefabs/`.
- Pruebas: inspección de transform/importación, comparación con cubo de 1 m y pruebas de huecos.
- Validación: pendiente; medidas del plan son propuestas de ensayo.

### BAS-03 — Crear escena de pruebas independiente
- Estado: [ ] Pendiente.
- Sistema / prioridad: Escenarios / P0. Dependencias: BAS-02.
- Descripción: suelo, obstáculo y paso estrecho para ensayos; conservar SampleScene.
- Aceptación: escena abre con URP correcto, escala coherente, colisiones y ausencia de referencias perdidas.
- Archivos: ninguno aún; previstos `ProyectoRPGRTS/Assets/01_Scenes/TechnicalSandbox.unity` y assets necesarios con `.meta`.
- Pruebas: apertura, guardado, Play Mode y revisión visual del entorno.
- Validación: pendiente de implementación y Unity.

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
