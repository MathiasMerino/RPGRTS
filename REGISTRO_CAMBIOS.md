# Registro de cambios de Crónicas de Aethernia

Conservar entradas anteriores; añadir fecha, tareas, alcance, archivos y validación al terminar cada actividad.

## 2026-10-28 — Diagnóstico y preparación inicial

Tareas: PRE-01, PRE-02 y PRE-03. Alcance autorizado: analizar y preparar el proyecto, sin programar mecánicas.

Se localizó el proyecto real `ProyectoRPGRTS/` y se leyó íntegramente el GDD 0.2. Se revisaron escena, assets, scripts, prefabs, paquetes, ajustes e instalación del IDE. Resultado: plantilla URP organizada, una escena y ninguna mecánica propia implementada. Se distinguieron definiciones conceptuales, propuestas experimentales y decisiones pendientes.

Se crearon `AGENTS.md`, `CHECKLIST.md`, `PLAN_DESARROLLO.md` y este registro en la raíz de trabajo, fuera de Assets para no generar importaciones documentales. Se añadieron `.gitignore` y `.rgignore` para excluir cachés/archivos locales y acotar búsquedas. Se prepara `Docs/GDD_LECTURA.md` como copia textual consultable, conservando el Word original y las tablas.

Se verificaron Visual Studio Community 2026, la carga de trabajo ManagedGame, Visual Studio Tools for Unity y el paquete de integración Unity 2.0.26. No se instalaron extensiones adicionales: la integración necesaria ya está presente. No se modificó configuración global del equipo ni se añadieron servicios externos.

Validación realizada: inspección estática de archivos; versiones cotejadas con manifest/lock; lectura de todo el contenido del GDD y sus partes complementarias; revisión de escena y ausencia de código propio. Se capturaron hashes SHA-256 de los 60 archivos originales relevantes para comprobar que no se alteren. La búsqueda de errores C# en el log histórico no encontró coincidencias; hay avisos históricos de licencia/puerto cuya vigencia no está confirmada.

Validación documental completada: 303 párrafos con texto y dos tablas cubiertos en la copia de lectura; hashes SHA-256 de los 60 archivos originales idénticos; 26 tareas con todos sus campos, estados y dependencias válidos; documentos UTF-8 y exclusiones comprobados. PRE-03 completada. No se abrió Unity, no se compiló ni se probó Play Mode/build; no se declara verificada ninguna funcionalidad jugable.

Git local inicializado en esta raíz con identidad existente y sin cambiar configuración global; PRE-04 en curso hasta registrar y comprobar el commit inicial. Siguiente paso: completar Git y presentar el plan para su aprobación antes de BAS-01 y de cualquier implementación.

## 2026-10-28 — Cierre de preparación y verificación Git

Tarea PRE-04 completada: commit base `7e009d8` con 64 archivos fuente/documentales. Se verificaron exclusiones de Library, Logs, UserSettings y proyectos C# generados; estado limpio después del commit y ningún remoto configurado. Se utilizó la identidad Git existente. La escritura del índice y del commit requirió autorización del entorno, concedida para las operaciones locales.

La revisión de whitespace del estado inicial señala espacios ya existentes en YAML de Unity; se conservaron para respetar los originales. La revisión limitada a los siete documentos/filtros nuevos no detectó problemas. La copia textual del GDD incluye 303 párrafos con texto y dos tablas; los 60 archivos originales inventariados conservan sus hashes.

Se actualiza el checklist a 4 tareas verificadas y 22 pendientes. Se registrará este cierre en un segundo commit documental. No se cambió ningún asset, escena, paquete, script ni configuración Unity, y no se añadieron extensiones. Compilación, Play Mode y build siguen pendientes.

Siguiente actividad: PRE-05, aprobación del plan por el usuario según su prompt; después BAS-01. Ninguna mecánica está autorizada todavía.

## 2026-10-28 — Aprobación del plan e inicio de validación Unity

