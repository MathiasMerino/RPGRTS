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

## 2026-10-30 — Fase 3, bloques A/B

Se leyó el prompt adjunto y se verificaron base Git cc9691d limpia, evidencia 34/34 y dos players de fase 2, hash GDD y herramientas locales. CLI Unity 6000.5.8f1 presente; sin herramienta Unity/MCP expuesta ni paquete MCP en manifest; sin instalaciones. Inspeccionados UI/escena/prefabs/materiales/referencias métricas y captura antes. Punto de control local codex/fase3-base-cc9691d.

F3-A01 y F3-B01 verificadas documentalmente. Se crea Documentacion/DIRECCION_ARTISTICA.md por ruta expresa del usuario, con paletas/materiales/iluminación/arquitectura, conceptos Guerrero/Mago/Monje/Pícaro y contrato futuro de hijo VisualRoot. Medidas aplicadas centralizadas en Docs/ESCALA_REFERENCIA.md; propuestas distinguidas. Checklist secuencial de ocho tareas, histórico conservado y F2-07 pendiente. Se mantiene uGUI; continúa primera mejora C1 de UI contextual, sin modificar mundo/reglas. D/E/C2 no se declaran implementadas.

F3-C01: aplicada UI contextual mediante ContextUiBuilder, con cuatro entradas sobre panel existente y acciones reutilizadas. Tema ampliado con tipografía/espaciado. Prueba dirigida 1/1, salida 0; catorce comprobaciones de layout, cambio de clase/equipo/comercio y listeners. Revisadas ocho capturas (siete contextos y Resumen 1080p). Se detectó el botón Interactuar deshabilitado pero aún visible fuera de exploración; ajuste pendiente antes de cierre. Regresión completa en curso; comparación de 76 bloques YAML no UI idénticos al punto de control. No se modifican modelos, colisiones, reglas o paquetes.

La primera regresión completa de C1 aprobó 35/35, salida 0. Se conserva Phase3-before-final-polish-results.xml/log. La inspección de capturas llevó a ocultar también Interactuar en RPG/construcción/RTS (antes solo estaba deshabilitado), sin cambiar el guardado o las reglas; nueva aserción verifica esa visibilidad en cada contexto. Repetición final completa en curso antes de build y commit funcional. F2-07 conserva su estado pendiente.

F3-C01/F3-F01 completadas y verificadas: cierre 35/35, sin fallos/omisiones, salida 0 y 8,33 s de casos. Interactuar ahora está oculto fuera de exploración y la aserción se repite en cada modo. Se conservaron ocho capturas representativas; las tres afectadas se revisaron nuevamente. La UI mantiene los mismos comandos y guardado, nombre/clase en Personaje y ayuda específica; 34 casos anteriores conservados con una adaptación de acceso al campo de nombre. F3-G01 inicia build y recorrido independiente. D/E/C2 y F2-07 pendientes; no se declara toda la fase 3 completada.

Revisión de diff C1: fuentes/docs/PowerShell sin errores de whitespace; seis campos m_Name vacíos nuevos conservan el espacio que escribe Unity en YAML. Se mantiene la serialización nativa, igual que en entregas anteriores; no es un fallo funcional. Commit de C1 preparado tras suite 35/35; sin cambios en paquetes/prefabs/escala del mundo.

## 2026-10-30 — Entrega A/B/C1 de fase 3

F3-G01 completada: build 170.931.692 bytes, 55,46 s, salida 0. Recorridos independientes sin gráficos (7,37 s) y con GPU RTX 3050 Laptop/DX12 (7,24 s) aprobados, sin excepciones/agentes inválidos. El slot de ensayo en Logs confirma Mago, espada/túnica, taller, reputación +10/-5 y hechos persistentes. No hay evaluación humana ni métricas comerciales de FPS.

ZIP Aethernia-Fase3-UI-C1-Windows.zip: 69.570.489 bytes, 305 entradas, CRC correcto, SHA-256 db2d6a04acb149d24635e05e85cd656d4875a4b2627f93a5ee1e0a693a800d48. Exe, DLL runtime y level0 coinciden con la build. Históricos preservados, cachés/builds/logs excluidos de Git. Ocho capturas reales de Play Mode revisadas y documentadas; layouts 7×2 resoluciones aprobados. Commits A/B 9f5f409 y C1 2316506; cierre documental/evidencia visual en commit local separado.

Estado de fase 3: 5/8 tareas verificadas; D/E/C2 pendientes. F2-07 continúa pendiente. La dirección estética y cuatro clases están documentadas como confirmaciones/propuestas; no se afirma creación de modelos definitivos ni muestra de escenario medieval. Siguiente actividad recomendada: D01, suelo/camino/fachada/decorados sobre una zona pequeña, respetando medidas/rutas, seguida de E01 y C02.

Verificación final G: GDD y capítulos históricos del checklist (26 iniciales + fase 2) intactos; F2-07 pendiente; fase 3 5/8 con todos los campos. Metadatos/GUID sin faltantes/huérfanos/duplicados, runtime sin UnityEditor/APIs retiradas, scripts PowerShell válidos. ZIP actual y dos históricos comprobados por hash; contenido actual exe/DLL/escena coincide con build. Diff confirma escenas de referencia, prefabs, reglas, NavMesh, sistemas no UI, paquetes y ProjectSettings sin cambios. Fuentes/docs/scripts sin problemas de whitespace; se preservan seis espacios nativos de YAML de Unity ya revisados. Se registra cierre local de documentación/capturas, sin publicación remota ni configuración global.

## 2026-10-30 — Inicio D01

El usuario autoriza únicamente escenario medieval y biblioteca visual, sin personajes definitivos ni mecánicas nuevas. Git limpio en 8197b5f; checkpoint codex/d01-base-8197b5f. GDD intacto por SHA-256; consultados AGENTS/checklist/dirección artística/medidas y autoría de escena. No hay herramienta Unity/MCP expuesta; se usa Unity CLI 6000.5.8f1 existente sin instalar dependencias.

Plan acotado: materiales URP y texturas procedurales propias, piezas modulares de fachada/techo/vanos, barril/cofre/caja/vegetación y superficies; presentación sobre colliders/NavMesh anteriores. Ajuste de luz/encuadre inicial con controles de cámara conservados. Comparar firmas de física, navegación real por puerta y snapshots a encuadre común; suite previa más verificaciones D01, build y smoke. No se marca completado hasta verificar.

## 2026-10-30 — Autoría y revisión D01

Recorrido previo 1/1 aprobado antes de alterar la presentación; registros D01-before-results.xml/log conservados. Creación única mediante MedievalQuarter: once prefabs visuales, ocho materiales URP Lit compartidos, dieciséis mapas procedurales propios de 512² y mallas métricas. Sin descargas ni dependencias nuevas. Source renderers ocultos, GameObjects lógicos/colliders conservados; firmas físicas y SHA-256 del NavMesh idénticos antes/después. Casa, cubierta y módulos anidados; barril/cofre/cajas y plantas bajas; suelo/camino/luz renovados. Escenas/prefabs técnicos originales intactos.

Primeros tres casos D01 aprobados. La revisión de imágenes detectó juntas de piedra/pizarra demasiado oscuras y una unión cruzada del camino: corregidos mediante refinado de assets propios, preservando GUID. Se añaden soportes visuales bajo la ventana de referencia y se ajusta encuadre inicial; no se introduce física nueva.

Prueba aislada de cámara primero falló (esperaba tamaño 12, obtuvo 7): Mathf.Sign(0) provocaba acercamiento cada frame. Corrección limitada en IsometricCameraRig.Update para conservar cero cuando no hay rueda. No se cambian controles ni reglas. Se conserva D01-idle-before-fix-results.xml/log. Cuatro casos dirigidos posteriores aprobados, salida 0; se reemplaza además una llamada de búsqueda obsoleta por la sobrecarga actual de Unity 6.5 antes de la regresión final.

Se inspeccionaron ocho vistas comparables reales de Play Mode (sector/fachada y UI 720p/1080p). «Antes» reconstruye temporalmente renderers/materiales/luz técnica conservados; «Después» activa D01, ambos con cámara corregida y encuadre idéntico por par. La comparación nunca guarda esos cambios temporales. La revisión detectó recorte de chimenea en la vista general; ampliado ese encuadre para la captura final. Firma física/nav y navegación de puerta/cofre/obstáculo verificadas. Regresión completa en curso; aún sin declarar validada la build final o sesión humana.

## 2026-10-30 — Cierre técnico D01

Regresión final 39/39 aprobadas, cero fallos/omisiones, salida 0, 8,13 s de casos; D01-final-results.xml/log conservados. Sin avisos C# de API obsoleta, excepciones ni agentes inválidos en esa ejecución. Ocho capturas finales copiadas a Docs/Images y revisadas; sector incluye la chimenea completa y UI comprobada a 720p/1080p. Once prefabs visuales con raíces métricas, ocho materiales compartidos, dieciséis mapas propios y veintitrés mallas usadas. Grupo D01: 9.312 triángulos/113 renderers; no se confunde con un perfil de FPS/draw calls. Casa modular anidada; materiales reutilizados por instancia, sin pooling/LOD preventivo.

Build Windows: 175.686.522 bytes, 258,73 s incluyendo shaders nuevos, salida 0. Recorrido independiente sin gráficos: PASS, 9,02 s; con GPU RTX 3050 Laptop: PASS, 7,16 s; ambos salida 0, sin excepciones ni agentes inválidos según lanzadores. Comprobantes y logs D01-headless/graphics preservados. ZIP Aethernia-Fase3-D01-Windows.zip: 72.545.489 bytes, 305 entradas, CRC correcto; SHA-256 8e2262f981164205e658476d594ad03526c5128d04d6fbe821fac33a1c736111. Exe, DLL runtime y level0 coinciden con build; tres ZIP históricos intactos por hash.

CHECKLIST F3-D01 completada y verificada: fase 3 pasa a 6/8, E01/C02 pendientes; F2-07 sigue sin sesión humana. Se entregan informe/límites, instrucciones, biblioteca, medidas centralizadas y registro de procedencia propia. No hay corte contextual de techo ni física incluida en nuevos prefabs; conservar proxies existentes en esta escena y validar física al reutilizar en otro mapa. No se declara arte definitivo ni validación humana de diversión/accesibilidad.

