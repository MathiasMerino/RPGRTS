# E02-B — Monje provisional integrado

Fecha: 2026-10-30, America/La_Paz. Alcance: presentación del Monje autorizada por el nuevo brief, conservando E01/E02 y D01. El uso provisional del concepto está autorizado; su aprobación artística final y F2-07 siguen pendientes. No se inicia otra fase.

## Implementación

`Assets/02_Prefabs/Characters/MonkProvisional/Monk_ConceptProxy.prefab` presenta cabeza rapada, torso descubierto parcialmente, paño naranja asimétrico sobre hombro izquierdo, faja oscura, pantalones amplios, vendas claras y calzado ligero. Es geometría estática provisional, no un modelo humanoide final.

| Medida/recurso | Resultado comprobado en Unity |
| --- | --- |
| Escala y ejes | 1 unidad = 1 m; raíz a escala uno, Y arriba, frente +Z, pies Y=0 |
| Bounds incluyendo equipo | 0,663 × 1,780 × 0,400 m |
| Física conservada | Cápsula/agente 0,60 × 1,80 m; radio 0,30 m |
| Geometría incluyendo equipo inactivo | 5.252 triángulos, 10 renderers; seis mallas de cuerpo/ropa agrupadas por material |
| Materiales nuevos | BurntOrange y DarkTrousers, URP/Lit; reutilizan mapas D01 |
| Materiales reutilizados | Skin, MonkCloth, Leather y Hair de E01; sin modificarlos |
| Equipo | Espada de 0,96 m y sobrevestido E01 anidados, activados por Weapon/Armour existentes |

Mallas/materiales/ilustración: `Assets/07_Characters/MonkProvisional/`. La autoría se horneó una vez con MonkIdentityBuilder/MonkProxyGeometry; no crea primitivas o mallas en ejecución. Los recuentos no son un perfil de FPS/draw calls.

El catálogo existente cambia solo la alternativa visual del Monje y añade datos opcionales de ilustración/UV. CharacterVisualPresenter y CharacterVisualModel siguen gestionando clase/equipo; controlador, reglas y guardado v1 no cambian. Guerrero/Mago, variante original E01, candidato vacío E02 y biblioteca D01 se conservan. La escena solo añade la tarjeta y su enlace al HUD; firma física estática y hash de NavMesh sin cambios.

## Interfaz y procedencia

En Explorar → Personaje → Monje aparece `Assets/02_Prefabs/UI/ClassReferenceCard.prefab` dentro del panel existente. Comparte fuente/color con MedievalTheme; muestra ilustración, nombre y estadísticas **base** reales (20 PV, daño 5, alcance 1,8 m, velocidad 4 m/s). La vista del héroe permanece en el mundo, sin segunda cámara/controlador. La tarjeta se oculta en otras clases, páginas y modos.

La hoja original generada E02-A se copia sin editar el PNG: SHA-256 `6da0717d6f5682ce80e9d22992f4b92ad990404c6c695f078b1b659b814fbdb1`. RawImage recorta cabeza/torso frontal mediante UV y AspectRatioFitter conserva proporción; no incluye textos de la lámina. Se importa una única textura, sin mipmaps. La captura de referencias del usuario permanece fuera de Assets y no se distribuye en la build. No hay descargas o instalaciones nuevas.

## Evidencia y pruebas

Tres pruebas nuevas en MonkIdentityTests: geometría/métrica/URP y conservación de recursos; selección por botones existentes, raycast de cápsula, ruta real hasta cofre, interacción, inventario/equipo, guardado aislado/carga y victoria RPG; tarjeta sin recortes/solapamientos a 1280×720 y 1920×1080 con comparación visual controlada. Ejecución dirigida: **3/3, cero fallos/omisiones, salida 0**. Comprobantes: `ProyectoRPGRTS/Logs/E02B-targeted-results.xml`, log, geometría y guardado de ensayo.

| Validación ejecutada con Unity 6000.5.8f1 | Resultado |
| --- | --- |
| Regresión completa (46 anteriores + 3 nuevas) | **49/49**, cero fallos/omisiones, salida 0; casos 6,89 s, excluyendo arranque/recargas |
| Compilación Windows | Succeeded, salida 0; 178.268.191 bytes, 53,44 s |
| Recorrido del ejecutable sin gráficos | PASS, salida 0; 7,29 s de recorrido |
| Recorrido del ejecutable con GPU | PASS, salida 0; 7,87 s; RTX 3050 Laptop, Direct3D 12 |
| Revisión de logs finales | Sin errores/avisos C#, excepciones o fallos de agente detectados |
| Integridad | 259 .meta/GUID sin faltantes/huérfanos/duplicados; reglas/NavMesh, D01/E01, candidato/perfil/controller E02, GDD, paquetes y ProjectSettings intactos; historial previo del checklist preservado |

