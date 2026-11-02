# Recuperación de demo E10–E13

Rama feature/e10-e13-demo-jugable desde66cf80f/E09. Commits1ee7ae2 E10,22e627d E11,00ed6c1 E12,dd56e9c E13. Main/dev y PR#1–#4 sin cambios; publicación nueva todavía NO realizada. Autoridad/plan/evidencia en Docs/DEMO_E10_E13.md, prompt completo en C:/Users/Mathy/.codex/attachments/11d5c84a-de6e-4622-afd8-b0bfe4ab584c/Pasted text.txt. No subagentes. Continuar autónomamente hasta core E10–E13 verificado; E14/E15 condicionales y sin iniciar.

- Escena nueva Assets/01_Scenes/AetherniaFirstDemo.unity; fuentes nuevas Runtime/Demo y Editor/Characters/Demo/FirstDemoBuilder.cs. E07/E08/E09 y escenas originales preservadas.
- E10 editor3/3 PASS, menú TMP SDF2048/fuente LiberationSans/OFL del paquete uGUI instalado; 7capturas reales en Logs/E10/Captures (cuatro resoluciones720p–4K, creación/prólogo/créditos). No repetir FirstDemo ni importar/regenerar fuentes/assets; ya creados. Runtime LegacySdfPresenter dibuja TMP conservando bindings C02. InputField legado conserva caret; nuevo TMP_InputField real.
- E11 editor2/2 PASS: tres clases con RPG real desde prólogo, WASD real/animación/cámara, pausa/foco/modo y save/continue. Fixtures InputSystem deben temporalmente AllDeviceInputAlwaysGoesToGameView + IgnoreFocus, restaurar en finally; de lo contrario eventos van al buffer Editor sin foco y el juego lee cero. No rebajar aserciones. Capturas Logs/E11, XML en Docs/Validacion.
- E12 editor1/1 final PASS con tres clases;12 reglas E09 también PASS. RtsBattleController ampliado, mismo héroe/HP/modelo; arrastre/Shift/grupos1..3, roles, guardia/recuperación/órdenes, NPC Capitán en(-12,8), victoria -> campamento(9,-10). Mago perdió en primera pasada: corregido mantener rango incluso durante cooldown/guardia; ración y recuperación legales. XML fallido conservado. Capturas Logs/E12.
- E13 código/autoría y focal6/6 PASS: SettlementRules/Operations, WorldConstruction ampliado, cinco prefabs3x3 con shell/materiales E07, UI DemoSettlementPanel; Army/Level/ProductionPaused aditivos v1, migración y slot separado demo-v1.json. Estado inicial de tutorial50oro/20madera/10alimentos, raciones2 y equipo inicial existente. Raza humano/elfo/enano solo metadatos sin bonos/modelos raciales; prosa provisional en DemoContent.asset.
- Autoría FirstDemoSettlement PASS. Focal DemoSettlementTests6/6, ningún Unity activo salvo Hub. XML final Docs/Validacion/E13_ASENTAMIENTO.xml; primera5/6 de timing preservada.
- Funciones de autoría posteriores Controls/Rts/Settlement son ajustes acotados de la escena nueva; no volver a Create. Modos Tools/Invoke-UnityValidation.ps1: FirstDemoControls/FirstDemoRts/FirstDemoSettlement, BuildFirstDemo. Una sola instancia Unity/proyecto, exacto6000.5.8f1/URP17.5.0.
- Build nueva prevista Builds/FirstDemoWindows/Aethernia.exe, todavía NO creada. Ejecución real/reinicio/evidencia/regresión completa/ZIP/PR pendientes. Preservar12ZIP históricos (E09 incluido). La build E09 en Builds/Windows y su ZIP permanecen previos.

## Próximo paso exacto

