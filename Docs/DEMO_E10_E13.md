# Primera demo integrada E10–E13

Entrega consolidada y resultados finales en [ENTREGA_E10_E13.md](ENTREGA_E10_E13.md). Los checkpoints siguientes conservan el historial y sus fallos intermedios.

Base66cf80f/E09, Unity6000.5.8f1/URP17.5.0. PR#1–#4 abiertos/borrador; main/dev intactas. Rama feature/e10-e13-demo-jugable desde E09, PR apilado hacia E09. GDD0.2 original preservado; §§2/3/5/12 consultados. Nuevo encargo autoriza implementar secuencialmente E10–E13; E14/E15 solo tras núcleo estable.

## Decisiones de alcance

- Escena nueva AetherniaFirstDemo basada en la aldea existente, escenas históricas intactas. Un solo GameSession/PrototypeState/controladores de combate, RTS y construcción; estados de presentación y progreso diferenciados.
- E10: menú sobre escenario existente, TMP SDF/fuente de licencia incluida, opciones reales persistentes separadas del guardado. Nuevo slot demo separado del legado y respaldo antes de reemplazarlo. No control musical mientras no haya música real.
- E11: catálogo de clases existente; linajes humano/elfo/enano como metadatos provisionales, sin bonos ni modelos raciales nuevos (aspecto humano provisional explícito). Prólogo textual provisional basado en El Silencio de los Umbrales, omisible y externo al código. Tutorial RPG con reglas E09, victoria real y reintento explícito. WASD relativo a cámara solo exploración; unidades y cámara RTS conservan sus controles.
- E12: extender RtsBattleController con arrastre/Shift/grupos, roles y mismo héroe real/HP/equipo. Adaptadores de acciones existentes, sin héroe duplicado. Datos nuevos de unidades militares son provisionales; modo clásico6v6 preservado.
- E13: ampliar construcción/economía existentes con centro, almacén, taller, granja y cuartel; costes/producción/capacidad/reclutas explícitos y provisionales. Recursos oro/madera/alimento existentes; piedra/hierro y población no necesarios aún. Producción solo tiempo jugado, pausa en combates/menús, sin offline. Migración aditiva compatible con v1 y sin duplicación.
- E14/E15 y arte/animación/narrativa definitivos condicionados; aprobación humana E09-H/E08-H/E07-H/F2-07 no inferida.

## Secuencia y recuperación

E10-A auditoría/rama/plan; E10-B tipografía/menú/opciones; E11-A selección/estado/prólogo/tutorial; E11-B WASD/cámara/interacción; E12 RTS/héroe/acciones; E13 economía/construcción/ejército; QA regresión/capturas/resoluciones/end-to-end/reinicio; entrega build/ZIP/PR. Cada bloque requiere compilación y aceptación antes de marcarlo terminado. No regenerar assets de fases previas.

Checkpoint inicial: auditoría terminada, Git limpio, rama creada. Próximo: crear menú TMP y su autoría incremental, validar en editor exacto. Estado real y evidencia se añadirán aquí; aún no se declara ninguna fase nueva funcional.


## E10 — Primer checkpoint verificado en editor

3/3 focales PASS y salida0. Escena nueva/prefab DemoFrontEnd y contenido externo creados; escena E09 no cambió. FontAsset SDF dinámico2048, fuente LiberationSans/OFL y esenciales TMP extraídos del paquete uGUI2.5 ya instalado conservando GUID (la importación asíncrona no terminaba antes de -quit). No descargas ni dependencias nuevas. Metadatos de linaje sin bonos, guardado aditivo v1 compatible y slot separado; respaldo al reemplazar la demo, lectura del legado sin sobrescribirlo.

Capturas auténticas del menú1280/1920/2560/3840 y creación/prólogo/créditos: textos sin overflow y raycast de cada botón válido. Originales inspeccionados a720p; evaluación humana de nitidez/artística pendiente. Exit seguro en editor; cierre real Windows pendiente. Opciones reales: master/efectos/cámara/calidad/resolución/ventana con PlayerPrefs separados; sin control de música ficticio. El adaptador SDF conserva bindings/gestor C02; campo de nombre legado conserva Text/caret, creación nueva usa TMP_InputField. No declarar aún RTS/economía nuevos funcionales.

