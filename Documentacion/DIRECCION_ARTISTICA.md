# Dirección artística — Crónicas de Aethernia

Fecha: 2026-10-30. Bloque B de fase 3. Fuente: GDD v0.2, §§12.1, 12.4 y 12.5, original preservado. Este documento se crea en Documentacion/ por petición expresa; las medidas existentes siguen centralizadas en [ESCALA_REFERENCIA](../Docs/ESCALA_REFERENCIA.md).

## Confirmado y provisional

**Confirmado:** fantasía medieval con proporciones realistas y elementos oscuros; regiones con contraste de atmósfera; lectura isométrica; interfaces originales de identidad medieval; 1 unidad Unity = 1 metro; Guerrero, Mago y Monje jugables. Pícaro solo conceptual en esta entrega. No trasladar personajes, interfaces, símbolos o recursos protegidos de las referencias funcionales.

**Provisional y reversible:** las paletas, materiales, siluetas, familias arquitectónicas, vestuario, tamaños de accesorios, iluminación y distribución de UI siguientes. No fijan culturas, historia, razas, reglas RPG ni arte comercial final. A/B/C1 aportaron auditoría, dirección y UI; D01 añade una muestra medieval acotada y E01 prepara tres figuras geométricas de clase sustituibles. Los modelos/rigs definitivos siguen pendientes.

## Paletas y materiales

Mantener formas y valores de luminosidad reconocibles antes de añadir detalle. Reservar acentos saturados para selección, objetivo y magia. Una clase debe distinguirse también por silueta; un bando también por indicadores, no solo color.

| Uso | Paleta propuesta | Aplicación |
| --- | --- | --- |
| UI existente, conservada en C1 | Fondo #1F1A16, botón #473824, oro #D9AB57, texto #F0E3C2 | Colores del MedievalTheme compartido; pestaña seleccionada con texto dorado y negrita |
| Aldea inicial, muestra futura D | Tierra #665641, piedra #76786F, madera #5B4230, yeso #BCB29A, follaje #475B3B | Saturación moderada, camino legible respecto al terreno; desgaste localizado |
| Mazmorra futura D | Piedra #4D5359, fondo #242C32, metal #6B706D, luz cálida #DDA85C | Contraste frío/cálido; preservar suelo transitable y objetos interactivos visibles |
| Guerrero | Azul pizarra #3F5866, cuero #6E4B32, acero #8B918D | Masa de torso/hombros y escudo reconocibles |
| Mago | Índigo #4A4264, tejido gris #807D87, acento ámbar #CCA664 | Línea vertical de túnica/bastón; efecto arcano diferenciado del indicador de selección |
| Monje | Lino #B8A98B, rojo óxido #8D4938, cuero #654B35 | Brazos/piernas libres, faja y vendas, centro de gravedad estable |
| Pícaro, conceptual | Carbón #383E43, verde apagado #526050, cuero #644F42 | Asimetría ligera, capa corta y cinturón; evitar silueta que desaparezca en sombras |

Para la muestra D usar materiales URP Lit compartidos, albedo sin sombras pintadas, variación de rugosidad y normales discretas. Propuestas de smoothness inicial: piedra/madera/tejido 0,1–0,25; cuero 0,2–0,35; metal 0,35–0,6 con metallic apropiado. Son rangos de autoría para comparar, no parámetros aplicados a todo el proyecto. Evitar una textura/material exclusivo por piedra o soldado.

La iluminación debe describir volúmenes; los metales se leen por reflejo y valor, no por convertir todas las superficies en brillantes. Proponer primero una superficie propia pequeña y revisar repetición a distancia isométrica antes de extender texturas.

## Iluminación y cámara

D01 aplica una primera muestra propia: entramado de roble/yeso con zócalo de piedra, cubierta de pizarra, camino de tierra, plantas bajas y accesorios. Ocho materiales URP Lit compartidos con albedo/normal procedurales propios; luz principal cálida con sombras suaves y ambiente Trilight frío/neutro. No se añaden postprocesos o luces por accesorio. Medidas aplicadas en ESCALA_REFERENCIA.md y comparación real en [D01_ESCENARIO_MEDIEVAL](../Docs/D01_ESCENARIO_MEDIEVAL.md). La cámara conserva inclinación/controles/límites; se corrige el zoom involuntario con rueda quieta y se ajusta el encuadre inicial. El resto de las propuestas siguientes continúa provisional.

