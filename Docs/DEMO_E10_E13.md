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
