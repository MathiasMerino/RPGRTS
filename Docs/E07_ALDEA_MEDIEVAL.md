# E07 — Aldea medieval integrada

Fecha 2026-10-31. Encargo E07 autorizado por el usuario, sin iniciar E08. Rama feature/e07-aldea-medieval creada desde dab39c2, cabeza E03–E06; PR #1 abierto/borrador y NO fusionado. El PR E07 se apilará hacia feature/e03-e06-personajes-vertical-slice. main/dev no se fusionan.

## Plan secuencial

1. E07-P0: revisión, línea base física/navegación, inventario y diez encuadres antes/después en AetherniaVillageDemo. Misma cámara real, posiciones/zoom comparables; conservar originales.
2. E07-P1: biblioteca compartida de materiales y módulos de piedra/entramado, cubiertas inclinadas/aleros/cumbreras, vivienda, posada y almacén con tamaños métricos. Conservar recursos D01.
3. E07-P2: suelo/caminos/plaza, bosque enmarcado, arbustos y utilería funcional. Edificios nuevos con física/carving propios fuera de ruta misión, RTS y construcción; suelos de presentación sin cambiar altura navegable.
4. E07-P3: integrar autoría sobre la escena existente, mejorar tarde cálida/sombra fría y visibilidad contextual reutilizando RoofCutaway. Mantener IDs, posiciones NPC/carga, lógica y datos RPG/UI.
5. E07-P4: capturas comparables, navegación/oclusiones y regresión existente; comprobar mejoras visuales reales y medir complejidad/recursos/draw calls cuando estén disponibles. Corregir problemas y repetir pruebas pertinentes.
6. E07-P5: build Windows, recorridos con/sin GPU, ZIP verificado, documentación y PR dependiente sin fusionar el #1. Cierre técnico separado de evaluación humana/artística.

## Fuentes y límites iniciales

GDD original v0.2 intacto por SHA-256 dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661; lectura §§12.1/12.2/12.4/12.5. Unity 6000.5.8f1 / URP 17.5.0. Reutilizar D01, CharacterVisualCatalog/E03, RoofCutaway, misión Facts v1 y C02. No hay herramientas Unity/Blender expuestas; se utiliza autoría nativa mediante C# de editor, prefabs y geometría modular. Las texturas originales de piedra/madera/tierra/pizarra/hierba se generaron con la skill imagegen y herramienta integrada; se importarán como superficies, nunca como capturas de juego o modelos 3D. Procedencia/prompts se guardarán con los recursos. No descargas de terceros ni instalaciones.

F2-07 conserva 7/8 comprobaciones humanas informadas; X/ESC pendiente de confirmación. Personajes estáticos, sin rigs/clips finales. Todas las decisiones visuales E07 son provisionales, reversibles y pendientes de aprobación artística humana.

## Resultados

Preparada técnicamente para revisión; aprobación humana/artística independiente.


### E07-P0 verificado

PR #1 abierto/borrador/no fusionado, dev 742c04f y base feature dab39c2. Compilación nativa y prueba dirigida 1/1 aprobadas. Doce capturas 1080p de cámara real con encuadres explícitos conservadas en Docs/Images/E07/Before; snapshot de física original versionado, hash de NavMesh registrado en Logs. NPC (−7,5;−3), carga (−10;3), casa (6;6), geometría funcional y referencias identificados. Primer arnés de captura intentaba salir de un combate activo; se corrigió para completar el encuentro real sin alterar reglas o pruebas existentes. Las capturas de antes muestran una plataforma de hierba, casa D01 y objetos técnicos dispersos.


### E07-P1 verificado técnicamente

Doce prefabs nativos: siete módulos (entramado/piedra/puerta/ventana/cubierta/cerca/muro) y cinco conjuntos (casa 6×8 m abierta sobre física original, posada 5,5×6 m, casa de piedra 4,5×4,5 m, almacén 4,2×5 m y cobertizo 3×3 m). Cubiertas de dos aguas, vuelos/aleros/cumbreras, cerchas, chimeneas, ventanas con jambas/contraventanas y puertas proporcionadas. Mallas se consolidan por material/grupo conservando la separación contextual de cubiertas/muros altos. Son modelos modulares generados por autoría, no edificios comprados o arte final.

