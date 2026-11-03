# UX-01 — Candidata en validación

Entrega todavía en desarrollo. Base E19 conservada; rama `feature/ux01-onboarding-camera-world`, [PR11 en borrador](https://github.com/MathiasMerino/RPGRTS/pull/11) sobre E19/PR10. Sin fusiones ni cambios main/dev.

La nueva escena es `Assets/01_Scenes/AetherniaUx01Demo.unity`. Reutiliza las cuatro razas, tres clases, estado del mundo y sistemas de E19. Su configuración es optativa: las escenas anteriores conservan su entrada habitual.

## Cambios implementados

- Menú, creación e introducción breve → exploración segura → habitante/cargamento → RPG real → recompensa única → patrulla segura → selección/movimiento/reagrupación → ataque ordenado → campamento, producción y reclutamiento → mundo libre existente.
- Seguimiento isométrico suave, anticipación pequeña, zoom limitado y encuadres por modo. Un solo controlador; WASD de exploración usa la velocidad derivada de raza/clase.
- Un candidato E cercano con histéresis y objetivo asociado al objeto real. Modalidad de ficha/ayuda/diálogo bloquea motor, cámara e interacción. El teclado funciona independientemente de la presencia de ratón.
- HUD contextual con recursos cuando son necesarios, ventanas anteriores conservadas, aviso temporal único, orientación ocultable y controles RTS compactos.
- Preparación RTS sin daño; hitos requieren llegada real y el ataque requiere orden explícita. Las órdenes de movimiento no caducan arbitrariamente a los tres segundos en UX. Se conservan salud, bajas y resolución auténticas.
- Persistencia aditiva de hitos, posiciones y selección de preparación. Continuar reconstruye la formación sin duplicar tropas; partidas avanzadas E19 conservan su progreso y ejército cultural.
- Corregido el contrato de rig para proporciones raciales registradas, manteniendo rechazo de escalas accidentales y escala física del actor.
- Escenario occidental con camino de4m y recursos compartidos. Una copia de Grass E16 aplana sólo campo/camino para impedir que colinas decorativas oculten cuerpos sobre navegación plana; original y escenas anteriores intactos. Campo físico40×30m, fondo visual64×60m.

## Evidencia disponible y pendiente

Dirigidas anteriores8/8PASS: tres clases, legado E19, doce variantes visuales, bloqueos modales y guardado/Continuar durante preparación. Los ensayos del editor usan las API del motor/interactor; no se presentan como pulsaciones físicas.

Windows preliminar4/4PASS: Humano–Guerrero y Enano–Mago, Nueva/recorrido completo y reinicio/Continuar. WASD y E verificados en player. Esta build precede a la corrección de superficie; **no es la entrega visual final**.

En curso: dirigidas con superficie corregida, capturas finales, nueva build, repetición nativa, ZIP/portable, regresión completa y actualización final de este informe. No se declara cerrado UX-01.

Los fallos previos se conservan en `Docs/Validacion/UX01`. La muerte del héroe en RTS con aliados supervivientes es una derrota real: no se cambiaron HP/daño para obtener victorias. La incidencia histórica de la prueba de reserva RTS sigue abierta y su aserción no se modifica. Evaluación humana de comodidad, arte y balance pendiente.

Consulta [GUIA_UX01](GUIA_UX01.md) y [decisiones de experiencia](UX01_EXPERIENCIA.md). No se inicia E20.
