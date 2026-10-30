# E01 — Base visual de Guerrero, Mago y Monje

Fecha: 2026-10-30. Fuente: GDD v0.2, catálogo de clases y §12.1; dirección artística vigente. El Word conserva SHA-256 `dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661`. Son estudios genéricos de clase, sin identidad narrativa, raza definitiva, nuevos atributos o habilidades.

## Guía común

Fantasía medieval de proporciones humanas, materiales sobrios y desgaste discreto. Altura 1,80 m, cabeza aproximadamente 0,24 m (7,5 cabezas), pies en Y=0, eje Y arriba y frente del prefab +Z. Evitar cabezas/manos gigantes; diferenciar las clases por masas y prendas, además de color. Los modelos provisionales son figuras geométricas estáticas, no esculturas realistas finales. No se crean rigs ni animaciones definitivas.

El agente/cápsula permanecen en radio 0,30 m, altura 1,80 m; la selección sigue separada. Hombros y accesorios pueden sobresalir ligeramente del proxy físico; no añaden alcance, protección ni colliders. Las medidas aplicadas viven en [ESCALA_REFERENCIA](ESCALA_REFERENCIA.md).

## Ficha — Guerrero

- Silueta: torso compacto, hombros reforzados y piernas separadas; peso visual en pecho/hombros, manteniendo anatomía humana.
- Vestuario inicial: chaqueta azul pizarra, pantalón, botas/cinturón de cuero y piezas metálicas de uniforme. Placa pectoral y hombreras proporcionadas.
- Materiales: tejido azul grisáceo mate, cuero marrón, acero gris con rugosidad moderada; madera en escudo guardado a la espalda.
- Accesorios: escudo de muestra guardado, sin mecánica de escudo ni bonificación. La espada compartida solo aparece si el estado tiene `training-sword` equipado. No se genera un objeto de inventario al cambiar de clase.
- Lectura isométrica: hombros anchos y brillo limitado del acero frente a los tejidos oscuros; no depende solo del azul.
- Reemplazo final: conservar pecho/hombros moderados y espacio de manos; preparar agarre de espada/escudo como sockets independientes. Las protecciones del uniforme no equivalen a una nueva regla de armadura.

## Ficha — Mago

- Silueta: línea vertical, túnica larga con volumen inferior y mangas amplias; capucha plegada al cuello, cabeza visible.
- Vestuario inicial: túnica índigo, cinturón de cuero, botas y pequeña bolsa. El bajo deja margen sobre el suelo.
- Materiales: tejido índigo poco brillante, cuero oscuro, madera de bastón y remate metálico discreto; sin emisión o efectos mágicos nuevos.
- Accesorios: bastón guardado como referencia de diseño; no es un ítem/habilidad nueva. La espada real equipada se presenta con el mismo binding que en las otras clases.
- Lectura isométrica: falda de túnica y bastón vertical, diferenciables del torso protegido del Guerrero y las extremidades libres del Monje.
- Reemplazo final: abertura de túnica y mangas que permitan locomoción; separar bastón/bolsa, evitar cloth simulation antes de medirla y validar penetraciones al disponer de animaciones.

## Ficha — Monje

- Silueta: cuerpo atlético sin exagerar musculatura, brazos descubiertos y piernas de lectura clara; sin armadura voluminosa.
- Vestuario inicial: chaleco/ropa cruzada de lino, pantalón corto funcional, faja roja oxidada, vendas en muñecas y calzado ligero.
- Materiales: lino beige mate, faja oscura rojiza, cuero y piel; las vendas son geometría y material compartido.
- Accesorios: extremos de faja visibles, manos libres mientras no haya arma real equipada. No se añaden puños mágicos, patadas, esquiva o energía espiritual a las reglas existentes.
- Lectura isométrica: antebrazos descubiertos y franja de faja frente a la túnica del Mago y las hombreras del Guerrero.
- Reemplazo final: buena deformación en hombro/codo/cadera/rodilla; manos utilizables para futuros golpes, sin fijar ahora una colección de animaciones.

## Contrato modular y equipo

`CharacterVisualCatalog` es configuración compartida (ScriptableObject), con `ClassId → prefab`. `CharacterVisualPresenter` observa `GameSession.Changed`; solo sustituye el modelo cuando cambia la clase y reutiliza la instancia en eventos de reloj/mensajes. Mantiene la suscripción acotada a OnEnable/OnDisable. No calcula daño, iniciativa, velocidad o inventario.

La escena incorpora `VisualRoot` bajo el héroe funcional. El render de la cápsula se oculta, conservando su collider y el NavMeshAgent. Deshabilitar el presentador recupera la cápsula y oculta la presentación. El prefab técnico original permanece intacto: soldados RTS/oponentes no reciben automáticamente estas apariencias.

