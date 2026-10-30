# Referencias técnicas de escala

Fecha: 2026-10-28. Tarea BAS-02. Valores de ensayo para el prototipo, sujetos a las pruebas registradas en el checklist. No representan dimensiones definitivas del mundo del GDD.

La unidad de trabajo es el metro: 1 unidad Unity = 1 m. Usar Y hacia arriba, X para ancho y Z para fondo. Los prefabs de referencia tienen raíz con escala `(1,1,1)` y pivote en el suelo. Los tamaños se aplican a sus piezas; comparar el tamaño real del collider y del renderer, no solo el valor de Transform de la raíz.

| Referencia | Dimensión | Uso |
| --- | --- | --- |
| ReferenceCube_1m | 1 × 1 × 1 m | Comprobar escala de importaciones |
| ReferenceHuman_180cm | 1,80 m alto; 0,60 m de diámetro | Cápsula provisional; pivote a nivel de pies |
| ReferenceDoor_120x220cm | Hueco libre de 1,20 × 2,20 m | Marco con postes de 0,20 m y profundidad 0,25 m; ancho exterior 1,60 m |
| ReferenceWindow_100x120cm | Hueco libre de 1 × 1,20 m, de Y=1 a Y=2,20 | Antepecho a 1 m; marco de 0,20 m |
| ReferenceHouse_6x8m | Huella 6 × 8 m; muros hasta Y=3 m | Cubierta de 0,15 m; altura exterior total 3,15 m; puerta con el hueco anterior |
| Calle de ensayo | 4 m de ancho y 12 m de largo | Comparación con dos agentes; la marca superficial no añade collider |
| Suelo de ensayo | 40 × 40 m; superficie Y=0 | Terreno plano técnico, sin representar una región completa |
| Obstáculo | 3 × 2 × 2 m | Colisión y futuras pruebas de navegación |
| Paso incompatible | Hueco libre de 0,40 m | Debe bloquear una cápsula de diámetro 0,60 m |

## Colisiones y navegación

La cápsula de referencia mide 1,80 m de alto y tiene radio 0,30 m. Su centro está a Y=0,90 m; los centros de los extremos esféricos están a Y=0,30 y 1,50 m. Para comprobar huecos sin tocar el suelo, desplazar la cápsula de ensayo 0,02 m hacia arriba. La puerta de 1,20 m ofrece 0,30 m de margen por lado con la cápsula centrada; el paso de 0,40 m debe producir contacto.

El humano de referencia es una medida estática, sin controlador ni atributos RPG. El agente NavMesh por defecto de SampleScene (radio 0,50 m, altura 2 m) no se adopta automáticamente. Al implementar movimiento, configurar collider y agente de forma conjunta y volver a hornear navegación; comprobar puertas, giros, obstáculos y separación entre unidades.

Las dimensiones militares, equipamiento y edificios definitivos se validarán con sus modelos y reglas reales. La casa es geometría de prueba, sin construcción, economía ni interiores artísticos.

## Comprobación reproducible

1. Ejecutar `Tools/Invoke-UnityValidation.ps1 -Mode GenerateSandbox` una sola vez para crear los prefabs y la escena mediante Unity. La herramienta rechaza rutas de salida existentes y no sobrescribe el trabajo previo.
2. Ejecutar `Tools/Invoke-UnityValidation.ps1 -Mode Foundation` con el editor cerrado. Comprueba dimensiones de los assets importados, colisiones, referencias, materiales y Play Mode; genera una captura con el dispositivo gráfico en modo batch.
3. Revisar el XML y la imagen en `ProyectoRPGRTS/Logs/`. El informe de validación resume el resultado real; un prefab generado no implica aceptación sin comprobarlo.

Para inspección manual opcional, abrir `Assets/01_Scenes/TechnicalSandbox.unity`. Localizar los grupos por nombre en Hierarchy y comprobar colliders y medidas en Inspector. La cámara de inspección es fija; todavía no incorpora controles isométricos del juego.

## Medidas aplicadas adicionales (fase 3, consulta central)

| Elemento vigente | Medida comprobada en la autoría existente | Alcance |
| --- | --- | --- |
| Héroe/agente | Cápsula 0,60 × 1,80 m; agente radio 0,30 m/altura 1,80 m | Mantener base funcional al sustituir el hijo visual |
| Taller | Huella 3 × 3 m; cuerpo 2 m alto y cubierta hasta 2,20 m | Obstacle box 3 × 2,20 × 3 m con carving |
| Cofre técnico | 0,80 × 0,80 × 0,60 m; centro Y=0,40 m | Radio de interacción 2,20 m; presentación provisional |
| Hitos/portales técnicos | Cubos de 0,80 m | No representan puertas físicas ni NPC finales |
| Mazmorra técnica | Suelo 10 × 10 m; muros traseros 3 m y parapetos delanteros 0,80 m | Conservar huella/paso/visibilidad antes de modelar |