Checkpoint: rama feature/e10-e13-demo-jugable, E10 nativo compilado/focal3/3. Próximo exacto: controles WASD/cámara por modo y pruebas legales del primer combate con tres clases; luego extender RTS existente. E14/E15 sin iniciar.


## Adición de diseño al GDD — datos de demo propuestos

El Word original queda preservado. Esta sección es una adición provisional del encargo nuevo; no canon, narrativa final ni cambio de las reglas RPG autorizadas. Estado: diseñados, implementación y pruebas pendientes.

RTS clásico conserva6v6,10PV/2daño/1.5m/2.5m-s/intervalo1s (+4PV en escalada). Variante con héroe: mismo actor/HP/clase/equipo del RPG, más patrulla; espadachín conserva datos clásicos, luchador10PV/2daño/1.3m/3.2m-s, arquero10PV/2daño/6m/2.5m-s. Intervalo de acción1s del RTS existente. Héroe usa daño/alcance/velocidad de su perfil RPG y bono de equipo existente. Guardia reduce1 el siguiente impacto y ocupa el mismo intervalo que atacar; recuperación+4 hasta máximo, una vez por encuentro. No mana/hechizos/atributos paralelos. Soldados tienen perfiles propios, no copias de estadísticas del héroe. Raza/cultura/facción separados.

Economía propuesta exclusivamente de demo, sin producción offline: oro/madera/alimento actuales; centro15oro/8madera (habilita instalaciones), almacén10/5 (capacidad200base+100por almacén), taller10/5 (regla actual:10s,1alimento→1madera), granja8/5 (10s,+2alimento), cuartel12/8 (reclutar). Recluta5oro/2alimento, requiere cuartel, máximo6 persistentes iniciales. Mejora productiva a nivel2:10oro/5madera y doble rendimiento/capacidad; no mejora militar arbitraria. Huellas3x3m y máximo32edificios, como el prototipo. Población/piedra/hierro/corona no se introducen sin necesidad. Producción pausada en combate/menús y estado/ciclo persistidos impiden duplicación.


## E11 — Segundo checkpoint verificado en editor

2/2 pruebas PASS/salida0: diagonal relativa normalizada y aventura real con Guerrero/Monje/Mago. Cada clase completa el primer encuentro con dados/acciones/turnos E09, sin forzar HP/victoria; pasa automáticamente a explorar. InputSystem desplaza realmente al héroe, Generic observa velocidad y la cámara sigue; al soltar se detiene, al escribir/pausar/entrar RTS se bloquea. Selección/linaje/introSeen y posición guardan/cargan v1 sin repetir prólogo.

Incidencia de QA conservada en E11_WASD_INTERMEDIO.xml: los eventos sintéticos iban al buffer Editor sin foco, aunque allí W figuraba pulsada; el Update del juego leía(0,0). Confirmado en InputManager.defaultUpdateType del paquete instalado. La fixture dirige temporalmente toda entrada a GameView y usa IgnoreFocus, restaurando ambos ajustes en finally. Sin eliminar/rebajar la aserción de desplazamiento. El producto no ignora el foco globalmente. Tres capturas de exploración reales en Logs/E11; selección para entrega al final.

Controles opt-in solo en nueva escena: WASD para héroe en exploración; flechas/arrastre medio/rueda conservan cámara, RTS mantiene WASD para cámara. Colisiones por NavMesh y cápsula; velocidades de clase sin cambiar. E cercano puede escoger interacción próxima sin obligar a ratón. Controlador/animación/persistencia existentes reutilizados. Próximo exacto: extender RtsBattleController, marcador narrativo de patrulla y selección real/héroe/acciones; después E13.


## E12 — Tercer checkpoint verificado en editor

Recontrol de patrulla1/1 PASS/salida0 con las tres clases, desde prólogo y combate RPG reales hasta ruta/Capitán/E/RTS/victoria/campamento/continuar. Además12/12 reglas E09 pasan tras extraer helpers comunes de daño/guardia. Sin nuevo héroe: misma ExplorationUnit, modelo, clase, inventario y salud anterior; perfiles militares independientes y acciones ejecutadas en el controlador RTS actual.

Arrastre/Shift, selección individual, grupos Ctrl+1..3, órdenes de movimiento/objetivo y rangos/LOS, guardia con cooldown sin duplicación, recuperación real y HP de unidades. Espadachín/luchador/arquero usan modelos/rigs originales y arco visual piloto; no retargeting ni arte final. Soldados10PV/2daño, roles documentados; guardia/recuperación y daño compartidos sin cambiar tiradas RPG. Transición por interacción de mundo y resultado real, sin botones de depuración. El Cargamento perdido conserva su manejador; interacción ahora encadena handlers y libera suscripciones.