Cinco PNG originales 1254² intactos se importan hasta 1024 con mipmaps, repetición y anisotropía 4. Trece materiales E07 URP Lit nuevos comparten esos albedos y microrrelieve procedural D01 de baja intensidad; ese normal aproximado no es un PBR escaneado. Hierro/cuero/lino se reutilizan sin copiar materiales. Variantes clara/oscura y piedra cálida/gris comparten mapas con tintes intencionales. Procedencia/prompts exactos en E07_TEXTURAS_ORIGINALES.json. Generación/compilación salida 0 y prueba dirigida 1/1 aprobada; apariencia integrada se verificará tras P3.


### E07-P2 verificado técnicamente

Veintiún prefabs adicionales: dos árboles de ramas/hojas tridimensionales (9.696 tris/3 renderers cada uno, alturas medidas 6,689 y 7,525 m), arbusto, plantas bajas, roca y tronco caído; fondo de suelo 80 m, tramo de camino irregular y plaza de adoquines; banco/mesa/saco/leña/farol/antorcha/señal/herramientas/cesta/pozo/carreta/monolito. Barriles, cajas y cofres D01 se reutilizan sin modelar copias equivalentes. Total E07: 33 prefabs. Terreno plano dentro del área navegable de 40×40 m, relieve de presentación solo fuera de ella; sin sustituir NavMesh. Árboles con colisión/carving del tronco, plantas blandas transitables y props sólidos con física. Generación salida 0 y 2/2 dirigidas aprobadas. Métricas completas en Logs/E07-library-geometry.txt; la distribución integrada sigue en P3.

La inspección de RTS detectó su despliegue fijo X −12/−6 y Z 11..15,5; esa franja y su conexión quedan libres de construcciones/troncos. Se reserva también el spawn real del enemigo junto a la carga. La casa de piedra se ubicará al suroeste, evitando esos sectores; no se mueven coordenadas de misión para acomodar decorados.


## Distribución aplicada y conservación

| Zona | Ubicación X/Z, metros | Función y conservación |
| --- | --- | --- |
| Entrada | −10 / −16 | Pilares y dintel; camino de acceso al suroeste |
| Plaza | −3 / −3 | Empedrado, pozo, banco, farol y accesos; NPC en −7,5 / −3 intacto |
| Casa abierta | 6 / 6 | Presentación nueva sobre colisiones originales; conserva entrada de 1,20×2,20 m |
| Posada provisional | −1 / 12 | Dos niveles, entramado, chimenea, puerta cerrada, mesa y banco |
| Casa de piedra | −15 / −7 | Cimiento/mampostería, puerta y acceso propios, fuera del spawn de combate |
| Almacén | 14 / 4 | Sacos, barril, caja y leña; no ocupa la prueba de taller en 12 / −10 |
| Cobertizo | −2 / 5 | Cubre el obstáculo de suministros anterior, conserva su colisión y añade solo postes propios |
| Bosque/cargamento | carga −10 / 3 | Sendero ramificado, carreta y suministros; conserva NPC/carga/portal y combate real |
| Despliegue RTS | X −12/−6; Z 11..15,5 | Franja libre de nuevos obstáculos; árboles enmarcan la zona |

No se sustituye AetherniaVillageDemo por otra escena. PlayablePrototype y assets D01/E03 quedan intactos; su grupo visual D01 permanece en la demo como recuperación desactivada, con la física original fuera de ese grupo activa. Los proxies visuales técnicos se cubren con arquitectura/ruinas/señales y suministros en sus mismas coordenadas. Los edificios cerrados nuevos tienen BoxCollider/NavMeshObstacle; las colisiones originales y el asset NavMesh no se modifican. El terreno nuevo extiende el fondo visual fuera del cuadrado navegable; no amplía los límites jugables ni tapa la mazmorra situada desde X=45. Relieve solo de fondo, Y=0 dentro del área jugable; adoquines y caminos son láminas de presentación de pocos centímetros.

## Arquitectura, escala y sustitución

Raíces de prefabs a escala uno, 1 unidad = 1 m. Las dimensiones nominales describen cuerpo/huella; aleros, cumbrera, chimeneas y accesorios amplían los bounds. [Inventario medido completo](Validacion/E07_BIBLIOTECA.txt) contiene las 33 piezas, triángulos, renderers y colliders.

