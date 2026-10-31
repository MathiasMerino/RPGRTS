# Plan de desarrollo de Crónicas de Aethernia

Actualizado: 2026-10-30, America/La_Paz. Estado: fase 3, primera iteración A/B/C1 entregada; D/E/C2 y evaluación humana F2-07 pendientes.
El usuario indicó «continua con el prompt» después de recibir este plan. Se continúa con la base técnica y sus verificaciones; las reglas y sistemas todavía pendientes de diseño mantienen sus decisiones abiertas.

## Avance verificado

Al 2026-10-29 están cerradas las 26 tareas del checklist dentro del alcance de prototipo y planificación aprobado. La base jugable y el prototipo vertical incluyen cámara, personaje, selección/movimiento, interacción, UI reutilizable, tres clases, guardado, equipo, región/mazmorra, facciones/consecuencias, turnos, economía, construcción y una batalla RTS con escalada experimental opcional. La suite final pasó 23/23 y la build de Windows completó su recorrido automático independiente con salida 0.

Consultar `COMO_JUGAR.md` y `Docs/VALIDACION_PROTOTIPO.md` para ejecutar, repetir las pruebas y conocer los límites. `Docs/ESPECIFICACION_PROTOTIPO.md` y `Docs/PLAN_EXPANSION.md` fueron aprobados por el usuario. Este cierre no significa que el continente, contenido narrativo, arte ni sistemas avanzados del videojuego comercial estén terminados: pertenecen a la expansión planificada.

## Diagnóstico inicial

El proyecto está en `ProyectoRPGRTS/`. Su nombre técnico real es `ProyectoRPGRTS`, no `PROYECTO`; no se renombra. La visión jugable está documentada, pero el contenido corresponde todavía a una plantilla URP organizada.

| Elemento | Evidencia observada |
| --- | --- |
| Editor | Unity 6000.5.8f1, según ProjectVersion.txt; instalado junto con 6000.5.5f1 |
| Render | URP 17.5.0; perfiles PC/Mobile; GraphicsSettings referencia PC_RPAsset |
| Escenas | Solo `Assets/01_Scenes/SampleScene.unity`, habilitada en EditorBuildSettings |
| Escena actual | Main Camera, Directional Light, Global Volume; cámara en perspectiva y rotación cero; sin escenario jugable |
| Código y assets | Ningún `.cs` ni prefab propio; `02_Prefabs`, `03_Scripts` y `04_Sprites` vacías |
| GDD | `Assets/05_Docs/GDD_GAMERPGTS.docx`, versión 0.2, leído íntegramente |
| Entrada | Input System activo; mapas Player y UI de plantilla, sin control isométrico propio |
| Navegación | Paquete presente; escena sin NavMeshData horneado |
| Serialización | Force Text (`m_SerializationMode: 2`); assets con `.meta` |
| Versionado inicial | No existía Git; Unity conserva proveedor Unity Version Control; paquete instalado no demuestra repositorio conectado |
| IDE | Visual Studio Community 2026 18.10.3; workload ManagedGame y componente Unity comprobados con vswhere |

Paquetes directos relevantes comprobados en manifest y lock:

| Paquete | Versión |
| --- | --- |
| URP | 17.5.0 |
| Input System | 1.20.0 |
| AI Navigation | 2.0.14 |
| Visual Studio Editor | 2.0.26 |
| Test Framework | 1.7.0 |
| uGUI | 2.5.0 |
| Timeline | 1.8.12 |
| Visual Scripting | 1.9.12 |
| collab-proxy / Unity Version Control | 2.13.5 |
| Rider Editor | 3.0.38 |
| Multiplayer Center | 1.0.1 |

Los paquetes están en caché. No se actualizan ni eliminan: Multiplayer Center no convierte el juego en multijugador. No se ha validado una compilación actual ni Play Mode. El log histórico contiene avisos de token de licencia y de puerto del depurador; comprobarlos si reaparecen, sin considerarlos bloqueos actuales confirmados. La búsqueda específica no encontró errores C# en ese log; esto no demuestra una compilación actual correcta.

## Análisis del GDD

Lectura completa de 306 párrafos XML del cuerpo, incluidas dos tablas. Las notas y el pie no contienen texto sustantivo; el cuerpo no contiene imágenes ni inserciones/eliminaciones controladas. `Docs/GDD_LECTURA.md` conserva texto y orden, sin reproducir la maquetación. El Word original sigue siendo la referencia principal.