Revisión Git: punto previo codex/d01-base-8197b5f (8197b5f); rama de trabajo codex/d01-medieval-quarter. Los 245 bloques originales de escena sobreviven; 122 nuevos y cambios de presentación/luz/encuadre, sin cambios en scripts de reglas/sistemas salvo la corrección mínima de cámara. Escenas de referencia, prefabs técnicos, paquetes, ProjectSettings, NavMesh y GDD intactos. Metadatos sin faltantes/huérfanos/GUID duplicados y mallas sin assets sobrantes. Fuentes/docs/PowerShell sin errores de whitespace; se conserva la serialización YAML nativa de Unity. Se registra commit local de D01; sin publicación remota ni instalaciones nuevas.


## 2026-10-30 — Inicio E01

D01 limpio en ca5ba35, 39/39 y builds anteriores preservadas. GDD conserva su hash; revisadas dirección artística, contrato VisualRoot, estado/clases/controlador y criterios E01. Checkpoint codex/e01-base-ca5ba35 y rama codex/e01-character-visuals. Sin herramienta Unity/MCP expuesta; se usa CLI 6000.5.8f1 instalado, sin plugins nuevos. Plan: demostrar oclusión del techo y validar recorte visual reversible; luego catálogo de tres prefabs métricos, presentador por eventos y equipo visual ligado al estado real. No alterar reglas/controlador/guardado ni completar evaluación humana.


## 2026-10-30 — Implementación y regresión E01

Oclusión demostrada por intersección de un mesh real del techo con el rayo al héroe; render comparativo oculto/visible. RoofCutaway oculta solo renderers, restaura al salir/desactivar y no toca física, materiales o controles. Los primeros ensayos fallaron por una clausura con variables locales creada antes de EnterPlayMode que no sobrevivió a la recarga; sustituida por bucle explícito. El ensayo de diagnóstico posterior solo falló al rechazar sus propios Debug.Log; retirados. Se conservan logs inicial/diagnóstico. No se atribuyen esos fallos al juego o a la API de cámara.

Autoría única CharacterVisuals: tres modelos genéricos geométricos, catálogo compartido, ocho materiales con mapas D01 reutilizados, cinco prefabs (clases y dos equipos) y veintiuna mallas. Presentación por eventos en VisualRoot de la instancia de héroe; controlador/collider/agente y prefab técnico intactos. CharacterVisualModel refleja Weapon/Armour; bastón/escudo guardados solo son referencia de diseño sin estadísticas. Presentador reversible y fallback de configuración inválida sin acumular modelos. Consolidación de autoría reutilizada con parámetro de carpeta, sin reestructurar D01.

Tres casos dirigidos aprobados. Revisión de siete imágenes llevó a aclarar acero y corregir coronilla mediante assets propios, sin regenerar escena o cambiar GUID. Regresión final 42/42, 0 fallos/omisiones, salida 0; 7,83 s de casos, excluidas recargas/arranque. Sin avisos C# o excepciones en log final. Verificados pasos/colliders/selección, clases/equipo/carga, cámara/cubierta, UI y regresión de RPG/RTS/construcción/economía/guardado. Fichas e importación futura documentadas; no se afirman animaciones/modelos definitivos ni evaluación humana. Build independiente en curso.

## 2026-10-30 — Entrega E01

Build Windows aprobada: 178.001.257 bytes, 45,46 s, salida 0. Player independiente sin gráficos PASS 7,50 s; con GPU RTX 3050 Laptop PASS 7,33 s; ambos salida 0 y sin excepciones/agentes inválidos. Se preservan E01-build-summary/log, E01-final-results.xml/log y E01-headless/graphics-smoke/player.log. ZIP Aethernia-Fase3-E01-Windows.zip: 73.584.566 bytes, 305 entradas, CRC correcto; SHA-256 1fd5dd2ba203a288557f497c30ddea607e955b17f59fe3369bf0b8ae14f6e61c. Exe/DLL runtime/level0 coinciden con build; cuatro paquetes históricos intactos por hash.

F3-E01 completada y verificada: fase 3 pasa a 7/8 (C02 pendiente); F2-07 humana pendiente. Siete capturas reales preservadas en Docs/Images; fichas de clase, medidas, importación, fallback, equipo y límites en E01_PERSONAJES.md. La entrega conserva los 367 bloques D01 de escena y añade once; biblioteca D01, prefab técnico del héroe, reglas, NavMesh, paquetes/ProjectSettings y GDD intactos. Metadatos sin faltantes/huérfanos/GUID duplicados. Las figuras son geométricas y estáticas; no se declaran arte, razas, criaturas, rigs o animaciones definitivos. Siguiente actividad recomendada: revisión humana de legibilidad/oclusiones/equipo, seguida de un modelo piloto importado conforme al contrato, sin iniciar C02 automáticamente.

Se registra la entrega en un commit local de la rama codex/e01-character-visuals, con recuperación codex/e01-base-ca5ba35; sin remotos, herramientas nuevas ni cambios globales. Se conservan la serialización YAML nativa de Unity y todos los historiales del checklist; fuentes/docs/scripts revisados antes del commit.


## 2026-10-30 — Inicio E02

Base E01 limpia 2d95959, checkpoint codex/e02-base-2d95959 y rama codex/e02-humanoid-pipeline. GDD preservado por hash; consultados contrato E01, dirección, clases/equipo/controlador y criterios del prompt. Inventario Assets: cero FBX/modelos intercambiables, Avatars, clips o controllers de animación existentes. Sin herramienta Unity/MCP expuesta; se conserva Unity 6000.5.8f1/URP y la CLI instalada. Alcance: infraestructura nativa de importación y validación, candidato Monje vacío, siete estados sin movimientos ficticios y bindings visuales sobre Weapon/Armour existentes. Recurso y clips reales quedan pendientes separados; no se altera inventario/combate ni se inicia arte definitivo.


## 2026-10-30 — Preparación y pruebas dirigidas E02

Se crea una plantilla Monje vacía (sin Renderer/Avatar), perfil de siete clips nulos y grafo Idle/Walk/Run/Attack/Defend/Hit/Defeat. El catálogo conserva Prefab E01 y añade candidato Humanoid; plantilla inválida mantiene el Monje anterior. Driver solo observa agente y parámetros visuales, sin root motion/eventos de clips; no activa Animator sin rig/Idle aplicado. Bindings de ropa/armadura/arma/bastón/accesorio observan Weapon/Armour, sin nuevos ítems/slots persistentes; campos E01 siguen compatibles. Herramientas editor nativas configuran FBX, auditan y preparan candidatos sin activarlos ni sobrescribir prefabs. No se cambia ningún sistema de reglas.

Primera prueba FBX falló por la sintaxis/cabecera del archivo ASCII técnico, no por un recurso humanoide; corregidos cabecera y normales/UV del fixture. Se preserva E02-first-results.xml/log (3/4). Cierre dirigido posterior 4/4, salida 0: importación FBX nativa de cubo 1x1x1 m, flags Human comprobados sin fingir un rig humanoide, rechazo de candidato incompatible, limpieza del fixture, bindings/guardado v1, fallback y lectura de movimiento sin animaciones. Grafo/perfil rechazan clips vacíos/no humanos. E02-targeted-results.xml/log y comprobantes FBX/estructura conservados. Regresión completa en curso; modelo/Avatar/clips reales y UI manual del workbench pendientes de recursos/revisión.

## 2026-10-30 — Entrega de infraestructura E02

Regresión completa 46/46 aprobadas, cero fallos/omisiones, salida 0; 8,03 s de casos (arranque/recargas excluidos). E02-final-results.xml/log conservados. Sin errores/avisos C# ni excepciones en el log final. Build Windows 178.013.219 bytes, 64,97 s, salida 0. Recorridos independientes sin gráficos 7,40 s y con GPU RTX 3050 Laptop 7,38 s, ambos PASS/salida 0 sin excepciones/agentes inválidos; E02-build y E02-headless/graphics preservados.

ZIP Aethernia-Fase3-E02-Windows.zip: 73.589.346 bytes, 305 entradas, CRC correcto; SHA-256 23a51daced714b1fc4fe9e3e8b9202b5966dcb78f5c922b1b38ddf92b180a83c. Exe/DLL runtime/level0 coinciden con build; cinco ZIP anteriores conservan su hash. La build sigue usando E01; el flujo de importación es una herramienta de editor, no un personaje nuevo en la build.

F3-E02 infraestructura completada y verificada. F3-E02-M (modelo/rig real), F3-E02-A (siete clips, retargeting y adaptación de eventos de acciones) y F2-07 humana permanecen pendientes. No se afirma haber importado un personaje humanoide real, reproducido animaciones o usado manualmente el workbench con recursos ausentes. Manifiesto exacto, instrucciones y límites en Docs/E02_IMPORTACION_HUMANOID.md. No iniciar la siguiente fase automáticamente.

Escenas, prefabs/materiales/meshes E01/D01, física/NavMesh, reglas, GDD, paquetes y ProjectSettings intactos. El catálogo preserva GUID y alternativa; solo añade el candidato vacío. Todos los historiales previos del checklist preservados. Metadatos sin faltantes/huérfanos/duplicados; .gitkeep ignorados por Unity conservan carpetas de recursos vacías. Cambios locales en codex/e02-humanoid-pipeline con checkpoint codex/e02-base-2d95959; sin remotos, instalaciones ni configuración global.


## 2026-10-30 — Inicio E02-A artístico

Git limpio 69c78eb; checkpoint codex/e02-arte-base-69c78eb y rama codex/e02-a-monk-art. Leídos brief adjunto, dirección/contrato E02 y pasaje artístico del GDD con hash intacto. Captura del usuario preservada en Documentacion/Personajes/Referencias, solo como fuente de estudio fuera de Assets. Se distinguirán observaciones, requisitos del brief e interpretaciones. Se aplica skill imagegen para una hoja conceptual 2D original, pendiente de aprobación; no FBX ni animaciones. Se conserva el ID anterior F3-E02-A (clips) y se registra la actividad artística como F3-E02-ARTE-01 más aprobación pendiente F3-E02-ARTE-AP.


## 2026-10-30 — Entrega de propuesta artística E02-A

Ficha MONJE_DISENO_BASE.md con doce apartados, referencias visibles/directrices/interpretaciones distinguidas, talla de brief 1,78 m, paño sobre hombro izquierdo, paleta HEX, materiales, vistas, piezas modulares, requisitos Blender/FBX/Humanoid y riesgos de tela/manos en siete estados. Dorso, cierres, rostro/piel y calzado se señalan como propuesta original. No copiar emblemas/collares/bases. Dirección/fichas anteriores y protocolo E02 enlazados, sin reconstruir sistemas.