El usuario indicó «continua con el prompt» tras recibir el diagnóstico y el plan; PRE-05 queda aprobada. Se inicia BAS-01: importación, compilación, revisión de paquetes/referencias y entrada/salida de Play Mode con Unity 6000.5.8f1. La aprobación permite avanzar de forma incremental con la base técnica; no fija las reglas RPG, las tres clases ni otros diseños pendientes del GDD.

Archivos actualizados: checklist, plan y este registro. Resultado de ejecución Unity pendiente; no se marca BAS-01 como completada todavía.

## 2026-10-28 — BAS-01 verificada sin control de ventana

El usuario solicitó priorizar pruebas automáticas en segundo plano y autorizó continuar. Se incorporaron un lanzador PowerShell y un conjunto de pruebas de editor con EnterPlayMode/ExitPlayMode. El lanzador comprueba la versión instalada, el bloqueo real del proyecto, el código de salida y un XML nuevo con pruebas ejecutadas; no considera una ejecución sin resultados como válida.

Resultado: 3/3 pruebas aprobadas, sin omisiones, con Unity 6000.5.8f1 y salida 0. Se verificaron paquetes declarados frente a registrados, URP configurado, ausencia de scripts/referencias serializadas perdidos en SampleScene y entrada/salida real de Play Mode. Evidencia local: `ProyectoRPGRTS/Logs/Baseline-results.xml` y `Baseline.log`. No se utilizó la ventana de Unity en esta ejecución.

Durante la preparación se corrigieron referencias duplicadas en el asmdef, un nombre PackageInfo ambiguo y una API InstanceID retirada en Unity 6.5. Se adoptó EntityId y se repitió hasta compilar y pasar las pruebas. No se modificaron los paquetes para resolverlo.

Se inicia BAS-02 y la preparación de BAS-03 con una guía métrica, un generador de prefabs/escena que rechaza sobrescrituras y pruebas de dimensiones, colisiones y render. Todavía pendientes de generar y verificar. La escena original se conserva.

## 2026-10-28 — BAS-02 y BAS-03 completadas y verificadas

Unity generó seis prefabs con raíces a escala 1: cubo de 1 m, humano de 1,80 m, puerta con hueco 1,20 × 2,20 m, ventana con hueco 1 × 1,20 m, casa de 6 × 8 m y suelo de 40 × 40 m. Se crearon siete materiales URP compartidos y la escena independiente TechnicalSandbox, con calle marcada, obstáculo, paso estrecho, iluminación y cámara fija de inspección. Todos los GUID y `.meta` fueron generados por Unity. Se conserva también SceneTemplateSettings.json, añadido automáticamente por el editor al crear la escena.

El conjunto final aprobó 12/12 pruebas con salida 0 y ninguna omisión: regresión del entorno original, tamaños importados, colisiones de paso, referencias, materiales, suelo y Play Mode con render GPU. Se corrigió el SetUp de la prueba gráfica para soportar su reinvocación durante Play Mode antes de repetir el conjunto completo. La captura 1280×720 se revisó visualmente y se conservó en `Docs/Images/TechnicalSandbox.png`.

Documentación: `Docs/ESCALA_REFERENCIA.md` y `Docs/VALIDACION_BASE_TECNICA.md`, además del plan y checklist actualizados. Se registró en AGENTS.md la preferencia de validación en segundo plano y el requisito EntityId de Unity 6.5. La comparación Git no detectó cambios de contenido en SampleScene, el GDD, los paquetes ni los ajustes preexistentes.

Resultado acumulado: 8/26 tareas verificadas y 18 pendientes. La escena es un entorno técnico; no incorpora controles de cámara, selección/movimiento, navegación horneada, interacción, UI ni sistemas RPG/RTS completos. La build de integración se realizará en BAS-09. Siguiente tarea autorizada: BAS-04, cámara isométrica con desplazamiento y zoom.

## 2026-10-29 — Autorización del prototipo completo e implementación de la base

El usuario solicitó completar el prompt en segundo plano, eligió Guerrero/Mago/Monje y aprobó expresamente las reglas provisionales de `Docs/ESPECIFICACION_PROTOTIPO.md`. Se autoriza trabajar de forma incremental en todos los sistemas del prototipo descrito, conservando su carácter provisional y sin escribir historia definitiva.

