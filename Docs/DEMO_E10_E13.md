# Primera demo integrada E10–E13

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
