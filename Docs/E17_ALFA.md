# E17 — candidata alfa de desarrollo

Base b0afdc2 (E16, PR7 abierto). Rama feature/e17-art-pipeline-alpha-polish. Encargo8693f0cc autoriza E17; no E18. Reglas/GDD/versiones y escenas históricas conservadas. Sin subagentes ni herramientas nuevas.

Secuencia aprobada: A diagnóstico RTS con trazas y repeticiones, B pipeline/fallback y análisis local Meshy, C composición y UI a720p/1080p/1440p (4K si viable), D terreno/materiales/shaders URP e iluminación, E transiciones/contextos y combate, F regresión/rendimiento/capturas/build/ZIP/PRdraft.

Estado inicial: A en desarrollo, B–F pendientes. No se declara resuelta la intermitencia por repetir. Meshy experimental: análisis y escena aislada locales; no publicar FBX, derivados ni incorporarlo al héroe sin aprobación. Arte y evaluación humanas pendientes.


## A — diagnóstico inicial

Lote baseline5escenarios (tres clases en secuencia con cap solicitado default/30/120 y Mago aislado30/120):4/5aserciones de victoria aprobadas, diez batallas observadas de once previstas (se detuvo la secuencia30enMonje). Hubo nueve victorias y una derrota real; la recuperación se aceptó una vez en cada batalla. La derrota termina conHP0, cinco aliados y cero enemigos; se emite Resolved(false) y la transición de derrota correcta. No hay evento perdido en esta reproducción. La hipótesis inicial de recuperación nunca aceptada queda descartada para este lote.

La carga idéntica no garantiza que sobreviva el héroe con trayectorias NavMesh/temporización distintas; no se cambia balance ni se adjudica victoria al ejército con héroe muerto. La prueba histórica mantiene sus aserciones de victoria. Se añaden comprobaciones separadas de resolución/derrota/reintento con cinco escenarios, sin exigir ganar todas las cargas. Esto no se presentará como corrección de la expectativa intermitente de victoria. XML y traza reducida baseline conservados. Revisión de estrategia/balance y determinismo físico pendientes; no se concluye una causa única del resultado E16 sin su traza.


Pase de contratos intermedio1/6: cinco fallos de la nueva prueba por consultar Won tras Clear(), aunque la victoria ya había devuelto la exploración. Error de comprobación reconocido, XML conservado. Se usa el evento emitido y se conserva LastResolvedVictory hasta la siguiente batalla aceptada, útil para presentar el resultado tras la transición. No se atribuye retrospectivamente este problema de prueba nuevo a E16. Se verifica evento único, snapshot y estado; se mantienen las aserciones históricas y las trazas de derrota real.


Contratos posteriores4/5: el caso30validó derrota/reintento pero su limpieza intentaba volver a explorar durante un RTS activo; la siguiente creación era rechazada correctamente por IsBusy. Se corrige únicamente la limpieza del fixture con Clear() después de comprobar el reintento y se exige que el nuevo intento no conserve un resultado anterior. XML conservado; repetir soloThreeClasses30. No se reduce ninguna aserción de combate.


Cierre A: cuatro contextos de contrato aprobados en lote y ThreeClasses30 aprobado aisladamente tras reparar solo limpieza del fixture (RTS_CONTRACT_30_FINAL.xml). Se verifican selección simple/múltiple, órdenes, héroe, resolución única, retorno/victoria y derrota/reintento reales. La prueba histórica de victoria conserva sus aserciones. LastResolvedVictory es un snapshot de presentación, sin cambios de daño, salud, costes, cadencia, NavMesh ni guardado. No se declara corregido el resultado intermitente de la carga automática; sigue pendiente revisar estrategia/balance/determinismo, con derrota real reproducida y evidencia. Continúa B sin bloquear arte por esta expectativa pendiente.


## B — pipeline verificado

Contrato/fábrica compartida para héroe y apoyos; candidatos inválidos o sin animación compatible recurren a la clase anterior, conservando estado/controlador. El modelo ya cargado sigue visible si se pierden referencias.14/14 de pipeline/animación/Humanoid más1/1 ampliado de fallback con tres clases, movimiento y guardado. Catálogo Alpha sin dependencias Meshy/LocalOnly. [Guía y ficha del recurso](E17_PIPELINE_PERSONAJES.md). Estudio/captura privados generados; datos técnicos en Validacion/E17/MESHY_DATOS_TECNICOS.json. Originales FBX/meta intactos. No autorización artística final ni publicación de derivados.


## C — presentación verificada

Nueva escena AetherniaAlphaCandidate conserva E16. Tres opciones de clase verticales con retrato/realce, ilustración de referencia y preview3D del modelo E08; nombre/linaje/confirmación con comandos existentes. Previews aislados sin sesión/agentes/controladores, se apagan al jugar; la composición no altera estado. Monje sin arma visible y Mago con su bastón en la vista previa, sin tocar inventario. La pose/arte/rig siguen provisionales.

2/2 pruebas aprobadas con20capturas reales a1280×720/1920×1080/2560×1440/3840×2160, comprobación de límites/raycast/overflow y captura4K vista en resolución original. Se corrigió persistencia de overrides de prefab. La fuente SDF existente es dinámica: glifos españoles se comprueban con resolución/fallback y generación real, no exigiendo que estén precargados. No se regeneró fuente/atlas ni se modificó su asset. F2-07/humano pendiente. Contextos de otros paneles se revisan en la integración E.

D verificado: Shader Graph Lit17.5 editable,15materiales compartidos, normales originales, bordes de camino y variación procedural. Viento GPU0.035m, límites visuales restaurables; sin cambio de colliders. Neutral/contraste5/saturación−5/exposición+0.08/bloom0.04. Marcadores entrada/salida separados1.2m de spawn y luz de apoyo2.8.3/3+1/1 PASS; World capturas reales inspeccionadas.