1. E13 PASS, guardar commit del bloque (en curso) y continuar QA integral. Archivo ajeno sin seguimiento Assets/07_Characters/MonkMeshy + meta, preservar y excluir de commits; no ocultarlo ni declarar workspace totalmente limpio.
2. Completar validación de colisión WASD y UI/textos/contextos. Ya arreglada limpieza de barras/grupos RTS entre batallas, incorporada a prueba E13. Edificios producen solo tras activar; ciclo10s online, producción/clock pausados en combates/menú, sin offline.
3. Crear prueba real de demo integral Windows: menú->clase/linaje->intro->RPG legal->WASD->Capitán->RTS mismo héroe->campamento->centro/taller->producción/recluta->save->salir->nuevo proceso Continuar. Tres clases; usar APIs/acciones reales, no modificar HP o victoria. Source fixture de economía NO equivale a recorrido narrativo.
4. Ejecutar regresión existente89 + nuevos tests (ahora101 aproximadamente), timeout20–25min; suele tardar15min por domain reload. BuildFirstDemo y recorridos headless/GPU + reinicio, capturas/posible vídeo/ZIP CRC/SHA. Adaptar captura para TODOS los Canvas del menú; StandaloneGameCapture viejo solo busca HUD activo y no basta en menú.
5. Actualizar checklist/registro/AGENTS/COMO_JUGAR/plan/informe compacto/evidencia. Añadir propuesta de diseño en informe sin tocar Word GDD. Revisión humana E09-H/E08-H/E07-H/F2-07/X/ESC y arte/sonidos/Generic siguen pendientes.
6. Push feature normal, PR borrador apilado hacia feature/e09-combate-rpg-ia si#4 sigue abierto; verificar API/GCM en memoria (no gh instalado). No merge/force/main/dev/global config. Dejar repo limpio sincronizado.

Datos E13 provisionales documentados: centro15oro/8madera; taller10/5 consume1alimento->1madera/10s; granja8/5 produce2alimento/10s; cuartel12/8 permite recluta5oro/2alimentos,6máximo; almacén10/5 capacidad+100 sobre200base; mejora10/5 nivel2 duplica producción/capacidad. Oro/madera/alimento existentes, no piedra/hierro/población necesarios. Ejército separado de compañeros E14. Reclutas tienen ID/rol/HP persistentes y se despliegan en próximas patrullas; bajas persistentes, entrenamiento no duplica recompensa.

## QA en curso (2026-11-01 ~19:57)

- Nueva DemoCollisionTests1/1 PASS: colisión física WASD sin NavMeshObstacle y Options a720p/1080p/1440p/4K. XML Docs/Validacion/DEMO_COLISION_OPCIONES.xml; primer fallo fixture bloqueada por combate conservado en DEMO_COLISION_INTERMEDIO.xml. No es fallo de navegación del producto.
- Captura Options reveló slider fill/handle sobredimensionados pese a texto/botones correctos. Corregidos anclajes DemoWidgets, nueva aceptación geométrica en DemoCapture.Validate; Pause ahora oculta HUD, Resume lo restituye. Se añadió comprobación real del evento Slider/master aplicado y persistido en clave aislada.
- **Regresión completa102 pruebas ACTIVA**, launcher exec sesión2860, Unity PID19532, Logs/Vertical.log/results. NO editar C# hasta terminar. Al finalizar copiar XML a Docs/Validacion/DEMO_REGRESION.xml y revisar resultado. No iniciar segundo Unity.
- Primera BuildFirstDemo PASS (anterior a corrección sliders/pausa). Aún no entregar, necesita recompilar tras QA.
- Nuevos Runtime/Validation/DemoSmokeProbe y DemoPlayerCapture: instalados solo con flag -aetherniaDemoSmoke vía RuntimeInitialize, no edición de escena. Tools/Invoke-DemoSmoke.ps1 ejecuta iniciar + nuevo proceso continuar por clase, slots aislados/capturas/recibos/WS.
- Primer smoke Headless warrior FAIL en Logs/DemoQA/Headless-20261101-235455/warrior.json.start.txt: movimiento WASD ocurrió (>0.25m), ObservedSpeed observado solo al final era0 al llegar a obstáculo. **Próximo ajuste pendiente del probe**: medir pico ObservedSpeed durante los0.4s de movimiento, exigirlo>0.1 y mantener aserción de desplazamiento; no rebajar comprobación de animación. No escribir salud/victoria/recursos. Solo cambiar probe tras finalizar editor102.
- Después: BuildFirstDemo incremental, smokes Headless3clases +GPU3clases (cada uno reinicia proceso), conservar fallos intermedios, verificar capturas. Regresión actual cubre código gameplay/UI final; si hay corrección de producto adicional repetir pruebas pertinentes.
- Doce ZIP antiguos SHA guardados Logs/DemoQA/historical-zips-before.json. No tocar Builds/Windows ni estos ZIP; nuevo FirstDemoWindows/ZIP aparte.
- API GitHub recontrastada, main/dev/PR1..4 sin cambios, snapshot Logs/DemoQA/github-pre-delivery.json. Aún no nuevo push/PR.
- COMO_JUGAR actualizado con nueva escena/controles/reglas, build aún en curso; guía nueva Docs/PRUEBA_MANUAL_DEMO.md enteramente pendiente humana.


## Recuperación QA ~20:11

