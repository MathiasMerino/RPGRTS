# E03–E06 — Personajes y misión integrada

Fecha: 2026-10-31. Entrega técnica verificada en `feature/e03-e06-personajes-vertical-slice`, desde dev `742c04f`. Los tres commits de propuestas artísticas estaban locales; se sincronizaron con origin/dev antes de crear la rama. No hay ramas de reparación F2-07 locales/remotas observadas. Las hojas Monje v02, Guerrero v01 y Mago v01 existen en Documentacion/Personajes/Conceptos; son originales generados, autorizados por el nuevo encargo para integración provisional, sin aprobación de arte definitivo.

## Plan y dependencias

1. E03: inspección de modelos/herramientas, nuevas mallas estáticas por material y prefabs de clase conservando E01/E02-B; importación íntegra de fuentes. Compilar/validar antes de commit.
2. E04: extender catálogo/modelo existentes, retratos recortados en Unity desde originales, metadatos y equipo consistente. Mantener IDs/reglas/guardado y plantillas C02. Compilar/validar antes de E05.
3. E05: zona existente D01, NPC de aldea, objetivo de carga y diálogo independiente del gestor de ventanas. Misión «El cargamento perdido» provisional; usa Facts v1, combate real y recompensa única, sin nuevo gestor de misiones genérico. Validar recorrido con las tres clases.
4. E06: regresión, teclado/puntero de cierre (sin afirmar validación humana), capturas GPU 720p/1080p, métricas acotadas, build Windows/recorridos, documentación y PR hacia dev. No main ni release final.

## Decisiones iniciales

GDD v0.2/lectura por hash intacto; §12.1/12.2 conserva fantasía medieval realista e integración de estado. No hay FBX/GLB/Blend humanoides en Assets, Blender detectable o herramientas Unity/modelado expuestas. Se mejoran representaciones mediante mallas horneadas, sin rig/animaciones reales. La biblioteca MonkProxyGeometry existente se reutiliza. Controlador/cápsula/agente mantienen 0,60×1,80 m; Monk 1,78 m y otras clases 1,80 m, pies Y=0/frente+Z. No se aplica la talla artística Guerrero de 1,86 m a la física.

Inventario solo define espada, túnica y ración; no existe bastón equipable o ranura vacía interactiva. El bastón del Mago será accesorio de identidad explícito, no arma con estadísticas nuevas. La espada real se mostrará al equiparla en las tres clases, como permite el prototipo; Monje no tendrá espada por defecto. Armadura visual de identidad del Guerrero no se anuncia como equipo/bonificación. Guardado v1 admite hechos libres y se conserva.

F2-07: el usuario informa 7/8 comprobaciones humanas confirmadas; no se infieren cuáles filas históricas fueron evaluadas. X/ESC permanece pendiente de confirmación/corrección. No se sobrescribe una rama de reparación; no se cambia el gestor C02 durante E03–E05. El diálogo de misión será independiente. Las pruebas nuevas distinguirán automatización de esa confirmación humana.

## Resultados

La aceptación técnica por etapa se registra debajo. La evaluación humana y el arte definitivo tienen estados independientes.


### E03 — Completada técnicamente; revisión humana pendiente

Generación/compilación salida 0 y 2/2 pruebas dirigidas aprobadas: métrica/URP/recuperación y carga/render Play Mode de las tres clases. Capturas nativas E03-monk/warrior/mage en Docs/Images, inspeccionadas. Monje 0,711×1,780×0,368 m / 5.252 tris / 9 renderers; Guerrero 0,715×1,800×0,365 / 6.704 / 9; Mago 0,733×1,800×0,550 / 6.096 / 11, incluyendo equipo inactivo. Tres prefabs E03, tres materiales propios compartidos con materiales previos y fuentes PNG íntegras. Sin colisiones/Animator en modelos, física anterior preservada. Anatomía/manos y paños siguen geométricos; el Mago es más oscuro que la lámina y se ajustará su paleta compartida en E04. Originales E01/E02-B y plantillas Humanoid conservados. Evidencia E03-results.xml/validation.log/geometry.txt.


