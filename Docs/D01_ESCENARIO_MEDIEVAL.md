# D01 — Escenario medieval y biblioteca visual

Fecha: 2026-10-30. Alcance autorizado: una sección pequeña de PlayablePrototype, sin personajes definitivos ni mecánicas nuevas. Fuente: GDD v0.2 y Documentacion/DIRECCION_ARTISTICA.md. Original Word preservado; SHA-256 `dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661`.

Actualización posterior E01: la oclusión aquí registrada se reprodujo técnicamente y se añadió un recorte contextual reversible de cubierta; véase [E01_PERSONAJES](E01_PERSONAJES.md). Las capturas y resultados D01 siguientes conservan el estado histórico de esa entrega. La evaluación humana sigue pendiente.

## Resultado

Casa de entramado de madera, zócalo de piedra, yeso, cubierta inclinada de pizarra, chimenea, ventanas y entrada abierta. Suelo con variación natural, camino de tierra, barril, cajas arriostradas, cofre y vegetación baja. Luz principal cálida con sombras suaves y relleno ambiental frío en URP. Es una muestra inicial, con geometría sencilla y proporciones métricas; no arte comercial definitivo.

Se conservan los objetos lógicos, colliders, interacción, NavMesh y sistemas anteriores. La nueva biblioteca aporta presentación sobre sus volúmenes existentes. La cápsula azul, los portales técnicos, la mazmorra y los talleres conservan su aspecto provisional. La biblioteca no es todavía un kit de interiores, puertas animadas ni edificios construibles.

## Biblioteca y procedencia

- Prefabs: `ProyectoRPGRTS/Assets/02_Prefabs/Environment/Medieval/`, separados en Architecture, Props y Vegetation.
- Architecture: WallBay_2m, OpenDoor_120x220cm, Window_100x120cm, GableRoof_660x860cm y TimberHouse_6x8m. La casa reutiliza las piezas mediante prefabs anidados.
- Props: Crate_80cm, CrateStack_3x2x2m, Barrel_100cm y Chest_80x80x60cm.
- Vegetation: GroundFern y GrassClump; plantas blandas transitables.
- Materiales/mallas/texturas: `ProyectoRPGRTS/Assets/06_Environment/Medieval/`. Ocho materiales compartidos URP Lit: Ground, Earth, Stone, Oak, Plaster, Slate, Iron y Foliage; dieciséis mapas propios de albedo/normal de 512 × 512, repetibles y con mipmaps.

Toda la nueva geometría y las texturas se crean mediante código de autoría determinista del proyecto. Sin descargas, fuentes de licencia desconocida ni recursos comerciales. Los mapas no incorporan sombras pintadas; la iluminación proviene de URP. Se aprovechan los objetos físicos, materiales técnicos de comparación y componentes de cámara/UI existentes. No hay materiales exclusivos por instancia de barril, caja o planta.

Inventario medido: 23 assets de malla; casa 1.688 triángulos/55 renderers por sus módulos anidados, barril 636/2, almacén de cajas 1.584/1. Grupo D01 de escena: 9.312 triángulos/113 renderers, incluidas las plantas y soportes. Se consolidan piezas de cada módulo por material al autorizarlo, manteniendo la casa modular; no se introduce pooling, LOD o combinación global sin perfilado. Son recuentos de geometría, no tiempos de frame o draw calls medidos. Informe reproducible: Logs/D01-geometry.txt.

Las dimensiones vigentes están centralizadas en [ESCALA_REFERENCIA](ESCALA_REFERENCIA.md). Todas las raíces de los once prefabs tienen escala uno. Son **prefabs visuales sin colliders**: para colocarlos en otra escena jugable hace falta asignar física adecuada y comprobar su NavMesh. Esto evita duplicar obstáculos en esta escena. La cubierta amplía la silueta exterior, sin crear una superficie transitable nueva; la física conserva la cubierta técnica anterior. El barril utiliza el antiguo proxy cúbico de un metro, algo más conservador que su contorno curvo.

## Corrección de cámara

La verificación encontró un fallo previo: `Mathf.Sign(0)` producía acercamiento continuo cuando la rueda estaba quieta. La prueba aislada reprodujo tamaño 12 → 7 en cinco frames. Ahora el valor cero conserva el zoom y las direcciones de rueda mantienen el comportamiento anterior. Encuadre inicial: foco (3, 0, 4), tamaño ortográfico 15; se conservan controles, inclinación y límites 7–25.

Durante la revisión también se corrigieron el contraste excesivo de las juntas de piedra/pizarra y la unión de la malla del camino. Los ajustes actualizan assets propios conservando sus `.meta` y GUID; no se repiten los generadores iniciales sobre la escena existente.

## Evidencia y límites

