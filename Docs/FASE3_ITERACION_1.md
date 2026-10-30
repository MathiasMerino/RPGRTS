# Fase 3 — iteración A/B/C1

Fecha: 2026-10-30. Alcance autorizado por el prompt adjunto: auditoría visual, dirección artística y primera mejora funcional de UI. El escenario D, personajes E y ampliación de interfaz C2 se secuencian después. F2-07 conserva estado pendiente.

## Auditoría A

Git inicial limpio, commit cc9691d. Punto de control local: tag `codex/fase3-base-cc9691d`. Se verificaron XML 34/34 y comprobantes de build/player de fase 2 contra ese estado; la nueva implementación tendrá su propia regresión. GDD SHA-256 sin cambios: dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661; lectura consultada §§12.1–12.6. Plan, checklist, registro y arquitectura revisados por sus apartados actuales.

Captura antes revisada: Docs/Images/Fase2-Exploracion-1280.png. Problemas: nueve botones de creación/equipo/comercio/partida simultáneos; nombre visible en cualquier actividad; ayuda de exploración permanece en combate; siluetas cápsula indiferenciadas por clase; casa/suelo/decorado de prueba sin lenguaje material realista. Las dos últimas carencias pertenecen a D/E, no se ocultan con un cambio de UI.

Escena: PlayablePrototype, cámara y héroe referencian prefabs existentes; UI con paneles/botones y tema compartidos. Materiales URP simples; referencias métricas y NavMesh ya probados. No hay recursos de personajes finales en Assets/04_Sprites ni un pipeline artístico adicional configurado. Se conserva el escenario funcional.

Herramientas comprobadas: ejecutable Unity 6000.5.8f1 instalado, manifest con URP 17.5.0/uGUI 2.5.0/AI Navigation 2.0.14/Input System 1.20.0/Test Framework 1.7.0; lanzadores locales y ensamblados editor/runtime. No hay herramientas Unity/MCP expuestas por los conectores de esta sesión ni paquete MCP en el manifest. No se afirma su ausencia en otros entornos. El catálogo de skills no ofrece una skill Unity; para modificar uGUI y documentos Markdown bastan las herramientas locales. No se instala ni duplica integración.

## Criterios de esta iteración

- Resumen inicial sin la parrilla de nueve acciones; Personaje, Inventario y Partida reutilizan botones/panel, sin ventanas duplicadas.
- Modos de RPG, construcción y RTS muestran únicamente sus acciones existentes y ayuda específica. No hay botones falsos de habilidades/mapa/formaciones.
- Fuente/paleta/tamaños/espaciado centralizados; límites y superposición medidos en render 1280×720 y 1920×1080.
- Ningún cambio de reglas, escala, colliders, NavMesh, guardado o clases. Referencias y GUID preservados.
- Pruebas funcionales de UI, suite previa, capturas inspeccionadas y nueva build. Resultados pendientes hasta ejecutar.

Dirección y conceptos de las cuatro clases: [DIRECCION_ARTISTICA](../Documentacion/DIRECCION_ARTISTICA.md). Los valores artísticos no confirmados se identifican como propuestas reversibles.

## UI C1 implementada y validación en curso

ContextUiBuilder amplió una sola vez la escena existente, reutilizando el prefab de botón y todos los comandos anteriores. VerticalSliceHud contiene únicamente estado de presentación (página/modo), mantiene referencias y libera listeners. MedievalTheme centraliza fuente, tamaños, separaciones y botón; no hay nuevos paquetes ni cambios de reglas/guardado. El nombre se oculta fuera de Personaje. El generador ContextUi rechaza una segunda aplicación para evitar duplicados.

La prueba dirigida PagesPreserveActionsAndModesHaveReadableSeparateLayouts aprobó 1/1 con salida 0. Comprueba las cuatro secciones, cambio real a Mago, equipo/comercio mediante botones, una sola emisión tras cinco reactivaciones, transición RPG/RTS y catorce layouts renderizados (siete contextos × dos resoluciones). No pulsa guardar/cargar del slot normal del usuario; su lógica se cubre con archivos de Logs en la suite previa.

