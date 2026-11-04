# E20.1 — Estabilización y candidata alfa

## Patrulla

La prueba histórica a 30 FPS reprodujo la caída del Guerrero y dejó el capítulo en `FirstRts`. La última posición de reserva quedaba dentro de los 6 m de alcance del arquero. Revisando el controlador se encontraron dos limitaciones que prolongaban la exposición. La patrulla insistía en un objetivo bloqueado y calculaba aproximaciones hacia el mismo lado, aunque hubiera otro enemigo alcanzable o una posición lateral disponible.

La patrulla conserva el objetivo ordenado para la marcha y puede responder a contactos hostiles visibles al alcance. Reutiliza la línea de visión de E20, que respeta la cobertura y los cuerpos rivales. Busca posiciones de ataque navegables y evita reinstalar el mismo camino mientras está en marcha. La orden explícita de ataque reemplaza la guardia anterior. La preparación sigue exigiendo llegada a los hitos y confirmación del jugador. La victoria sigue exigiendo que el héroe sobreviva y que los rivales caigan.

La misma prueba histórica pasó con Guerrero a 23 PV, Monje a 19 PV y Mago a 10 PV. Se conservan los perfiles, daño, recuperación y reglas de progresión. El ajuste actúa sobre la ejecución táctica, con mediciones del recorrido nuevo separadas por clase y raza.

## HUD y transiciones

El HUD UX usa un único control de interacción centrado. El componente anterior podía volver a mostrar el botón cuando llegaba un mensaje de sesión, entre dos actualizaciones del HUD, y lo dejaba parcialmente fuera del encuadre. El aviso actual permite clic o **E** y deja pasar el puntero cuando no hay objetivo. El resultado de patrulla y conquista se anuncia después de un guardado correcto para que el aviso del guardado no lo tape.

## Guardado

El formato de guardado y las referencias de la escena se conservan. El proceso de Continuar comprueba identidad, nivel, experiencia, equipo, habilidades, inventario, recursos, edificios, tropa, conquista y hechos únicos con un proceso nuevo. La compatibilidad con partidas UX‑02 se verifica en pruebas dirigidas.

## Revisión humana y siguiente revisión

Usa la guía para comparar la patrulla de las tres clases, la lectura del HUD y la oclusión en una pantalla física. Después de esa revisión, prioriza ajustes de aprendizaje y trazas comparables de las maniobras antes de ampliar el ejército o añadir regiones. E20.1 mantiene el alcance de estabilización.

[Guía](GUIA_E201.md) · [Checklist](../CHECKLIST.md)