Primera pasada: Guerrero/Monje ganan y Mago pierde. Se detectó aproximación de unidades en guardia/distancia pese a estar en alcance durante cooldown: se detienen al alcanzar rango/LOS. La prueba permite usar la recuperación existente y una ración pagada del inventario, sin escribir vida ni victoria; recontrol completo de tres clases pasa. XML intermedio y primer/final PASS conservados. Barras reducidas para evitar superposición; rasgos de clase/HP reales. Capturas originales en Logs/E12. Windows/entrada mouse completa/nitidez humana y regresión global pendientes. Próximo: E13 y liberación de widgets RTS entre encuentros, luego QA end-to-end/reinicio y build.


## E13 — Cuarto checkpoint verificado en editor

6/6 focales PASS/salida0: transacciones/requisitos/NaN, activación y ciclos únicos, capacidad/mejoras/partición temporal, reclutamiento pagado/finito, persistencia/legado sin offline y un recorrido de economía con fixture explícita ya desbloqueada. Cinco edificios reales3x3, coste rechazado al solapar, taller activo produce, transición a juego libre, arquero pagado y mejora de almacén; save/load conserva recursos/niveles/ejército. Se despliega el recluta real, combate de práctica sin recompensa nueva y limpieza de barras/referencias al terminar. Esa fixture no se presenta como recorrido narrativo completo.

Primera pasada5/6: producción fue correcta pero la prueba observó el progreso antes del Update siguiente del coordinador. Se espera el cambio durante hasta1s conservando la aceptación; final6/6. Los prefabs usan colisión/NavMeshObstacle original y shell/materiales/props E07 compartidos; sin interiores transitables, huella3x3 y altura aproximada3.2m, vano visual1.5x2.2m. Capturas auténticas en Logs/E13; fuente C#/assets y migración aditiva se conservan. El entrenamiento despliega reclutas persistentes, mantiene bajas/salud y no paga otra recompensa.

Nuevo archivo ajeno detectado Assets/07_Characters/MonkMeshy (FBX y meta): preservado, no integrado ni incluido en commits de esta demo, procedencia/licencia no auditada en este encargo. Puede dejar el workspace original con elementos sin seguimiento; no ocultarlos ni publicarlos para simular limpieza. Nuevos resultados técnicos todavía no equivalen a aprobación humana.

Próximo exacto: QA de demo completa sin fixture desde menú y build Windows/reinicio para tres clases; regresión89+nuevas, fuentes/capturas y paquete/PR. E14/E15 siguen sin iniciar mientras se cierra el núcleo prioritario.


## QA — Colisión y revisión de capturas

DemoCollisionTests1/1 PASS (DEMO_COLISION_OPCIONES.xml): teclado real en Play, muro físico temporal sin obstáculo NavMesh bloquea el cuerpo y retirarlo permite continuar; opciones a cuatro resoluciones con botones/textos/raycast. La fixture abandona el encuentro mediante Clear explícito para aislar colisión; no representa un tutorial ganado. Primer intento0/1 conservado: la fixture dejó el encuentro activo y el cambio de modo se rechazó correctamente.

La inspección visual de Options mostró fill/handle del slider fuera de límites; se corrigieron los anclajes, se añadieron aserciones geométricas y verificación de evento/volumen/persistencia aislada. Pausa oculta HUD para evitar mezcla visual. Regresión completa y repetición de captura en curso; no declarar todavía aceptado este último ajuste.

Primera compilación FirstDemoWindows correcta. Primer probe Windows sin gráficos avanzó menú/creación/prólogo/RPG/movimiento, pero falló al observar velocidad solo al final, cuando se había detenido junto a una colisión; se conserva el comprobante. Se corregirá la ventana de observación de la prueba, manteniendo movimiento y animación requeridos. No se declara recorrido integral aprobado aún.


## QA — Compatibilidad de carga detectada por regresión