Regresión102 terminó91PASS/11FAIL (XML Docs/Validacion/DEMO_REGRESION_INTERMEDIA.xml). Causa reproducida: JsonUtility serializa por valor Demo=null como DemoProgress inicial, por eso Director.Load rechazaba slots de escenas históricas como si fueran campaña nueva. Añadido HasDemoProgress marcador explícito derivado al guardar; Load sin marcador solo reconoce campaña intermedia si tiene hecho demo-starter-kit, resto Demo=null. Nuevas partidas legítimas preservadas; no cambió Version1. Nuevo FirstDemoTests.LegacyRoundtripDoesNotInventDemoProgress (totalahora103).

Probe WASD ajustado a medir pico de ObservedSpeed durante los0.4s, manteniendo desplazamiento>0.25m y velocidad>0.1; órdenes RTS siguen mover/atacar legales. Sin cambios de HP/recursos/victoria.

Focal6 (FirstDemoTests4 +Vertical1 +LostCargoMage1) ACTIVO: exec73865/UnityPID10596. No editar scripts hasta cerrar. Al finalizar preservar XML y comprobar6/6; después BuildFirstDemo actualizado y smokes3clases/reinicio, luego regresión103 completa (puede paralelizar player independiente con pruebas, nunca dos editores). Aún NO entregar. Regresión anterior reportada honestamente en comentario al usuario.


Focal6 terminó5/6: FirstDemo4 y Verticalintegración PASS (carga legado corregida). LostCargoMage ahora pasa aceptación/carga/combate/recompensa y falla al guardar E05-complete-mage.json (line65); queda.tmp con payload correcto y destino intacto antiguo. Se preservaron ambos Logs/DemoQA/SaveIncident. XML Docs/Validacion/DEMO_CARGA_INTERMEDIA.xml. Añadido mensaje LastWriteFailure a esa aserción, sin cambiar aceptación. **Focal1 en curso exec23760/Unity28684**, no editar scripts hasta cerrar. Obtener HRESULT/mensaje del fallo antes de modificar almacenamiento; puede relacionarse con incidente File.Replace E08 previamente no explicado. No afirmar causa todavía.


~20:17: repetición LostCargoMage1/1 PASS sin cambios al almacenamiento, XML DEMO_CARGA_MISION.xml. Incidente de File.Replace no volvió a reproducirse; no causa confirmada ni fallback destructivo. LastWriteFailure ahora figura en aserción de final de misión. Marcador HasDemoProgress corrigió cargas históricas en focal. Falta regresión103 final.

Añadido DemoRecordingProbe opcional (-demoRecord y -aetherniaDemoSmoke solamente), imagen nativa continua720p/12Hz captura fija + AudioRenderer DSP/timeline/WAV; Runtimeasmdef referencia Unity.Collections ya instalado, sin paquete nuevo. Tools/Invoke-DemoSmoke.ps1 -WithGraphics -Record graba solo primera clase/proceso inicial. No altera lógica ni valores; tasa de captura no es FPS medido. Compilación actual exec88543/Unity8244, esperar sin editar scripts. Probe WASD mide pico durante movimiento y ya incluye orden mover antes de atacar. Próximo: smokes Headless3/GPU3+reinicio (slots nuevos absolutos). Si pasan, iniciar regresión103, montar vídeo con imageio_ffmpeg disponible, seleccionar capturas/documentar/ZIP/PR. Todavía no entregar.


## Windows integral aprobado sin gráficos (~20:21)

BuildFirstDemo actual PASS. Headless seis procesos PASS (crear y reiniciar/Continuar para warrior,monk,mage) en Logs/DemoQA/Headless-20261102-001842. 28.64/28.68/35.98s recorridos, continuaciones0.59/0.59/0.57s. Recursos finales reales8oro/0madera/7alimento, tres edificios+arquero,Produced1; sin fixture/valores forzados. Recompilación incluye HasDemoProgress/slider/pausa y picoWASD. Save/load/restart real confirmaestado.

GPU3clases+reinicios ACTIVO exec35452, primer player17932, con -Record solo warrior. Directorio Logs/DemoQA/GPU-<timestamp>; capturas PNG y Recording/frame-XXXXX.jpg/Native-DSP.wav/proof.txt/timeline.txt. Encodificar con imageio_ffmpeg ya disponible solo si proofPASS y revisarframes/audio; no instalar. Cap12Hz noFPSmedido.

**Regresión final103 ACTIVA** (nuevo launcher a continuación de snapshot, buscarPID/log si compactación). No editar C# hasta terminar. Puede correr junto al player independiente; único editor/proyecto. XMLfinaldebe103/103 y sin omisiones antes de cerrar. Si falla File.Replace otra vez, preservarXML y registrarHRESULT medianteaserción, sin fallbackdestructivo.