| Conjunto | Bounds reales X×Y×Z, metros |
| --- | --- |
| Casa 6×8 nominal | 6,800 × 5,998 × 9,090 |
| Posada 5,5×6 nominal | 6,300 × 7,328 × 7,090 |
| Casa de piedra | 5,300 × 5,146 × 5,590 |
| Almacén | 5,000 × 5,238 × 6,090 |
| Árboles | alturas 6,689 y 7,525; tres renderers y 9.696 tris cada uno |
| Carreta, incluidos ejes/varas | 1,820 × 1,325 × 2,991 |
| Saco | 0,680 × 0,750 × 0,680 |
| Banco | 1,800 × 0,910 × 0,460 |

Guerrero/Mago 1,80 m y Monje 1,78 m conservan mallas, retratos, estadísticas y cápsula/agente de E03. No se introduce rig, animación, población con IA ni interiores nuevos de edificios cerrados. Las piezas modulares se conservan como biblioteca y los conjuntos se hornean por material/grupo de oclusión mediante código de autoría; modificar un módulo no regenera automáticamente los conjuntos ya horneados. No repetir generadores sobre assets existentes.

## Luz, cámara y visibilidad

Luz natural cálida de tarde con sombras suaves, relleno Trilight frío para leer fachadas, tres puntos de farol sin sombras y emisión moderada. Volume propio de escena con ACES, contraste +8, saturación −7, exposición +0,18, bloom 0,10 y viñeta 0,08; niebla leve. Se reutiliza SSAO existente del renderer URP, sin cambiar el pipeline, añadir paquetes o hornear iluminación prematuramente. No se añade una reflection probe sin necesidad demostrada. Solo la assembly de autoría declara referencias a los paquetes URP/uGUI ya instalados.

Cámara/controles conservados: isométrica 35,26439°/45°, zoom normal 7..25, pan/rueda y guardas de UI. Encuadre inicial de esta demo ajustado a foco −1/1 y tamaño 13. Sigue siendo cámara manual, sin añadir seguimiento automático. Las tomas comparativas fijan foco/zoom sobre Camera.main; las de detalle de clase usan zoom temporal 4,5 para inspección, sin cambiar el límite del juego.

RoofCutaway existente oculta/restaura cubiertas y muros altos conservando el zócalo; copas y troncos también se recortan cuando obstruyen al héroe. No cambia colisiones o materiales, no requiere un gestor nuevo. La prueba reproduce el bloqueo real con los componentes desactivados y comprueba rayos contra triángulos al activarlos, dentro de la casa, detrás del edificio, en el bosque, junto a un tronco y de regreso al NPC. Es una comprobación de posiciones concretas, no una aprobación humana de todas las situaciones de cámara.

## Ajustes basados en evidencia

Primera muestra integrada: 1.292 draw calls y 1.258.614 triángulos de frame según UnityStats en el encuadre general; 294 renderers/269.263 triángulos retenidos E07. Se agrupa solo follaje bajo por material y celda, manteniendo instancias fuente sin render para edición; árboles conservan recorte individual. Se retiran sombras innecesarias de suelo, plantas bajas y árboles lejanos. Nuevas franjas de hierba rompen bordes de camino; se varía su distribución para evitar una línea uniforme. Texto direccional se adelanta por el eje de cámara para que la tabla no recorte letras. El suelo de fondo se extiende sin alterar el prefab de 80 m o la mazmorra. Las muestras posteriores se registran en la evidencia final; son contadores de editor/offscreen, no FPS.

Una expectativa nueva de llegada usaba distancia al punto pedido, aun cuando el controlador permite muestrear NavMesh a 0,60 m junto a obstáculos originales. Se corrige el arnés para exigir llegada al destino navegable muestreado dentro de stoppingDistance + 0,08 y comprobar interacción real con el NPC; no se cambian controlador, obstáculos originales o pruebas anteriores. La primera compilación del contador intentó UnityStats.batches, ausente en este editor; se eliminó ese contador, conservando solo APIs disponibles. Incidencias y logs locales se preservan.

## Revisión manual