Se generó PlayablePrototype.unity a partir de una copia de TechnicalSandbox, con cámara ortográfica controlable, héroe reutilizable, selección por ratón, movimiento por NavMesh, un objeto interactuable y HUD uGUI mediante prefabs/tema compartidos. El agente y el horneado usan radio 0,30 m y altura 1,80 m; se actualizó NavMeshAreas.asset de forma explícita. La escena de arranque de la build es la nueva escena; las anteriores se conservan en el proyecto.

Pruebas automáticas en curso para BAS-04–BAS-08. Todavía no se declaran verificadas ni se da la build por terminada.

## 2026-10-29 — Base jugable y prototipo vertical completados

Se cerraron BAS-04–BAS-09 con cámara controlable, selección, rutas completas de navegación, interacción por alcance y HUD medieval reutilizable. La base pasó 15/15 pruebas y su primera build arrancó sin ventana, caminó e interactuó con salida 0. Se resolvió la restauración de variables capturadas del test de UI después del cambio de dominio de Play Mode; no era un defecto del recorrido del player.

Con las reglas provisionales aprobadas se incorporaron estado compartido, JSON versionado, inventario/equipo, nombre y tres clases, turnos con iniciativa/d20/alcance/guardia, región y mazmorra, dos facciones, objetivo persistente, comercio, producción, taller con carving y una batalla RTS de seis contra seis. La escalada opcional transporta una victoria RPG como +4 de salud a los aliados; el calendario económico se pausa durante ambos combates.

La suite final aprobó 23/23, sin fallos ni omisiones. Incluye rechazo de cargas corruptas/no finitas, límites de inventario/acciones/movimiento, costes, persistencia, navegación modificada por taller, bloqueo de clics y escritura sobre la UI y todas las referencias serializadas de la escena. Se revisaron capturas en dos resoluciones y de los modos; los muros delanteros de la mazmorra se bajaron a parapetos para revelar el portal de salida sin cambiar su huella ni hacerlo transitable.

La build definitiva de Windows compiló (170.913.663 bytes, 37,42 s de compilación incremental) y su sonda independiente terminó con salida 0 en 7,40 s. El player comprobó recorrido, equipo, construcción, mazmorra/consecuencia, guardar/cargar, victoria RPG, orden de grupo y victoria RTS. El estado final contiene los cuatro hechos esperados, clase Mago y un taller. Las mediciones de frames/memoria pertenecen al modo de automatización y no se presentan como requisitos comerciales ni FPS con render.

Se entregó un ZIP portable de 71.065.469 bytes excluyendo símbolos Burst DoNotShip. Ejecutable, paquete y comprobantes permanecen en Builds/Logs excluidos de Git; el código, assets, pruebas, informes y capturas se registran. Unity reserializó perfiles/ajustes URP y batching al construir; se revisaron esas diferencias. No se modificó el GDD ni las escenas SampleScene/TechnicalSandbox y no se actualizaron paquetes.

El usuario aprobó también la hoja de ruta posterior. FUT-01 se cierra como planificación aprobada, con presupuesto/calendario comercial explícitamente diferidos antes de iniciar producción. EXP-01 se cierra como experimento mecánico/visual verificado y conservado opt-in; la evaluación humana de diversión y equilibrio sigue en esa hoja de ruta. No se afirma haber producido el juego comercial completo.

Documentos entregados: COMO_JUGAR.md, ESPECIFICACION_PROTOTIPO.md, ARQUITECTURA.md, VALIDACION_PROTOTIPO.md y PLAN_EXPANSION.md, junto con checklist/plan/historial actualizados. Resultado: 26/26 tareas del alcance aprobado cerradas y verificadas. Siguiente actividad futura: evaluación del prototipo con jugadores y elección de una fase de expansión concreta.

## 2026-10-30 — Inicio de fase 2 y auditoría

Tarea F2-01. El usuario autorizó auditoría de los doce sistemas, integración y mejoras pequeñas verificables, con propuestas previas para cambios grandes. Se conservan las 26 tareas anteriores. Se releyó el GDD íntegro mediante copia cuyo hash coincide con el Word, plan, registro, reglas/hoja de ruta y todos los scripts; Git limpio en c2da95a. Revisados XML 23/23, informe Build y comprobante smoke previos. Nueva repetición de base en curso.

