# E02 — Importación Humanoid y piloto Monje

Fecha: 2026-10-30, America/La_Paz. Fuente: GDD v0.2, §12.1 y catálogo de clases; hash del Word conservado `dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661`. Las fichas artísticas de E01 siguen en [E01_PERSONAJES](E01_PERSONAJES.md); las medidas comunes en [ESCALA_REFERENCIA](ESCALA_REFERENCIA.md). No se duplica aquí la dirección artística.

## Estado real

Al comenzar E02, Assets no contenía FBX/modelos externos, Avatar, clips o Animator Controller. Se prepara infraestructura y un **candidato vacío**, no un Monje 3D terminado. El Monje geométrico E01 sigue disponible y activo como alternativa. No se descargan modelos, no se crean clips falsos y no se instalan paquetes/importadores.

Se conservan controlador, clases/habilidades, cápsula/agente, inventario, reglas, guardado v1, escena D01, recorte de techo y UI. No se inicia C02, arte definitivo, razas, subclases o criaturas. La sesión humana F2-07 sigue pendiente.

| Entrega | Estado |
| --- | --- |
| Configuración/auditoría FBX nativa, plantilla y bindings | Implementados; resultados exactos al final |
| Modelo humanoide real, orientación y deformación del piloto | Pendiente: falta recurso aprobado |
| Avatar Humanoid válido del Monje | Pendiente con el FBX real; no se inventa un esqueleto para simular aceptación |
| Siete estados y perfil de clips | Preparados; sin motions asignadas |
| Reproducción/retargeting de animaciones reales | Pendiente: faltan siete clips y Avatar real |
| Equipo desde Weapon/Armour y compatibilidad de guardado | Reutiliza las fuentes actuales; no añade ranuras de inventario |

## Componentes y recursos reutilizables

- `Assets/07_Characters/Humanoid/Animation/MonkAnimations.asset`: HumanoidAnimationProfile, referencias explícitas a siete clips y umbral de carrera de presentación.
- `Assets/07_Characters/Humanoid/Animation/HumanoidBase.controller`: estados Idle, Walk, Run, Attack, Defend, Hit y Defeat, actualmente vacíos.
- `Assets/02_Prefabs/Characters/Humanoid/Monk_ImportTemplate.prefab`: ModelRoot sin geometría/Avatar, Animator desactivado, HumanoidVisualRig/AnimationDriver/CharacterVisualModel y cinco sockets por función.
- Carpetas de entrada: `Assets/07_Characters/Humanoid/Models`, `Textures` y `Equipment`.
- `Runtime/Presentation/HumanoidVisualRig.cs`: comprueba Avatar/malla y escala. `HumanoidAnimationProfile.cs`: configuración compartida. `HumanoidAnimationDriver.cs`: lee velocidad del agente y controla únicamente parámetros visuales.
- `Editor/Characters/`: HumanoidImportPipeline, HumanoidPipelineBuilder y ventana HumanoidImportWindow. Assembly de editor independiente; no se incluye en el player.

CharacterVisualCatalog mantiene `Prefab` como alternativa E01 y añade `HumanoidPrefab` como candidato. Solo escoge un candidato utilizable con Avatar Humanoid válido y skin; la plantilla vacía no supera esa condición. El presentador existente crea el modelo bajo VisualRoot y le entrega la referencia del actor, sin reprogramar movimiento ni guardar referencias Unity en la partida. No se modifican los prefabs E01 para convertirlos en figuras supuestamente finales.

## Monje piloto: contrato de entrega

Conservar el concepto E01: atleta de anatomía humana, ropa monástica medieval sobria, lino/tejido oscuro, faja discreta y protecciones ligeras. Manos con dedos y pesos adecuados para puños, guardia y agarre; bastón opcional y separado. Son referencias conceptuales, no identidad narrativa o diseño definitivo.

Objetivo anatómico 1,80 m; raíz de prefab/VisualRoot a escala uno, pies en Y=0, Y arriba, frente +Z. El auditor usa margen técnico de talla 1,70–1,90 m y pivote a ±5 cm del suelo para detectar errores de unidades; ese margen no aprueba una nueva talla de juego. La cápsula sigue en 0,60 × 1,80 m. Una variación real requiere validar puertas y colisiones y actualizar las referencias centrales.

