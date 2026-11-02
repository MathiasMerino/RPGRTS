# Recuperación E14–E15

Núcleo técnico completado. Rama feature/e14-e15-expediciones-conquista desde c33e34d/PR5. No E16 ni subagentes. Tres texturas E07 restauradas por autorización; FBXMeshy ajeno intacto/no integrado/no staged.

Entrega y límites: Docs/ENTREGA_E14_E15.md. Uso: Docs/GUIA_EXPEDICIONES.md. Arquitectura/reglas provisionales: Docs/E14_E15_EXPEDICIONES.md. Escena AetherniaExpeditions; originales/GDD/paquetes/versiones intactos. No repetir Expeditions/Create ni generadores históricos.

Validación:121/121 regresión,11/11 focales después de pulido y1/1UI final después del recorte de escena (8vistas a1280×720/1024×768, texto/bounds/raycast, ventanas y héroe sin HUD).20procesos Windows PASS:6Headless,6GPU anteriores,6GPU finales,2portable; tres clases, victorias reales, economía pagada y reinicio. FinalGPU/Portable sin errores ni avisos internos de memoria temporal. Los avisos de ejecuciones anteriores constan en sus resúmenes; no se atribuyó su causa.

QAfinalGPU: ProyectoRPGRTS/Logs/ExpeditionQA/GPU-20261102-054838. Portable: Headless-20261102-055148. Medios:22PNG nativos+2PlayModeUI y2MP4 continuos con audioDSP (23.8s/8.1s), manifiesto/SHA/decodificación. Captura12Hz no es FPS medidos. Todos los procesos de prueba/editor han terminado; solo UnityHub permanece.

ZIP: ProyectoRPGRTS/Builds/Aethernia-E14-E15-Expediciones-Windows.zip; SHA958df84773407fe81274c366588ba60e97b4fe19feca69763c37be38912baf36;87,458,324bytes. CRC/SHA individual/extracción PASS;13ZIP anteriores intactos. Buildoriginal Builds/ExpeditionsWindows; extraída Builds/PortableE14E15-20261102-055044. No sobrescribir esosZIP al repetir.

Commits funcionales:959962f datos/reglas;11b2cd6 seguimiento/mazmorra;153e173 territorios/escena;0ebb5a2 legibilidad/cámara/recortes; e55f450 tests/recorridos/herramientas. Solo documentación/medios después de validar el código. Humano/arte/balance/perfilado completo pendientes independientes.

Último paso administrativo pendiente: publicar esta rama mediante push normal y crear PRdraft apilado hacia feature/e10-e13-demo-jugable (PR5 abierto). No fusionar1..5 ni tocar main/dev. GCM autenticado, token solo en memoria; APIurllib sin gh. Comprobar SHA local=origin=PR y conservar Meshy untracked. Cerrar EXP-QA y enlazar PR solo después de verificar publicación; EXP-H no se cierra automáticamente.