### E04 — Completada técnicamente; revisión humana pendiente

Configuración/compilación salida 0; 4/4 dirigidas aprobadas. Catálogo existente ampliado con título visual, fuentes/portrait sprites, prefab anterior, materiales compartidos, talla/política de equipo y estado provisional, sin copiar estadísticas. Fuentes importadas Sprite/sRGB/NPOT None, recorte nativo 180×240 (3:4), mismo PNG sin editar; ficha/header usan UV correspondientes. Anclajes RightHand_Weapon y bindings existentes conectados a espada/tela v1. Ocho equipamientos repetidos por clase no duplican instancia/renderers; retirada visual por estado vacío y guardar/cargar restauran clase/equipo. Seis capturas UI 720p/1080p validadas por límites/textos/raycast; tres seleccionadas en Docs/Images. Paleta Mago aclarada moderadamente en materiales nuevos. Reglas y formatos inalterados; bastón no es item y no hay interfaz para desequipar actualmente.


### E05 — Completada técnicamente; revisión humana pendiente

Escena separada AetherniaVillageDemo, conservando PlayablePrototype byte a byte. Habitante interactivo, arcón D01 con carving y diálogo/objetivo independientes del gestor C02. Misión con aceptación, encuentro RPG real, retorno automático tras victoria, entrega única y guardado v1 en cada etapa. Recompensa provisional 20 oro y ración si hay espacio; inventario lleno recibe oro sin bloquear misión. No hay cargo nuevo como item, se registra por Facts. Cinco pruebas dirigidas aprobadas en primera ejecución y tras pulir etiquetas: datos/compatibilidad/pago único/capacidad, recorridos completos Monje/Guerrero/Mago con rutas reales y guardado/carga, y rechazo de victorias ajenas o activación sin aceptar. Semilla 42 solo en pruebas; juego normal mantiene semilla 0/aleatoria. No estadísticas alteradas. Cuatro capturas seleccionadas E05-* en Docs/Images; protocolo en Docs/E05_CARGAMENTO_PERDIDO.md. Primera autoría falló por ruta de cofre, corregida reutilizando el NPC parcial; etiquetas inicialmente por encima del diálogo corregidas a sorting -10.


### E06 — Incidencias de interfaz y método de prueba

La prueba inicial con el campo de nombre activo produjo una actualización recursiva al desactivar el InputField (`GameObject is already being activated or deactivated`). El HUD ahora difiere los refrescos reentrantes, omite refrescos cuando está desactivado y Rename no emite Changed para un nombre idéntico. ESC se procesa mediante una InputAction con alta/baja ligada al componente, en lugar de consultar un pulso desde Update. Se conserva el gestor C02, sus botones y su arquitectura.

Prueba dirigida final: 1/1 aprobada. Comprueba raycast superior y callback de puntero del botón ×, eventos KeyboardState de ESC con nombre enfocado e inventario, en 720p/1080p. El runner oculto no tiene Game view enfocada: el arnés configura temporalmente entrada en segundo plano y fuerza el update Manual interno de la versión instalada mediante reflexión, restaurando los ajustes al salir. Los intentos intermedios con Update del editor no entregaban eventos a las acciones de juego; no prueban por sí solos un fallo de ESC en el player. Las primeras evidencias fallidas se conservan en Logs/E06-input-first/second-*; la evidencia final en E06-input-results.xml/validation.log. La confirmación humana F2-07 sigue pendiente, incluso tras el resultado automático positivo.

### Sustitución futura segura de las tres figuras

Conservar ClassId `warrior`, `mage`, `monk`, datos de clase y persistencia; reemplazar únicamente las referencias visuales del catálogo. Producir mallas propias/autorizadas a 1 m/unidad, raíz con escala 1, pies Y=0 y frente +Z, pose A/T coherente y rig Humanoid válido. Altura visual provisional 1,80/1,80/1,78 m; talla final y nueva cápsula requieren pruebas de paso/cámara. Separar ropa, cuerpo y equipo cuando necesiten variantes, preparar UV/PBR URP, pesos y nombres consistentes; no hornear el arma equipable al cuerpo. La espada usará un anclaje de mano real con el mismo binding; el bastón requerirá una decisión de inventario antes de convertirse en arma. Seguir Docs/E02_IMPORTACION_HUMANOID.md para Avatar/clips y registrar licencia/autoría. Probar mapping, skinning, retargeting y agarre con recursos reales antes de activar el Animator. El controller preparado no contiene animaciones finales.

