# E17 — informe técnico de revisión

Rama feature/e17-art-pipeline-alpha-polish, base E16 b0afdc2 / PR7 en borrador. Unity6000.5.8f1 y URP17.5.0 conservados; GDD original sin cambios. No E18 ni nuevos plugins. Este informe distingue pruebas automáticas e inspección técnica de capturas de la evaluación humana, que sigue pendiente.

## Estado por bloque

| Bloque | Resultado técnico | Evidencia |
|---|---|---|
| A | Diagnóstico y contratos comprobados; expectativa de carga ganadora pendiente | Validacion/E17/RTS* |
| B | Fábrica/contrato de visual y fallback integrado; Meshy aislado | E17_PIPELINE_PERSONAJES.md, PIPELINE*, MESHY_DATOS_TECNICOS.json |
| C | Menú y selección Alpha, tres retratos y preview3D reutilizado | UI_FINAL_2.xml;20vistas720p–4K |
| D | ShaderGraph Lit, superficies, viento y ruinas | WORLD_3.xml y WORLD_FINAL_1.xml;SURFACES.json;WIND_GPU.txt |
| E | Selección RTS y paneles corregidos, avisos/fundidos cortos | EXPERIENCE_REPRODUCTION.xml;EXPERIENCE_FINAL_2.xml;WINDOW_INPUT.txt |
| F | Perfiles dirigidos2/2; regresión142/143; compatibilidad de prueba fallback5/5PASS; build/medición/14procesosWindows/ZIP verificados | QUALITY_2.xml; resultados finales por registrar |

## RTS: causa observada y límite de la conclusión

Cinco escenarios iniciales controlaron orden de clases y caps solicitados default/30/120, además de Mago aislado30/120:4/5aserciones de victoria, diez batallas observadas, nueve victorias y una derrota real Monje30. El héroe acabó con0PV pese a sobrevivir cinco aliados y quedar cero enemigos; resoluciónfalse correcta. Recuperación aceptada una vez por batalla. Las trazas no reprodujeron evento perdido ni inicialización defectuosa. La trayectoria NavMesh y temporización varían; no existe evidencia histórica suficiente para atribuir una causa única al Mago E16.

La prueba histórica conserva su estrategia y aserciones de victoria. Nuevos escenarios comprueban evento único, estado coherente, selección/grupos/órdenes, derrota/reintento y retorno reales. Se repararon errores propios del nuevo fixture (consultar Won después de Clear y limpiar un reintento todavía activo); XML intermedios conservados. LastResolvedVictory mantiene un snapshot hasta el siguiente Begin aceptado. No se cambió salud, daño, cadencia o recompensa. E17-A-RTS y E16-QA-RTS permanecen abiertos: no se promete una carga ganadora universal ni se cierra por un reintento.

## Integración artística

CharacterVisualFactory y CharacterVisualContract son comunes a héroe y apoyos. Prioridad: reemplazo revisado optativo → humanoide E08 → prefab actual/anterior → cuerpo técnico. Se valida y crea antes de retirar el modelo existente; no se duplican agentes/controladores de gameplay. Catálogo Alpha referencia los prefabs E08 y conserva el estado de partida.14/14más1/1ampliado verificaron animación, fallback, tres clases, movimiento/equipo y guardado.

Meshy experimental permanece solo local:99.466triángulos/222.717vértices,1submesh/material,UV0ausente,0skins/huesos/Animator/Avatar y sin texturas finales. Copia aislada normalizada a1.78m, sin rotación adicional arbitraria. Necesita retopología, UV, texturas, rig y pesos en Blender u otra herramienta autorizada. Ni original ni derivados se publican o entran en el catálogo/build. No es automáticamente compatible con animaciones por estar importado Generic.

## Presentación y UI

Escena nueva AetherniaAlphaCandidate conserva las anteriores. Menú con fondo ambiental y preview3D real; selección con tres opciones verticales, retrato, ilustración existente y modelo E08 animado aislado. El preview no modifica la clase/equipo del jugador, no tiene física/IA y deja de renderizar al jugar. Fuentes SDF dinámicas originales y glifos españoles/fallback comprobados; no se regeneró atlas.20vistas reales1280×720,1920×1080,2560×1440,3840×2160; límites/raycast/overflow comprobados y captura4K inspeccionada a tamaño original. Las ilustraciones tienen resolución fuente limitada; los cuerpos E08 siguen siendo provisionales.

Se reprodujeron dos defectos: construcción bajo ayuda y7/7indicadores aliados desactivados al sustituir modelos. Corregidos: exclusión contextual y preservación del renderer de selección. Mando RTS lateral solo Alpha despeja al héroe; barras aliadas/rivales mantienen colores existentes. × por raycast y Escape por InputSystem comprobados en ficha/inventario/sesión/grupo. Avisos y fundido0.18s reutilizan HUD/eventos sin bloquear entrada, tocar reloj/reglas ni añadir gestor UI. Se mantienen feedback/sonidos/animaciones/cámara existentes; no hay cinemáticas largas ni cambios narrativos.

