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

En C1 de fase 3, VerticalSliceHud posee únicamente la sección de exploración seleccionada; no se guarda como estado del mundo. Los HudBinding conservan comandos existentes y añaden navegación de presentación. Textos se cachean al activar y listeners se liberan al desactivar. MedievalTheme comparte fuente/tamaños/espaciado; ContextUiBuilder aplica el layout una vez desde editor con los prefabs existentes. ExplorationHud oculta su control fuera de exploración.

E01 implementa el contrato VisualRoot: CharacterVisualCatalog comparte configuración, CharacterVisualPresenter observa eventos de GameSession y CharacterVisualModel presenta el equipo real. No modifica reglas, movimiento, collider/agente ni el esquema de guardado. Solo se crea una instancia nueva al cambiar la clase; si la entrada no tiene binding válido se conserva la cápsula y no se acumulan modelos. Las suscripciones se liberan al desactivar y la presentación se revierte. Los rigs/Animator futuros pertenecen al modelo, sin duplicar el controlador raíz. RoofCutaway es un componente independiente que lee cámara/objetivo y oculta renderers de cubierta de forma reversible; no usa física ni modifica materiales. Fichas y límites en Docs/E01_PERSONAJES.md.

E02 añade HumanoidPrefab opcional al catálogo: solo se elige con rig/skin utilizables; E01 permanece como alternativa. HumanoidVisualRig guarda referencias visuales, HumanoidAnimationProfile comparte datos y HumanoidAnimationDriver lee el NavMeshAgent, sin root motion ni efectos de combate. Un perfil sin Idle aplicado mantiene Animator apagado. CharacterVisualModel admite bindings por función que observan Weapon/Armour, conservando sus campos E01 y el guardado v1. Ropa skinned debe venir preparada para el mismo esqueleto; no hay rebinding/retargeting automático de ropa. La importación/auditoría/fábrica de candidatos y generación de controller viven en una assembly exclusiva de editor. Los siete estados vacíos son infraestructura, no clips. Detalles y pendientes: Docs/E02_IMPORTACION_HUMANOID.md.

E02-B sustituye solo Prefab de la entrada monk por un proxy de mallas horneadas por material; HumanoidPrefab conserva el candidato vacío E02. Catálogo añade ReferenceArt/ReferenceUv opcionales y FindEntry; ClassReferenceCard muestra esos datos y estadísticas base dentro del panel existente. VerticalSliceHud conserva selección/eventos y decide visibilidad; la tarjeta no añade controlador, estado guardable o suscripciones. Equipo anidado E01 y CharacterVisualModel se reutilizan. MonkIdentityBuilder/MonkProxyGeometry viven solo en editor; ninguna malla se genera en ejecución. Para un FBX válido posterior se usa HumanoidPrefab manteniendo fallback. Informe: Docs/E02B_MONJE_PROVISIONAL.md.
