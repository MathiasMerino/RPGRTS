# MONJE — diseño base

**Estado: PROPUESTO · pendiente de aprobación artística del usuario. Versión 0.1.**

Fecha: 2026-10-30. Hito artístico E02-A solicitado por el usuario; seguimiento `F3-E02-ARTE-01`. El ID anterior `F3-E02-A` sigue designando clips/retargeting: no se reutiliza ni se marca completado. Fuente de estilo: GDD v0.2, §12.1 y [DIRECCION_ARTISTICA](../DIRECCION_ARTISTICA.md). GDD original preservado, SHA-256 `dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661`.

Esta ficha prepara revisión y producción futura. No crea FBX, rig, texturas PBR, animaciones o un personaje nuevo en Unity. La apariencia E01 y la infraestructura E02 permanecen intactas.

Actualización E02-B: el usuario autoriza usar esta propuesta **provisionalmente** en Unity. Se aplica un proxy estático de 1,78 m y la ilustración en la UI, conservando la variante E01 y la infraestructura E02. [Implementación, pruebas y límites](../../Docs/Historial/Etapas/E02B_MONJE_PROVISIONAL.md). Esta actualización no sustituye la aprobación artística final pendiente ni modifica la condición de propuesta de la ficha.

## 1. Concepto general y lectura de referencias

Humano adulto masculino de clase Monje: presencia disciplinada, atlética y ágil; ropa monástica de tejido mate, naranja quemado sobre oscuros terrosos. Combate sin armas como lectura principal y bastón largo opcional, sin armadura pesada. No tiene nombre, orden religiosa, origen regional, símbolos o historia definitivos.

La [captura del usuario](Referencias/MONJE_REFERENCIAS_USUARIO.png) contiene dos figuras. Sus textos y elementos de navegador son contexto visual, no instrucciones adicionales. Autoría/licencia original de las esculturas no identificadas: se conservan como referencias privadas de estudio fuera de Assets; no se usan como texturas, modelos ni recursos distribuibles del juego. No se copian cara, emblema, collar, bases o pose exacta de ninguna figura.

| Fuente | Visible en la captura | Uso en la propuesta / límite |
| --- | --- | --- |
| Izquierda, vestimenta y color | Cabeza rapada, torso ampliamente descubierto, tejido naranja/rojizo en cintura/cadera, pantalón oscuro holgado, envolturas de tela y calzado cerrado; musculatura realista y fuerte contraste de valores | Retener naranja envejecido, tela oscura y ligereza. No copiar colgante, brillo del antebrazo o efectos azules |
| Derecha, anatomía y combate | Cabeza rapada, brazos definidos, guardia abierta, postura flexionada, prenda verde, pantalón naranja, faja y vendas azules, collar voluminoso y sandalias | Retener energía marcial y articulaciones despejadas; moderar la estilización de manos/musculatura. No trasladar su paleta verde/azul ni el collar |
| Brief del usuario | Varón adulto, talla provisional 1,78 m, túnica cruzada sobre un hombro, faja, pantalón amplio, vendas y fantasía medieval realista | Son directrices del encargo. La túnica de un hombro no se observa completa en la figura izquierda; no se atribuye esa construcción a una zona que la captura no muestra |
| Interpretación original | Dorso, cierres, patrón del tejido, entrepierna, suela, rostro propio, tono de piel y ubicación del paño | Se proponen a continuación; necesitan aprobación artística. El dorso no se deduce de una fotografía posterior inexistente |

## 2. Silueta y anatomía

Talla propuesta 1,78 m, unas 7,4 cabezas; cráneo rapado, cuello proporcionado, cintura contenida y hombros funcionales. Edad visual propuesta 30–40 años, expresión serena y concentrada; facciones originales y tono de piel marrón cálido medio, ambos sujetos a revisión. No definir una raza del catálogo a partir de esta elección de piel.

Dorsales, deltoides y antebrazos se leen por planos anatómicos, no por volumen de culturista o músculos marcados como placas. Manos y pies de tamaño humano. El diseño debe reconocerse por hombro descubierto, diagonal naranja, pantalón holgado y vendas claras; no necesita efectos mágicos ni armas para identificar la clase.

## 3. Vestimenta