1. Seleccionar la rama feature/e07-aldea-medieval; depende de E03–E06 y no presupone sus PR fusionados. Abrir ProyectoRPGRTS en Unity 6000.5.8f1. Esperar importación, revisar Console, abrir Assets/01_Scenes/AetherniaVillageDemo.unity y pulsar Play. No ejecutar generadores.
2. Revisar Game a 1280×720 y 1920×1080; elegir Monje/Guerrero/Mago, comprobar retrato/equipo, escribir nombre y cerrar con ×/ESC. Confirmación humana pendiente.
3. Seleccionar al héroe y caminar con clic derecho; WASD/flechas/rueda para cámara. Recorrer entrada, plaza, casa abierta y bosque; comprobar cortes/restauración de cubiertas y árboles. Casas cerradas/pozo/utilería son ambientación; interacciones anteriores siguen identificadas por sus etiquetas.
4. E/Interactuar cerca del habitante (−7,5;−3), aceptar; dirigirse al cargamento (−10;3). Acercarse, esperar llegada, atacar/recuperar y finalizar turnos hasta ganar. Regresar y entregar una sola vez: 20 oro y ración si cabe. Guardar/cargar F5/F9 fuera de combate en cada etapa y repetir con las otras clases.
5. Probar Inventario/cofre/equipo y Menú/construcción/RTS. El despliegue RTS conserva su zona original; la escalada requiere victoria RPG. Registrar lectura, oclusiones y atascos, además de aprobación artística o cambios solicitados.
6. Para Windows, extraer el ZIP completo y ejecutar Aethernia.exe junto a Aethernia_Data y DLL; no necesita herramientas internas. Es build Development de revisión, no release final.

F2-07 sigue con 7/8 informado, sin confirmación humana nueva de X/ESC. Aprobación artística E07 y modelos/rig/clips definitivos pendientes. Siguiente actividad recomendada: revisión humana de esta aldea y sus transiciones de visibilidad; E08 no se inicia automáticamente.


## Pruebas y capturas reales

Regresión completa **67/67 aprobadas**, cero fallos/omisiones, Unity salida 0; XML/logs E07-final-* locales. Runner EditMode con UnityTest entrando en Play Mode. Cinco casos nuevos (3 Test y 2 UnityTest) verifican biblioteca, terreno/colliders, física/nav/referencias de escena, vistas reales y ruta/oclusión. Las 62 pruebas anteriores siguen ejecutándose sin modificar sus expectativas para acomodar E07: incluyen tres clases, equipo, misión completa con retorno/recompensa única/persistencia v1 por etapa, inventario lleno/victorias ajenas, cámara, UI/puntero/ESC, RPG, construcción/economía y RTS.

Muestra final del encuadre general: 1.079 draw calls frente a 1.292 iniciales (−16,49 %), SetPass 42 y 1.071.218 triángulos de frame. E07 retiene 305.573 tris y 340 renderers, de los que 271 están activos en esa muestra; 281.993 tris de geometría activa. Los datos retenidos incluyen fuentes de follaje sin render, deliberadamente conservadas para edición/recuperación. Valores editor/offscreen que incluyen pases de render; no son FPS ni presupuesto comercial. [Resultados resumidos y hashes](Validacion/E07_RESULTADOS.json).

Las 24 imágenes de la tabla son renders auténticos de Unity: mismo escenario, Camera.main, ángulo y foco/zoom explícitos, 1920×1080. Las texturas generadas se usan en materiales; no sustituyen estas imágenes. El HUD normal y las otras pruebas también se comprobaron a 1280×720/1920×1080.

| Vista | Antes | Después |
| --- | --- | --- |
| Vista general | [Antes](Images/E07/Before/01-Overview.png) | [Después](Images/E07/After/01-Overview.png) |
| Plaza central | [Antes](Images/E07/Before/02-Plaza.png) | [Después](Images/E07/After/02-Plaza.png) |
| Arquitectura | [Antes](Images/E07/Before/03-Architecture.png) | [Después](Images/E07/After/03-Architecture.png) |
| Camino al bosque | [Antes](Images/E07/Before/04-Forest.png) | [Después](Images/E07/After/04-Forest.png) |
| Monje | [Antes](Images/E07/Before/05-Class-monk.png) | [Después](Images/E07/After/05-Class-monk.png) |
| Guerrero | [Antes](Images/E07/Before/05-Class-warrior.png) | [Después](Images/E07/After/05-Class-warrior.png) |
| Mago | [Antes](Images/E07/Before/05-Class-mage.png) | [Después](Images/E07/After/05-Class-mage.png) |
| NPC y entorno | [Antes](Images/E07/Before/06-Npc-area.png) | [Después](Images/E07/After/06-Npc-area.png) |
| Diálogo | [Antes](Images/E07/Before/07-Dialogue.png) | [Después](Images/E07/After/07-Dialogue.png) |
| Objetivo/ruta | [Antes](Images/E07/Before/08-Quest-route.png) | [Después](Images/E07/After/08-Quest-route.png) |
| Encuentro RPG | [Antes](Images/E07/Before/09-Rpg.png) | [Después](Images/E07/After/09-Rpg.png) |
| Interfaz medieval | [Antes](Images/E07/Before/10-Hud.png) | [Después](Images/E07/After/10-Hud.png) |

