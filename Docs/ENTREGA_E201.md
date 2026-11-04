# E20.1 — Estabilización y candidata alfa

## Patrulla

La prueba histórica a 30 FPS reprodujo la caída del Guerrero y dejó el capítulo en `FirstRts`. La última posición de reserva quedaba dentro de los 6 m de alcance del arquero. Revisando el controlador se encontraron dos limitaciones que prolongaban la exposición. La patrulla insistía en un objetivo bloqueado y calculaba aproximaciones hacia el mismo lado, aunque hubiera otro enemigo alcanzable o una posición lateral disponible.

La patrulla conserva el objetivo ordenado para la marcha y puede responder a contactos hostiles visibles al alcance. Reutiliza la línea de visión de E20, que respeta la cobertura y los cuerpos rivales. Busca posiciones de ataque navegables y evita reinstalar el mismo camino mientras está en marcha. La orden explícita de ataque reemplaza la guardia anterior. La preparación sigue exigiendo llegada a los hitos y confirmación del jugador. La victoria sigue exigiendo que el héroe sobreviva y que los rivales caigan.

La misma prueba histórica pasó con Guerrero a 23 PV, Monje a 19 PV y Mago a 10 PV. Se conservan los perfiles, daño, recuperación y reglas de progresión. El ajuste actúa sobre la ejecución táctica, con mediciones del recorrido nuevo separadas por clase y raza.

## Balance del recorrido nuevo

| Identidad en el ZIP | Inicio | Tras primer RPG | Despliegue de patrulla | Victoria de patrulla | Tras conquista |
|---|---:|---:|---:|---:|---:|
| Guerrero humano | 26/26 | 17/29 | 29/31 | 15/31 | 23/34 |
| Monje elfo | 20/20 | 11/22 | 22/24 | 9/24 | 10/26 |
| Mago enano | 16/16 | 2/18 | 14/20 | 18/20 | 18/22 |

Cada ruta ganó la patrulla al primer intento. El Guerrero obtuvo 27/31 PV en la build original y 15/31 en la extracción del mismo binario. La simulación puede producir resultados diferentes. El Monje pasó con 9/24, frente al caso de 1 PV de E20. El Mago quedó con margen estrecho en el primer RPG. En ese tramo el recorrido fija la semilla y usa **Acercarse**, que lleva al héroe a distancia corta aunque el Mago pueda atacar a distancia. Queda pendiente comparar ambas tácticas con personas y más semillas antes de ajustar las estadísticas.

El daño de ataque inicial observado fue 7, 6 y 9 respectivamente. La reducción de Guardia fue 1, 1 y 2. Son perfiles de clase más raza. El Guerrero tiene más vida, el Monje conserva la movilidad y su técnica marcial, y el Mago conserva alcance y técnica arcana. La recuperación de hasta 4 PV, las raciones existentes y el presupuesto de acciones siguen vigentes. En la prueba histórica los tres héroes de reserva usaron una recuperación de 4 PV. Las trazas nuevas registraron 0 de daño al héroe y 0 producido por el héroe en esa estrategia, pues combatió la patrulla. [Datos y tiempos por etapa](Validacion/E201/BALANCE.json) · [Trazas históricas](Validacion/E201/HISTORICAL.json)

## HUD y transiciones

El HUD UX usa un único control de interacción centrado. El componente anterior podía volver a mostrar el botón cuando llegaba un mensaje de sesión, entre dos actualizaciones del HUD, y lo dejaba parcialmente fuera del encuadre. El aviso actual permite clic o **E** y deja pasar el puntero cuando no hay objetivo. El resultado de patrulla y conquista se anuncia después de un guardado correcto para que el aviso del guardado no lo tape.

## Guardado

El formato de guardado y las referencias de la escena se conservan. El proceso de Continuar comprueba identidad, nivel, experiencia, equipo, habilidades, inventario, recursos, edificios, tropa, conquista y hechos únicos con un proceso nuevo. La compatibilidad con partidas UX‑02 se verifica en pruebas dirigidas.

## Evidencia y build

- 17/17 dirigidas de HUD, mundo y E20. La prueba histórica separada pasó de 0/1 a 1/1 con sus aserciones originales.
- Windows x64 en `f9da1b3`. Guerrero desde la build original, 2/2 procesos. Guerrero, Monje y Mago desde la extracción, 6/6 procesos con Nueva partida y Continuar. La automatización de E20.1 ejecutó una sola tentativa de patrulla por partida y ejercitó entradas de teclado y ratón, controles UI y acciones públicas del juego.
- 33 capturas reales. Interacción verificada a 1280×720, 1920×1080 y 2560×1440 en Play Mode. La captura del ejecutable a 1280×720 confirma el botón completo. La revisión de todas las pantallas y resoluciones con una persona sigue pendiente.
- ZIP con CRC, SHA‑256 por archivo y los 23 ZIP previos verificados. El paquete inicia desde `Aethernia.exe` y contiene la guía. El contenido del ZIP se comprobó sin partidas preparadas ni lanzadores de salto.

[Resultados Windows](Validacion/E201/WINDOWS.json) · [Integridad del ZIP](Validacion/E201/PORTABLE.json) · [Interacción nativa](Images/E201/Portable/warrior-07-NpcPrompt.png) · [Comparación anterior](Images/UX02/Windows/warrior-W01-ForestFork.png) · [Capturas](Images/E201)

## Revisión humana y siguiente revisión

Usa la guía para comparar la patrulla de las tres clases, la lectura del HUD y la oclusión en una pantalla física. Después de esa revisión, prioriza ajustes de aprendizaje y trazas comparables de las maniobras antes de ampliar el ejército o añadir regiones. E20.1 mantiene el alcance de estabilización.

[Guía](GUIA_E201.md) · [Checklist](../CHECKLIST.md)