- Conservar cámara ortográfica, encuadre/control y límites actuales durante C1. Evaluar oclusiones con el zoom mínimo y máximo antes de modificar escenario.
- Aldea: luz principal natural cálida y relleno ambiental neutro/frío; sombras suficientes para separar pies/suelo. Mantener puerta y camino legibles bajo UI.
- Mazmorra: ambiente frío y focos cálidos que orienten hacia salida/objetivo; oscuridad localizada, sin perder navegación ni ataques legibles.
- Comparar una captura neutral con la propuesta atmosférica. Bloom, niebla, exposición, color grading y luces adicionales requieren muestra medida antes de generalizarlos.
- No convertir la dirección cinematográfica en oscurecimiento uniforme. No afirmar un presupuesto de luces/FPS sin hardware objetivo y perfilado con render.

## Arquitectura y decorado modular

Para D: una fachada de la casa existente, camino, suelo y dos o tres decorados reutilizables (barril, poste, pila de leña). Proponer piedra en base, madera estructural y paños de yeso, con encuentros y desgaste que respondan a función/escala. No crear una ciudad ni convertir nombres regionales provisionales en culturas aprobadas.

Organizar piezas por función (muro, esquina, vano, cubierta y adorno), pivotes previsibles y materiales compartidos. La muestra debe conservar la huella de colisión, el acceso a la puerta, los recorridos de prueba y el área de despliegue RTS. Decorados de presentación no deben añadir colliders por accidente; si los necesitan, validar navegación y línea de ataque.

## Personajes y sustitución visual

Base dimensional vigente: altura 1,80 m, diámetro de cápsula 0,60 m, agente radio 0,30 m/altura 1,80 m, raíz a escala 1 y pies en Y=0. Una futura variación de talla exige comprobar collider/puertas/agente; no escalar la raíz del personaje para aparentar una clase diferente.

| Clase | Silueta, vestuario y equipo inicial propuestos | Variantes propuestas | Animación futura |
| --- | --- | --- | --- |
| Guerrero | Torso protegido, hombros moderados, chaqueta acolchada/cota y botas. Espada y escudo de ensayo, metal/cuero/tejido azul | Capa corta opcional, herrajes, tono de cuero, rostro/cabello; sin cambiar dimensiones funcionales | Reposo, caminar/correr, giro, ataque de arma, guardia, impacto y derrota |
| Mago | Túnica por debajo de rodilla con abertura para caminar, cinturón y bastón; capas finas de tejido índigo | Cuello/capucha opcional, ribetes, bolsa/libro, longitud de mangas compatible con manos | Locomoción, canalización/lanzamiento legible, reacción, reposo y derrota; hechizos aún sin reglas nuevas |
| Monje | Cuerpo atlético realista, ropa cruzada ligera, faja, antebrazos vendados y piernas libres. Manos/pies visibles | Vendas/faja, tejido, cabello/rapado y calzado ligero. Sin armadura voluminosa | Postura marcial disciplinada, pasos cortos, desplazamiento rápido, puño/patada, esquiva/guardia e impacto; sin exagerar anatomía |
| Pícaro (conceptual) | Cuero flexible sobre tejidos oscuros, capa corta asimétrica, cinturón y daga; accesorios acotados para no saturar vista | Capucha, bolsillos, paleta y rostro/cabello | Locomoción, agacharse/sigilo, ataque corto y reacción; habilidades y clase no implementadas |

**Contrato visual para E:** mantener ExplorationUnit/NavMeshAgent, collider y selección como base funcional; crear un hijo VisualRoot sin lógica de combate ni colisiones nuevas. Un presentador de apariencia podrá observar ClassId y elegir configuración/prefab de clase. Sustituir solo ese hijo por una malla/rig futuro. Configuración compartida en ScriptableObjects; estado de salud/equipo permanece en PrototypeState. El presentador no calcula daño ni altera velocidades.

E01 aplica este contrato mediante CharacterVisualCatalog/Presenter/Model, tres prefabs visuales y equipo compartido ligado a Weapon/Armour. Fichas completas, procedencia, importación de modelos finales y evidencia en [E01_PERSONAJES](../Docs/E01_PERSONAJES.md). Son figuras estáticas de proporción humana; no se declaran modelos realistas finales ni animaciones. Escudo/bastón guardados son accesorios de diseño sin estadísticas. La cápsula técnica se conserva como física y presentación reversible; el prefab técnico utilizado por RTS/oponentes no cambia. D01 recibe un recorte contextual reversible de cubierta, sin tocar su física ni los controles de cámara; límites y pruebas en el informe E01.

