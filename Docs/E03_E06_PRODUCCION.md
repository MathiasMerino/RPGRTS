# E03–E06 — Personajes y misión integrada

Fecha: 2026-10-31. En desarrollo en `feature/e03-e06-personajes-vertical-slice`, desde dev `742c04f`. Los tres commits de propuestas artísticas estaban locales; se sincronizaron con origin/dev antes de crear la rama. No hay ramas de reparación F2-07 locales/remotas observadas. Las hojas Monje v02, Guerrero v01 y Mago v01 existen en Documentacion/Personajes/Conceptos; son originales generados, autorizados por el nuevo encargo para integración provisional, sin aprobación de arte definitivo.

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

Pendientes de ejecución. Evidencias e incidencias se añadirán por etapa, sin marcar implementación por solo existir código.


### E03 — Completada técnicamente; revisión humana pendiente

Generación/compilación salida 0 y 2/2 pruebas dirigidas aprobadas: métrica/URP/recuperación y carga/render Play Mode de las tres clases. Capturas nativas E03-monk/warrior/mage en Docs/Images, inspeccionadas. Monje 0,711×1,780×0,368 m / 5.252 tris / 9 renderers; Guerrero 0,715×1,800×0,365 / 6.704 / 9; Mago 0,733×1,800×0,550 / 6.096 / 11, incluyendo equipo inactivo. Tres prefabs E03, tres materiales propios compartidos con materiales previos y fuentes PNG íntegras. Sin colisiones/Animator en modelos, física anterior preservada. Anatomía/manos y paños siguen geométricos; el Mago es más oscuro que la lámina y se ajustará su paleta compartida en E04. Originales E01/E02-B y plantillas Humanoid conservados. Evidencia E03-results.xml/validation.log/geometry.txt.


### E04 — Completada técnicamente; revisión humana pendiente

Configuración/compilación salida 0; 4/4 dirigidas aprobadas. Catálogo existente ampliado con título visual, fuentes/portrait sprites, prefab anterior, materiales compartidos, talla/política de equipo y estado provisional, sin copiar estadísticas. Fuentes importadas Sprite/sRGB/NPOT None, recorte nativo 180×240 (3:4), mismo PNG sin editar; ficha/header usan UV correspondientes. Anclajes RightHand_Weapon y bindings existentes conectados a espada/tela v1. Ocho equipamientos repetidos por clase no duplican instancia/renderers; retirada visual por estado vacío y guardar/cargar restauran clase/equipo. Seis capturas UI 720p/1080p validadas por límites/textos/raycast; tres seleccionadas en Docs/Images. Paleta Mago aclarada moderadamente en materiales nuevos. Reglas y formatos inalterados; bastón no es item y no hay interfaz para desequipar actualmente.


### E05 — Completada técnicamente; revisión humana pendiente

Escena separada AetherniaVillageDemo, conservando PlayablePrototype byte a byte. Habitante interactivo, arcón D01 con carving y diálogo/objetivo independientes del gestor C02. Misión con aceptación, encuentro RPG real, retorno automático tras victoria, entrega única y guardado v1 en cada etapa. Recompensa provisional 20 oro y ración si hay espacio; inventario lleno recibe oro sin bloquear misión. No hay cargo nuevo como item, se registra por Facts. Cinco pruebas dirigidas aprobadas en primera ejecución y tras pulir etiquetas: datos/compatibilidad/pago único/capacidad, recorridos completos Monje/Guerrero/Mago con rutas reales y guardado/carga, y rechazo de victorias ajenas o activación sin aceptar. Semilla 42 solo en pruebas; juego normal mantiene semilla 0/aleatoria. No estadísticas alteradas. Cuatro capturas seleccionadas E05-* en Docs/Images; protocolo en Docs/E05_CARGAMENTO_PERDIDO.md. Primera autoría falló por ruta de cofre, corregida reutilizando el NPC parcial; etiquetas inicialmente por encima del diálogo corregidas a sorting -10.
