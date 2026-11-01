# Crónicas de Aethernia

## Contexto y fuentes
- Responder en español. Raíz de trabajo y Git: esta carpeta. Proyecto Unity: `ProyectoRPGRTS/`.
- Motor observado: Unity `6000.5.8f1`, C#, URP `17.5.0`; conservar versiones salvo necesidad demostrada.
- Unity 6.5 exige APIs `EntityId`; evitar `GetInstanceID` y `objectReferenceInstanceIDValue` obsoletas. Usar la API actual y comprobar compilación con el editor exacto.
- Fuente de diseño: `ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx`, versión 0.2. Preservar el original.
- Prototipo aprobado: `Docs/ESPECIFICACION_PROTOTIPO.md`; clases Guerrero/Mago/Monje y reglas provisionales autorizadas. Hoja de ruta posterior aprobada en `Docs/PLAN_EXPANSION.md`, sin iniciar producción futura automáticamente.
- Fase 3: A/B/C1, D01, E01/E02/E02-B y HUD C02; dirección en `Documentacion/DIRECCION_ARTISTICA.md`. F2-07 y recursos definitivos siguen pendientes; el registro UI usa hechos existentes y no hay minimapa/compañeros.
- Lectura consultable: `Docs/GDD_LECTURA.md`; comprobar su SHA-256 contra el Word si cambia el GDD.
- Leer primero el resumen de `CHECKLIST.md`; consultar el plan y las secciones del GDD relacionadas con la tarea.
- El GDD es conceptual: distinguir confirmado, propuesto y pendiente. No convertir ejemplos en requisitos.

## Alcance y método
- Primera fase: diagnóstico y planificación. No programar mecánicas hasta que el usuario apruebe el plan.
- Tras la aprobación, trabajar una tarea acotada del checklist, explicar el alcance, implementar y validar.
- No desarrollar historia, reglas RPG completas ni varios sistemas grandes simultáneamente sin autorización.
- Analizar antes de editar escenas, scripts o documentos. Respetar decisiones y trabajo existentes.
- Actualizar el checklist y añadir entradas al registro después de cada actividad; conservar el historial.
- Cada tarea registra ID, descripción, sistema, prioridad, dependencias, estado, aceptación, archivos, pruebas y resultado.
- Estados: `[ ] Pendiente`, `[ ] En desarrollo`, `[ ] Bloqueado`, `[x] Completado y verificado`.
- Código escrito no implica tarea completada: exigir compilación en Unity y comprobación de aceptación. Si falta, registrar validación pendiente.
- Terminar con tareas terminadas, pendientes, problemas y siguiente actividad; no repetir todo el plan en cada respuesta.

## Implementación futura
- Aplicar SOLID con criterio: responsabilidades pequeñas y separación entre datos, lógica y presentación.
- ScriptableObjects para configuración compartida; separar estado mutable de partida y guardado.
- Usar interfaces y eventos donde aporten utilidad; liberar suscripciones y referencias.
- Evitar gestores monolíticos, búsquedas repetidas, duplicación y capas preventivas sin necesidad.
- Escala inicial: 1 unidad = 1 metro. Medidas provisionales y pruebas de paso/navegación en el plan.
- UI actual: uGUI con prefabs/MedievalTheme compartidos y secciones Resumen/Personaje/Inventario/Partida; mantener separación por modo y justificar una migración.
- Perfilado antes de optimización compleja. Pooling para creaciones frecuentes que lo justifiquen.
- Conservar `.meta` y GUID; mover assets desde Unity o manteniendo su `.meta`.
- No editar cachés ni proyectos C# generados. No copiar recursos o interfaces protegidos.