Exportar FBX desde la herramienta de creación con unidades documentadas, transformaciones aplicadas y pose T/A consistente. Mantener esqueleto y skins de cuerpo/ropa compatibles. Evitar escalas negativas y rig optimizado que oculte sockets durante esta primera validación. No instalar un importador para `.blend` u otro formato fuente: Unity recibirá FBX.

## Usar el flujo dentro de Unity

1. Abrir `ProyectoRPGRTS/` en **Unity 6000.5.8f1**. No repetir el modo de generación HumanoidPipeline sobre los assets existentes.
2. Colocar el FBX aprobado en `Assets/07_Characters/Humanoid/Models`. Conservar su `.meta` al actualizarlo; registrar autor/licencia/procedencia. El formato FBX se importa mediante las herramientas nativas de Unity. [Importación de modelos de Unity](https://docs.unity3d.com/6000.0/Documentation/Manual/FBXImporter-Model.html).
3. Abrir **Aethernia → Personajes → Importar Humanoid** y arrastrar el FBX. La acción 1 configura globalScale=1 con unidades del archivo, Humanoid/Create From This Model, animación importable, sin colliders/cámaras/luces, sin optimización de jerarquía y sin generar materiales nuevos. Solo cambia el FBX seleccionado al pulsar el botón; no hay un postprocesador global que reimporte modelos ajenos.
4. Revisar **Rig → Configure**. Corregir el mapping y pose hasta obtener Avatar válido; verificar manos y dedos. Un FBX no garantiza una estructura Humanoid válida. [Avatar y mapping](https://docs.unity3d.com/6000.0/Documentation/Manual/ConfiguringtheAvatar.html).
5. Preparar/remapear materiales URP Lit en el Inspector de importación. Se puede suministrar un material URP compartido opcional a la ventana para una revisión técnica; aplicarlo a todos los submeshes no sustituye una autoría por tejidos/piel/metal. Importar normales como Normal Map y albedo en sRGB; comprobar visualmente canales y rugosidad.
6. Pulsar **Auditar**. Detecta raíz con escala/rotación incorrecta, talla/pivote fuera de tolerancia, Avatar/manos/skin ausentes, materiales no URP y componentes físicos impropios. Revisar manualmente frente +Z, pesos, dedos, deformaciones y lectura isométrica: no se certifican por bounds.
7. Pulsar **Crear prefab candidato**. Rechaza recursos incompatibles, no sobrescribe un prefab y conserva vínculo con el FBX. La raíz recibe CharacterVisualModel, rig/driver y sockets; los de arma/bastón se sitúan bajo RightHand cuando el Avatar está disponible. No se asigna al catálogo automáticamente.
8. Preparar vistas de equipo bajo sus sockets y asignarlas en **Bindings** del CharacterVisualModel. Revisar escala/agarre y materiales. Para ropa/armaduras deformables, usar skins preparadas para el mismo esqueleto y enlazar sus huesos en el prefab; no hay retargeting automático de ropa ni conversión de una pieza rígida en skin.
9. Tras revisión y aprobación del recurso, asignar el candidato a **HumanoidPrefab** de `monk` en `Assets/07_Characters/Provisional/ClassVisuals.asset`. Mantener `Prefab` apuntando al Monje E01. Un candidato inválido vuelve a esa alternativa; quitar HumanoidPrefab también revierte la elección.
10. Probar clase/equipo, selección, paso de puerta y guardado/carga en PlayablePrototype. Repetir `Tools/Invoke-UnityValidation.ps1 -Mode Vertical`, después Build y los recorridos de `Invoke-PrototypeSmoke.ps1`, con el editor cerrado sobre el proyecto.

La ventana y sus métodos están preparados; su uso con un FBX humanoide real y la revisión manual del mapping siguen pendientes. El candidato se puede mostrar estático con clips ausentes; el driver mantiene Animator desactivado hasta que haya un Idle Humanoid compatible, no vacío y aplicado al controller, además de un rig válido.

## Animación preparada, no terminada

| Estado | Dependencia real / comportamiento preparado |
| --- | --- |
| Idle | Clip humanoide de reposo; requisito mínimo para habilitar Animator |
| Walk | Clip cíclico en el sitio; la velocidad proviene del NavMeshAgent |
| Run | Clip cíclico en el sitio; umbral de presentación configurable, sin nueva orden de correr |
| Attack | Clip marcial de una acción; trigger Attack; no calcula daño ni consume turnos |
| Defend | Guardia mantenida; bool Defending; no modifica defensa RPG |
| Hit | Reacción de una acción; trigger Hit; no resta salud |
| Defeat | Derrota; bool Defeated derivable de salud; no añade reglas de muerte |

Parámetros: Speed (float), Locomotion (int 0/1/2), Attack/Hit (trigger), Defending/Defeated (bool). Locomoción lee el actor; no mueve transform ni utiliza root motion. `applyRootMotion=false` y eventos de clips desactivados separan movimiento/efectos de la animación. [API de root motion](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Animator-applyRootMotion.html).

Asignar clips al perfil y usar **Actualizar estados desde clips**: rechaza clips vacíos, Legacy o sin curvas Humanoid. Al aplicar Idle real desde el grafo base, crea un controller de reproducción propio del perfil; el grafo vacío permanece compartido y reutilizable para Guerrero/Mago. No asignar perfiles distintos al mismo controller de reproducción editable. Con clips nulos, los estados permanecen sin motion. No se crean `.anim` vacíos para aparentar una animación terminada. Se comprueba compatibilidad de curvas mediante [AnimationClip.humanMotion](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AnimationClip-humanMotion.html); la deformación/retargeting debe verse con el Avatar real.

Para FBX de animación separados, revisar su pestaña Rig: Humanoid y Avatar del mismo esqueleto cuando corresponda (Copy From Other Avatar); usar avatar propio válido si el esqueleto es distinto y comprobar el retargeting. Configurar bucles de Idle/Walk/Run/guardia y revisar raíz/pies en la pestaña Animation. Arrastrar las AnimationClip importadas al perfil; no basta con asignar el archivo FBX como si fuera un clip.

TryPlay(Attack/Hit/Defend/Defeat) y SetGuarding son puntos de presentación disponibles. Attack/Hit/Defend **todavía no están conectados automáticamente a los eventos del combate**: su adaptador y timing se validarán con clips reales en E02-A, sin alterar ahora TurnBattle/TurnEncounter. La derrota visual puede leer salud; sin clip no se reproduce nada. No se afirma que golpear, defenderse o caer esté animado.

## Equipo y persistencia

EquipmentVisualBinding admite Clothing, Armour, Weapon, Staff y Accessory. Cada vista pertenece al modelo y observa un ItemId de Weapon o Armour ya existentes. Las vistas externas al modelo no se activan/desactivan. Los campos antiguos de espada/túnica continúan funcionando para E01.

Hoy solo existen espada de ensayo y túnica equipables. Ropa/protecciones/accesorios pueden ser varias vistas de ese mismo equipo aprobado; no hay cinco ranuras persistentes nuevas. Un bastón como arma independiente requiere un ítem y reglas aprobados posteriormente. La prueba sintética de vista de bastón no introduce ese ítem en el juego ni lo guarda. Guardar/cargar conserva v1 y reconstruye vistas a partir de los dos campos existentes.

## Archivos que faltan para completar el Monje

Entrega mínima concreta, pendiente de crear/conseguir y aprobar:

| Archivo solicitado | Contenido requerido |
| --- | --- |
| `Monk.fbx` | Rig Humanoid válido, malla skinned de cuerpo/manos, ropa monástica base y protecciones ligeras separadas en submeshes/objetos, UV y pesos; pose de referencia y unidades documentadas. Manos/dedos preparados para artes marciales |
| `Monk_Albedo.png` | Atlas de color para piel/tejido/cuero/protecciones, sin sombras pintadas |
| `Monk_Normal.png` | Normales tangentes compatibles con la exportación, para importar como Normal Map |
| `Monk_MetallicSmoothness.png` | Máscara preparada para URP Lit (metal/smoothness), revisada visualmente; puede ser neutra donde no haya metal |
| `Monk_Idle.fbx` | Toma Humanoid Idle en el sitio y cíclica |
| `Monk_Walk.fbx` | Toma Walk en el sitio y cíclica, pies comprobables |
| `Monk_Run.fbx` | Toma Run en el sitio y cíclica |
| `Monk_Attack.fbx` | Toma Attack, un ataque marcial sin desplazamiento global impuesto |
| `Monk_Defend.fbx` | Toma Defend de guardia mantenible |
| `Monk_Hit.fbx` | Toma Hit de reacción, no cíclica |
| `Monk_Defeat.fbx` | Toma Defeat, no cíclica, final estable |
| `ORIGEN_RECURSOS.md` | Autoría/licencia, permiso de uso, fuente y unidades/convenciones de exportación de todos los recursos |

Los siete clips pueden entregarse como siete tomas con esos nombres dentro de un único `Monk_Animations.fbx`; es equivalente, no se requieren ambas entregas. Opcionales, **no imprescindibles para el piloto base**: `Monk_Staff.fbx` y sus materiales, AO si la autoría lo necesita y variantes de ropa/armadura skinned para el mismo esqueleto. Esas variantes necesitan su textura/atlas compatible y revisión de huesos, no una segunda lógica de inventario. La resolución de mapas y presupuesto final se fijarán con calidad/rendimiento medidos, no se inventan requisitos comerciales.

## Pruebas y límites de esta entrega

| Prueba ejecutada | Resultado / evidencia local |
| --- | --- |
| Cuatro pruebas E02 dirigidas | 4/4 aprobadas, salida 0; Logs/E02-targeted-results.xml/log |
| Regresión completa | **46/46 aprobadas**, cero fallos/omisiones, salida 0; 8,03 s de casos (sin contar arranque/recargas); Logs/E02-final-results.xml/log |
| FBX nativo y unidades | Malla técnica importada con bounds 1 × 1 × 1 m, flags Human comprobados, candidato sin rig rechazado y archivo temporal eliminado; Logs/E02-fbx-import.txt |
| Estructura preparada | Siete motions nulas, candidato vacío, rechazo de clip vacío/no Humanoid y alternativa E01; Logs/E02-structure.txt |
| Equipo y persistencia | Cinco tipos de vistas, sin mutación del estado, sin activar objetos externos, equipo real y round-trip v1; Logs/E02-equipment-save.json |
| Puente de movimiento sin recursos | Lee velocidad real, mantiene Animator desactivado, rechaza acciones no disponibles y conserva navegación del Monje E01 |
| Regresión existente | Clases, movimiento/colisiones, cámara, UI, equipo, RPG/RTS, construcción/economía y guardado anteriores aprobados |
| Build Windows | 178.013.219 bytes, 64,97 s, salida 0; Logs/E02-build-summary.txt y E02-build.log |
| Player independiente | Sin gráficos 7,40 s y GPU RTX 3050 Laptop 7,38 s, ambos PASS/salida 0, sin excepciones/agentes inválidos; comprobantes y logs E02-headless/graphics en Logs |

Se separa la prueba técnica de importación de la aceptación de un personaje: el FBX de ensayo es un cubo original de un metro, temporal y eliminado al terminar; no es un modelo humanoide ni se incorpora como personaje del juego. La primera ejecución falló por el formato del fixture ASCII; corregidos cabecera, normales y UV antes del cierre positivo. No se atribuye ese fallo a un personaje o plugin ausente.

No verificado todavía: importación y mapping de un Humanoid real, pesos de dedos/ropa, retargeting y reproducción de los siete clips, agarre en la mano real, sincronización de golpes, rendimiento del personaje final y evaluación humana. Se registran en F3-E02-M, F3-E02-A y F2-07 pendientes.

Paquete portable de verificación: `ProyectoRPGRTS/Builds/Aethernia-Fase3-E02-Windows.zip`, 73.589.346 bytes, 305 entradas, CRC correcto; SHA-256 `23a51daced714b1fc4fe9e3e8b9202b5966dcb78f5c922b1b38ddf92b180a83c`. Exe/DLL runtime/level0 coinciden con la build. Cinco ZIP históricos intactos por hash. La build conserva apariencias E01: el workbench de importación solo existe en el editor. Comprobante: Logs/E02-package.json.

Recuperación: `codex/e02-base-2d95959`. Esta entrega prepara E02 y no inicia automáticamente la fase siguiente.