| Comprobación ejecutada | Resultado / evidencia local |
| --- | --- |
| Recorrido de la escena antes de D01 | 1/1 aprobado; Logs/D01-before-results.xml y D01-before.log |
| Cámara en reposo, reproducción previa | Falló como se esperaba: 12 → 7 sin rueda; Logs/D01-idle-before-fix-results.xml/log |
| Cuatro pruebas D01 después del refinado/corrección | 4/4 aprobadas, salida 0; Logs/D01-targeted-results.xml/log |
| Regresión final completa en Unity 6000.5.8f1 | **39/39 aprobadas**, 0 fallos/omisiones, salida 0; 8,13 s de casos, sin incluir arranque/recargas; Logs/D01-final-results.xml/log |
| Integridad física y navegación | Firma de colliders estáticos idéntica y NavMesh con el mismo SHA-256; sin colisiones nuevas en biblioteca |
| Movimiento real en Play Mode | Agente cruza entrada hasta (6,0,5), vuelve al cofre y lo activa; almacén sigue bloqueando. Casos anteriores conservan paso estrecho, construcción, RPG/RTS y guardado |
| Interfaz y cámara | Regresión UI contextual 7 modos × 2 resoluciones; cámara quieta estable, zoom/controles/límites y bloqueo por UI comprobados |
| Compilación Windows | 175.686.522 bytes, 258,73 s (incluye shaders nuevos), salida 0; Logs/D01-build-summary.txt y D01-build.log |
| Recorridos independientes del ejecutable | Sin gráficos: 9,02 s; con GPU RTX 3050 Laptop: 7,16 s; ambos PASS/salida 0, sin excepciones ni agentes inválidos; comprobantes D01-headless-smoke.txt / D01-graphics-smoke.txt y logs D01-headless-player.log / D01-graphics-player.log en Logs |
| Revisión visual | Ocho capturas reales inspeccionadas; vista general corregida para incluir toda la chimenea |
| Sesión humana en ventana, diversión/FPS | **No realizada**; F2-07 pendiente |

Las capturas son renders reales de Play Mode con GPU, sin retoque ni generación de imágenes. La comparación controlada «Antes» reactiva temporalmente los renderers/materiales técnicos conservados y su iluminación anterior; «Después» muestra D01. Ambos usan el mismo estado del héroe y los mismos encuadres, con la cámara corregida. La prueba restaura todo al salir y no guarda esta comparación en la escena. Los primeros registros previos a la modificación también permanecen en Logs/D01-before.log y su XML.

| Vista | Antes | Después |
| --- | --- | --- |
| Sector sin UI, 1920 × 1080 | [Antes](Images/D01-Before-Quarter.png) | [Después](Images/D01-After-Quarter.png) |
| Fachada y techo, 1920 × 1080 | [Antes](Images/D01-Before-House.png) | [Después](Images/D01-After-House.png) |
| UI, 1280 × 720 | [Antes](Images/D01-Before-UI-1280.png) | [Después](Images/D01-After-UI-1280.png) |
| UI, 1920 × 1080 | [Antes](Images/D01-Before-UI-1920.png) | [Después](Images/D01-After-UI-1920.png) |

La inspección de estas imágenes comprueba apariencia, proporciones, sombras y lectura bajo UI. No equivale a una sesión humana con ratón en la ventana: F2-07 sigue pendiente. No se afirma haber medido FPS, accesibilidad, diversión, rendimiento comercial ni todas las oclusiones posibles. Un edificio opaco puede ocultar al héroe dentro o detrás; el prototipo no incorpora corte de tejado ni transparencia contextual. No se añaden esas mecánicas en D01.

Paquete portable nuevo: `ProyectoRPGRTS/Builds/Aethernia-Fase3-D01-Windows.zip`, 72.545.489 bytes, 305 entradas, CRC correcto. SHA-256: `8e2262f981164205e658476d594ad03526c5128d04d6fbe821fac33a1c736111`. Ejecutable, DLL runtime y level0 coinciden con la build comprobada. Incluye LEEME y excluye DoNotShip; los tres ZIP históricos conservan su hash. Comprobante en Logs/D01-package.json. Descomprimir todo y abrir Aethernia.exe; no requiere el editor.

## Revisar en Unity

1. Abrir `ProyectoRPGRTS/` en Unity **6000.5.8f1**, esperar importación y abrir `Assets/01_Scenes/PlayablePrototype.unity`.
2. Seleccionar `D01 Medieval Quarter` en Hierarchy para localizar la presentación. Los objetos de referencia originales permanecen separados; no habilitar sus renderers sobre la nueva fachada.
3. Pulsar Play en Game 1280 × 720 o 1920 × 1080. Seleccionar la cápsula azul; clic derecho para caminar, WASD/botón central para mover cámara y rueda para zoom. Con la rueda quieta el encuadre debe permanecer estable.
4. Caminar hasta la entrada sur de la casa, rodear las cajas y acercarse al cofre para interactuar con E. Revisar los modos existentes siguiendo COMO_JUGAR.md. Las plantas bajas no bloquean y el almacén de cajas sí conserva su obstáculo.
5. Revisar piezas reutilizables en Project → Assets/02_Prefabs/Environment/Medieval; materiales y mapas en Assets/06_Environment/Medieval. Las escalas físicas se consultan en ESCALA_REFERENCIA.md.

Con ese editor cerrado, repetir `Tools/Invoke-UnityValidation.ps1 -Mode Vertical`, luego `-Mode Build` y `Tools/Invoke-PrototypeSmoke.ps1` (opcionalmente `-WithGraphics`). No ejecutar los modos de generación inicial sobre los assets existentes.

## Recuperación y siguiente actividad

Punto previo conservado en Git: tag `codex/d01-base-8197b5f` (8197b5f). SampleScene, TechnicalSandbox y los prefabs técnicos originales se preservan. No se publica a remotos ni se actualizan paquetes.

Recomendación: revisión humana de esta muestra y de la visibilidad del héroe antes de ampliar el poblado. E01 y C02 permanecen pendientes y requieren su propio alcance; esta entrega no los inicia. La dirección artística completa, los personajes definitivos y una ciudad extensa no se declaran terminados.