| Sección | Definición y estado | Implicación |
| --- | --- | --- |
| 1. Visión | Libertad, profundidad de rol, mundo vivo, conexión RPG/estrategia y consecuencias persistentes | No imponer profesión, destino ni conquista obligatoria |
| 2. Personajes | Personalizados y de origen; clases consideradas: Guerrero, Bárbaro, Paladín, Clérigo, Mago, Hechicero, Brujo, Druida, Explorador, Pícaro, Bardo y Monje | Reglas, razas y tres clases iniciales pendientes; Monje en catálogo no implica seleccionarlo para el prototipo |
| 3.1. Exploración | Isométrica 3D, personaje y compañeros, NPC, localizaciones, ciudades, mazmorras, objetos y decisiones | Construir base técnica antes del contenido y las pruebas de habilidad |
| 3.2. RPG | Turnos, iniciativa, acciones/movimiento, dados, atributos, magia, estados y terreno | Reglas exactas, niveles y equilibrio pendientes; no adoptar reglas de D&D automáticamente |
| 3.3. RTS | Ejércitos, reclutamiento, escuadrones, formaciones, infantería, distancia, caballería, comandantes, asedios, moral y suministro | Estadísticas militares simplificadas; no simular cálculos RPG completos por soldado |
| 3.4. Construcción | Terrenos, asentamientos, viviendas, talleres, edificios militares, defensas, trabajadores y producción | Catálogo, costes y colocación pendientes |
| 3.5. Economía | Oferta/demanda, mercados regionales, rutas, producción/consumo, caravanas, negocios y vínculos entre facciones | Empezar con reglas acotadas; no implementar todo el mercado continental |
| 3.6. Política | Facciones, alianzas, tratados, conquista y administración | Impuestos, vasallaje, sucesión, leyes y gobierno sin especificar |
| 4. Mundo | Mapa continental estratégico y regiones 3D; geografía principal manual, cambios dinámicos | No asumir mundo continuo sin cargas ni generación procedural de todo el mapa |
| 4.2. Regiones | Valdoria, Kharum, Eryndor, Puerto Ámbar, Marcas de Ceniza y Dominio del Velo propuestos | Nombres, culturas y conflictos no definitivos |
| 4.3. Tiempo | Calendario compartido entre exploración, viajes, combate, economía y guerra | Velocidad y sincronización de turnos/RTS deben definirse antes de implementarse |
| 5. Narrativa | Historia central opcional; El Silencio de los Umbrales es provisional | No escribir ahora trama, actos, misiones ni desenlaces |
| 6. Compañeros | Sistema confirmado, personajes pendientes; historias, opiniones, conflictos, abandonos y misiones | Aprobación, confianza, lealtad y afinidad ideológica son dimensiones propuestas |
| 7. Progresión | Se contemplan poder individual, reputación, influencia y patrimonio | No reducir progreso a combate; fórmulas pendientes |
| 8. IA/simulación | NPC, facciones, diplomacia, comercio, ejércitos, política, asentamientos y regiones inactivas | Simulación simplificada; no simular cada habitante |
| 9. Arquitectura | Lista preliminar de 16 sistemas: juego, mundo, guardado, exploración, personajes, RPG, RTS, inventario, misiones, facciones, economía, construcción, eventos, UI, cámara e IA | No exige 16 gestores ni singletons; arquitectura final después de requisitos |
| 10. Primera versión | Región, creación básica, tres clases, asentamiento, mazmorra, turnos, dos facciones, economía, construcción, batalla RTS y misión persistente | Prototipo vertical posterior a la base técnica; volumen ajustable según viabilidad |
| 11. Pendientes | Razas, atributos, clases/subclases, niveles, magia, objetos, equipamiento/fabricación, enemigos, culturas/religiones, historia, economía, diplomacia, edificios, organización militar, compañeros, misiones, UX, arte/audio, guardado, balance y producción | Abrir decisiones antes de implementar cada sistema dependiente |
| 12.1. Arte | Fantasía medieval realista y parcialmente oscura, confirmada conceptualmente | Proporciones realistas; detalle, materiales, iluminación y rendimiento sujetos a pruebas |
| 12.2. Integración | Estado compartido y consecuencias comprensibles, confirmado | Conexiones por hechos claros; no toda acción afecta a todos los sistemas |
| 12.3. RPG a RTS | Transiciones ocasionales por escalada, propuestas/experimentales | Experimento posterior; no obligatorias en cada combate ni requisito de la base |
| 12.4. Libertad | Amplia con límites coherentes, confirmada | «80 %» orientativo; no métrica de mapa ni aceptación técnica |
| 12.5–12.6. UI | Identidad medieval y controles clásicos PC confirmados conceptualmente | Paneles/distribución pendientes; ratón, cámara, zoom, atajos y grupos sujetos a pruebas |