`CharacterVisualModel` enlaza la espada y una sobreprenda acolchada al estado `Weapon`/`Armour`. Los dos accesorios de equipo se reutilizan como prefabs anidados. El vestuario base, el escudo y el bastón guardados son referencia visual de clase; no implican equipo de inventario ni estadísticas. El modelo no necesita conocer GameSession: recibe los valores del presentador. Guardar/cargar conserva el esquema v1 y reconstruye la apariencia a partir de la clase/equipo cargados; no guarda mallas ni referencias Unity en la partida.

Biblioteca aplicada: `Assets/02_Prefabs/Characters/Visuals/` (tres clases, espada y sobreprenda), catálogo/materiales/mallas en `Assets/07_Characters/Provisional/`. Ocho materiales de personaje compartidos; reutilizan mapas propios de D01. Bastón/escudo aprovechan Oak; sin texturas descargadas, fuentes comerciales o plugins nuevos. La autoría comparte la consolidación por material de D01 y mantiene materiales por referencia; los prefabs finales permanecen sustituibles.

Inventario geométrico medido, incluidos los dos accesorios equipables aunque estén inactivos: Guerrero 19.312 triángulos/10 renderers, Mago 18.668/10 y Monje 17.032/9. Altura exterior con pelo 1,808 m; los 8 mm adicionales no alteran la cápsula de 1,80. Se conservan las formas provisionales de primitivas; estos recuentos no son un presupuesto definitivo ni una medición de FPS/draw calls. No se añade pooling o LOD sin necesidad medida. Se corrigió el acero excesivamente oscuro y la intersección de coronilla en una revisión de assets propios, preservando GUID.

## Preparación de modelos finales

1. Entregar FBX u otro formato aprobado por el proyecto, escala de origen en metros. Comprobar dimensión importada contra la referencia de 1 m; no corregirla escalando el controlador. Aplicar transformaciones de exportación y documentar cualquier conversión de ejes.
2. Prefab final a escala (1,1,1), pivote entre los pies en Y=0, Y arriba, frente +Z. Incluir malla/rig solo dentro de VisualRoot; sin scripts de combate, colliders o segundo NavMeshAgent.
3. Proponer un esqueleto humano común y pose de referencia T/A coherente para las tres clases. Validar Humanoid y asignación de huesos en el editor exacto antes de adoptar retargeting; no darlo por aprobado por importar un FBX. Evitar escalas negativas y huesos auxiliares innecesarios.
4. Crear un prefab contenedor con CharacterVisualModel y el FBX como hijo. Separar manos/armas/escudo/bastón mediante sockets y prefabs de equipo; adaptar las referencias del binding sin modificar reglas ni guardado. El catálogo rechaza un prefab sin CharacterVisualModel y recupera la cápsula, evitando instancias repetidas inválidas. No unir permanentemente espada y cuerpo en una sola malla.
5. Preparar normales/tangentes, UV sin estiramientos visibles y materiales URP Lit: albedo sin iluminación pintada, normales y mapas de metal/rugosidad convertidos a la convención de smoothness de Unity. Reutilizar materiales/atlas cuando la autoría lo permita; no imponer resoluciones o LOD comerciales sin medir.
6. Probar cabeza/manos a distancia isométrica, puertas de 1,20 × 2,20 m, giro, selección, sombras y separación de compañeros. Comprobar que ropa y accesorios no parezcan ampliar el área de impacto.
7. Sustituir una entrada del catálogo por vez y repetir regresión de clase/equipo/guardado/movimiento/combate. Añadir Animator dentro del modelo cuando existan clips comprobados; el controlador raíz mantiene el desplazamiento. Cualquier root motion futuro requiere una decisión explícita, no se activa por importar el rig.

## Techo D01 y validación

