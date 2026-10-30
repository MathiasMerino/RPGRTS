# Arquitectura del prototipo

Los componentes del prototipo separan configuración, estado mutable, reglas, escena y presentación. Las decisiones se mantienen pequeñas y reemplazables para ampliar el juego sin convertir los datos de prueba en sistemas definitivos.

| Responsabilidad | Archivos principales | Dependencias |
| --- | --- | --- |
| Configuración de clases | PrototypeRules y ApprovedRules.asset | ScriptableObject de solo configuración |
| Estado de partida | PrototypeState, ItemStack y BuildingRecord | Datos serializables; no modifica assets |
| Inventario/economía/consecuencias | WorldOperations | Operaciones sobre estado con costes y límites |
| Persistencia | PrototypeSaveStore | JSON versionado; valida antes de cargar y usa escritura temporal para guardar |
| Combate por turnos | TurnBattle | Reglas separadas de escena y presentación; semilla controlable |
| Representación de encuentros | TurnEncounter | Actor, enemigo y sincronización del resultado |
| Cámara | IsometricCameraRig | Input System y límites configurables |
| Navegación/selección | ExplorationUnit y PlayerCommands | NavMeshAgent, rutas completas y rayos por pantalla |
| Interacción | InteractionPoint | Alcance y solicitud al estado compartido |
| Construcción | WorldConstruction | Validación espacial, costes, prefab y carving de navegación |
| RTS | RtsBattleController | Lista acotada de unidades, órdenes de grupo y ciclo de combate simplificado |
| Coordinación | GameSession y PrototypeDirector | Eventos de estado, modos, viajes y conexiones entre módulos |
| Presentación | ExplorationHud y VerticalSliceHud | Eventos y enlaces de UI; no almacenan las reglas del juego |
| Estilo/controles UI | MedievalUiTheme y UiInputGuard | Prefabs comunes, colores/fuente y bloqueo de entradas del mundo |
| Herramientas de autoría | TechnicalSandboxBuilder, PlayablePrototypeBuilder y VerticalSliceBuilder | Solo editor; generan o amplían salidas conocidas sin reemplazar las escenas originales |
| Validación | Aethernia.Editor.Tests y BuildSmokeProbe | Pruebas de modelo, Play Mode, capturas y recorrido del ejecutable |

La lógica de tiempo pausa la economía durante RPG/RTS según la regla provisional aprobada. Los hechos de misión y batalla, reputación, recursos, equipo y construcciones pertenecen al mismo estado guardable. La UI recibe eventos y mantiene sus suscripciones durante activación/desactivación.

Los prefabs de personaje, cámara, paneles y botones son reutilizables. Los materiales URP y el tema se comparten. Los scripts de editor y de pruebas viven fuera del ensamblado runtime; la build conserva únicamente la sonda de validación de desarrollo, que se activa mediante un argumento explícito.

No se incorporó un contenedor de dependencias ni una jerarquía de gestores globales. Las referencias entre componentes se asignan al crear la escena. La batalla RTS usa doce unidades y una frecuencia de decisión acotada; cualquier cambio para ejércitos mayores requiere medir antes de introducir pooling, particiones espaciales u otras optimizaciones.

En fase 2 se explicita el ciclo de vida: los módulos de combate poseen y retiran sus participantes transitorios; el director los cierra al volver al mundo o cargar. El hecho de victoria pertenece a PrototypeState y no depende de conservar soldados. El despliegue RTS se valida completo antes de crear participantes/cambiar modo. ExplorationUnit comparte cálculo de rutas entre movimiento continuo y paso instantáneo del enemigo por esquinas de NavMesh, sin trasladar reglas RPG al controlador de cámara/UI. Los presentadores muestran los rechazos y equipo; no sustituyen las barreras de modo de la lógica. La validación de guardado exige coherencia entre reloj y ciclo de producción para impedir reproducir ciclos anteriores al cargar.