## Fases y dependencias

| Fase | Tareas | Salida |
| --- | --- | --- |
| F0. Preparación | PRE-01 a PRE-05 | GDD, documentación, Git y aprobación del plan |
| F1. Entorno/escala | BAS-01 a BAS-03 | Editor comprobado, medidas y escena de pruebas |
| F2. Exploración | BAS-04 a BAS-07 | Cámara, personaje, selección/movimiento e interacción |
| F3. Base jugable | BAS-08 y BAS-09 | UI reutilizable y recorrido probado en editor/build |
| F4. Diseño vertical | VER-01 y VER-02 | Reglas aprobadas, estado y guardado mínimo |
| F5. Personaje/RPG | VER-03 a VER-05 | Creación con tres clases elegidas, objetos y encuentro por turnos |
| F6. Región | VER-06 | Asentamiento, mazmorra, dos facciones y misión persistente aprobada |
| F7. Gestión | VER-07 y VER-08 | Economía simplificada y construcción básica |
| F8. RTS | VER-09 | Una batalla acotada, órdenes y resultado persistente |
| F9. Integración | VER-10 | Recorrido vertical completo, perfilado y evaluación de viabilidad |
| Experimento | EXP-01 | Evaluar transición RPG–RTS y decidir conservar/modificar/descartar |
| Largo plazo | FUT-01 | Descomponer expansión tras evaluar el prototipo |

F1–F3 son la base jugable priorizada en el prompt. F4–F9 son el prototipo vertical del GDD, no el alcance de la siguiente actividad. No se fijan fechas ni presupuestos sin medir el trabajo real.

## Arquitectura propuesta

Conservar las carpetas existentes y añadir subcarpetas funcionales solo al necesitarlas. Separar lógica comprobable, componentes Unity y presentación. Usar ScriptableObjects para configuración; mantener el estado mutable y los identificadores de guardado independientes. Cada evento tendrá propietario y ciclo de suscripción. Evitar gestores globales y frameworks anticipados.

Probar AI Navigation para el movimiento inicial: superficie, pendientes, obstáculos, parada y clic inválido. Evaluar multitudes RTS después con sus volúmenes reales. Probar cámara ortográfica para la lectura isométrica, sin presentarla como requisito del GDD.

Separar acciones de cámara, órdenes y UI; un clic sobre paneles no debe mover al personaje. Mantener contratos pequeños de selección e interacción. Añadir un sistema de modos cuando exista un segundo modo real. Diseñar sincronización temporal antes de integrar combate y simulación.

## Escala inicial propuesta para BAS-02

1 unidad = 1 metro, por instrucción del usuario. Los valores siguientes son referencias de prueba, no medidas definitivas del GDD. Ejes locales X ancho, Y alto, Z fondo; comprobar huella y espacio libre.

| Elemento | Referencia inicial | Verificación |
| --- | --- | --- |
| Cubo patrón | 1 × 1 × 1 m | Escala de importación y comparación visual |
| Humano provisional | 1,80 m alto; radio de colisión 0,30 m | Cápsula, pivote en pies y paso libre |
| Puerta peatonal | Hueco 1,20 m ancho × 2,20 m alto | Paso con collider/agente y margen |
| Ventana | Hueco 1 × 1,20 m; antepecho a 1 m | Proporción con fachada; no asumir paso navegable |
| Calle local | 4 m libres | Cruce y giro de dos agentes |
| Casa de prueba | Huella 6 × 8 m; planta de 3 m | Coherencia entre puerta, suelo y colisiones |
| Terreno de pruebas | 40 × 40 m | Área abierta, paso estrecho y obstáculo |
| Equipamiento | Espada provisional de 1 m de largo | Encaje con mano y cuerpo |
| Caballería/portón | Conjunto 3 m largo × 1 m ancho × 3 m alto; hueco 4 × 4 m | Reservado para RTS; validar con modelo y formación reales |