Evidencia local: `ProyectoRPGRTS/Logs/E02B-final-results.xml`/log, E02B-build.log/summary, E02B-headless-smoke/player, E02B-graphics-smoke/player y E02B-integrity.json. Las pruebas de editor ejecutan Play Mode y acciones reales por APIs/botones; los recorridos nativos cubren sistemas generales. No equivalen a probar todos los controles con un usuario ni a medir FPS.

Entrega portable: `ProyectoRPGRTS/Builds/Aethernia-Fase3-E02B-Windows.zip`, 73.710.653 bytes, 305 entradas, CRC correcto, SHA-256 `fd91da79bf62154eb5569fbb90bf3863d667fd002bbc3dcdad0ea400b6ce956d`. Exe, Aethernia.Runtime.dll y level0 coinciden por hash con la build probada; seis ZIP históricos conservan su hash. No se incluyen directorios DoNotShip. Comprobante E02B-package.json. Descomprimir todo y ejecutar Aethernia.exe; no requiere abrir Unity.

Capturas nativas GPU en Play Mode, no renders conceptuales:

| Captura | Qué permite revisar |
| --- | --- |
| [Antes E01](../../Images/E02B-Before.png) / [después E02-B](../../Images/E02B-After.png) | Misma posición, cámara, zoom e iluminación; UI oculta temporalmente para comparar modelos |
| [Dorso](../../Images/E02B-Back.png) | Paño posterior y pantalón; giro de prueba |
| [Equipo](../../Images/E02B-Equipment.png) | Espada/sobrevestido existentes visibles |
| [Personaje 720p](../../Images/E02B-Character-1280.png) / [1080p](../../Images/E02B-Character-1920.png) | UI real y cámara isométrica con zoom permitido de 7 m |
| [RPG](../../Images/E02B-RPG.png) | Monje en encuentro existente; tarjeta de exploración oculta |

Las cuatro capturas de detalle utilizan ortográfica de 2,35 m únicamente en la prueba; no cambian límites ni transform de cámara guardados. Revisadas por el agente: silueta/ropa reconocibles, materiales presentes y recorte/UI sin texto cortado. No se realizó una sesión humana de ratón/teclado: F2-07 y aprobación estética permanecen pendientes.

## Límites y sustitución futura

Anatomía, rostro, manos sin dedos, uniones de extremidades y pliegues son aproximaciones geométricas visibles. La ropa usa paños planos; no hay simulación, rig, skinning ni animación. El sobrevestido genérico cubre parte del paño y admite intersecciones: refleja la capacidad existente, sin crear ropa adaptativa o un sistema de combate sin armas. La espada mantiene su postura estática E01. La legibilidad humana a distintas distancias y la calidad artística final quedan por evaluar; no se afirma rendimiento medido.

Para sustituir: preparar FBX métrico con pivote a pies/frente +Z, malla skinned, Humanoid válido, materiales URP y sockets según [contrato E02](../../E02_IMPORTACION_HUMANOID.md) y [ficha Monje](../../../Documentacion/Personajes/MONJE_DISENO_BASE.md). Usar el workbench para un **nuevo candidato HumanoidPrefab**; mantener este proxy como fallback. No añadir agentes/controladores/colliders al modelo ni root motion. Modelo/rig reales (F3-E02-M) y clips/retargeting (F3-E02-A) siguen pendientes; los siete estados vacíos E02 no son animaciones.

## Revisar y recuperar

1. Abrir ProyectoRPGRTS con Unity **6000.5.8f1**, escena `Assets/01_Scenes/PlayablePrototype.unity`; esperar importación y revisar Console.
2. Play → Personaje → Monje. Comprobar ilustración y modelo; alternar Guerrero/Mago/Monje. Seleccionar héroe, caminar con clic derecho y acercarse al cofre; E entrega equipo. Inventario → equipar espada/túnica.
3. Partida → Guardar, cambiar clase y Cargar; comprobar Monje/equipo. Probar RPG con movimiento/ataque/fin de turno. Revisar UI a 1280×720 y 1920×1080. Registrar observaciones humanas, sin cerrar F2-07 automáticamente.
4. Prefab aislado: abrir Monk_ConceptProxy; editar únicamente recursos nuevos. No repetir MonkIdentity/HumanoidPipeline ni otros generadores sobre assets existentes.

Punto de recuperación Git previo: `codex/e02-b-base-afa1e7e`; rama de trabajo `codex/e02-b-monk-proxy`. Reversión visual local: devolver Entry monk.Prefab a `Characters/Visuals/monk_Visual.prefab` y retirar/desconectar la tarjeta. GUID del catálogo, física y recursos anteriores preservados. Desactivar el presentador recupera la cápsula como antes.