| Comprobación ejecutada | Resultado / evidencia |
| --- | --- |
| Ensayo inicial de techo | Intersección de un mesh real con el rayo al héroe; antes oculto y después visible. Primeras ejecuciones detenidas por captura de variables del ensayo al reiniciar Play Mode y luego por mensajes de diagnóstico; corregidos, sin atribuirlos al juego |
| Tres casos E01 dirigidos | 3/3 aprobados, salida 0; Logs/E01-targeted-results.xml/log |
| Regresión final con refinado | **42/42 aprobadas**, 0 fallos/omisiones, salida 0; 7,83 s de casos (excluye arranque/recargas); Logs/E01-final-results.xml/log |
| Cubierta reversible | Recorta, restaura al alejarse y al deshabilitar; cámara ortográfica y agente preservados; Logs/E01-occlusion.txt |
| Escala/configuración | Cinco prefabs sin colliders/agentes/Animator; tres clases, ocho materiales, veintiuna mallas; catálogo/bindings válidos y medidas en Logs/E01-geometry.txt |
| Integración de personaje | Instancia reutilizada ante mensajes, cambio de clase, desconexión/reconexión del presentador, fallback ante modelo inválido sin acumular objetos, espada/sobreprenda según equipo y carga de partida |
| Física y juego existente | Paso por puerta y bloqueo del almacén, firmas físicas/NavMesh D01 intactos; regresión anterior de selección/UI/combate/RTS/construcción/persistencia aprobada |
| Renders reales | Siete imágenes de Play Mode/GPU revisadas: tres clases, equipo, juego con UI y comparación de techo |
| Build Windows | 178.001.257 bytes, 45,46 s, salida 0; Logs/E01-build-summary.txt/log |
| Recorridos independientes | Sin gráficos 7,50 s y GPU RTX 3050 Laptop 7,33 s; ambos PASS/salida 0, sin excepciones/agentes inválidos; comprobantes y logs E01-headless/graphics en Logs |
| Sesión humana / animaciones finales / FPS | **No realizados**; F2-07 permanece pendiente |

Capturas sin retoque ni imágenes generadas, tomadas dentro del escenario D01: [Guerrero](Images/E01-warrior.png), [Mago](Images/E01-mage.png), [Monje](Images/E01-monk.png), [equipo real](Images/E01-Equipment.png), [en juego con UI](Images/E01-InGame.png), [techo antes](Images/E01-Roof-Before.png) y [techo después](Images/E01-Roof-After.png). Las vistas cercanas usan una cámara temporal de prueba para revisar forma/material; la escena entregada conserva su zoom y controles normales. La comparación del techo mantiene el encuadre y desactiva/reactiva exclusivamente RoofCutaway.

Paquete portable: `ProyectoRPGRTS/Builds/Aethernia-Fase3-E01-Windows.zip`, 73.584.566 bytes, 305 entradas, CRC correcto; SHA-256 `1fd5dd2ba203a288557f497c30ddea607e955b17f59fe3369bf0b8ae14f6e61c`. Exe, DLL runtime y level0 coinciden con la build validada; excluye DoNotShip y conserva por hash los cuatro ZIP históricos. Comprobante: Logs/E01-package.json. Descomprimir todo y ejecutar Aethernia.exe.

Propuesta implementable: `RoofCutaway`, recorte contextual reversible de los renderers de la cubierta. Detecta interposición entre cámara y punto alto del héroe mediante bounds de renderers y un rayo ortográfico paralelo a la vista. No cambia colliders, NavMesh, materiales compartidos, posición o controles de cámara. El ensayo independiente comprueba intersecciones de triángulos reales para demostrar la oclusión inicial. El detector runtime es conservador por bounds: puede recortar un poco antes de que cada teja tape al actor. No es una solución general a paredes, varios pisos o multitudes.

La evaluación humana F2-07 sigue pendiente. Los renders reales y las comprobaciones automáticas no certifican diversión, animación final ni visibilidad en todas las situaciones.

## Revisar y sustituir en Unity

Abrir `ProyectoRPGRTS/` con Unity **6000.5.8f1** y `Assets/01_Scenes/PlayablePrototype.unity`; pulsar Play. En Personaje, elegir Guerrero, Mago o Monje. Seleccionar con clic izquierdo y caminar con clic derecho. Probar inventario/equipo y guardar/cargar con la guía COMO_JUGAR.md. Entrar por la puerta sur de la casa D01: la cubierta se recorta al interponerse y vuelve al alejarse. No es necesario ejecutar un generador.

Para revertir solo la presentación en una revisión: desactivar CharacterVisualPresenter en el héroe recupera la cápsula; desactivar RoofCutaway en la instancia GableRoof recupera la cubierta. Para cambiar modelos, seleccionar `Assets/07_Characters/Provisional/ClassVisuals.asset` y sustituir una entrada por su prefab preparado. Mantener ClassId, controlador, VisualRoot y equipo separados. Repetir las pruebas con el editor cerrado: `Tools/Invoke-UnityValidation.ps1 -Mode Vertical`, después `-Mode Build` y `Tools/Invoke-PrototypeSmoke.ps1` (también `-WithGraphics`). No repetir CharacterVisuals; CharacterPolish es autoría, no un paso de revisión.

Siguiente actividad recomendada: revisión humana de siluetas, tamaño en pantalla, oclusiones y claridad del equipo; después sustituir una clase por un modelo 3D comprobado antes de extender el catálogo. C02, razas, subclases, criaturas y animaciones definitivas no se inician aquí. Recuperación previa: tag `codex/e01-base-ca5ba35`; rama local `codex/e01-character-visuals`.