Se revisaron visualmente las siete vistas a 720p y Resumen a 1080p: contenido cabe, pestaña actual resaltada por color/negrita y acciones separadas. Algunas etiquetas del primer Resumen se ven suavizadas; no se declara tipografía final ni accesibilidad humana verificada. La revisión detectó Interactuar todavía visible/deshabilitado fuera de exploración; se corregirá antes de cerrar C1. La regresión completa está en ejecución.

Comparación del YAML con cc9691d: 76 bloques de transform/cámara/renderer/físicas del mundo idénticos; prefab y NavMesh de mundo no se modifican. Las líneas nuevas de escena pertenecen al layout, estilos y cuatro botones de navegación reutilizados.

## Cómo probar C1 manualmente (pendiente)

Abrir ProyectoRPGRTS con Unity 6000.5.8f1, cargar Assets/01_Scenes/PlayablePrototype.unity y entrar en Play. En Resumen comprobar que no aparecen las nueve acciones antiguas. Abrir Personaje: cambiar clase y escribir nombre; comprobar que WASD mientras se escribe no desplaza cámara. Abrir Inventario, recoger cofre cercano y probar equipamiento/comercio. Abrir Partida para usar el guardado/carga normal, o F5/F9 fuera de encuentro.

Cambiar a Construir y revisar coste/huella e instrucciones, después volver a Explorar. En combate RPG observar iniciativa/objetivo/acción/movimiento; la ayuda y los controles son propios del modo. Tras una victoria, escalar a RTS y comprobar órdenes/selección, con Interactuar oculto. Repetir la lectura en Game a 1280×720 y 1920×1080, valorar nitidez/contraste real y anotar dificultades. Este guion no se registra como sesión realizada y no completa F2-07.

Siguiente bloque de producción propuesto: F3-D01, muestra pequeña de suelo/camino/fachada/decoración sobre el escenario existente según la dirección documentada; después E01 para apariencias y C02 para mapa/diario/retrato. No se afirma que C1 incluya esos recursos.

## Cierre técnico de C1/F

Suite final con Unity 6000.5.8f1: 35/35 aprobadas, cero fallos/omisiones, salida 0; 8,33 s de casos excluyendo importación/arranque y recargas del editor. Evidencia en ProyectoRPGRTS/Logs/Phase3-final-results.xml y Phase3-final.log. Mantiene las 34 pruebas previas; la prueba del campo de nombre abre ahora Personaje antes de enfocar. La nueva prueba funcional cubre secciones, comandos reales invocados programáticamente, bloqueo de entrada, listeners, catorce layouts y transición de modos. No equivale a ratón/teclado físicos.

Interactuar se oculta en construcción/RPG/RTS; se revisaron otra vez sus tres capturas finales. Las capturas de los siete contextos a 720p y Resumen a 1080p se conservan en Docs/Images/Fase3-*. No se afirma revisión visual de las seis variantes 1080p restantes; sí pasaron las comprobaciones de geometría/texto del test.

| Contexto | Acciones del panel |
| --- | --- |
| Resumen | 0, más cuatro entradas de sección |
| Personaje | 3 clases, con campo de nombre visible |
| Inventario | 4: espada, túnica, ración, comercio |
| Partida | 2: guardar/cargar |
| Construcción | Colocación mediante clic derecho; coste/huella/producción visibles |
| RPG | Cinco acciones existentes y escalada opcional condicionada |
| RTS | Dos órdenes existentes; cantidades de unidades e instrucciones |

No se ha medido una mejora de FPS. La lectura batch del tramo RTS fue 6,16 s, 4.099 frames y 701.737.780 bytes asignados en una muestra del editor; no es rendimiento comercial ni comparación controlada con otro equipo. No se introdujeron pooling o dependencias.