## Escenarios y shaders

Shader Graph17.5 ya instalado, graph nativo editable Lit y HLSL propio modesto: ruido de superficie en coordenadas de mundo, mezcla suave en bordes de camino con hierba existente, normales originales, smoothness/metallic adecuados y viento0.035m solo en dos materiales de hojas.15materiales compartidos y377asignaciones; cero texturas nuevas duplicadas. Bounds visuales se amplían y restauran sin modificar mesh/collider. Viento GPU92.901píxeles cambiantes; control estático0. Firma física igual a E16, sin Terrain nuevo ni navegación recalculada.

Volumen propio Neutral, contraste5/saturación−5/exposición+0.08, bloom0.04/umbral1.4 y vignette0.10. Luz de apoyo existente2.8en ruinas. Marcadores de entrada/salida apartados1.2m de spawn; triggers/puntos de viaje siguen iguales. Viaje ida/vuelta y recorte/restauración de techos comprobados. Se conservan distribución, antorchas y geometría E16; ninguna ampliación de mazmorra.

## Calidad y rendimiento

Bajo/Medio/Alto son tres assets URP propios, con originales conservados. Bajo:80%render3D,MSAA1,sombras20m/512,una luz por objeto y AO/postdesactivados. Medio:100%,MSAA2,40m/1024,dos luces. Alto:100%,MSAA4,60m/2048,cuatro luces y sombras suaves. UI a resolución nativa. Índice1histórico pasa a Medio; formato de guardado intacto.2/2dirigidas verifican pipeline/cámara y estado sin mutación. No hay opciones simuladas ni LOD destructivos o paquetes nuevos.

Medición nativa final:18escenarios, tres perfiles×720p/1080p×aldea pausada/menú3D/RTS pausado con despliegue real. Intel i5-12450H y RTX3050Laptop. Batch exige peticiones URP explícitas de escena/UI/preview: se comprueba una por cuadro y contadores gráficos positivos. TargetMSAA1/2/4real; calentamiento2s, muestreo≥3s, capturas fuera del muestreo, sin editor/grabación simultáneos.

| Escenario1080p | Perfil | Ciclo CPU medio / p95(ms) | SetPass | Triángulos medios | GC bytes/cuadro |
|---|---|---:|---:|---:|---:|
| Aldea pausada | Bajo | 4.50 / 6.42 | 14 | 252902 | 1138 |
| Aldea pausada | Medio | 6.22 / 8.86 | 26 | 574686 | 1156 |
| Aldea pausada | Alto | 6.35 / 8.57 | 28 | 713587 | 1157 |
| Menú3D | Bajo | 7.11 / 9.93 | 23 | 449927 | 187 |
| Menú3D | Medio | 9.44 / 13.10 | 41 | 1013709 | 199 |
| Menú3D | Alto | 9.33 / 13.04 | 42 | 1172323 | 199 |
| RTS pausado | Bajo | 7.62 / 10.00 | 17 | 961705 | 11642 |
| RTS pausado | Medio | 8.93 / 12.28 | 27 | 2252221 | 11657 |
| RTS pausado | Alto | 9.19 / 12.05 | 29 | 2828736 | 11661 |

Son tiempos del ciclo CPU que incluye peticiones de render fuera de pantalla, no FPS de presentación ni tiempoGPU. Triángulos contabilizan pases renderizados, no geometría única. DrawCalls/Batches no se obtuvieron con los nombres y aliases enumerados disponibles; no se sustituyen por SetPass ni se tratan como cero. No hay comparativa de rendimiento equivalente E16. El primer intento sin render gráfico quedó invalidado y se conserva; el intermedio con render se refinó para respetar MSAA. PERFORMANCE.json contiene disponibilidad, nombres, muestras, hardware y todos los datos.

El perfil Bajo reduce trabajo gráfico medido; la previsualización se apaga al jugar y se reutilizan materiales/recursos. No se aplicó optimización compleja sin datos, no hay LOD destructivos. RTS pausado muestra~11.6KB de GC/cuadro frente a~1.15KB en aldea, incluida instrumentación. La vista RTS todavía construye listas/cadenas y evalúa solapamientos cada cuadro: candidato a perfilado de callstacks y reutilización antes de escalar. No se midieron pausas GC, combate RTS activo masivo, GPU timing, VRAM ni FPS interactivos; no se prometen grandes ejércitos con este piloto.

## Validación, arte y revisión humana

Las pruebas citadas son automáticas en Unity exacto; la inspección de imágenes es técnica mediante archivos reales. No se realizó una sesión manual humana nueva. Gameplay RPG/RTS/economía/construcción/grupo/ruinas/conquista/persistencia ejercitados en regresión y14procesosWindows, incluidos reinicios y ZIP extraído. Arte/modelos/rig/animaciones/audio finales, balance, diversión y aprobación de controles/legibilidad siguen pendientes. Consultar GUIA_ALFA_E17.md y CHECKLIST.md; conservar todos los fallos intermedios y el historial anterior.