## Validación y herramientas
- Abrir `ProyectoRPGRTS/` con el editor exacto. Revisar Console, referencias, Play Mode y pruebas pertinentes.
- Preferencia del usuario: validar en segundo plano mediante `Tools/Invoke-UnityValidation.ps1`; usar la ventana solo si una comprobación visual lo requiere. No iniciar dos instancias sobre el mismo proyecto.
- Escena jugable: `Assets/01_Scenes/PlayablePrototype.unity`. Validación completa: modo `Vertical`; build: modo `Build`; ejecutar la build sin ventana con `Tools/Invoke-PrototypeSmoke.ps1`. No repetir los generadores sobre assets existentes.
- E03–E06: demo `Assets/01_Scenes/AetherniaVillageDemo.unity`; build `BuildVillage`. Catálogo existente usa prefabs estáticos E03 y retratos de fuentes autorizadas; anteriores preservados. Misión/validación en Docs/E03_E06_PRODUCCION.md y Docs/E05_CARGAMENTO_PERDIDO.md. No repetir ProductionCharacters/ProductionPresentation/VillageDemo. F2-07 humano 7/8 informado, cierre X/ESC pendiente de confirmación.
- E09: misma demo, turnos escalonados por RequestEndTurn, IA melee y presentación compartida en Assets/09_Feedback/CombatE09. Informe/guía Docs/E09_COMBATE_TACTICO.md; mantener daño autoritativo separado de animación/VFX/audio, balance/v1/Generic. Validar TacticalCombat* y player con -RequireTacticalE09. No repetir CombatE09 ni Generate-E09Audio sobre recursos existentes; audio/arte piloto y evaluación humana pendientes.
- E08: demo usa catálogo separado Assets/07_Characters/AnimatedE08 y prefabs E08 con rigs Generic/skin/clips originales; E02 sigue estricto Humanoid. Fuente C# en Editor/Characters/E08. Informe Docs/E08_PERSONAJES_ANIMADOS.md; no repetir AnimatedCharacters ni generar encima de assets. Animación cosmética sin root motion/daño. Proxies/escena original conservados; aprobación artística/humana pendiente. Validar AnimatedCharacterTests y player con -RequireAnimatedE08.
- E07: misma AetherniaVillageDemo con raíz E07 Medieval Village; biblioteca en 06_Environment/VillageE07 y 02_Prefabs/Environment/VillageE07. Informe Docs/E07_ALDEA_MEDIEVAL.md. No repetir VillageE07Library/Nature/Transform/Refine; son autoría, no arranque. Mantener el área RTS y ruta de misión libres. PR apilado desde E03–E06 mientras #1 esté pendiente; no fusionar automáticamente.
- D01: grupo visual `D01 Medieval Quarter`, once prefabs en `Assets/02_Prefabs/Environment/Medieval/`; física original conservada. Evidencia/límites en `Docs/D01_ESCENARIO_MEDIEVAL.md`. E01 añade recorte reversible de cubierta.
- E01: VisualRoot conserva controlador/collider; catálogo `Assets/07_Characters/Provisional/ClassVisuals.asset`. Prefabs con CharacterVisualModel, sin agentes/colliders/Animator. Desactivar presentador recupera cápsula; RoofCutaway recupera techo al desactivarse.
- E02: flujo nativo en Aethernia/Personajes/Importar Humanoid; plantilla vacía y perfil/grafo en Assets/07_Characters/Humanoid. Guía/manifiesto Docs/E02_IMPORTACION_HUMANOID.md. No hay modelo/Avatar/clips reales aprobados: E02-M/E02-A pendientes; no repetir HumanoidPipeline.
- E02-B: Monje usa proxy estático de 1,78 m y tarjeta en Personaje; recursos propios en Assets/07_Characters/MonkProvisional, alternativa E01 preservada. Informe Docs/E02B_MONJE_PROVISIONAL.md. No repetir MonkIdentity; no es FBX/rig/animaciones finales ni aprobación humana.
- C02: HUD compacto, ventanas por ×/ESC, Menú para actividades/guardado y Ayuda opcional. MedievalHudView/InventoryPanelView conservan comandos del HUD; atlas en Assets/08_UI/Medieval. Informe Docs/C02_INTERFAZ_MEDIEVAL.md. No repetir MedievalHud; afinación no es paso de arranque.
- Antes de cada tarea: revisar `git status`, dependencias y criterios. Revisar el diff y registrar cambios importantes.
- No sobrescribir cambios ajenos, publicar remotos sin autorización de la sesión ni modificar configuración global como parte de tareas locales.
- Visual Studio Community 2026 y Tools for Unity ya instalados; paquete IDE Unity `2.0.26` presente.
- Instalar herramientas solo para necesidades concretas y compatibles; no por supuesto ahorro de tokens.

## GitHub, ramas y Pull Requests
- Remoto `origin`: `https://github.com/MathiasMerino/RPGRTS.git`. Repositorio público; conservar el historial y revisar credenciales/archivos grandes antes de publicar.
- `main`: versión estable, predeterminada en GitHub. Incorporar versiones futuras solo mediante PR `dev` → `main` con aprobación del usuario y evidencia de validación; compilar no equivale a aprobación humana.
- `dev`: rama principal de desarrollo y rama local para continuar. Crear desde `dev` únicamente las ramas temporales necesarias: `feature/<descripcion>`, `fix/<descripcion>` o `test/<descripcion>`. Esta convención sustituye el prefijo histórico `codex/` para trabajo nuevo.
- PR de una rama temporal → `dev`: explicar problema/resultado, alcance, pruebas ejecutadas y pendientes. Conservar commits mediante merge; no reescribir historia publicada ni usar push forzado. Tras publicar una versión en `main`, incorporar su avance en `dev` mediante fast-forward o merge normal cuando corresponda.
- No publicar ramas históricas/checkpoints en bloque, borrar referencias previas, usar `push --force`, reinicializar Git ni modificar permisos/protecciones sin encargo explícito. Autenticarse con Git Credential Manager; nunca guardar tokens en archivos, URLs o commits. Si falta autenticación, pedir intervención del usuario.
- La conexión inicial solicitada el 2026-10-31 autoriza publicar el estado `cacad54` y la documentación de conexión como base común de `main`/`dev`, conservando los 14 commits anteriores. No aprueba arte final ni cierra F2-07 o los demás pendientes del juego.

## Contexto y búsquedas
- Mantener este archivo breve; guardar detalles y evidencia en los documentos de seguimiento.
- Usar `rg` por sistema. `.rgignore` excluye cachés, registros y generados; abrirlos expresamente para diagnóstico.
- No recorrer `Library/`, `Temp/`, `Logs/`, `obj/`, `.vs/` ni paquetes completos en tareas ordinarias.
- Reutilizar la lectura del GDD mientras coincida su hash y consultar secciones concretas.
- Cargar solo skills e integraciones necesarias. No delegar ni crear subagentes salvo petición expresa del usuario.