Mantener escala raíz `(1,1,1)` cuando sea viable y documentar importaciones. La escena tiene agente por defecto de radio 0,5 m y altura 2 m; no copiarlo silenciosamente al humano propuesto. Ajustar collider y navegación conjuntamente.

## UI reutilizable

Propuesta para la primera interfaz: uGUI, ya instalado y compatible con el flujo de prefabs para paneles/botones e indicadores del mundo. Centralizar colores, fuentes, tamaños, fondos e iconos. Separar presentadores y órdenes de la lógica. Construir primero un panel y un botón base, sin inventarios completos.

Evaluar UI Toolkit para herramientas de editor o pantallas de datos que se beneficien de UXML/USS cuando aparezca esa necesidad. Evitar dos frameworks para la misma pantalla inicialmente. BAS-08 comprobará resolución, legibilidad, navegación y bloqueo de clics antes de consolidar la decisión. No se afirma ventaja de rendimiento sin medir.

## Herramientas y contexto

Visual Studio Tools for Unity y el paquete editor ya están instalados; no reinstalarlos. VS Code existe y dispone de Codex, pero no necesita convertirse ahora en un segundo IDE C#. No se añaden plugins de IA ni servicios externos sin necesidad concreta. La búsqueda del catálogo no aportó una integración Unity/Visual Studio pertinente para este alcance.

`AGENTS.md` concentra acuerdos breves; el checklist guarda estado; la lectura Markdown permite consultar secciones. `.rgignore` evita recorrer cachés en búsquedas ordinarias y `.gitignore` evita versionarlas. Estas medidas acotan contexto y repetición; no se ha medido un porcentaje de ahorro.