### Regresión general y ajuste de compatibilidad de la prueba

REGRESION_GENERAL.xml conserva142/143 aprobadas,0omitidas, en fuente f56d0bb. Único fallo: CharacterVisualTests.ClassEquipmentLoadMovementAndVisualReplacementStayConsistent, línea138, antes de llegar a movimiento/guardado. La expectativa E01 exigía cápsula pese a existir un modelo aprobado utilizable; contradice la política E17-B de conservar el modelo ya cargado si falla el catálogo. Se actualiza esa expectativa y se fortalece el caso: identidad del modelo/estado intactos, sin acumulación, y cápsula cuando también se invalida el modelo cargado mediante escala incompatible. Movimiento, equipo, guardar/cargar y capturas posteriores permanecen en el caso. No es un cambio de reglas o del resultado RTS. Verificación dirigida5/5PASS(FALLBACK_COMPAT_5.xml): caso histórico completo más los cuatro casos de pipeline. No se afirma143/143 de una nueva regresión global.

Las14resoluciones RTS de este lote fueron victorias (RTS_REGRESSION_TRACES.json); la derrota inicial reproducida se conserva y el pendiente no se cierra por este resultado.

## Entrega verificable

ALPHA_CANDIDATE_E17, desarrollo provisional para revisión; no v1.0. Fuente de runtime:0802286(precisión completa en WINDOWS.json/NATIVE_PRE_PORTABLE.json). BuildUnityPASS,206.493.085bytes informados,72.39s incremental.12procesosGPU de demo/expedición (inicio+Continuar×3clases) y2procesos desde ZIP extraído PASS:14/14. Se usan partidas E13 reales, reclutamiento/construcción pagados y combate real; sin fondos/HP/victorias fabricados. Recibos/identidad/métricas en Validacion/E17/Windows.

ZIP local: ProyectoRPGRTS/Builds/Aethernia-E17-Alpha-Candidate-Windows.zip,87.816.123bytes. SHA256`4c16177bc696334549eebc942f21fc6ebcd63b151fafac79eb7d9910d4b6e87d`. CRC/per-fileSHA/extracción PASS,17ZIPanteriores intactos, carpetas DoNotShip excluidas. Compilaciones/ZIP ignorados en Git; PORTABLE.json conserva manifiesto. Versiones r1/r2 de desarrollo conservadas localmente; solo el ZIP indicado es la entrega final.

82PNG reales con origen/dimensiones/SHA en CAPTURAS.json, incluyendo24anteriores E16,20vistasUI720p–4K, controles gráficos y tomas nativas. [Comparación de12tipos](COMPARATIVA_E17.md). Algunas tomas difieren en momento de combate/encuadre/calidad; no se atribuye todo cambio al shader. Inspección técnica de capturas originales UI4K, aldea, ruinas, RTS, RPG y combate de grupo; no sesión humana.

[Vídeo expedición24s](Videos/E17/E17-Expedicion.mp4) y [conquista8.08s](Videos/E17/E17-Conquista.mp4): secuencias continuas del player, audioDSP nativo, decodificación completa verificada. Origen1483cuadros a paso12Hz; no son FPS medidos. VIDEOS.json/VIDEO_PROOF.txt/timeline conservan evidencia.

Como límite visual observado, combate de grupo conserva algunos atajos repetidos entre pie y ventana lateral y el objetivo textual usa un identificador técnico heredado. No se ocultan estos detalles bajo una aprobación UX automática. Revisar jerarquía/nombres en sesión humana, junto con tamaño de personajes/cámara, oscuridad, sonido, diversión y balance. No hay arte de personaje final.

Los originales GDD, modelos/texturas/materiales previos, paquetes, escenas históricas y Meshy permanecen intactos; SOURCE_AUDIT.json verifica hashes y revisión acotada de credenciales. Sin dependencias experimentales en escena/catálogo públicos, sin descargas/plugin nuevo. Source code y evidencia se publican mediante PRdraft apilado sobre E16; main/dev y PRanteriores conservados.

Siguiente actividad recomendada: revisión humana de la candidata con GUIA_ALFA_E17.md, registrar decisiones de UX/balance, perfilar RTS activo y asignaciones de interfaz antes de aumentar unidades, y preparar UV/retopo/rig/modelos finales mediante pipeline. No se inicia E18 ni se aprueba arte automáticamente.

Commits por bloque: A`ab20917`,B`0cf3d34`,C`6d7d377`,D`9b83395`,E`49780b1`; F`f56d0bb`,`4fcb224` y refinamientos del medidor`e3f5878`,`521e17f`,`0802286`. Commit de entrega/documentación y PR registrados al publicar.
