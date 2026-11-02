# Guía de expediciones E14–E15

Unity6000.5.8f1, URP17.5.0. Escena nueva `Assets/01_Scenes/AetherniaExpeditions.unity`; no regenerar escenas ni recursos. Las escenas históricas siguen disponibles. Abrir el proyecto `ProyectoRPGRTS/`, esperar importación, abrir esta escena y pulsar Play. Consola sin errores antes de probar.

La build Windows está en `ProyectoRPGRTS/Builds/ExpeditionsWindows/`. Ejecutar Aethernia.exe para una partida normal: menú → personaje → prólogo/RPG → capitán/RTS → fundar campamento. Continuar recupera la partida local E13 compatible. Las partidas con compañeros/territorios requieren esta escena nueva.

Los lanzadores Revisar-Guerrero, Revisar-Monje y Revisar-Mago permiten revisar desde copias de partidas auténticas E13 incluidas, sin tocar la partida personal. Esas copias conservan la salud y los recursos reales del recorrido anterior. Se modifican al guardar: respaldarlas para repetir. El argumento opcional `-aetherniaSave "ruta.json"` selecciona un slot normal; no ejecuta pruebas, no regala recursos ni omite combates.

1. Administrar campamento: seleccionar el taller existente y pausarlo para reservar alimento. El reloj sigue avanzando mientras exploras/administras; combate y pausa detienen la economía.
2. Si faltan fondos, recoger el cofre del poblado, cerca de(−2,−5): requiere dos espacios de inventario. Su recompensa única aporta10oro/4madera. También sigue disponible El cargamento perdido.
3. Volver al campamento(9,−10), abrir **Grupo y expediciones**. Incorporar espadachín, arquero o luchador cuesta5oro/2alimento por rol único; hasta3compañeros más el héroe. Retirar pasa a reserva, conserva identidad/salud y permite reingreso. Salud0 impide reingresar hasta recuperarse.
4. **Descansar10s** recupera hasta4PV por miembro, sin muerte permanente. Repetir si es necesario. Debes permanecer en campamento; caminar o cambiar de partida cancela. Viajar no cura.
5. WASD mueve al héroe y el grupo lo sigue. Flechas, botón medio y rueda controlan cámara. E/interactuar cerca del acceso de ruinas(4,−11) reúne al grupo y cambia al interior. La transición conserva salud/equipo/recursos.
6. En ruinas, acercarse a cada guardianía y pulsar E. El orden incluye héroe, compañeros vivos y enemigos. Elegir objetivo; acercarse o indicar suelo con clic derecho; atacar/guardia/recuperar; finalizar turno. Movimiento4m y una acción por actor. Los enemigos actúan automáticamente. Obstáculos bloquean línea de ataque. Un paso ocupado puede cancelar movimiento; elegir otra posición o finalizar turno conserva el coste gastado.
7. Derrota: retirada al campamento con salud real, luego descanso. No se borra ningún compañero ni se entrega recompensa. Victoria: continuar al segundo encuentro. Tras vencer ambos, interactuar con objetivo al fondo:30oro/15madera/6alimento, una sola vez. Alijo opcional5oro, también único. Salir por el acceso de entrada.
8. Reinvertir: granja8oro/5madera, por ejemplo(11.5,−14), activar producción. Cuartel existente permite soldados militares5oro/2alimento; capacidad6, independiente del grupo RPG. El taller consume alimento para producir madera; puede permanecer pausado.
9. Pestaña Ejército: preparar soldados concretos. Pestaña Territorios: marchar al paso(11,8), interactuar E para asegurar el neutral. Marchar después al puesto(−12,−13), E para iniciar conquista. Marchar por sí solo no cambia propietarios.
10. RTS: mismo héroe y salud, solo soldados preparados; los compañeros RPG esperan. Arrastrar selección múltiple, clic derecho para mover/atacar, botones de acción del héroe según disponibilidad. Tres defensores usan perfiles existentes. Ganar vivo transfiere el puesto; perder conserva al enemigo y registra bajas militares.
11. Paso y puesto controlados aportan1madera cada uno por ciclo10s jugados, con capacidad de almacén. No producen offline ni reinician ciclos al cargar. Raza/linaje, cultura, facción y reino son datos independientes; no hay coronación.
12. F5 guarda/F9 carga fuera de combate; menú permite volver al título/Continuar. Comprobar nombres, clase, equipo, salud, compañeros activos/reserva, soldados, recursos y propietarios después de cerrar y abrir.

## Qué revisar personalmente

Legibilidad a distintas resoluciones, navegación en esquinas/entre personajes, cámara, claridad de objetivos/turnos, dificultad de varias composiciones, ritmo de descanso/economía y diversión. Es evaluación humana pendiente. Los modelos/ruinas/animaciones son provisionales; los recortes visuales conservan colisiones. No hay aprobación artística o narrativa definitiva.

## Repetir QA

`Tools/Invoke-UnityValidation.ps1 -Mode Vertical` ejecuta regresión. `-TestFilter Aethernia.Tests.Editor.ExpeditionIntegrationTests` prueba grupo/mazmorra; CampaignIntegrationTests prueba composición reducida y derrota. BuildExpeditions compila la escena nueva. `Tools/Invoke-ExpeditionSmoke.ps1` usa copias aisladas de partidas E13 versionadas y lanza un segundo proceso para verificar persistencia. `-WithGraphics -Record -Classes warrior` añade capturas/cámara real y audio DSP. Los flags de QA no se usan para jugar normalmente. No confundir una ejecución automática con una prueba humana.