Las dimensiones de accesorios/decorados todavía no aplicadas se proponen en Documentacion/DIRECCION_ARTISTICA.md. No duplicar esta tabla en un segundo catálogo de medidas ni escalar raíces existentes para cambiar apariencia. La iteración C1 no modifica estas medidas.

## Muestra medieval D01 — medidas aplicadas

Fecha: 2026-10-30. Biblioteca visual sin colliders, con raíces de escala uno; la escena mantiene los colliders/NavMesh anteriores. Bounds medidos después de instanciar los prefabs en Unity. Las pequeñas salientes de herrajes/aleros no redefinen la huella física.

| Prefab / elemento | Dimensiones visuales X × Y × Z, metros | Compatibilidad física |
| --- | --- | --- |
| TimberHouse_6x8m | 6,760 × 5,595 × 8,780, incluidos aleros/chimenea | Huella original 6 × 8; muros 3 m; techo técnico físico anterior a 3,15 m conservado |
| GableRoof_660x860cm | 6,760 × 2,220 × 8,780, incluidos remates | Paneles base 6,60 × 8,60, subida 2 m; colocado a Y=3,10; no es plataforma transitable |
| WallBay_2m | 2 × 3 × 0,260 | Módulo visual de muro con zócalo, postes y diagonal |
| OpenDoor_120x220cm | 1,610 × 2,400 × 0,268 con herrajes | Hueco libre 1,20 × 2,20; conserva entrada para agente 0,60 × 1,80 |
| Window_100x120cm | 1,400 × 1,600 × 0,200 | Hueco visual 1 × 1,20; marco desde Y=0,80 en fachada; soporte en muestra independiente |
| Barrel_100cm | 0,970 × 1,020 × 0,970 | Proxy cúbico existente 1 × 1 × 1; margen conservador junto al contorno curvo |
| Crate_80cm | 0,800 × 0,800 × 0,800 | Presentación del mercado, con su proxy anterior |
| CrateStack_3x2x2m | 3 × 2 × 2 | Coincide con el obstáculo existente; impide tránsito |
| Chest_80x80x60cm | 0,800 × 0,813 × 0,645 con herrajes | Volumen base 0,80 × 0,80 × 0,60; interacción y collider originales |
| GrassClump / GroundFern | 0,400 × 0,230 × 0,392 / 0,853 × 0,260 × 0,844 | Plantas blandas sin colisión; instancias con escala uniforme 0,70–1,40 |
| Camino de tierra D01 | Ancho aproximado 2,65–2,95; superficie Y=0,018 | Malla continua sin collider sobre suelo técnico; la marca de calle 4 × 12 anterior queda oculta |

El humano y los talleres mantienen las medidas de las tablas anteriores. Recorrido de puerta y bloqueo del almacén se comprueban en Play Mode; las firmas de todos los colliders estáticos y el SHA-256 del NavMesh se comparan con el estado anterior. Un prefab visual trasladado a otro mapa requiere física y validación propias.

## E01 — base visual de personajes

Las tres figuras tienen pies en Y=0, altura anatómica 1,80 m y cabeza de 0,24 m; raíz de prefab y VisualRoot con escala uno. Y arriba y frente +Z. El cuerpo geométrico usa una rotación interna de autoría para entregar ese frente; no se debe trasladar esa corrección a la raíz del controlador al importar un modelo final.

El controlador conserva cápsula/agente de 0,60 × 1,80 m (radio 0,30 m). Hombreras del Guerrero: ancho aproximado 0,61 m. La espada compartida mide 0,96 m de largo; bastón de diseño 1,65 m y remate de 0,08 m. Escudo de muestra 0,48 × 0,60 m; faldón del Mago hasta 0,56 m de diámetro y bajo a Y=0,18. Los accesorios no tienen colliders ni amplían alcance/huella funcional. Medidas de bounds totales y geometría comprobadas en Logs/E01-geometry.txt; no equivalen a dimensiones físicas de ataque.

Bounds de presentación con todo el equipo incluido: Guerrero 0,670 × 1,808 × 0,395 m; Mago 0,685 × 1,808 × 0,560; Monje 0,665 × 1,808 × 0,285. La coronilla del pelo añade 0,008 m a la talla anatómica. Las raíces funcionales y el hueco de puerta no cambian.