~20:30: GPU6procesos PASS, Logs/DemoQA/GPU-20261102-002045, nativeRTX3050Laptop. VideoDSPPASS381framesfuente,MP4codificado380/12Hz=31.667s, audioRMS0.0044055. Docs/Images/DemoE10E13 contiene17PNG copiados exactos+MP44.78MB+manifiestos; Docs/Validacion/DemoE10E13 recibos12procesos/estados/metrics/proveniencia/video. ContinuarPNG del probe se capturó pausado (HUD oculto deliberadamente), no seleccionado como muestra de UI normal. CampamentoPNG real desdefondos50/20/10 (tresedificios/recluta, final8/0/7).

ZIP nuevo87409438bytes/306archivos/SHA8971e1926590815aaf296542d9968a5f6c21727fe0b55b9a8553c0c736de988f, CRC y SHA individual correctos; DoNotShip debugexcluido sinborrarcarpeta.12ZIPviejosSHAintactos. Extraído en Logs/DemoQA/Unpacked y otros2procesosWarrior crear/Continue PASS (Headless-20261102-002827). Paquete/evidencia todavía pendientes de cierre regresión103 activaexec22716/Unity23400. No editar scripts aún.


## Estado más reciente ~20:42 — NO cerrar todavía

Regresión103 terminó101PASS/2FAIL. Todas las cargas legadas ya correctas; fallos E08 RealMovement...line115 y E09 ThreeClasses...line108 al reemplazar JSON antiguo. E09 obtuvo **IOException0x80070497 Unable to remove the file to be replaced** (1175). XML Docs/Validacion/DEMO_REGRESION_103_INTERMEDIA.xml. No afirmar causa externa confirmada.

Implementado AtomicFileReplacement interno: repetir MISMO File.Replace atómico solo Win32 32/33/1175,4reintentos50/100/200/400ms (750ms totalmáximo), conservar destino/tmp si agota, sin borrar fallback. LastWriteRetries diagnóstico. Runtime/AssemblyInfo InternalsVisibleTo Tests. AtomicReplacementTests5casos (3errorestransitorios, agotamiento, permisos no retry). Totalahora108.

Cierre opciones nativo añadido: DemoPreferences.SessionStorageKey solo QA bootstrapBeforeSceneLoad, aislar PlayerPrefs por slot y borrar al cerrar resume. FrontAwake aplica pantalla guardada también en player (!Editor). Probe ajusta sliders realesMaster.63 Effects.57 Camera.5=>14, resolución1920x1080, calidad cambia/aplica/restaura, verifica efectos/Screen real y persiste en nuevo proceso; Headless solo datos/aplicaciónaudio/cámara, no pantalla. Captura Continue ahora DESPUÉS de Resume, HUD visible. Necesita build NUEVA y repetir recorridos/evidencia.

**Focal12 activo exec79110 Unity9324** (Atomic5+E08/E09 integrados2+FirstDemo4+Collision1), compila (sinerrorobservado). NO editar scripts hasta cerrar. Próximos trasresultado: inicializar displayIndex desde resolucióncargada en FrontAwake (evita primer clic sin cambio al cargar1920); añadir prueba Windows real de archivo destino bloqueado200ms, Save con retries y LoadGold73 sin perder anterior (test adicional1=>109). Luego BuildFirstDemo y native Headless3/GPU3+Record+reinicios: nuevos checks opciones. Regresión109 completa una vez aceptado producto final. No afirmar108/109PASS hastaresultado.

Paquete/vídeo/17PNG ya generados son de build ANTERIOR a retries/opciones, conservar como evidencia intermedia en Logs, reemplazar entrega final tras nueva build. ZIPactual87409438/sha8971...reproducido portable, mover su propio ZIP a Logs/DemoQA/pre-display-options-package.zip (NO tocar12 históricos), después crearfinalmismonombre. Extraer nueva copia en UnpackedFinal paraejecutar. MP4Docactual4.78MB380frames es inicial; rawLogsGPU20261102-002045 conserva381frames/audio/prueba. Nueva grabación se puede volver a codificar/copiar sobre nuestra salidaDoc, actualizarsha/manifest/evidenciaconfuentesnuevas (preservarviejaenLogs).

Faltan cierrevalidación, paqueteactual, Docs/COMO_JUGAR/checklist/registro final, commitQA(s), push y PR borrador apilado haciaE09. Main/dev/PR1..4 sin cambios. MonkMeshy ajeno permaneceuntracked; nointegrar/stage/publicar/ocultar. E14/E15 condicionales sininiciar y DEMO-H humana pendiente.