Herramienta integrada image_gen.imagegen mediante skill imagegen: generada una hoja raster 2D original de 1536x1024, seis vistas/poses y detalles; prompt exacto y PNG copiados al workspace fuera de Assets. Revisada continuidad de hombro/ropa/figura, pies completos, paleta general y ausencia de bases. La proyección y manos no certifican medidas, topología o skinning; discrepancias se resolverán en modelado futuro. No .blend, FBX, rig, texturas PBR o animaciones creados. Referencia PNG usuario SHA-256 75fee9b94d04919cbb0170c08e1b70d5345eb703c3cfb1733446c1d41c9e2a08; concepto SHA-256 6da0717d6f5682ce80e9d22992f4b92ad990404c6c695f078b1b659b814fbdb1.

F3-E02-ARTE-01 completada como entrega de documentación/concepto; F3-E02-ARTE-AP pendiente y todo el diseño sigue PROPUESTO. Se conserva F3-E02-A (animaciones) sin reutilizar ID. No se reemplaza el Monje E01 ni se modifica cápsula, catálogo, prefabs, mecánicas o guardado; 1,78 m no se aplica a Unity. El diff del proyecto está vacío y el GDD conserva su hash. No se ejecutan Unity/Blender, tests o build nuevos por una entrega exclusivamente documental y raster; se consultó evidencia E02 46/46 y build previa como historial. F2-07, modelo/Avatar/clips reales y aprobación artística siguen pendientes. Commit local de propuesta en codex/e02-a-monk-art; sin instalaciones ni publicación remota.


## 2026-10-30 — Inicio E02-B

Git limpio afa1e7e, checkpoint codex/e02-b-base-afa1e7e y rama codex/e02-b-monk-proxy. El nuevo prompt autoriza implementación provisional basada en el concepto; no se interpreta como aprobación de arte final o permiso para FBX/rig/animaciones definitivos. Revisados contrato E02, catálogo/modelo, HUD/autoría uGUI, ficha/concepto y GDD por hash. Sin herramienta Unity/MCP expuesta; Unity CLI existente. Plan acotado: proxy de mallas horneadas por material (sin primitivas runtime), dos materiales nuevos con texturas compartidas, equipo E01 reutilizado, alternativa original preservada, y tarjeta de clase dentro del panel existente con recorte UV de la hoja original (sin editar el PNG). Física/clases/guardado y Guerrero/Mago se conservan; validación antes de cierre.

## 2026-10-30 — Entrega E02-B: Monje provisional e ilustración de clase

Monk_ConceptProxy de 1,78 m, cabeza rapada/torso parcial, paño naranja asimétrico, faja/pantalón oscuros, vendas y calzado ligero. Seis mallas horneadas por material; 5.252 triángulos y 10 renderers incluyendo equipo inactivo. Dos materiales URP nuevos con mapas D01 compartidos; piel/vendas/cuero/detalles E01 reutilizados. Espada/sobrevestido E01 anidados y activados por CharacterVisualModel, sin reescribir inventario/controlador. Catálogo cambia solo Prefab del Monje y datos opcionales de ilustración/UV; candidato vacío E02 preservado. Variante E01, Guerrero y Mago intactos.

ClassReferenceCard reutiliza panel Personaje/MedievalTheme y muestra recorte frontal proporcionado, nombre y estadísticas base actuales. PNG original E02-A copiado sin alterar bytes (SHA 6da0717d6f5682ce80e9d22992f4b92ad990404c6c695f078b1b659b814fbdb1); recorte nativo RawImage.uvRect/AspectRatioFitter, sin nueva imagen generada o edición raster. Captura de referencias del usuario fuera de Assets. HUD mantiene selección y suscripciones; la tarjeta se oculta en otras clases/páginas/modos. Escena añade únicamente tarjeta/enlace; firma física y NavMesh iguales. Sin paquetes/herramientas instalados.

Unity exacto 6000.5.8f1: generación una sola vez salida 0; pruebas dirigidas 3/3 y regresión **49/49**, cero fallos/omisiones, salida 0 (casos 6,89 s, recargas excluidas). Verificados Monje/estadísticas, raycast y selección, ruta real/cofre/interacción, inventario/equipo, guardado aislado/carga, victoria RPG, URP/geometría y UI 720p/1080p. Siete capturas reales GPU revisadas, incluida comparación E01/E02-B con misma cámara/luz/posición; acercamiento 2,35 m solo durante captura, sin cambiar cámara guardada. Build 178.268.191 bytes, 53,44 s, salida 0; recorridos nativos sin gráficos 7,29 s y GPU RTX 3050 Laptop 7,87 s, ambos PASS/salida 0, sin excepciones/agente inválido. Sin errores/avisos C# en logs finales. Evidencia E02B-* conservada en Logs e imágenes en Docs/Images.

ZIP Aethernia-Fase3-E02B-Windows.zip: 73.710.653 bytes, 305 entradas, CRC correcto, SHA-256 fd91da79bf62154eb5569fbb90bf3863d667fd002bbc3dcdad0ea400b6ce956d. Exe/DLL runtime/level0 iguales por hash a build probada; seis archivos históricos conservados. Checklist F3-E02-B completado como identidad **provisional**; guías/escala/arquitectura/plan/inventario y COMO_JUGAR actualizados. Informe único Docs/E02B_MONJE_PROVISIONAL.md con límites y sustitución futura.

No se afirma escultura/rig/animaciones o arte final. Manos, articulaciones y paños simplificados; armadura genérica con posibles intersecciones; equipo en postura estática. Pruebas automáticas/revisión de capturas no son sesión humana. F3-E02-M, F3-E02-A (clips), F3-E02-ARTE-AP, F2-07 y C02 siguen pendientes; no se inicia otra fase. GDD, reglas/guardado v1, D01/E01, recursos E02, paquetes y ProjectSettings preservados; 259 metadatos/GUID sin faltantes/huérfanos/duplicados e historial previo de checklist intacto. Cambios locales en rama codex/e02-b-monk-proxy; sin publicación remota o configuración global.


## 2026-10-30 — Inicio C02

Git limpio 25c1a10, checkpoint codex/c02-base-25c1a10 y rama codex/c02-medieval-ui. Brief C02 autoriza rediseño completo de presentación, no de mecánicas. GDD §12.5/hash, tema/controles/prefabs existentes e informes revisados. Tres referencias privadas fuera de Assets; no se tratan como capturas Unity. Skill imagegen integrada genera atlas original; prompt y PNG preservados. Plan secuencial C02.1..6 en informe; uGUI se conserva. HUD compacto/ventanas cerrables/ficha compartida/inventario seleccionable y contexto por modalidad reutilizan comandos. No minimapa, compañeros, niveles o habilidades nuevos. F2-07 sigue pendiente; generar y validar antes de cierre.


## 2026-10-30 — C02: autoría y correcciones de aceptación

Atlas original por imagegen integrada, prompt exacto y PNG íntegro preservados; cuatro sprites/nine-slice y dos retratos de modelos propios horneados en Unity. HUD compartido conserva suscripciones/acciones; vistas compacta/inventario solo presentan datos/selección. Primera compilación detectó método Image que ocultaba el tipo UI; renombrado. Generación posterior salida 0, sin cambios de física/NavMesh. Primeros ensayos detectaron overrides anidados con tamaño perdido, texto UTF-8 recodificado, etiquetas estrechas y orden de dibujo que ocultaba botones tras fondos. Afinación de UI desacopla instancias de esta escena conservando los prefabs fuente; corrige tamaños/captions y orden. Capturas de los ensayos y resultados fallidos quedan en Logs/C02-first/second/full-first/third.

Se amplía QA para comprobar raycast superior de cada botón a cuatro resoluciones: los rectángulos en pantalla por sí solos no detectaban oclusión. Ensayo dirigido posterior 2/3: layouts y estilo aprobados; fallo restante en prueba de inventario por cierre capturado en IEnumerator que no sobrevivía al domain reload. Trasladado a helper normal, sin modificar lógica del juego. Prueba histórica que buscaba por GameObject.Find un botón ahora oculto se adapta para comprobar ocultación y bloqueo, conservando aceptación de navegación/encuentro. Suite final en curso; F2-07 no realizada.


## 2026-10-30 — Entrega C02

Suite final 52/52 aprobadas, cero fallos/omisiones, salida 0; 7,57 s de casos, recargas/arranque excluidos. Tres pruebas C02 más 49 previas; 44 layouts nativos GPU (once estados por cuatro resoluciones), límites/tamaños/textos y raycast superior de cada botón. Comprobados nombre/tres clases/retratos, inventario seleccionado/equipo/ración/comercio, ausencia de duplicación de eventos y guardado aislado/carga. Regresión de mundo/navegación/mazmorra/economía/construcción/reputación/RPG/RTS conservada. Trece capturas representativas en Docs/Images (dos históricas E02-B y once actuales), revisadas. Fuente/arte/personajes siguen provisionales, sin sesión humana.

Build Windows Succeeded/salida 0, 183.099.535 bytes y 39,44 s; recorridos independientes sin gráficos 7,06 s y con GPU RTX 3050 Laptop/Direct3D 12 7,31 s, ambos PASS/salida 0. Sin errores/avisos C#, excepciones o fallos de agente en logs finales. Comprobantes C02-final/build/headless/graphics en Logs. ZIP Aethernia-Fase3-C02-Windows.zip: 77.005.206 bytes, 305 entradas, CRC correcto; SHA-256 2629012e4853500e12fbd495dbd1129452f2d80500f1c4b08d8ef2e00a8e734d. Exe/DLL runtime/level0 coinciden con build; siete ZIP históricos intactos.

Atlas compartido SHA-256 7ba8dda76e69a8c5abebc881c770e412103251f519e9564862d734111e404693; recurso nativo editor ~11,21 MB, 95 Graphic totales incluyendo inactivos, un Canvas/cámara de juego. Franjas permanentes 23,29 % de pantalla; métricas no equivalen a FPS/GC/draw calls ni a rendimiento mejorado demostrado. Contraste base calculado 12,15:1 texto/madera y 9,55:1 pergamino/tinta, sin certificar todos los píxeles. 278 metadatos/GUID sin faltantes/huérfanos/duplicados; GDD/hash, modelos/materiales, física/NavMesh, mundo/guardado v1, prefabs UI históricos, paquetes y ProjectSettings intactos; historial previo del checklist conservado.