Referencias oficiales consultadas el 2026-10-28: [AGENTS.md](https://learn.chatgpt.com/docs/agent-configuration/agents-md) y [Visual Studio Tools for Unity](https://learn.microsoft.com/en-us/visualstudio/gamedev/unity/get-started/getting-started-with-visual-studio-tools-for-unity). El inventario de versiones procede del equipo y los archivos locales.

## Riesgos y decisiones

1. Aprobar el plan y el primer hito antes de BAS-01. No comenzar mecánicas todavía.
2. Confirmar hardware, resolución y presupuesto de rendimiento antes de fijar métricas. PC es referencia de controles, no plataformas comerciales definitivas.
3. Elegir tres clases y reglas antes de VER-03/VER-05. Definir tiempo, persistencia y límites de región en VER-01.
4. Mantener contenido pequeño hasta demostrar un recorrido completo; el GDD combina muchos sistemas.
5. Compilación, Play Mode, render y build independiente se comprobaron para el prototipo aprobado. Las nuevas mecánicas o el contenido futuro necesitan sus propias pruebas; no heredan automáticamente esta validación.

Tras aprobar: abrir `ProyectoRPGRTS/` con Unity 6000.5.8f1, esperar importación, revisar Console y Package Manager, abrir SampleScene y entrar/salir de Play Mode. Crear después una escena de pruebas independiente y aplicar los criterios del checklist.

## Segunda etapa autorizada: validación e integración

La solicitud del 2026-10-30 autoriza auditar los doce sistemas, integrar lo existente y corregir fallos pequeños verificables. El orden operativo es F2-01 auditoría, F2-02 ciclo de vida de encuentros, F2-03 geometría táctica, F2-04 persistencia/mensajes, F2-05 regresión/revisión visual, F2-06 entrega y F2-07 evaluación humana. Véanse criterios y estados en CHECKLIST.md y resultados en Docs/AUDITORIA_FASE_2.md.

Resultado técnico al 2026-10-30: F2-01 a F2-06 verificadas, 34/34 pruebas y dos recorridos de la nueva build (sin gráficos/con GPU), todos con salida 0. Se corrigieron participantes residuales, órdenes entre modos, despliegue bloqueado, geometría/tránsito RPG, coherencia del guardado, mensajes/UI y activación del agente al arrancar. ZIP nuevo Aethernia-Fase2-Windows.zip, sin sustituir el ZIP histórico. Las capturas y la automatización no completan F2-07; el siguiente paso es una sesión humana y la selección de una mejora pequeña de UX, antes de aprobar expansión o reemplazo artístico amplio.

Se mantiene el estado compartido PrototypeState, los módulos existentes, la escala métrica y las escenas funcionales. No se implementan nuevos sistemas grandes, contenido narrativo ni transiciones complejas. Cambios mayores de arte, experiencia o sincronización requieren primero una propuesta. La evaluación humana es una puerta de salida para decidir la próxima iteración; no se declara superada mediante un smoke automático.

## Fase 3 — primera iteración A/B/C1

Solicitud autorizada el 2026-10-30: comenzar auditoría visual/dirección artística y una primera UI funcional. Fuente/decisiones en Documentacion/DIRECCION_ARTISTICA.md; auditoría/resultados en Docs/FASE3_ITERACION_1.md. Se mantiene uGUI por compatibilidad y reutilización demostradas. Primera mejora: secciones de exploración y ayuda por modo con los botones existentes. Escenario D, apariencias E y ampliación C2 quedan como tareas posteriores; sin producción masiva ni reglas nuevas. F2-07 continúa pendiente. Punto de control previo: tag codex/fase3-base-cc9691d.

Iteración A/B/C1 verificada: 35/35 pruebas, catorce layouts automatizados y ocho capturas representativas inspeccionadas; nueva build y dos recorridos independientes con salida 0. Entrega Aethernia-Fase3-UI-C1-Windows.zip. Siguiente tarea secuenciada: F3-D01 (muestra acotada de entorno); después E01 y C02, con validación propia. El resultado actual mejora acceso/contexto de UI y documenta la dirección; todavía no sustituye el escenario o los personajes provisionales.

## Fase 3 — D01 autorizado

El usuario autoriza una pequeña muestra medieval y biblioteca reutilizable, conservando todos los sistemas. Orden aplicado: checkpoint `codex/d01-base-8197b5f`, recorrido/captura previos, materiales y módulos propios, presentación sobre la física anterior, revisión comparativa, regresión y build. Alcance/resultados en Docs/D01_ESCENARIO_MEDIEVAL.md; medidas en Docs/ESCALA_REFERENCIA.md. La corrección del zoom involuntario es un ajuste pequeño verificado por una prueba de reproducción, sin nuevos controles. No se inicia E01, C02, ciudad extensa ni sistemas nuevos. F2-07 permanece pendiente; se recomienda probar la muestra antes de ampliarla.

## Fase 3 — E01 autorizado

La solicitud posterior autoriza tres apariencias de clase, contrato modular de modelos y solución reversible a la oclusión si se reproduce. Se conserva D01 y se parte de `codex/e01-base-ca5ba35`. Orden: ensayo de oclusión con triángulos reales, recorte de renderers validado, figuras métricas/configuración compartida, equipo visual conectado al estado, importación futura documentada y regresión/build. No se alteran clases/habilidades, guardado o navegación; no se inicia C02, razas/subclases ni animaciones definitivas. Fichas/estado real en Docs/E01_PERSONAJES.md. F2-07 permanece pendiente.

## E02 — Infraestructura y piloto humanoide

Autorizado preparar importación nativa FBX/Humanoid, Animator y equipo visual con Monje como piloto. Inventario sin modelo ni clips reales: construir candidato vacío, validación y fallback E01; no fabricar arte final o clips. F3-E02 registra infraestructura, F3-E02-M recurso/Avatar real y F3-E02-A clips/retargeting pendientes. Guía única y manifiesto exacto en Docs/E02_IMPORTACION_HUMANOID.md, reutilizando fichas E01/medidas. No cambiar movimiento, combate o esquema de guardado ni iniciar la fase siguiente. F2-07 permanece pendiente.

El encargo posterior «E02-A — Diseño artístico del Monje basado en referencias» autoriza únicamente una propuesta revisable. Ficha en Documentacion/Personajes/MONJE_DISENO_BASE.md y hoja conceptual 2D; seguimiento F3-E02-ARTE-01 y aprobación F3-E02-ARTE-AP para evitar colisión con el ID de animaciones. Talla propuesta 1,78 m sin aplicar al controlador de 1,80. No se modela, exporta, integra o anima el personaje en esta entrega; los recursos reales y F2-07 siguen pendientes.
