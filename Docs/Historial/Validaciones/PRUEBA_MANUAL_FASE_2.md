# Prueba manual de la fase 2

Estado al 2026-10-30: **pendiente de realización**. Este guion no es evidencia de una sesión realizada. Las pruebas automáticas y las capturas figuran por separado en AUDITORIA_FASE_2.md.

## Preparación en Unity

1. Abrir `ProyectoRPGRTS/` con Unity **6000.5.8f1** desde Unity Hub. No ejecutar el lanzador batch mientras ese editor tenga el proyecto abierto.
2. Esperar importación/compilación y revisar Console; abrir `Assets/01_Scenes/PlayablePrototype.unity`. No ejecutar generadores de escenas ya creadas.
3. En Game elegir 1280×720 y pulsar Play; repetir la revisión de UI en 1920×1080. Comprobar que no aparezcan errores nuevos en Console.
4. Para probar sin editor, extraer el ZIP completo de fase 2 y ejecutar Aethernia.exe. El guardado normal es manual (F5/F9); no usar la sonda automática para evaluar controles.

## Registro de sesión

- Fecha/hora y participante: pendiente.
- CPU/GPU/RAM, sistema y resolución/escala de pantalla: pendiente.
- Unity o ejecutable, commit/archivo usado: pendiente.
- Duración, experiencia previa y asistencia necesaria: pendiente.
- Incidencias: anotar pasos exactos, esperado/observado, frecuencia y captura/log si corresponde.

| Sistema | Acción humana | Criterio observable | Resultado real |
| --- | --- | --- | --- |
| Cámara | WASD/flechas, rueda, arrastre central; escribir nombre con WASD | Movimiento/zoom controlables, límites razonables, escribir y pulsar UI no mueve el mundo | Pendiente |
| Personaje | Seleccionar azul, clic derecho; intentar obstáculo/paso de 0,40 m | Respuesta clara, ruta evita barreras, sin vibración o atasco; comparar velocidad Monje | Pendiente |
| Creación | Probar Guerrero/Mago/Monje, nombre largo/vacío | Salud/velocidad/nombre visibles; entender que cambiar clase cura solo como facilidad de ensayo | Pendiente |
| Inventario | Cofre cercano/lejos/repetido, equipar espada/túnica, ración con daño | Recompensa única, equipo visible en panel, rechazo conserva razón, consumo comprensible | Pendiente |
| NPC/objetos | Mercado A y representante B; pulsar E y botón | Identificar objetivo, distancia y consecuencia; estos hitos no son NPC con diálogo/IA | Pendiente |
| Exploración | Portal gris, hito y salida verde; guardar en mazmorra | Encuadre permite orientarse y regresar; objetivo una sola vez | Pendiente |
| RPG | Alcance/cobertura, Acercarse/clic derecho, acción, recuperación/guardia y fin de turno | No atacar a través de muros; esperar llegada antes de terminar turno; comprender iniciativa/acción/movimiento y derrota | Pendiente |
| Comercio | Comprar madera con/sin oro | Precio y recursos legibles; no cobros fallidos | Pendiente |
| Construcción | Taller en terreno libre, sobre obstáculo/otro taller/cerca del héroe | Cobro solo válido, huella comprensible, camino evita edificio; observar 10 s de producción y agotamiento de alimento | Pendiente |
| Facciones | Comparar reputación antes/después del hito y tras cargar | A +10/B -5, sin inferir diplomacia/precios no implementados | Pendiente |
| RTS | Victoria RPG, escalada, seleccionar uno/todos, órdenes y conclusión | Comprender +4 salud, colores/órdenes/resultado; al salir no quedan soldados; evaluar sesgo del combate | Pendiente |
| Guardado | F5/F9, cambiar recursos/posición entre ambos, cargar desde región diferente | Recupera estado y edificios una vez; fallo explicado; durante combate no permite guardar/cambiar/cargar | Pendiente |

## Evaluación y decisión

Anotar dónde hizo falta explicación, errores de selección, texto ilegible/recortado, fatiga de cámara, esperas sin feedback y obstáculos que ocultaron objetivos. Valorar comprensión de escalada, ritmo, justicia y diversión con ejemplos, sin dar por aprobado un sistema por ausencia de excepciones.

La siguiente iteración debe elegirse después de esta sesión: primero fallos reproducibles; después una propuesta acotada de recorrido guiado/feedback y una muestra artística propia. No comenzar continente, nuevas clases, asedios o transiciones complejas sin revisar alcance.


## Actualización del usuario — E03–E06, 2026-10-31

El usuario informa 7/8 comprobaciones humanas confirmadas y cierre X/ESC pendiente. No se identifican las siete comprobaciones con las doce filas históricas de esta tabla. Se conserva su estado para evitar atribuir resultados no comunicados. Las pruebas automáticas de entrada de E06 se documentan por separado; no sustituyen la confirmación humana del cierre, la cámara, las proporciones o la experiencia jugable.