C02.1..6 cerradas dentro del nuevo alcance autorizado. Informe único Docs/C02_INTERFAZ_MEDIEVAL.md, checklist/guías/plan/arquitectura/inventario/AGENTS actualizados. F3-C02 histórico no certifica minimapa ausente; F2-07, aprobación artística, personajes/rig/clips definitivos siguen pendientes. Siguiente recomendación: revisión humana de C02, sin iniciar otra fase. Commit local en codex/c02-medieval-ui, sin remotos, instalaciones o configuración global.


## 2026-10-31 — Conexión de GitHub y ramas main/dev

Raíz Git C:/Users/Mathy/Desktop/PROYECTO GAME, estado inicial limpio en cacad54 y 14 commits en una cadena. Remoto https://github.com/MathiasMerino/RPGRTS.git vacío: cero referencias y tamaño 0 por API; público y main ya predeterminada. Autenticación existente mediante Git Credential Manager con cuenta autorizada; tokens solo en memoria, sin guardarlos en archivos/URL/logs ni solicitar nueva autenticación.

Revisión de los 757 blobs del historial, 682 versiones de texto por patrones: sin credenciales candidatas o rutas generadas/sensibles versionadas; mayor blob 3.003.405 bytes, ninguno supera 10 MiB. .gitignore Unity correcto para cachés/IDE/logs/builds/.env; se añaden extensiones de claves/certificados/keystores. No se precisa Git LFS, migración o reescritura del historial. La revisión es por patrones y no garantiza ausencia absoluta de secretos.

Se añade origin sin reinicializar Git, se crean solo main/dev y se publica la base cacad54 con push atómico normal y upstreams. HEAD remoto y origin/HEAD apuntan a main; dev queda activa. La API confirma ambas ramas idénticas y las 615 rutas/blob SHA de la base iguales al árbol local. Clonación independiente desde GitHub conserva los 14 IDs originales y supera git fsck --full --no-reflogs. Referencias históricas master/codex y checkpoints se conservan localmente, sin publicarlas en bloque o borrarlas.