## Último checkpoint ~20:57 (continuar desde aquí)

Focal12PASS preservado DEMO_ATOMICO_UI_12.xml. AñadidosNativeLockedDestinationRecovers... (bloqueorealFileShareRead200ms, LastWriteRetries>0/LoadGold73) yActualTutorialDefeat... (omitirprólogo/cederturnos hastaHP0/RetryUI/nuevoencuentro sin cambiaroro/linaje); focal2 **PASS**, XML DEMO_BLOQUEO_DERROTA.xml. Total **110** tests. FrontdisplayIndex inicializa desde resolucióncargada; aviso de partida ahora genérico compatiblelegado. MageNative usa botónSkip real; otrosNext.

Build posterior fue correcta, pero nuevossmokes Headless/GPU20261102-005233 pasaron crearWarrior/opciones/RPG/RTS/campamento y fallaron ambosresume por **NullReference en Front.Audio.EffectsGain**. Diagnóstico real: al guardar prefabFrontend se perdió refexterna Audio aescena; Front.Audio era null, controlEffects no actuaba y selecciónUI sin audio. Se conservaFAIL en Logs; NOdisfrazarcomoQA.

Corregido FirstDemoBuilder.Create asignaAudio DESPUÉS de instanciarprefab y guardaoverride; Controls safe enlazaAudioexistente enescena y valida no null. Nueva aceptación en FirstDemoTests.FontAndProvisionalData... abre escena yexigeAudio. **FirstDemoControls ejecutadoPASS**, escenaAetherniaFirstDemo modificada únicamenteoverrideAudio, sin regenerarrecursos.

**BuildFirstDemo ACTIVO** exec reciénlanzado (ver output deltool anterior), únicaUnityinstancia. Al terminar: repetir nuevosHeadless3/GPU3+Record y reinicios (NativeKeyaislada por slot, cleanalresume; verificar opcionespersistentes1920/1080/calidad/master.63/efectos.57/cámara14). Si funcionan, lanzarregresión110 completa timeout25 (puedeirjuntoaplayerportátil independiente). No edicionesruntime tras cierre salvofallosreales.

EntregaDocimágenes/vídeo/ZIPactuales son ANTERIORES (GPU002045, ZIP8971...); aúnactualizar trasfinalbuild. MovernuestroZIPintermedio aLogs/DemoQA/pre-display-options-package.zip sin tocar12históricos. ZIPfinalnombreigual; extracciónnuevaUnpackedFinal ysmokewarrior2procesos. NuevoNativeVideoRMS/proof/capturas conAudiofuncionando; re-encodeDocsMP4 ymanifiestos (fuentesanteriorespreservadasenLogs).

Faltan docsfinal/QAcommit(s)/push/PRborradorapiladoE09. Main/dev/PR1..4 intactos. MonkMeshy ajeno untracked preservarfuera. E14/E15 no iniciados, humanos pendientes. Usuarioinformado del falloAudio real ycorrección. No volverareleerGDD; hash originalverificadoDBA82...sinmodificar.


## Último estado ~21:01

BuildconAudioOverridecorrectoPASS (exec24203 terminó0). Nuevosplayers ACTIVO Headless exec69438 yGPUexec95603 (conRecordwarrior); ambosWarrior/Monk crear+resume **PASS**,incluyechecksdeopcionespersistentes/AudioGain/Screen1920enGPU, Magecorriendo. DirectoriosverúltimoglobHeadless/GPU, timestampaprox20261102-0059xx. PrimeraNREAudionorecurre. PredatosLegacyOldScene sinmodificaryV1nullnormalizado.

**Regresión110 final ACTIVA** exec reciénlanzadoenúltimotool, únicaUnitysobreproyecto. Esperarresultado sineditarfuentes. ActualFirstDemoFonts incluyeaserciónAudioescena. No inventarPASS hastaXMLfresco.

Mientras: terminarplayers/extraerreceiptsfinales/recodificarvideoDesktopDsp/actualizar17PNGmanifests. MoverZIPintermedio87409438aLogs/DemoQA/pre-display-options-package.zip, zipBuildfinalCRC/SHA/12históricos, extraerUnpackedFinal y2procesoswarriorconExecutableopcional. ActualizarDocs/CHECKLIST/REGISTRO/COMO_JUGAR/PROGRESOcompacto ycommitsQA/importante. Pushnormalfeaturerama yPRborradorbaseE09 (4 sigueabierto). VerificarSHAsmain/dev/PR1..4intactos. Usuarioinformado delAudiofixyrecorridos/opcionesenrepetición. MonkMeshy fuera de commits. Humanos/E14/E15 pendientes.
