# El cargamento perdido — misión provisional

Escena: `Assets/01_Scenes/AetherniaVillageDemo.unity`. Copia independiente de PlayablePrototype con su D01, camino/casa/almacén/decoración/portales/recorte de techos y NavMesh existentes. La escena principal permanece intacta. NPC «Habitante de la aldea», misión y diálogo son contenido de ensayo autorizado, no canon aprobado.

## Recorrido manual

1. Play; seleccionar clase en Personaje y cerrar la ficha. Seleccionar al héroe con clic izquierdo y caminar con clic derecho.
2. Buscar el habitante junto al camino (etiqueta visible); acercarse, seleccionarlo y pulsar E o Interactuar. Leer el diálogo y pulsar **Aceptar misión**.
3. El panel de objetivo indica buscar el arcón al oeste de la casa. Acercarse a «Cargamento perdido», seleccionarlo y activar E/Interactuar. Se inicia un encuentro RPG real.
4. Usar Acercarse o caminar el tramo permitido, esperar llegada, Atacar/Guardia/Recuperar y Fin de turno conforme a las reglas existentes. No hay botón de victoria. Al ganar, se registra carga recuperada y se vuelve a exploración. Una derrota conserva el comportamiento previo: salir de combate, elegir clase si se necesita reiniciar salud y reintentar la misión aceptada.
5. Regresar al habitante, interactuar y **Entregar cargamento**. Recompensa una sola vez: 20 oro y una ración si cabe; con inventario lleno se entrega solo el oro y se informa. No se pierde la misión por falta de espacio.
6. Guardar/cargar desde Menú o F5/F9 fuera de combate, tanto aceptada como recuperada o completada. Repetir la conversación no paga otra vez. Probar también las otras clases, construcción y RTS en esta escena.

## Integración y persistencia

Se reutilizan InteractionPoint, PlayerCommands, GameSession, TurnEncounter/TurnBattle, WorldOperations, inventario y guardado v1. LostCargoQuest es una extensión específica de la misión sobre la lista Facts existente, no un gestor paralelo genérico. Hechos: lost-cargo-accepted, lost-cargo-recovered y lost-cargo-completed. No se cambian identificadores de clases/items, enumeraciones persistentes, Version=1 ni campos del guardado. Partidas anteriores sin esos hechos muestran la misión disponible.

GameSession ofrece un punto opcional de interacción con devolución nullable; los IDs ajenos siguen su ruta previa. LostCargoController registra y libera ese enlace y el evento de resolución del encuentro. Solo una victoria real del encuentro iniciado en el cargamento registra recuperación; el hecho genérico rpg-victory o un combate ajeno no completa esta misión. La generación de enemigo se sitúa lateralmente para no atravesar el arcón. La semilla de reproducción de pruebas vale 0 normalmente (aleatoriedad del motor) y no cambia estadísticas/reglas.

LostCargoView muestra diálogo/aceptación/entrega y objetivo actual con tema C02, sin modificar su gestor de ventanas. Oculta el objetivo mientras se usa una ventana C02 y cierra solo su diálogo con su botón/ESC. Integrar el objetivo en el Registro central queda para una revisión posterior del gestor; el panel independiente ya presenta nombre, objetivo y estado. El cierre X/ESC C02 conserva su pendiente humano F2-07.

Prefabs reutilizables: Quests/VillageResident, LostCargo y CargoDialogue; NPC usa visual E01 existente, arcón usa D01/Props/Chest_80x80x60cm. Colliders y obstáculos de carving pertenecen solo a la escena demo: NPC 0,50×1,80 m, arcón 0,80×0,80×0,65 m; no se altera el bake compartido. Etiquetas World Space se dibujan detrás del HUD. No hay IA social, diálogo ramificado, animaciones nuevas, edificios o mapa extenso.

## Evidencia

Pruebas iniciales dirigidas 5/5 aprobadas: reglas/guardado/compatibilidad, recorrido completo con cada clase y rechazo de progreso por victoria ajena o sin aceptar. Incluyen ruta NavMesh real, diálogo, combate real, retorno automático, pago único, persistencia de cada etapa y caso de inventario lleno. Primera autoría detectó ruta errónea del cofre; corregida a Props reutilizando el NPC creado. Capturas detectaron etiquetas superpuestas sobre diálogo; se ajustó su orden de dibujo. Evidencia inicial E05-first-* en Logs; cierre final se registra en Docs/Historial/Etapas/E03_E06_PRODUCCION.md.