AGENTS.md define main estable con aprobación de PR dev → main, desarrollo en dev y ramas temporales feature/*, fix/* y test/* desde dev; PR hacia dev con alcance/evidencia, merges que conservan commits y sin push forzado. La primera base es autorizada por la petición de conexión, sin convertir F2-07/arte final en aprobados. Informe Docs/GITHUB_FLUJO.md y GH-01 en checklist; documentación/ignore se incorporan con commit normal y avance fast-forward de main, seguido de publicación y comprobación final de ambos árboles/remotos/limpieza. Evidencia GitHub-* en Logs. Sin jugabilidad, Unity, paquetes, ajustes, instalación, permisos/protecciones, visibilidad o configuración global modificados; no se vuelven a ejecutar tests/build por esta tarea.


## 2026-10-31 — Gitignore para evaluación docente

Estado inicial limpio en dev, 5a2e3ce. El usuario solicita excluir archivos innecesarios para evaluación posterior. Se adapta el .gitignore existente, con comentarios y rutas de Unity ancladas a ProyectoRPGRTS para no ocultar carpetas Library/Logs/Build legítimas dentro de Assets. Se excluyen cachés, temporales, ajustes locales, outputs, perfilado/grabaciones, estado de IDE, soluciones generadas, informes de fallos, credenciales/certificados y archivos auxiliares del sistema. No se ignoran globalmente .dll, .fbx, .png, .unity, .asset o .meta; se conservan Assets, Packages/manifest y lock, ProjectSettings, Tools, GDD y evidencias documentales.

README.md orienta al docente: abrir la subcarpeta Unity con 6000.5.8f1, esperar importación/restauración, cargar PlayablePrototype y Play; enlaces a controles, GDD, arquitectura, checklist e informe C02. Se distingue la evidencia histórica de 52 pruebas de una ejecución nueva. Validación git check-ignore --no-index: 18 casos correctamente excluidos, 19 conservados y los 616 archivos ya versionados no quedan ocultos; enlaces README y diff correctos. Comprobante local GitIgnore-docente-verification.json en Logs. Cambio pequeño de configuración/documentación en dev; sin modificar main, eliminar archivos/commits, cambiar jugabilidad o repetir tests/build Unity. F2-07 y recursos finales siguen pendientes.


## 2026-10-31 — Hoja conceptual Monje v02

Nueva solicitud artística: Monje adulto original, atlético/rapado, ropa asimétrica naranja/ocre, faja/pantalón oscuros, vendas beige, calzado ligero y accesorio monástico discreto. Skill imagegen/herramienta integrada genera hoja 2D 1536×1024: frente, perfil, espalda, 3/4, pose neutral y guardia; cuatro detalles de vendas/calzado/tejido/faja y paleta. Revisados coherencia de lado del hombro, cuerpo completo, silueta y materiales. PNG y prompt exacto guardados en Documentacion/Personajes/Conceptos/MONJE_HOJA_PROPUESTA_v02.*, sin sobrescribir v01. SHA-256 b0c0878974d4896414f614c6bc397221a46f96f18641aa7e4b4c12e3455b65a9.

F3-E02-ARTE-02 completa como entrega de imagen propuesta. No se modifica Unity, controlador, modelos, catálogo o UI, ni se afirma FBX/rig/animación o medidas calibradas; no se repiten pruebas/build por cambio exclusivamente documental/raster. La aprobación artística final y evaluación humana permanecen pendientes. Guardado local de la propuesta; esta solicitud no publica la imagen en GitHub ni sustituye el arte vigente.


## 2026-10-31 — Hoja conceptual Guerrero v01

Skill imagegen/herramienta integrada: generada hoja 2D original 1536×1024, Guerrero adulto robusto con rostro serio, acero/cuero funcionales, protección de torso/hombro/antebrazo, cinturón, botas resistentes, faldón corto y espada proporcionada. Presentación neutra y paleta sobria coherentes con el Monje; sin ornamentación fantástica excesiva. Incluye cuatro vistas, pose neutral, guardia de espada y grupos de detalle de armadura, cuero/cinturón, guante/brazal, bota/suela y arma/empuñadura. Revisada continuidad general de figura, silueta, mano de espada y encuadre completo. PNG/prompt exacto preservados en Documentacion/Personajes/Conceptos/GUERRERO_HOJA_PROPUESTA_v01.*; SHA-256 cf4a92e58101b073cdb12a7ef94cd734ba7d3ba5ae2e96c02c1327210f738705.

F3-ARTE-GUE-01 completa como entrega de propuesta 2D, sujeta a revisión artística. No se crea o integra modelo/rig/animaciones; no se modifican clases, UI, catálogo, controlador o arte vigente. La talla 1,86 m del prompt solo orienta proporciones de esta propuesta y no cambia las medidas E01 ni certifica escala de la lámina. No se repiten tests/build de Unity; F2-07 y aprobación de arte final siguen pendientes. Commit local, sin publicar en GitHub esta solicitud artística.


## 2026-10-31 — Hoja conceptual Mago v01

Skill imagegen/herramienta integrada: hoja 2D original 1536×1024, Mago masculino adulto de silueta vertical y complexión normal/delgada, expresión reservada, túnicas en capas azul grisáceo/púrpura apagado, cuero marrón, lino beige y metal discreto. Bastón fino, grimorio sujeto al cinturón, bolsas/contenedores de componentes y capucha bajada; efecto de conjuración leve sin ocultar cuerpo/mano. Incluye cuatro vistas, pose neutral con bastón separado, conjuración y cinco detalles de bastón, grimorio, cinturón, tejidos y accesorios. Revisadas figura completa, continuidad general del diseño y coherencia de presentación con las hojas Monje/Guerrero. PNG y prompt exacto en Documentacion/Personajes/Conceptos/MAGO_HOJA_PROPUESTA_v01.*; SHA-256 d11938b0d5125f2e11e698469173c3596956b7a8fa4d2a9150c7053b378ecb29.

F3-ARTE-MAG-01 completa como entrega artística propuesta, pendiente de revisión final. No se modifica Unity, clase, UI, catálogo, materiales o controlador vigente; no se produce FBX, rig, animación o turnaround calibrado. No se repiten tests/build por cambios únicamente raster/documentales; F2-07 sigue pendiente. Guardado/commit local, sin publicar esta solicitud en GitHub.


## 2026-10-31 — Inicio producción E03–E06

Brief adjunto autoriza provisionalmente las tres hojas originales y misión narrativa de ensayo. Git limpio, dev 742c04f sincronizado preservando tres commits artísticos, rama solicitada feature/e03-e06-personajes-vertical-slice. No hay rama F2-07 detectada; usuario informa 7/8 humanas, cierre X/ESC pendiente. GDD/hash intactos; inspección dirigida sin modelos humanoides ni herramientas de modelado expuestas. Plan/decisiones en Docs/E03_E06_PRODUCCION.md. Preservar recursos/escenas originales, código modular existente, Facts v1 y gestor C02.


## 2026-10-31 — E03: tres representaciones provisionales

Nuevos prefabs métricos E03, mallas horneadas por material reutilizando MonkProxyGeometry (Box añadido), armadura/correas/faldón Guerrero, túnica/capucha/libro/bastón de identidad Mago y uniones/hombros/vendas/paño Monje. Equipo actual anidado sin colliders ni runtime primitives; fuentes originales copiadas íntegras. Originales y física preservados. Generación salida 0, prueba métrica inicial 1/1 y aceptación ampliada 2/2 (incluye Play Mode y tres capturas revisadas). Métricas/resultados en informe E03_E06; no se afirman rig, animaciones o escultura final. Paleta Mago oscura, ajuste reversible previsto E04.


## 2026-10-31 — E04: presentación y equipo compartidos

Se amplía catálogo existente con metadatos visuales/sprites/fuentes/recuperación/materiales, sin estadísticas duplicadas. Recortes nativos 180×240 desde las hojas originales, importadas Sprite/sRGB/NPOT None; no PNG editados ni nuevas imágenes. Anclajes y bindings v1 reutilizados; paleta del Mago moderadamente aclarada. Configuración salida 0 y 4/4 pruebas dirigidas: tres clases, retratos, equipo repetido sin duplicados, estado sin arma/tela y carga restaurada; seis capturas 720p/1080p con layouts/raycast correctos. Tests históricos adaptan las referencias activas manteniendo la recuperación E01/E02-B. No gestor C02 modificado, nuevos items o reglas.


## 2026-10-31 — E05: El cargamento perdido

Demo independiente reutiliza D01 y NavMesh, añade NPC y carga con componentes existentes, diálogo/objetivo C02 independientes y reglas específicas sobre Facts v1. Misión aceptada → combate real → recuperación → regreso/entrega única; 20 oro y ración opcional por espacio. GameSession amplía interacción por hook nullable, TurnEncounter permite punto de spawn y evento final sin cambiar reglas. OnEnable/Disable libera enlaces. Escena principal intacta. Cinco pruebas dirigidas aprobadas dos veces; tres clases completan rutas reales, batalla, recompensa y persistencia por etapa; inventario lleno/partida previa/victoria ajena comprobados. Primera generación falló por ruta D01 sin Props, corregida sin duplicar NPC. Capturas detectaron etiquetas sobre diálogo: sorting ajustado detrás del HUD. Semilla de reproducción solo en pruebas, aleatoriedad normal intacta. Evidencias E05-* e informe de misión; sin canon definitivo o aprobación humana.


## 2026-10-31 — E06: regresión y demo Windows

HUD difiere refrescos reentrantes, Rename omite eventos idénticos y ESC usa InputAction con liberación al desactivar. Prueba de puntero/teclado con nombre enfocado e inventario a 720p/1080p aprobada; arnés oculto fuerza update Manual y restaura ajustes. Primer cierre detectó recursión al desactivar InputField; intentos siguientes aislaron foco/editor y no se atribuyen como fallos del player. Primera regresión 61/62 por expectativa errónea del nombre de Monje, corregida: repetición completa 62/62, cero omisiones. BuildVillage Windows x64 185.241.263 bytes/64,86 s. Probe independiente amplía recorrido real con las tres clases y persistencia de misión: headless 34,56 s y GPU RTX3050 33,64 s, ambos PASS/salida 0; RequireCargo exige comprobante nuevo y escena correcta. ZIP 77.833.297 bytes/305 entradas, CRC/binarios verificados y ocho históricos intactos. Capturas finales inspeccionadas, instrucciones y evidencia resumida versionadas. GDD/escena original/paquetes/configuración conservados; humano/artes finales pendientes. Preparado para publicar PR de feature hacia dev, sin main ni release final.


## 2026-10-31 — Publicación E03–E06

Commits 136fb87/50e860b/e7bc6f4/0f86fa9 por etapa, rama feature/e03-e06-personajes-vertical-slice publicada y PR #1 en borrador hacia dev: https://github.com/MathiasMerino/RPGRTS/pull/1. Checklist técnico E06 cerrado con evidencia; confirmación humana F2-07 y arte final permanecen pendientes. No merge a main/dev, release, force push o nueva fase. Cierre documental posterior sin cambios al código validado.


## 2026-10-31 — Inicio E07

PR #1 consultado: abierto, borrador y sin fusionar; rama feature/e07-aldea-medieval creada desde dab39c2. Revisión dirigida de GDD/arte/checklist/escena/biblioteca/misión/versiones; plan secuencial registrado. Cuatro fuentes de textura originales generadas con imagegen para piedra/madera/tierra/pizarra; todavía no se atribuye integración o aprobación visual. main y dev conservadas.


## 2026-10-31 — E07-P0 verificado

Línea base capturada con cámara real: doce vistas 1080p (tres clases, arquitectura, plaza, bosque, NPC, misión y RPG), física original y NavMesh registrados. Prueba nueva 1/1 y compilación salida 0; arnés respeta fin de combate real. Fuente/historial anteriores preservados; cinco albedos originales disponibles, sin assets externos.


## 2026-10-31 — E07-P1: arquitectura y superficies

Biblioteca de siete módulos y cinco conjuntos métricos, cubiertas inclinadas/aleros/cumbreras/chimeneas, colisión de edificios cerrados y grupos de recorte contextual separados. Trece materiales URP nuevos, cinco albedos originales importados a 1024/mips; mapas/recursos D01 y cuero/lino/hierro reutilizados, sin modificar originales. Generación salida 0 y dirigida de integridad/métrica/materiales/referencias 1/1 aprobada. Integración/revisión visual pendientes de P3/P4.


## 2026-10-31 — E07-P2: terreno y biblioteca de ambientación

Añadidos 21 prefabs propios de suelo/vegetación/utilería (total E07 33), con superficies compartidas, troncos sólidos y plantas blandas. Terreno mantiene Y=0 en el área jugable y limita relieve al fondo exterior; no se cambia NavMesh. Generación/compilación salida 0 y 2/2 dirigidas aprobadas. Detectadas coordenadas fijas de despliegue RTS y enemigo de misión, reservadas para la integración.


## 2026-10-31 — E07-P3: integración, iluminación y visibilidad

Misma AetherniaVillageDemo transformada con cuatro edificios/cobertizo, plaza, rutas, bosque y utilería. Originales D01/E03, física previa, NavMesh y coordenadas de misión conservados. Volume exclusivo, tarde cálida/relleno frío, SSAO existente y faroles sin sombras. RoofCutaway reutilizado para muros/techos/troncos/copas con restauración; comprobado contra triángulos y en trayectos reales. Agrupado follaje bajo por celdas tras medir 1.292 draw calls; muestra final 1.079 (−16,49 %), sin afirmar FPS. Añadidos bordes vegetales irregulares, fondo sin afectar mazmorra y texto direccional sin recorte por geometría. 5/5 dirigidas y 67/67 regresión aprobadas, cero omisiones. Expectativa nueva de llegada ajustada al contrato NavMesh existente, sin tocar pruebas anteriores/controlador. BuildSmokeProbe amplía únicamente la ruta explícita de validación para comprobar E07 y solicitar una captura del framebuffer GPU; validación en player pendiente.


## 2026-10-31 — E07-P4: evidencia y regresión

67/67 pruebas aprobadas con las 62 anteriores intactas y cinco nuevas, cero omisiones. Doce pares antes/después de cámara real, dos capturas de recorte y comprobante resumido versionados; integridad por hash/GUID/meta sin incidencias. Dirección artística y escala actualizadas sin modificar el GDD. Fuente de textura generada diferenciada de render del juego, perfiles originales conservados. Confirmación humana y arte final siguen pendientes.


## 2026-10-31 — E07-P5: Windows y captura independiente

Build final x64 195.637.067 bytes/43,71 s; headless 36,05 s y GPU RTX3050 35,59 s, ambos PASS/salida 0 con tres clases, misión/persistencia y sistemas anteriores. Captura real de player 1280×720 inspeccionada. Captura estándar/automática oculta no útil: se descarta; SingleCameraRequest URP explícita y comprobación de píxeles resuelven el diagnóstico, sin modificar pausa normal. Regresión 67/67 anterior al ajuste aislado del capturador, no se afirma repetirla; binario final recompilado y recorridos nativos repetidos. ZIP 83.358.761 bytes/305 entradas, CRC/binarios verificados, nueve históricos intactos. PR dependiente pendiente de cierre.


## 2026-10-31 — E07 publicado para revisión

Rama feature/e07-aldea-medieval publicada y PR #2 en borrador apilado hacia E03–E06: https://github.com/MathiasMerino/RPGRTS/pull/2. PR #1 reconsultado: abierto/borrador/no fusionado, cabeza dab39c2. E07-P0..P5 cerradas técnicamente con evidencia; revisión humana/artística E07-H y confirmación F2-07 pendientes. No merge, force push, release o inicio de E08. Historial de objetivos en informe E07; este cierre solo actualiza documentación.


## 2026-10-31 — Inicio E08

PR #1/#2 abiertos/borrador/no fusionados; rama feature/e08-personajes-animados desde 8f0108b. Auditoría dirigida y conceptos revisados. No Blender/bpy/modelos riggeados locales o clips disponibles; se evaluará autoría nativa Unity de skins/rig Generic/curvas reales, conservando pipeline Humanoid/proxies y sin instalaciones. Plan secuencial registrado; no se declara capacidad validada todavía.


### E08 — Autoría e integración incremental
- Tres mallas corporales continuas con ropa, rigs Generic válidos de 24 huesos, pesos/bindposes y trece clips originales (cuatro compartidos, nueve específicos); grafo E02 conservado, nuevo grafo común y overrides por clase. Catálogo E08 solo en la demo; proxies/escena original recuperables.
- Equipamiento existente a huesos, presentación de comandos aceptados/impactos/derrota sin alterar reglas, daño o guardado v1. Root motion y eventos de clip desactivados. Sin instalaciones ni descargas.
- Primeras pruebas 5/6; se detectó velocidad residual de llegada que mantenía Walk y retenía cues. Driver/presentador consultan también finalización real del trayecto. Capturas detectaron prendas enterradas y encuadre ocluido; ajuste de malla/pesos/ropa/anclajes conservando GUID y física. Repetición y cierre pendientes.


### E08 — 2026-11-01, refinamiento y verificación
- Refinamiento mediante APIs de Mesh para actualizar buffers CPU/GPU conservando GUID; tela subdividida adaptada al pecho, vendas visibles, normales coherentes, poses Idle de clase y anclajes corregidos. Fuente y proxies previos intactos.
- Caso reproducible de final de victoria: la vuelta automática a explorar podía borrar el golpe aceptado del mismo frame. Cue Completed conserva el último golpe pendiente sin retrasar recompensa, cambio de modo ni estadísticas; carga/nueva actividad siguen cancelando la cola. Prueba de aceptación añadida.
- Pruebas focales de siete estados y resultados reales de tres clases pasan. Primera regresión 73/73 (wrapper alcanzó el límite de diez minutos); segunda 72/73 por escritura de guardado tras mazmorra, con temporal válido conservado. Recontrol de ese mismo recorrido y capturas 2/2. LastWriteFailure aporta tipo/HResult/mensaje sin cambiar la operación atómica/esquema; se mantiene diagnóstico de la incidencia y se repite la regresión con límite explícito de quince minutos.
- MP4 de muestras articuladas empaquetados con FFmpeg local ya existente y decodificados con OpenCV; sin instalar/descargar software. Capturas de poses con luz de inspección, acciones integradas sin luz añadida; no confundirlas con evaluación humana ni medición de FPS.


### E08 — Verificación final y entrega local (2026-11-01)
- Regresión final 73/73, cero fallos/omisiones y salida 0. La incidencia intermedia de guardado no se reprodujo en recontrol, pasada final o player; causa concreta no confirmada y evidencia conservada.
- Build Windows x64 203.270.570 bytes; recorridos headless 54,77 s/GPU 55,46 s PASS con misión y animación de las tres clases y sistemas previos. Cuatro tomas reales del ejecutable inspeccionadas; 49 PNG/tres MP4 seleccionados y verificados en total.
- ZIP E08 local 86.334.027 bytes/305 archivos, CRC y hashes de cada archivo comprobados; diez paquetes históricos conservan SHA/tamaño. GDD SHA original y escena anterior/versiones intactos. Informes/checklist/instrucciones/fichas actualizados; publicación del PR apilado en curso, revisión humana/artística pendiente, sin E09.


### E08 — Publicación para revisión (2026-11-01)
- Rama feature/e08-personajes-animados publicada conservando historial E07. Commits de implementación 6242116, correcciones 53ee6ea, validación/evidencia b8e3861 e informe 4ca2b8c; cierre documental posterior.
- PR #3 abierto en borrador sobre feature/e07-aldea-medieval: https://github.com/MathiasMerino/RPGRTS/pull/3. #1/#2 continúan abiertos/borrador/sin fusionar; main/dev se mantienen. No release estable ni E09. Técnica E08 verificada, revisión humana/artística independiente pendiente.


### E09 — Inicio (2026-11-01)
- Auditoría dirigida: GDD/hash, especificación autorizada y combate/E08/HUD revisados. Git limpio inicial; PR #1/#2/#3 abiertos sin fusionar. Rama feature/e09-combate-rpg-ia desde a7acaca. Plan secuencial documentado; conservar balance, Generic, aldea y misión, sin E10.


### E09 — Turnos y presentación incremental (2026-11-01)
- TurnBattle existente extendido con fases, rondas y resultados únicos; política melee local separada, distancias/dados/balance preservados. HUD bloquea entradas durante Enemy; movimiento legal y revalidación LOS, sin daño desde animación. Commit4b21d4f del núcleo.
- Enemigo reutiliza visual/rig Generic E08; selección, Attack/Hit/Defeat, cola temporal de impactos, fallback de clips/perfil y panel contextual C02. Sin sistemas paralelos de ventanas/magia ni retargeting Humanoid. Se corrigió la barra sin sprite que no representaba el porcentaje visual.
- 14 WAV DSP originales, banco compartido, UI2D/mundo3D seis voces; ParticleSystem URP/material compartido y hasta8efectos. Recursos originales con manifiesto; sin descargas ni instalaciones.
- Focales11/11 reglas y16/16 ampliadas pasan. Misión real con cada clase: NPC/aceptación/ruta/dados/victoria/recompensa única/save-load. 120frames y audio DSP nativo no silencioso grabados; se revisa el encuadre antes de empaquetar. Fixtures collider hijo/estado capturado tras EnterPlayMode y APIs Unity6.5 corregidos; no se falsean resultados de reglas.
- Regresión final y prueba LOS físico adicional en curso; pendientes build/ZIP/PR y evaluación humana. La incidencia E08 de escritura se vuelve a observar durante los recorridos, sin atribuir causa raíz.


### E09 — Primera regresión y corrección de interfaz (2026-11-01)
- 88/89 pruebas: el caso integrado Monje detectó el nuevo botón Select real enemy de26px bajo el mínimo32px exigido por MedievalHudTests. Se aumenta el área del botón/texto a32px mediante refinamiento puntual de escena; no se debilita la prueba. Los88 casos restantes, incluida LOS física, guardado y tres clases E09, pasaron. XML intermedio preservado.
- El lanzador alcanzó15min y Unity terminó después; resultado del runner conservado, no se tomó el timeout por PASS. Repetición final con límite20min.
- Inspección del vídeo inicial detectó oclusión/ataques bloqueados por el entorno. La toma y sonda específica se trasladan al claro legal junto al cargamento; la grabación exigirá daño real de ambos combatientes, sin forzar vida/dados. No se mueve ni elimina geometría de la aldea.


### E09 — Recontrol de UI y grabación (2026-11-01)
- Monje integrado pasa con área de selección32px. La toma adicional falló su nueva aserción de daño: el guion de captura intentaba atacar en frames fijos mientras Enemy aún estaba activo y saltaba acciones legalmente. Se corrige solo el guion para esperar fase/acción reales; no se alteran dados, plazos ni HP del juego. XML intermedio conservado.
- Grabación corregida1/1 PASS y salida0:120frames reales, rival11/18PV/héroe18/24 al final, audioDSP estéreo48kHz no silencioso (RMS0.005859, pico0.076390). MP4 H264/AAC decodificado120frames y audio;18PNG empaquetados provisionalmente hasta capturas finales.
- Fallback enemigo sin perfil conserva cápsula/reglas y se añade al caso integrado. Repetición89 con límite20min en curso; el lanzador adopta20min por defecto tras observar15min insuficientes para esta batería. No se fuerza cierre de Unity.


### E09 — Regresión final (2026-11-01)
- 89/89 PASS, cero fallos/omisiones, salida0; XML final E09_REGRESION.xml. Focales y errores intermedios conservados. Selección32px/mesh HP, IA/LOS/pasos/fases/daño único, clips ausentes y perfil enemigo nulo, misión legal con las tres clases, recompensa única, guardado y sistemas previos verificados.
- No reaparece la escritura transitoria E08 en esta pasada; no se declara causa raíz resuelta. Pendientes humanos/artísticos, retargeting Humanoid y restricciones de armas por clase conservados. Commit audiovisual c545386; build Windows y sondas en curso.

### E09 — Build, recorridos y evidencia final (2026-11-01)
- Windows x64 Development203799042bytes, compilación85.00s y salida0. Recorridos independientes sin gráficos90.81s/con RTX3050 90.60s PASS, salida0; NPC/misión/tres clases/recompensa/v1, animación/daño/derrota, inventario/construcción/RTS y nueva sonda por fases. Audio real pico0.076364 en ambos. Sin excepciones/avisos de agente inválido detectados.
- 19PNG originales y MP4 de120frames/diez segundos con DSP estéreo48kHz empaquetados y decodificados. Barras/turnos/cámara/poses y captura nativa inspeccionados. Sin pose sampling ni métricas FPS inventadas; memoria nativa y working set muestreado documentados, GC por frame no perfilado.
- ZIP86756445bytes/306archivos, SHA11d3db3e940f286ef79ff7c2f52dcee3919f2a9accee4a7226b68d9cf2c17a79, CRC/hashes de cada archivo PASS; once ZIP previos intactos. GDD/escena original/recursos E07/E08/versiones/guardado preservados. Informe/checklist/guía/AGENTS actualizados; publicación en borrador pendiente, sin aprobación humana ni E10.

### E09 — Publicación y cierre técnico (2026-11-01)
- Rama feature/e09-combate-rpg-ia publicada. PR#4 abierto en borrador sobre feature/e08-personajes-animados: https://github.com/MathiasMerino/RPGRTS/pull/4. Main/dev conservan sus SHA iniciales; PR#1/#2/#3 abiertos/borrador/sin fusionar. Sin release estable, force push ni E10.
- Commits4b21d4f (turnos/reglas/IA),c545386 (presentación/recursos),a07dbd5 (pruebas/sondas),04a4799 (entrega/evidencia), más cierre documental. Seis tareas técnicas E09 completadas y verificadas; E09-H/F2-07/E07-H/E08-H y recursos finales pendientes. Sincronía HEAD/origin/PR y estado limpio comprobados al concluir.


### E10–E13 — Inicio autorizado
- Auditoría dirigida del nuevo prompt completo, GDD/convenciones y sistemas reutilizables; E09 real89/89, PR#1–#4 abiertos/borrador, main/dev intactas, base66cf80f limpia. Rama feature/e10-e13-demo-jugable creada sin merges.
- Plan secuencial y criterios añadidos; núcleo E10–E13 prioritario, E14/E15 condicionales, sin aprobar automáticamente lore/bonos/arte/humanos.


### E10 — Menú y tipografía, checkpoint editor
- Nueva escena AetherniaFirstDemo derivada de E09, originales intactos; prefab reutilizable de front-end, TMP SDF2048 y LiberationSans/OFL incorporada desde el paquete instalado. Ajustes reales persistentes, créditos, Continue/carga válida, salida segura en editor; datos de raza/intro y slot separado con respaldo.
- 3/3 focales y compilación exacta PASS; capturas reales720p/1080p/1440p/4K, creación/prólogo/créditos, textos y raycasts comprobados. Corregidos errores iniciales de firma C#/API TMP y recuperación de importación asíncrona de esenciales antes de -quit. Editor validado; ejecutable/flujo integral y humanos pendientes.


### E11 — Prólogo, combate y control directo verificados
- Primer encuentro E09 conectado a prólogo omisible; mismo estado/clase/linaje y equipo, victoria real activa exploración y guarda progreso; Continuar omite introducción completada. WASD relativo al agente, diagonal/cápsula/NavMesh y animación/cámara; E próximo, pausa y foco de UI. Escenas/modelos anteriores intactos.
- Focal2/2 PASS con las tres clases y teclado InputSystem. Primeras pasadas detectaron en QA el buffer Editor sin foco: fixture temporal AllDeviceInputAlwaysGoesToGameView/IgnoreFocus, restaurada en finally; ninguna aserción debilitada ni HP/victoria forzados. XML intermedio conservado; capturas de exploración reales. Siguiente RTS/héroe y economía; Windows/regresión global/humanos pendientes.


### E12 — Patrulla y héroe compartido verificados
- RtsBattleController existente ampliado opt-in;6v6 clásico y escalada preservados. Misma unidad/modelo/estado del héroe, tres roles militares, rangos/LOS, selección/cajas/Shift/grupos, órdenes y acciones con restricciones. Datos SO/controles/UI/feedback separados; helpers de daño/guardia conservan reglas RPG. NPC Capitán activa batalla; victoria real habilita campamento y guarda progreso, derrota ofrece reintento.
-12 reglas E09 PASS; patrulla1/1 final con Guerrero/Monje/Mago desde RPG legal hasta victoria RTS/save/continue. Fallo intermedio del Mago conservado: al guardar distancia/guardia se detienen unidades en rango incluso durante cooldown; se usan ración y recuperación legales. Sin HP/victoria forzados. Seis capturas reales, barras compactas y Generic/arco piloto. Próximo E13, limpieza de widgets de batalla y QA integral/Windows.


### E13 — Economía y asentamiento verificados
- Cinco tipos funcionales con catálogo SO, colisión/NavMeshObstacle3x3 y presentación modular E07; centro/requisitos, producción activable10s online, almacén/capacidad, mejoras, cuartel/reclutas pagados6máximo. Version1 aditiva para nivel/producción/ejército/linaje; no offline/duplicación al cargar, legado migrado. Reclutas se despliegan y conservan salud/bajas; práctica no genera otra recompensa. Liberación de widgets/grupos RTS entre encuentros.
-6/6 editor PASS, cinco casos de reglas y fixture económica real de colocación/producción/free/upgrade/recluta/deploy/continuar. Primera5/6 por observación del progreso antes del Update siguiente, condición conservada y espera finita corregida. XML/capturas guardados. Pendiente QA integral desde menú, Windows/reinicio/regresión global/evidencia/PR; humanos no aprobados.
- Nuevo MonkMeshy FBX/meta ajeno preservado fuera de estos commits e integración; no sobrescribir ni publicar procedencia no auditada.


## 2026-11-01 — QA E10–E13: colisión, opciones y primera build

- Focal de colisión1/1 PASS: teclado real, muro físico/retirada y opciones720p–4K; primer fallo de preparación (encuentro activo) preservado en XML, sin marcarlo como fallo del producto.
- Revisión de captura detectó relleno/tirador Slider fuera del rectángulo: corregidos anclajes compartidos y añadida aceptación geométrica, evento de volumen aplicado/persistido con clave aislada. Pausa oculta/restituye HUD.
- Primera BuildFirstDemo PASS; smoke Windows inicial pasa menús/prólogo/RPG/movimiento y falla observación de animación solo al detenerse ante obstáculo. Ajuste de ventana de observación pendiente, comprobante preservado; no se declara end-to-end aprobado.
- Lanzada regresión102. Nueva guía humana y controles documentados; original GDD/hash y escenas anteriores sin cambios. Doce ZIP históricos inventariados con SHA. GitHub main/dev y PR#1–#4 sin cambios; modelo MonkMeshy ajeno preservado fuera de commits.


## 2026-11-01 — Compatibilidad v1, reemplazo atómico y opciones nativas

- Regresión102:91/102. Reproducido JsonUtility convirtiendo Demo nulo en objeto; marcador HasDemoProgress derivado al guardar y normalización legada antes de validar. Sin cambiar Version1 ni GDD. Focal de carga/progreso y misión aprobadas; XML intermedios conservados.
- Regresión103:101/103. Dos fallos de reemplazo de JSON antiguo; HRESULT1175 (0x80070497) documentado. AtomicFileReplacement repite File.Replace para32/33/1175 hasta4reintentos50/100/200/400ms, sin borrar destino ni fallback no atómico; permisos permanentes no se reintentan. Diagnóstico LastWriteRetries. Focal12PASS y bloqueo Windows real+derrota/reintento2PASS.
- Opciones verificadas en nuevos procesos: master.63/efectos.57/cámara14/resolución1920x1080 y cambio/restauración de calidad. NativeKey de prueba separada, borrada tras continuar. Pantalla guardada se aplica al arrancar el player; índice del selector se sincroniza con la resolución cargada.
- Prueba nativa detectó referencia Audio nula del prefab de menú: corregida como override en la escena nueva mediante Controls seguro y aceptación de referencia persistida. No se alteró la escena histórica. Los dos FAIL de resume anteriores se preservan.
- Build actual y12procesos Windows finales PASS con tres clases, fondos normales, producción/recluta, guardado y reinicio; omitir prólogo probado con Mago. Capturas/video DSP nativos renovados; revisión humana no realizada. Regresión110 en curso, no se declara cerrada todavía.


## 2026-11-01 — Regresión110 aprobada y visibilidad del campamento

- Regresión global110/110 PASS, cero fallos/omisiones. XML DEMO_REGRESION_110.xml. No sumar los focales superpuestos como si fuesen casos distintos.
- Captura real identificó edificio nuevo ocultando al avatar. WorldConstruction.Spawn reutiliza RoofCutaway para la campaña; oculta contextualmente el conjunto visual agrupado por materiales de E07 y lo restaura al dejar de interponerse. Física/colisiones/NavMeshObstacle quedan activos. Guardado legado conserva la ruta anterior equivalente.
- Focal posterior6/6 PASS: oclusión/retirada de hastiales y vigas, restauración, bounds físicos, producción, reclutas, despliegue y carga. Primeros intentos preservados (fixture de cierre del IEnumerator y nodo Roof sin malla); captura nativa posterior detectó hastial aún visible y se amplió aceptación a todo el conjunto, sin cambiar reglas.
- Build posterior correcta; repetición Windows final y actualización de paquete/evidencia en curso. Evaluación artística/humana sigue pendiente, al igual que E14/E15.


## 2026-11-01 — Entrega técnica E10–E13 preparada

110/110 global y6/6 posteriores al recorte final;12procesos Windows finales con/sinGPU y2desdeZIP extraído PASS. Captura confirma avatar visible con retiro contextual reversible del conjunto visual, física intacta.18PNG originales, MP4nativo33.917s con DSP real; sin simulación de poses ni benchmarkFPS. ZIP306archivos/87411924bytes/CRC y SHA individual PASS;12ZIP históricos conservados. GDD/hash, escenas históricas, paquetes y configuración de proyecto preservados tras finalizar Unity. Informe Docs/ENTREGA_E10_E13.md, guía humana/COMO_JUGAR y fuentes de evidencia actualizados. Publicación/PR en cierre; E14/E15 y revisiones humanas no iniciadas/completadas automáticamente.


## 2026-11-01 — Publicación E10–E13

Rama feature/e10-e13-demo-jugable publicada mediante push normal; PR#5 abierto draft sobre feature/e09-combate-rpg-ia (PR#4 todavía draft abierto): https://github.com/MathiasMerino/RPGRTS/pull/5. Main5a2e3cef ydev742c04f6, predeterminadamain yPR#1–#4 contrastados sin cambios. Conservados los commits porfase ycorrecciones91d83b7/238d9e2; documentación de cierre en commitposterior. ChecklistQA técnica completada, revisiónhumana yE14/E15 pendientes. Git propio sin cambios; archivos ajenosMonkMeshy preservados sin seguimiento/no publicados. No merges, forcepush ni cambios globales.


## 2026-11-01 — Inicio E14–E15 autorizado

Auditoría acotada, GDD/hash yPR1..5 verificados (todosabiertos/draft). Rama nueva desdec33e34d, sin merges/main/dev. Plan/reglas provisionales y tareas dependientes en Docs/E14_E15_EXPEDICIONES.md yCHECKLIST. Eliminaciones locales ajenas de3texturas yFBXMeshy preservados; consulta pendiente, sin sobrescrituras. Se inicia estado/reclutamiento antes de sistemas dependientes.

### E14 — grupo y combate verificados (2026-11-02 UTC)
- Restauradas por autorización expresa las tres texturas E07; FBX Meshy ajeno intacto. Estado aditivo de compañeros, roles existentes, reserva e identidades persistentes.
- Iniciativa individual y acciones legales enlazadas a salud compartida; seguimiento y dos encuentros reales en nueva escena, originales preservados.
- QA:23/23 pruebas relacionadas;8/8 reglas nuevas;1/1 integración completa tras reproducir y corregir bloqueo por destinos ocupados. XML intermedios conservados; no se ocultan fallos. Cancelar movimiento/terminar turno no reembolsa metros. Validación Windows/E15 pendiente.

### E14 — mazmorra e integración económica
- Nueva escena AetherniaExpeditions, originales preservados; ruinas24×16m, dos encuentros, alijo, recompensa única y salida. Materiales/colisiones/roles existentes; suelo con UV por metro y recorte visual frontal sin cambiar barreras físicas.
- Tres recorridos Windows con partidas E13 reales, más tres reinicios independientes, PASS. Recursos del cofre pagan compañeros; recompensa de ruinas paga granja/ejército. Sin salud o fondos añadidos por el probe. Focales de composición reducida y derrota2/2 PASS.
- Seguimiento/representación compartida, controlador RPG ampliado y despliegue RTS seleccionable; patrulla anterior distingue ContextId y conserva sus reglas. Pausa/carga cancelan descanso del estado anterior. La revisión de capturas detectó recortes de etiquetas/orden visual del panel: corrección local pendiente, registrada sin aprobar visualmente la entrega.

### E14–E15 — pulido visual verificable
- Panel de grupo lateral; textos compactos completos, capa encima del HUD, cierre al abrir ficha/inventario/ayuda y adaptación4:3. Propietarios usan nombres de facción, marcadores actualizan propietario e instrucciones distinguen conquista/patrulla.
- Cámara de combate centra al héroe; recorte de paredes frontales/divisiones cercanas evita ocultarlo. Colliders originales3m y SHA del NavMesh conservados. Nueva escena únicamente; no se regeneraron autorías históricas.
- Regresión121/121 PASS; tras cambios de presentación11/11 focales PASS y1/1UI final con ocho vistas a1280×720/1024×768, límites/raycast/textos, ventanas históricas y héroe fuera del HUD. Build exacta recompilada; recorrido GPU final y portable en ejecución. Evaluación humana no realizada.

### E14–E15 — build, evidencias y portable finales
-20procesos Windows PASS:6headless,6GPU previos al pulido,6GPU finales y2desde ZIP extraído; Guerrero/Monje/Mago, economía pagada, dos victorias RPG, conquista real, producción y reinicio conservados. Los8procesos finales no presentan excepciones, agentes inválidos ni avisos JobTempAlloc observados en algunas ejecuciones anteriores.
-22PNG nativos+2PNG Play Mode para geometríaUI;2clips continuos23.8s/8.1s conaudioDSP real y decodificación/SHA verificados. Captura12Hz no implica FPS medidos. NavMesh/GDD/escenas originales/paquetes intactos; escalas y colliders conservados.
-ZIP nuevo87,458,324bytes, SHA256958df84773407fe81274c366588ba60e97b4fe19feca69763c37be38912baf36; CRC/extracción/SHA individual PASS,13ZIP anteriores intactos. Guía, partidas E13 reales y lanzadores separados incluidos. No autoriza arte final, balance o E16. Meshy ajeno sin incorporar.

### E14–E15 — publicación y cierre técnico
- PR#6 abierto en borrador sobre feature/e10-e13-demo-jugable: https://github.com/MathiasMerino/RPGRTS/pull/6. Main/dev/default y PR1..5 conservados; sin merges ni push forzado. Historial desde c33e34d preservado.
- E14-A/B/C, E15-A/B y EXP-QA completados técnicamente; EXP-H humano/arte/balance permanece pendiente. Cambios propios confirmados; Meshy ajeno sin versionar y excluido del PR. README/guía/entrega/recuperación actualizados.

### E16 — auditoría mínima y autorización
- Encargo visual leído; se revisaron dirección artística, resumen/checklist, UI/TMP/catálogos y autoría de expediciones. PR1..6 abiertos/borrador; snapshot y hash Meshy conservados. Rama nueva desde4efcd00, sin merges.
- Plan acotado en Docs/E16_PULIDO_VISUAL.md: reutilizar tres hojas, pulir presentación modular en escena nueva y validar; no alterar reglas/guardar/rigs ni iniciar otra fase. No plugins necesarios.

### E16 — menú, clases y presentación integrada
- Perfil opcional, tres tarjetas/retratos de hojas existentes, atlas320×64 y9prefabs compartidos; UI histórica conserva bindings. HDR/URP regional, FXAA,3variantes de material y decoración sin colisiones nuevas.
-6/6base y3/3focales PASS tras corregir recorte «Personaje», pivote, aviso y botones sobre ficha; firma física intacta.26capturasPlayMode en3resoluciones. Se corrigió iluminación demasiado oscura con relleno local moderado; no altera reglas o tiempo. BuildWindows en ejecución, aún no se declara validada.


## 2026-11-02 — E16: correcciones observadas en QA

- Captions del prefab adaptables al ancho real (botones grupales de 88–148 px), SDF legado sin doble dibujo, selección centrada y aviso de menú separado.
- Barras de salud conservan su color sin el sprite oscuro de fondo. Separadores RTS restaurados en UTF-8; etiquetas del mundo E16 se dibujan detrás del HUD para no tapar órdenes.
- Pase intermedio global: 124/125 por redondeo de altura 31.999939 frente a 32 px. Tolerancia explícita de 0.001 px en el test, sin reducir el botón. Se conservó el XML del fallo; comprobación dirigida posterior 7/7 y regresión repetida 125/125 PASS.
- Doce procesos nativos GPU r2 PASS para tres clases, menús/RPG/RTS y expediciones/conquista/reinicio. Revisión final del orden de dibujo en curso; no confundir estos comprobantes intermedios con la aprobación humana o arte definitivo.

- Tras el ajuste de capas: 9/10 dirigidas; el Mago perdió una patrulla del test histórico. Repetición aislada 1/1 PASS con las tres clases y sin cambios de reglas/recursos. Ambos XML conservados; variación del recorrido automático RTS registrada como limitación pendiente de estabilizar, sin ocultarla mediante reintentos o estadísticas nuevas.


## 2026-11-02 — E16: build r3 y evidencia final

- Menú y tres tarjetas de clase con hojas existentes; nueve prefabs, atlas compartido y SDF. HUD/ventanas y captions corregidos. Nueva escena AetherniaVisualPolish, atmósfera regional, variantes URP y decoración sin modificar física/reglas/escenas previas.
- Build Unity6000.5.8f1 PASS (205,946,388bytes informados). Doce procesos GPU r3 PASS: tres clases, menú/creación/RPG/RTS/campamento y expedición/reclutamiento/ruinas/conquista, con reinicios. Dos procesos desde ZIP extraído PASS. Recibos, métricas y capturas exportados con origen/SHA:57PNG reales.
- ZIP r3 de87,478,209bytes; SHA2568c3b39c6b2747d9ba9649740c6f862a604c2666b5ff272d10cb428c46ac29737. CRC y314archivos extraídos verificados;16ZIP anteriores intactos. Build y ZIP excluidos de Git; código/assets/evidencia incluidos.
- Se revisaron capturas técnicamente; ninguna sesión manual humana nueva ni perfil FPS/GC. E16-HUM y variación RTS E16-QA-RTS siguen pendientes. GDD, originales PNG, Nav, versiones/paquetes, escenas anteriores y Meshy ajeno conservados. Guía/informe/checklist actualizados; publicación en borrador en preparación.

- Publicación E16: [PR #7](https://github.com/MathiasMerino/RPGRTS/pull/7) en borrador sobre feature/e14-e15-expediciones-conquista; push normal, historial conservado. Main/dev y PR1..6 sin cambios, sin merges. Checklist técnico/documental E16 cerrado; valoración humana y variación del test RTS permanecen pendientes.

## 2026-11-02 — Inicio E17 autorizado

PR7/base b0afdc2 comprobados; rama nueva desde E16 sin merges. Seis bloques secuenciales; prioridad diagnóstico RTS con trazas. Meshy experimental local intacto y excluido de publicación hasta aprobación; no iniciar E18.

## 2026-11-02 — E17-A diagnóstico y contrato de resolución

Cinco escenarios baseline4/5, derrota real por héroeHP0 con ejército vencedor. Trazas verifican recuperación aceptada, resolución y transición; no se ha probado evento perdido. Snapshot LastResolvedVictory conserva el resultado tras Clear y se reinicia al aceptar otra batalla. Contratos en cinco contextos verificados (4/5+luego1/1tras corregir limpieza de fixture), sin quitar aserciones históricas. Se conserva evidencia de errores de prueba propios y del fallo real. Expectativa de victoria automática pendiente; no se modifican reglas para forzarla. Continúa B.

## 2026-11-02 — E17-B pipeline y estudio local

Contrato visual y fábrica reutilizados por héroe/apoyos, validación de malla/material/escala/rig/clips y fallback antes de sustituir el modelo. Catálogo Alpha comparte prefabs/materiales E08.14/14+1/1 funcionales con tres clases y persistencia. Meshy inspeccionado solo mediante copia local:99.466tris/222.717vértices,1submesh/material,UV0ausente,sinrig/Avatar. Normalización de inspección1.78m; original/meta intactos. Arte experimental y derivados excluidos de Git; no activados en héroe. Guía de importación actualizada. Continúa C.

## 2026-11-02 — E17-C menú, clases y tipografía

Layout opcional Alpha y preview3D reutilizando catálogo/fábrica, sin modificar escenas previas ni estado de selección antes de confirmar. Persiste overrides de prefab; se verifica fuente dinámica/glifos españoles sin reemplazar fuente.2/2pruebas,20vistas reales720p–4K, proporciones/límites/raycast/overflow, física y dependencias conservadas; preview desactivado durante gameplay. Capturas originales inspeccionadas técnicamente; arte/rig y valoración humana pendientes. Continúa D.

## 2026-11-02 — E17-D superficies y mundo

Shader Graph Lit URP17.5 nativo, variación de terreno, mezcla de bordes, normales originales y viento leve en dos materiales de hojas;15variantes compartidas/377asignaciones. Volumen propio Neutral, bloom reducido y luz de apoyo existente ajustada. Los marcadores de viaje ya no ocupan el punto de aparición. Sin cambios en física/nav/escenas anteriores.3/3pruebas GPU/compilación/viaje y1/1 posterior al ajuste, capturas reales. Continúa E.

## 2026-11-02 — E17-E integración visual

Reproducidos y corregidos ocultamiento de indicadores RTS al cambiar modelo (7/7→0/7) y solapamiento construcción/ayuda. Panel RTS lateral solo Alpha. Decoración sobre HUD existente con avisos/fundido0.18s y suscripciones liberadas; sin estado nuevo de gameplay. Cierres reales de automatización por puntero/ESC: ficha, inventario, sesión y grupo. Pase final2/2, XML de reproducción preservado. Evaluación humana pendiente. Continúa F.

## 2026-11-02 — E17-F preparación de validación

Bajo/Medio/Alto sustituyen nombres históricos Mobile/PC mediante tres assets URP propios (originales conservados). Diferencias reales de escala interna, MSAA, sombras, luces, AO/post;2/2dirigidas PASS. Medidor nativo solo con argumento explícito, captura existente reutilizada y opciones/partida aisladas; métricas aún no recogidas. Herramientas de ZIP/evidencia reutilizadas con parámetro de fase conservando E16 por defecto. Regresión/build/recorridos/portable pendientes; no se declara alfa entregada todavía.

## 2026-11-02 — E17-F regresión y compatibilidad fallback

General142/143,0omitidas; XML conservado. Único fallo antes de movimiento: prueba E01 esperaba cápsula aunque E17-B conserva un modelo aprobado válido. Expectativa actualizada con comprobaciones más fuertes de identidad/estado y cápsula cuando también se invalida el modelo cargado; equipo/movimiento/guardar/cargar conservados. Dirigidas5/5PASS. No cambia runtime de gameplay ni pruebas RTS;14victorias trazadas no cierran el pendiente inicial. Medidor explícito ampliado aRTS pausado (18escenarios previstos); compilación/nativo pendientes.

## 2026-11-02 — E17-F build y perfilado nativo

Build final fuente0802286, Unity6000.5.8f1,206493085bytes,72.39s(incremental; primer build777.24s). Medición inicial inválida porque batch omitía render por cuadro; JSON conservado sin usarlo como FPS. Medidor opt-in corregido a peticionesURP explícitas de escena/UI/preview, targetMSAA1/2/4 y guardas de render real;18escenarios PASS con SetPass/triángulos/CPU/GC, DrawCalls/Batches no obtenidos con aliases consultados. RTX3050Laptop/i5-12450H, datos/limitaciones en PERFORMANCE.json. No FPS de presentación ni extrapolación a grandes ejércitos. Seis procesos demoGPU PASS, expediciones y portable en curso.

## 2026-11-02 — E17-F entrega técnica

14/14procesosWindows PASS,12demo/expedición por tres clases y2portable.82PNG reales con manifiesto, dosMP4 continuos con DSP/decodificación verificada. ZIP87.816.123bytes, SHA2564c16177bc696334549eebc942f21fc6ebcd63b151fafac79eb7d9910d4b6e87d; CRC/per-fileSHA/extracción y17ZIPanteriores intactos. Informe/guía/comparativa/artedocs/checklist actualizados. SourceRuntime0802286; metadatos de prefiltrado normalizados por Unity solo en assets URP propios. Arte humano, UX/balance y estrategia RTS pendientes; no E18. Publicación PRdraft final en preparación.

## 2026-11-02 — Publicación E17

[PR#8](https://github.com/MathiasMerino/RPGRTS/pull/8) abierto en borrador sobre feature/e16-pulido-visual-ui (PR7 todavía abierto). Push normal de feature/e17-art-pipeline-alpha-polish; main/dev y PR1..7 comprobados contra snapshot inicial, intactos. F técnico/documental cerrado con sus resultados reales, E17-A-RTS y E17-HUM siguen pendientes; sin merges/force, sin Meshy/derivados publicados y sin E18.