No forzar Animator, retargeting o un controlador de habilidades antes de disponer de un rig/animaciones comprobados. El prototipo seguirá usando geometría provisional hasta el bloque E. El equipo visual futuro deberá reflejar el equipo real o identificarse como muestra, sin sugerir bonificaciones inexistentes.

E02 prepara explícitamente la infraestructura solicitada: candidato Monje vacío y grafo de siete estados sin clips, sin activar Animator sobre la geometría E01. Concepto del Monje se conserva; importación/especificación de manos/ropa, recursos faltantes y límites en [E02_IMPORTACION_HUMANOID](../Docs/E02_IMPORTACION_HUMANOID.md). El modelo y las animaciones reales requieren recursos aprobados y su propia validación; esta preparación no los declara implementados.

## Dimensionamiento

La tabla de medidas aplicadas vive en [Docs/ESCALA_REFERENCIA.md](../Docs/ESCALA_REFERENCIA.md), ampliada con taller/objetos. Las medidas siguientes son **propuestas no aplicadas**: espada 0,9–1 m, bastón 1,6–1,8 m, daga 0,3–0,4 m; barril 0,6 m diámetro/0,9 m alto. Pivotes de edificio/decoración al suelo, accesorios en punto de agarre y escala de importación comprobada en metros.

Antes de cambiar una dimensión: comparar con patrón de 1 m, comprobar transform/collider/renderer por separado, registrar huella y altura, probar paso/giro y ataque, actualizar NavMesh solo si cambia geometría funcional y repetir regresión. C1 modifica únicamente UI y tema; no cambia medidas, físicas o bake del mundo.

## Interfaz y decisión tecnológica

Mantener **uGUI**: ya existen CanvasScaler, prefabs de panel/botón, Input System, guardas de entrada y pruebas editor/player. Migrar a UI Toolkit ahora añadiría una segunda implementación del mismo flujo sin una ventaja demostrada. Revisar la decisión solo ante una necesidad concreta.

C1 reutiliza el panel contextual y sus acciones: exploración abre Resumen, Personaje, Inventario o Partida; nombre/clase solo en Personaje; equipo/consumo/comercio solo en Inventario; guardar/cargar en Partida. Construcción muestra coste/huella/producción; RPG iniciativa, objetivo, movimiento y acción; RTS unidades/órdenes existentes. La ayuda lateral cambia con el modo. Las cuatro entradas superiores son navegación entre actividades, no acciones de modalidades inactivas.

Centralizar fuente, tamaños, colores y separación en MedievalTheme. Propuesta inicial C1 a referencia 1280×720: cuerpo/botón 16 px, encabezados 20 px, separación 8 px y margen 16 px; botones de acción 116×34. Validar el tamaño proyectado también en 1920×1080. No comunicar selección únicamente por color; añadir negrita y texto de sección. Evitar abreviar reglas hasta ocultar costes/resultado.

La fuente actual LegacyRuntime.ttf es provisional y procede de Unity. Una futura tipografía con identidad medieval deberá tener licencia comprobada y conservar la lectura; reservar ornamentación para títulos, con cuerpo sobrio. C1 no declara finalizada la tipografía por haber centralizado sus tamaños.

C2 pendiente: retrato del único héroe (sin simular compañeros), mapa de la región técnica y diario de los hechos existentes; habilidades/estados y categorías/formaciones solo cuando tengan soporte de datos real. No añadir botones vacíos ni presentar nuevas mecánicas como implementadas.

## Validación y recursos

Criterios de C1: acciones incompatibles ocultas, todas las acciones anteriores accesibles en su contexto, sin doble suscripción, ratón/escritura no ordenan al mundo, sin recortes/solapamiento a 720p/1080p y regresión de navegación/guardado/RPG/RTS. Capturas reales antes/después y build con recorrido automatizado. F2-07 continúa pendiente: estas pruebas no evalúan diversión humana.

Para D/E registrar procedencia/licencia, escala, materiales, memoria/draw calls y sustitución reversible de cada recurso. Esta iteración no descarga assets ni instala dependencias. Los presupuestos geométricos/texturas y el objetivo de rendimiento se fijarán tras medir la muestra real; no adoptar cifras comerciales por intuición.