Se abre Docs/AUDITORIA_FASE_2.md y siete tareas secuenciales. Hallazgos a reproducir: participantes RTS residuales y órdenes del héroe al cambiar modo; movimiento enemigo RPG sin ruta; ataques sin bloqueo geométrico; fallos de carga sin explicación y validación de cantidades/ciclos. No se implementan sistemas nuevos ni se considera realizada una sesión humana por ejecutar pruebas automáticas.

La repetición F2-01 produjo XML nuevo con 23/23 Passed (8,17 s de casos). El log confirma fin de pruebas, pero el proceso Unity devolvió -1073741819 durante el cierre nativo. Se conserva Phase2-baseline.log/XML; no se registra como ejecución integral con salida 0. Se añaden nueve regresiones de fase 2 antes de corregir, para reproducir modos, participantes residuales, geometría táctica, carga y validación extrema. Se prepara guion manual explícitamente pendiente.

F2-01: reproducción inicial ampliada 23/32 Passed, 9 fallos, salida 2. Ocho señalan fallos del producto; uno pertenece al test por una variable capturada por LINQ restaurada tras EnterPlayMode. Se corrigió el test con un foreach sin captura y se sustituyeron sobrecargas obsoletas por FindObjectsByType<T>() de Unity 6.5. Se conserva el XML/log de reproducción. El rival RPG saltó una barrera: ruta equivalente de 6,129754 m frente a 3 m permitidos. Se añade además comprobación de despliegue RTS bloqueado por taller persistente; verificación dirigida en curso antes de implementar F2-02.

F2-02: las dos pruebas RTS dirigidas fallaron antes de la corrección: supervivientes con rutas activas y batalla iniciada pese a taller en despliegue. Se conserva Phase2-rts-reproduction-results.xml/log. Implementado cierre de participantes transitorios al salir/cargar, Stop del héroe al entrar en gestión/RTS y parada de supervivientes. El RTS comprueba terreno, cápsula libre y conectividad de los doce puntos antes de crear agentes/cambiar modo; rechaza con mensaje sin alterar el mundo si hay un taller. No reserva terrenos ni elimina edificios del jugador. La victoria pasa a describirse como hecho guardado, sin afirmar conquista de un mapa inexistente. Pruebas dirigidas de aceptación en curso.

F2-02 verificada: 3/3 pruebas dirigidas, salida 0. F2-03 en desarrollo: se conserva el desplazamiento instantáneo del rival entre turnos, pero recorre hasta 3 m siguiendo esquinas de ruta completa; héroe no puede reponer turno ni encadenar órdenes mientras está en tránsito. Ataques de ambos participantes requieren línea física libre; la acción inicial por iniciativa también respeta alcance/obstáculo. No se añaden hechizos, animación o reglas RPG nuevas. Aceptación dirigida y regresión RPG/escalada en curso.

F2-03 verificada: 8/8 dirigidas aprobadas, salida 0, evidencia Phase2-tactical-results.xml/log. Se conservó la victoria RPG/escalada/RTS y se eliminaron el salto de barrera y la reposición de turno durante tránsito. F2-04 en desarrollo: cantidades acotadas y suma larga, ProductionCycle igual al ciclo del reloj guardado, mensajes de carga por causa sin reemplazar estado al fallar; interacción guardada por modo e IDs reconocidos. El HUD conserva motivos específicos (distancia/repetición) y muestra arma/armadura, estados de combate y modos indisponibles. Regresión de persistencia/UI/integración en curso.

F2-04 verificada: 8/8 dirigidas, salida 0 (Phase2-persistence-ui-results.xml/log). Se amplían los casos extremos para pasar también por JSON real. F2-05 inicia suite completa de 33 casos; F2-06 prepara sonda del player con aserciones de equipo, restauración efectiva de oro, un único edificio, pausa del reloj y retirada de participantes. La sonda ahora falla si falta el director; antes podía terminar anticipadamente sin comprobar el recorrido completo. Se conserva el GDD, todos los assets/escenas existentes y las 26 tareas anteriores byte a byte en su sección histórica. Verificación de .meta/GUID sin faltantes, huérfanos ni duplicados.