Recorte reproducido: [cubierta activada/obstrucción](Images/E07/E07-cutaway-disabled.png) y [recorte contextual/héroe visible](Images/E07/E07-cutaway-enabled.png), sin cambiar las colisiones. Capturas iniciales anteriores al pulido se conservan localmente en Logs/E07-initial-visual. La inspección visual del agente verificó composición, materiales, carteles, objetivo, UI y figuras; no se etiqueta como sesión humana.


## Build Windows y cierre técnico

| Ejecución final | Resultado |
| --- | --- |
| Windows x64 Development, misma demo | Succeeded/salida 0; 195.637.067 bytes; 43,71 s incremental final (primera build E07: 174,89 s) |
| Player sin gráficos | PASS/salida 0; 36,05 s; tres clases, misión real y sistemas anteriores |
| Player GPU RTX 3050 Laptop / D3D12 | PASS/salida 0; 35,59 s; mismos recorridos y captura real válida |
| Persistencia | v1, etapas aceptada/recuperada/completada, clase/equipo/edificio/hechos y pago único |
| Memoria asignada al cierre | 114.757.691 bytes headless y 145.522.552 GPU/captura; muestra Development, no pico/FPS/presupuesto final |

[Captura de la build 1280×720](Images/E07/Standalone-1280.png), inspeccionada tras completar los tres recorridos: aldea, UI, clase final y misión completada. Taller dinámico y combatientes RTS/enemigos siguen como proxies técnicos anteriores.

La captura estándar en batch no produjo una imagen útil. Un primer proceso normal oculto se pausaba con runInBackground=0 y se detuvo solo ese proceso propio. La captura automática a RenderTexture también resultó vacía: se descartó y se consultó la API URP instalada. El capturador final solicita explícitamente Camera.main y su Canvas con SingleCameraRequest, comprueba variación de píxeles y conserva el PNG inspeccionado. Captura y ejecución en segundo plano solo se activan con -aetherniaSmoke; la pausa normal/configuración del juego no cambia.

La regresión 67/67 precede a ese último ajuste exclusivo de diagnóstico. Escena y código funcional no cambiaron; el capturador final se compiló y se repitieron los recorridos headless/GPU del binario final. No se afirma una segunda ejecución de las 67 pruebas tras el ajuste. Las assemblies declaran referencias a URP ya instalado, sin cambiar paquetes/versiones.

Paquete local: `ProyectoRPGRTS/Builds/Aethernia-E07-Aldea-Windows.zip`, **83.358.761 bytes**, 305 entradas, CRC correcto; exe/UnityPlayer/Aethernia.Runtime.dll/level0 coinciden con la build. Nueve ZIP históricos intactos. SHA-256 `967592477432b69635216ec85055892e9b9a5b96814bd3a73a736b073c33698a`. Builds/Logs quedan fuera de Git. Logs completos: E07-final-*, E07-build-final-*, E07-headless-final-*, E07-graphics-final-* y E07-package.json. [Resumen versionado](Validacion/E07_RESULTADOS.json).

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical
.\Tools\Invoke-UnityValidation.ps1 -Mode BuildVillage
.\Tools\Invoke-PrototypeSmoke.ps1 -RequireCargo -RequireVillageE07
.\Tools\Invoke-PrototypeSmoke.ps1 -WithGraphics -RequireCargo -RequireVillageE07
```

Código de build verificado: cecbed4. PR apilado pendiente de publicación en el cierre; main/dev/#1 no se fusionan. Evaluación humana E07, confirmación X/ESC y modelos/rig/clips finales pendientes. No se inicia E08.