Propuesta de construcción: **hombro izquierdo anatómico cubierto**, derecho libre. El paño nace en ese hombro, cruza la mitad izquierda del pecho y se fija hacia la cintura derecha; el pectoral/hombro derecho queda parcialmente expuesto. La elección de lado es una interpretación para mantener continuidad entre vistas, no una mecánica de lateralidad.

El mismo paño recorre la escápula izquierda y desciende diagonalmente hacia la cintura derecha por detrás. Se sujeta con costura/amarre plano y faja; no funciona como capa larga. Bajo la faja, dos paneles naranjas cortos y separados terminan por encima de la rodilla. Aberturas laterales y delanteras permiten abrir caderas y patear: evitar una falda tubular cerrada.

Pantalón marrón carbón amplio, con pieza de entrepierna que permita zancadas, rodillas flexionadas y patadas altas; volumen controlado y tobillos recogidos. Faja textil oscura de dos vueltas, amarre plano y cabos cortos. Vendas de lino envejecido en antebrazos/muñecas y pantorrillas: dedos, codos y rodillas libres. Calzado propuesto cerrado, ligero, de cuero oscuro y suela flexible. Prenda, dorso, cierres y calzado combinan referencias e interpretación; no son reconstrucción histórica documentada.

## 4. Paleta cromática propuesta

| Uso | HEX sRGB de referencia | Lectura |
| --- | --- | --- |
| Paño y paneles principales | `#A6572D` | Naranja quemado mate; acento de identidad |
| Variación oscura del paño | `#6E3F28` | Desgaste/variación de fibra, sin pintar sombras de iluminación |
| Ribete discreto | `#A97C42` | Ocre moderado, sin bordados luminosos |
| Pantalón | `#4A4337` | Marrón carbón; separar piernas del naranja |
| Faja y cuero | `#45352B` | Oscuro terroso, manteniendo pliegues legibles |
| Vendas | `#B8AA8B` | Lino envejecido, principal contraste de manos/tobillos |
| Piel, base orientativa | `#8B6047` | Propuesta de tono, no un color uniforme de toda la piel |
| Madera opcional | `#66503A` | Madera usada sin brillo mágico |

Los HEX son una especificación revisable de albedo, no colores que deban medirse literalmente en una imagen iluminada. La lámina generada interpreta estos valores; sus muestras no sustituyen la tabla ni constituyen texturas extraídas de referencias.

## 5. Materiales

Tejido naranja y pantalón: trama visible solo de cerca, rugosidad alta y pliegues que respondan a gravedad/tensión. Lino de vendas con capas reconocibles y bordes acotados. Cuero flexible con desgaste localizado en empeine/suela; piel con variación suave y reflejo amplio, sin aspecto aceitado uniforme. Cierre de madera sencillo; nada de grandes placas o brazales metálicos.