F2-05 aprobó inicialmente 33/33, cero omisiones, salida 0, y se inspeccionaron cinco capturas nativas. F2-06: primera build correcta (170.919.346 bytes, 65,52 s), smoke PASS/salida 0 en 7,12 s, pero log con aviso nativo de agente sin NavMesh. Se conserva la primera evidencia y no se acepta como cierre. Una repetición con stack completo confirmó aviso en arranque nativo; se leyó OnEnable/AddData del NavMeshSurface instalado, sin modificar paquetes/caché. Se difiere a Start la activación del agente de la instancia de escena mediante modificación puntual serializada (justificación técnica para tocar PlayablePrototype). Se añade regresión y rechazo del aviso/excepciones al lanzador del player. F2-05 reabierta hasta repetir suite/build.

F2-05 cerrada tras la corrección de arranque: 34/34, cero fallos/omisiones, salida 0, 8,66 s de casos excluyendo arranque/importación. Phase2-final-results.xml/log actualizados. Las cinco capturas se conservaron; 720p/mazmorra idénticas por hash y tres variantes revisadas nuevamente. Medición RTS editor GPU batch: 6,32 s, 6.278 frames de automatización, 687.901.244 bytes asignados en una muestra; no FPS ni memoria máxima de juego. Segunda build en curso; se comprobará player sin gráficos y con GPU sin interacción humana.

La segunda build compiló (170.919.383 bytes, 53,99 s, salida 0). El player corregido pasó sin gráficos (7,47 s) y con dispositivo GPU en segundo plano (7,22 s), ambos salida 0 y sin aviso de agente inválido/excepciones según el lanzador reforzado. Se conserva Phase2-final-player-smoke/log y Phase2-graphics-smoke/log. Se confirmó arranque en Intel UHD DX12 del player y capturas de editor en RTX 3050 Laptop; métricas batch no se presentan como FPS o requisitos mínimos. El guardado final tiene Mago, arma/armadura, un taller y cuatro hechos; limpieza de participantes y pausa del calendario verificadas. Paquete portable de fase 2 en verificación; F2-07 permanece pendiente.

## 2026-10-30 — Entrega técnica de fase 2

F2-01 a F2-06 completadas y verificadas; F2-07 continúa pendiente, con guion y doce comprobaciones humanas sin marcar realizadas. Se entregan checklist secuencial, auditoría con matriz real/propuestas, arquitectura/inventario/instrucciones actualizados y cinco capturas propias. Propuesta siguiente: sesión humana, después iteración acotada de feedback/ayuda/colocación y una muestra artística; no iniciar sistemas grandes ni transición compleja automáticamente.

ZIP portable Aethernia-Fase2-Windows.zip: 69.566.729 bytes, 305 entradas, CRC correcto, SHA-256 630b2c144c1f044d34bcf3395ab0bf03bdf68b1ef2ef21b66e934db9cfec22f2. Ejecutable comprobado contra el contenido del ZIP; paquete histórico conserva su hash. Pruebas finales 34/34, dos players salida 0, sin fallos NavMesh/excepciones. Se verificaron campos/estados de todas las tareas, 26 tareas históricas intactas, .meta/GUID sin problemas, runtime sin referencias UnityEditor/APIs retiradas, sintaxis PowerShell y diff sin errores de whitespace en fuentes/docs/scripts. GDD, escenas de referencia, prefabs, paquetes y ajustes conservados; PlayablePrototype solo añade override de habilitación inicial del agente por la incidencia reproducida.

La entrega se registra en un commit local de fuentes/documentación/evidencia visual. Builds/Logs/cachés siguen excluidos; no se publica a remotos ni se cambia configuración global. La incidencia aislada de cierre nativo del primer editor queda documentada y no se reprodujo en las verificaciones posteriores; no se afirma haber determinado su causa. Evaluación humana, tipografía con ventana/escalado real, equilibrio y diversión permanecen abiertos.