La identidad del Monje usa tela naranja asimétrica y vendas; Guerrero usa acero/cuero y faldón; Mago usa capas violeta/azul grisáceo, libro y bastón de identidad. La diferencia de silueta es provisional: manos, rostro, articulaciones y pliegues necesitan modelado posterior. No se han instalado herramientas ni descargado modelos externos.


### Capturas reales y comparación

Todas estas imágenes proceden del render de Unity con GPU; no son montajes de las hojas conceptuales. Para los planos de figura se acercó temporalmente la cámara y ocultó el HUD durante la prueba; no se guardaron cambios de cámara en la escena.

| Comprobación | Captura seleccionada |
| --- | --- |
| Monje en escena | [Monje](Images/E06-Class-monk.png) |
| Guerrero en escena | [Guerrero](Images/E06-Class-warrior.png) |
| Mago en escena | [Mago](Images/E06-Class-mage.png) |
| Fichas y retratos de las tres clases | [Monje](Images/E04-monk-1280.png), [Guerrero](Images/E04-warrior-1280.png), [Mago](Images/E04-mage-1280.png) |
| Inventario/equipo real | [Inventario 1080p](Images/E06-Inventory-1920.png) |
| NPC y diálogo | [Diálogo 1080p](Images/E05-NPC-1920.png) |
| Misión activa | [Objetivo 720p](Images/E05-Active-1280.png) |
| Encuentro RPG real | [Combate 1080p](Images/E05-Encounter-1920.png) |
| Misión completada | [Finalización 720p](Images/E05-Completed-1280.png) |

Recuperación/comparación: imágenes E01-warrior/E01-mage y E02B-After conservan las figuras anteriores en Docs/Images; prefabs previos siguen accesibles mediante PreviousPrefab. La estética y calidad anatómica actuales continúan siendo provisionales. La cámara normal permite leer colores/siluetas, mientras los rasgos finos requieren más trabajo de modelado. La legibilidad se revisó en las capturas y con aserciones de layout/raycast, sin atribuir aprobación humana a esas comprobaciones.

### Revisión manual exacta

1. Abrir ProyectoRPGRTS con Unity 6000.5.8f1, esperar importación y revisar Console. No ejecutar generadores. Abrir Assets/01_Scenes/AetherniaVillageDemo.unity y pulsar Play.
2. Personaje: cambiar Monje/Guerrero/Mago; comprobar figura, retrato y datos. Escribir un nombre y cerrar con × y ESC. Revisar Game view 1280×720 y 1920×1080. Esta confirmación humana sigue pendiente.
3. Seleccionar al héroe, caminar con clic derecho y usar E/Interactuar cerca del habitante (X/Z −7,5/−3). Aceptar el encargo, cerrar diálogo y seguir el objetivo hasta el arcón (−10/3).
4. Interactuar con el arcón, usar Acercarse/Atacar/Fin de turno y Recuperar cuando corresponda. Esperar la llegada antes de atacar. Ganar con las reglas existentes, volver al habitante y entregar una sola vez. No existe botón de victoria.
5. F5/F9 fuera de combate: comprobar carga aceptada, recuperada y completada. Hablar de nuevo no duplica recompensa. Recoger cofre inicial para probar espada/túnica; la espada visible depende del inventario real. El Mago no tiene bastón equipable nuevo.
6. Menú conserva construcción y RTS; la escalada RTS exige victoria RPG como antes. Volver a explorar para continuar. Repetir el recorrido con las otras clases y registrar atascos, oclusiones, lectura del objetivo o problemas de cierre.