Preparar materiales URP Lit compartidos por familia, sin material exclusivo para cada tira o venda. Metallicity de tejidos/piel/cuero/madera igual a cero; smoothness provisional bajo o medio según superficie, a ajustar en una muestra iluminada. Se mantienen los mapas y nombres de entrega del [manifiesto E02](../../Docs/E02_IMPORTACION_HUMANOID.md#archivos-que-faltan-para-completar-el-monje). La lámina conceptual no es albedo, normal map ni máscara PBR del modelo.

## 6. Accesorios

Un cierre de madera pequeño y sin emblema en la faja aporta detalle original. Se omiten collares de cuentas grandes, cadenas, símbolos reconocibles y adornos colgantes que interfieran con puños o pecho. La lámina muestra un cierre/toggle sencillo; forma y ubicación final pendientes de aprobación.

Bastón opcional, separado: madera recta usada, unos 1,65 m y 0,035 m de diámetro, zona de agarre sencilla de unos 0,18 m; sin puntas, emisión ni herrajes dominantes. No está representado como equipo obligatorio en la lámina v01. Es diseño visual opcional, no un ítem nuevo del inventario ni una habilidad implementada.

## 7. Proporciones y medidas propuestas

| Elemento | Objetivo de autoría, en metros | Naturaleza del dato |
| --- | --- | --- |
| Suelo a coronilla | **1,78** | Talla provisional indicada por el usuario |
| Cabeza | Aproximadamente 0,24 | Guía anatómica, ajustar al rostro |
| Ancho entre hombros anatómicos | Aproximadamente 0,44–0,47 | Interpretación; no ancho de collider |
| Mano, muñeca a dedo medio | Aproximadamente 0,18–0,19 | Guía para dedos/agarre, comprobar en malla |
| Pie | Aproximadamente 0,25–0,27 | Guía para apoyo y calzado |
| Ancho de faja | Aproximadamente 0,10–0,12 | Permitir flexión de abdomen/cadera |
| Cabos de faja | Aproximadamente 0,12–0,18 | Sin colas largas entre muslos |
| Cobertura de venda de antebrazo | Aproximadamente 0,22–0,27 | Sin bloquear codo ni dedos |
| Cobertura de venda de pierna | Aproximadamente 0,23–0,30 | Por debajo de rodilla, por encima de tobillo |

Son referencias de modelado, no medidas comprobadas de una geometría existente. La pose A ocupa más ancho que una cápsula en marcha y no define el área de impacto. No se asigna un alcance RPG a la longitud del brazo o del bastón.

La base funcional vigente conserva cápsula/agente de **0,60 × 1,80 m**, y la geometría E01 mantiene sus dimensiones documentadas. La propuesta de 1,78 m es 2 cm menor; no se reduce el controlador ni se escala su raíz. La talla está dentro de la tolerancia de auditoría E02, pero paso, selección, suelo y cámara deberán verificarse con la malla real tras aprobación. Medidas aplicadas: [ESCALA_REFERENCIA](../../Docs/ESCALA_REFERENCIA.md).

## 8. Vistas y poses de referencia

| Vista | Información a conservar y mostrar |
| --- | --- |
| Frontal | Hombro izquierdo anatómico cubierto (a la derecha del observador), pecho derecho libre, faja horizontal, separación entre paneles y pantalón, manos/dedos visibles |
| Lateral derecha | Pecho/hombro libre, profundidad de tórax, codo y rodilla sin abrazaderas rígidas; suela fina y caída del panel sin atravesar la pierna |
| Posterior | Diagonal desde escápula izquierda hacia cintura derecha, espalda parcialmente expuesta, faja continua y separación de paneles. Toda esta construcción es interpretación original |
| Tres cuartos | Continuidad de paño, faja y volumen de piernas entre frente y dorso; lectura de materiales bajo una luz neutra |
| Pose A neutral | Brazos separados unos 45° respecto del tronco, codos relajados, palmas abiertas/dedos separados, pies paralelos y apoyo simétrico; pose de trabajo propuesta, no un rig validado |
| Guardia marcial | Base estable, rodillas flexionadas, pelvis equilibrada y manos abiertas preparadas para cerrar puño. No copiar exactamente las poses de las esculturas; no representa una animación o una regla nueva |

Especificación de hoja: fondo gris cálido neutro, cuatro vistas de cuerpo completo alineadas, pose A y guardia adicionales, detalles de tejido/faja/vendas/calzado y paleta. Mismo rostro, proporciones, lado del paño, colores y accesorios en todas las vistas; sin peana, escenario, efectos o iconografía de las referencias. La lámina 2D es una guía de continuidad, no un plano ortográfico calibrado.

## 9. Diseño modular

Separar cuerpo/cabeza/manos, paño superior y paneles, pantalón, faja, vendas, calzado y accesorio opcional. Ropa deformable debe compartir el esqueleto del cuerpo; no basta con colgarla de un hueso como objeto rígido. Mantener volúmenes corporales de referencia bajo la ropa y una estrategia de ocultación para evitar penetraciones al cambiar prendas. Cualquier ocultación por variante debe quedar resuelta en el prefab, sin nuevas reglas de inventario.

E02 ya permite vistas de Clothing, Armour, Weapon, Staff y Accessory. Se enlazarán a `CharacterVisualModel` y a las fuentes reales `Weapon`/`Armour`. Hoy solo espada/túnica tienen soporte de ítem; no se inventan cinco ranuras persistentes ni un bastón equipable. Someter variantes y su correspondencia a ítems a una decisión posterior.

## 10. Requisitos para modelado 3D en Blender

Preparar escena métrica y comprobar dimensiones reales: configurar unidades no sustituye medir la geometría. Trabajar con escala coherente y aplicar transformaciones de malla/armature **antes** de enlazar pesos; evitar aplicar correcciones arbitrarias a una armature ya animada. La ficha propone un personaje de 1,78 m, no reescalar el controlador Unity. [Unidades de Blender](https://docs.blender.org/manual/en/latest/scene_layout/scene/properties.html).

Controlar la altura sobre el eje Z en la escena Blender y sobre Y al recibir el FBX en Unity; verificar la conversión con una referencia de un metro. Son pasos de producción futura, no una exportación ejecutada en esta tarea.

Topología con bucles de deformación en clavícula/hombro/axila, codo, muñeca, nudillos, cadera/entrepierna, rodilla y tobillo. Evitar polos y caras largas en pliegues articulares; conservar volumen al cerrar puño y al flexionar codo/rodilla. No dibujar músculos como surcos rígidos ni modelar cada fibra o pliegue pequeño. Revisar normales, tangentes, caras internas innecesarias y triangulación de la versión exportada.

UV con densidad coherente entre piel/tejido y margen entre islas; no espejar el paño asimétrico de forma que cambie de hombro o borre el desgaste direccional. Separar costuras por zonas poco visibles. Preparar mapas de superficie propios/licenciados con el manifiesto E02; no proyectar las esculturas de la captura como textura. Reutilizar materiales/atlas y conservar lectura en distancia isométrica. No se fija ahora un presupuesto comercial de polígonos, texturas o LOD sin medir un modelo real.

Exportar únicamente las mallas y armature necesarias a FBX; conservar el `.blend` fuente fuera del flujo de importación nativo si se produce posteriormente. Revisar ejes de exportación para entregar **Y arriba y frente +Z en Unity**. La conversión `-Z Forward / Y Up` es un punto de partida habitual, no sustituye revisar el resultado. Evitar huesos de hoja adicionales si no forman parte del rig previsto; no quitar huesos necesarios para manos o prendas. No se ejecuta Blender ni se exporta un archivo durante esta tarea. [Exportación FBX de Blender](https://docs.blender.org/manual/en/4.4/addons/import_export/scene_fbx.html).

## 11. Rig, skinning y animación

Armature humanoide común con pelvis, columna, clavículas, brazos, manos, piernas, pies y dedos mapeables en Unity Humanoid. Manos preparadas para todos los dedos, con articulaciones suficientes y pesos que permitan abrir palma, cerrar puño y sujetar bastón. Evitar controles extra exportados como huesos deformantes si no tienen una función. No se da por válido un Avatar únicamente por usar nombres conocidos.

Compartir bind pose y huesos entre cuerpo y piezas de ropa; normalizar pesos y comprobar transiciones antes de reducir influencias. Para un primer ensayo puede estudiarse un máximo de cuatro influencias por vértice, sujeto al caso real; comprobar que no destruya volumen en hombro, cadera o dedos. Las vendas han de acompañar el miembro y no formar anillos rígidos que atraviesen piel al doblar; separar dedos y evitar puños modelados permanentemente cerrados.

| Prueba futura / estado E02 | Riesgo a revisar con rig y clips reales |
| --- | --- |
| Idle | Pie estable, tela sin penetración del pecho, respiración discreta |
| Walk / Run | Entrepierna y paneles entre muslos, rodilla libre, tobillo/calzado, deslizamiento de pies |
| Attack | Cruce de brazos, rotación escapular, cadera y patada alta sin cerrar la falda; manos y faja sin penetración |
| Defend | Palmas/dedos cerca del rostro, vendas contra torso y codos en flexión |
| Hit | Torsión lateral, hombro cubierto y volumen de costillas/abdomen |
| Defeat | Cadera/rodillas y contacto de cuerpo, manos, paneles y suelo |
| Bastón opcional | Agarre, separación de dedos y muñeca; un segundo apoyo de mano requerirá revisar la solución de animación, no se presupone IK funcional |

Usar los siete estados/perfil de E02. El agente mueve la raíz; Animator pertenece al visual y conserva root motion desactivado. Preparar clips en el sitio; no añadir efectos de daño mediante eventos de animación. La tela se resolverá primero con patrón y skinning revisados; no se presupone cloth simulation, IK o retargeting validado. Compatibilidad real de todas esas poses permanece pendiente.

## 12. Adaptación a cámara isométrica e integración futura

Priorizar la diagonal naranja, silueta del cráneo/hombro, separación de rodillas y vendas claras. Evitar detalles de valor semejante que fundan manos con pantalón. Usar ocre en ribetes discretos, sin saturar torso o añadir contornos de emisión. Comprobar a zoom jugable y 720p/1080p; no aumentar manos/cabeza arbitrariamente para compensar una mala cámara. La lectura en escala de grises y sobre suelo D01 debe probarse con el modelo real.

Después de aprobación y producción del recurso, seguir el [flujo E02 existente](../../Docs/E02_IMPORTACION_HUMANOID.md#usar-el-flujo-dentro-de-unity): FBX nativo, auditoría de unidades/Avatar/URP, prefab candidato bajo VisualRoot, bindings por equipo real y catálogo con alternativa E01. La propuesta no reconstruye ese sistema ni sustituye ahora `monk_Visual`. Comprobar a futuro puertas, colisiones, selección, recorte de techo, ropa, clases, combate y guardado antes de aceptar la integración.

## Hoja conceptual y herramientas reales

**Entregable 2D generado:** [MONJE_HOJA_PROPUESTA_v01.png](Conceptos/MONJE_HOJA_PROPUESTA_v01.png), creado con la herramienta integrada `image_gen.imagegen` y la skill imagegen, usando la captura como referencia artística. [Prompt exacto ejecutado](Conceptos/MONJE_HOJA_PROPUESTA_v01.prompt.txt). Modo integrado; no CLI/API externa, instalación o modelado. Se conservó el original generado y se copió la selección al proyecto, fuera de Assets.

La revisión visual comprueba presencia de las seis vistas/poses, paño sobre el mismo hombro anatómico, paleta general, pies completos, detalles de materiales y ausencia de bases/colgantes copiados. Hay variaciones de iluminación y perspectiva entre vistas; contornos, caras y manos deben reconciliarse durante una futura autoría 3D. Las manos ilustradas no certifican dedos/topología/pesos, y la pose A dibujada no certifica ángulos exactos. Los HEX y medidas de esta ficha prevalecen como especificación revisable, no como datos medidos de la ilustración.

Disponible: generación/edición raster 2D y herramientas nativas de Unity ya verificadas en E02. No hay herramienta Blender/modelado 3D conectada expuesta en esta sesión; no se ha usado Blender para crear una malla ni un rig. El PNG no contiene geometría, UV, huesos, texturas de producción o animaciones.

## Decisiones pendientes de aprobación

1. Aceptar o corregir silueta/rostro/tono de piel propuestos y talla de referencia 1,78 m.
2. Confirmar hombro izquierdo cubierto, construcción dorsal y longitud de paneles abiertos.
3. Confirmar naranja/ocre, pantalón oscuro, faja, vendas claras y calzado cerrado.
4. Mantener o retirar el cierre de madera; decidir si la variante de bastón se desarrolla después.
5. Aprobar la lámina v01 como guía conceptual, con las discrepancias de proyección pendientes de resolver en modelado.

Todo continúa **PROPUESTO** hasta una respuesta explícita del usuario. La preparación/revisión documental puede cerrarse en checklist; la aprobación artística, modelado/rig/animaciones reales y evaluación humana F2-07 siguen pendientes. No se inicia una fase posterior automáticamente.


## Seguimiento técnico E08 (2026-11-01)

La hoja y esta propuesta conservan su carácter provisional. E08 prepara una representación 3D articulada original, con rig Generic, skin y clips nativos; no equivale a aprobación artística ni a un FBX final. Dimensiones, ficha de Monje/Guerrero/Mago, deformaciones, guía de sustitución y evidencia se mantienen en [E08_PERSONAJES_ANIMADOS](../../Docs/Historial/Etapas/E08_PERSONAJES_ANIMADOS.md), evitando duplicar especificaciones. La variante estática anterior permanece recuperable.