Primera batería completa102: **91PASS/11FAIL**, sin omisiones. XML íntegro DEMO_REGRESION_INTERMEDIA.xml. Los once fallos se concentraron en cargas históricas: JsonUtility serializa clases anidadas por valor y convirtió el Demo nulo en un progreso inicial; el director lo rechazó correctamente como campaña ajena a la escena vieja. Se añadió marcador explícito HasDemoProgress, derivado al guardar y normalizado antes de validar/cargar. Para campañas intermedias sin marcador se requiere el hecho demo-starter-kit; JSON legado conserva Demo=null y reglas antiguas. Version1 se mantiene, GDD/escenas previas intactos.

Nueva aceptación de regresión exige que el roundtrip legado siga sin progreso de campaña, que una campaña genuina sí lo conserve y que un marcador sin objeto válido se rechace. Focal de carga y repetición completa103 pendientes de cerrar; no se entrega un estado91/102 como estable.


## QA — Correcciones finales y aceptación nativa

La segunda regresión completa103 dio101PASS/2FAIL, ambos al reemplazar archivos JSON anteriores. Error capturado1175/0x80070497, sin afirmar antivirus u otra causa no probada. La escritura conserva File.Replace atómico; repite solo32/33/1175 con4esperas50/100/200/400ms (750ms máximo), preservando destino y temporal si agota. Permisos permanentes no se reintentan. AtomicReplacementTests cubre recuperación/exhaustión/permisos y bloqueo Windows real de200ms; LastWriteRetries permite observarlo. Focal12 más bloqueo real/derrota2 aprobadas. El tutorial se pierde cediendo turnos legalmente hasta0PV; Retry restaura la clase como nuevo intento explícito, sin premiar oro ni cambiar linaje.

Las opciones nativas revelaron Audio nulo en la instancia del prefab, referencia externa perdida al guardar la plantilla. Se enlazó el reproductor E09 existente mediante override de escena, se corrigió Create futuro y se añadió aceptación de referencia. No se regeneraron recursos. El player aplica la pantalla guardada al iniciar; el selector parte de la resolución cargada. Se conserva FAIL previo y no se presenta como prueba aprobada.

Recorridos finales Logs/DemoQA/{Headless,GPU}-20261102-010055: **12procesos PASS**, crear y cerrar/reabrir/Continuar para tres clases. Además del recorrido, verifica sliders reales y persistencia entre procesos de master.63/efectos.57/cámara14; calidad cambia/aplica/restaura y GPU confirma Screen1920x1080. Headless no comprueba pantalla física. Las claves PlayerPrefs de QA se aíslan por slot y eliminan tras resume; las preferencias personales se conservan. NativeSkip real para Mago y Next para otras clases; no se fuerzan vida/victoria/recursos.

Regresión final110 en curso; cierre técnico/PR condicionados a su resultado fresco. La evaluación humana de pantalla completa, sonido percibido, controles, legibilidad, balance y arte continúa pendiente.


## QA — Cierre global y recorte contextual

Regresión completa **110/110 PASS**, cero omisiones, salida0, exacto6000.5.8f1; copia DEMO_REGRESION_110.xml. Después se aplicó un ajuste únicamente de presentación a las construcciones de campaña: RoofCutaway existente oculta/restituye el conjunto visual si tapa al héroe. Los objetos E07 agrupan techo/hastiales/vigas por material; retirar solo Slate dejaba el hastial Plaster encima. La nueva aceptación comprueba todo el conjunto, además de colliderbounds/enabled, NavMeshObstacle y economía/save/reclutas/despliegue: **6/6 PASS** posterior. El legado mantiene instanciación equivalente, sin recorte nuevo. No se declara esta comprobación como aprobación artística humana.

Pruebas intermedias conservadas: cierre C#capturado antes de EnterPlayMode en fixture corregido sin cambiar aceptación, y Roof vacío en prefab con geometría agrupada. Una captura real reveló el hastial que aún ocultaba al actor y motivó la ampliación del recorte. Última build compilada; evidencia/player/paquete se renuevan antes de publicar.


Cierre técnico:110/110 global+6/6 después del último recorte,12procesos finales Headless/GPU-20261102-014152 y2desdeZIPUnpackedCutaway (Headless-20261102-014556) PASS. Hero visible en captura final; sin aprobación humana inferida.18PNG, vídeo407frames codificados/33.917s/audioDSP RMS.002746; paquete306archivos87411924bytes/CRC/SHA por archivo,12ZIP anteriores intactos. Informe consolidado ENTREGA_E10_E13.md. Publicación/PR en cierre, main/dev no se fusionan.