Para el paquete Windows, extraer el ZIP completo en una carpeta y abrir Aethernia.exe; conservar su carpeta Aethernia_Data y DLL junto al ejecutable. Seguir los mismos pasos. Es una build Development de demostración, no una release final. No comenzar otra fase automáticamente: siguiente actividad recomendada es la evaluación humana de esta secuencia y luego una propuesta acotada de modelado/rig.


### Cierre técnico E06 — 2026-10-31

| Ejecución real | Resultado |
| --- | --- |
| Regresión completa, editor exacto | **62/62 aprobadas**, cero fallos/omisiones, salida 0 |
| Plataforma del runner | EditMode; las pruebas UnityTest entran en Play Mode. Diez nuevas: 3 Test y 7 UnityTest; no son diez pruebas humanas |
| Build Windows x64, escena AetherniaVillageDemo | Succeeded/salida 0; 185.241.263 bytes; 64,86 s |
| Player sin gráficos | PASS/salida 0, 34,56 s; sistemas existentes y misión real con Monje/Guerrero/Mago |
| Player GPU RTX 3050 Laptop, D3D12 | PASS/salida 0, 33,64 s; mismos recorridos y guardado v1 por etapa |
| Entrada y legibilidad | × por raycast/callback y ESC por evento, nombre enfocado/inventario; 720p/1080p. Capturas seleccionadas inspeccionadas |
| Integridad | Fuentes PNG por hash, GUID sin duplicados/meta huérfanos; .gitkeep ignorados por Unity. GDD/escena original/reglas/Packages/ProjectSettings sin cambios |
| ZIP portable | CRC PASS, 305 entradas, 77.833.297 bytes; exe/UnityPlayer/runtime/level0 coinciden con build; ocho ZIP históricos intactos por hash |

Paquete local: `ProyectoRPGRTS/Builds/Aethernia-E03-E06-Demo-Windows.zip`, SHA-256 `b24c5e24eff8d35f938629bc982e9f9970f3afe14a462507205ab3ab801fa668`. Build/Logs excluidos de Git. [Comprobante resumido versionado](Validacion/E03_E06_RESULTADOS.json); XML/logs completos en Logs/E06-final-*, E06-input-*, E06-build.log y E06-headless/graphics-*.

La primera regresión dio 61/62: una expectativa histórica usaba un nombre de prefab incorrecto, no un fallo de navegación/equipo. Se corrigió a monk_Proxy y se repitió toda la batería con 62/62. La validación inicial del ZIP buscaba Assembly-CSharp.dll; esta arquitectura usa Aethernia.Runtime.dll, comprobada por hash sin modificar el paquete. Los comprobantes independientes exigen un archivo nuevo de misión (`-RequireCargo`); una build de la escena anterior no satisface esa comprobación.

Rendimiento acotado: 5.252–6.704 triángulos y 9–11 renderers por prefab incluyendo equipo inactivo, materiales URP compartidos, equipo repetido sin duplicación y recorridos sin bloqueo/errores de navegación/excepciones de juego. Memoria asignada al cierre del probe: 106.720.803 bytes sin gráficos y 117.058.021 con GPU; son muestras de una build Development batch, no un presupuesto final ni un benchmark de FPS. Faltan perfilado interactivo y revisión humana; no se aplica optimización compleja.

Completado técnicamente: E03, E04, E05 y validación/build de E06. Pendientes: confirmación humana F2-07 (7/8 informado, X/ESC pendiente), aprobación artística definitiva, modelos Humanoid/skinning/clips reales y eventual integración del objetivo en Registro tras revisar C02. No se comienza otra fase. [PR #1 en borrador hacia dev](https://github.com/MathiasMerino/RPGRTS/pull/1), rama publicada; main se conserva.


### Git y cierre de alcance

Commits por etapa: `136fb87` E03, `50e860b` E04, `e7bc6f4` E05 y `0f86fa9` E06 (código validado). El cierre documental registra el PR #1 y no modifica código/escenas después de la validación. Rama feature/e03-e06-personajes-vertical-slice publicada con seguimiento de origin; no se fusiona dev/main ni se publica release. Los commits anteriores permanecen como ancestros. E03–E06 finaliza técnicamente; evaluación humana y producción artística final permanecen abiertas.
