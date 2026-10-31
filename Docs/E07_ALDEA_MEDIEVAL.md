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

En desarrollo. No se marca validación por la sola existencia de archivos.


### E07-P0 verificado

PR #1 abierto/borrador/no fusionado, dev 742c04f y base feature dab39c2. Compilación nativa y prueba dirigida 1/1 aprobadas. Doce capturas 1080p de cámara real con encuadres explícitos conservadas en Docs/Images/E07/Before; snapshot de física original versionado, hash de NavMesh registrado en Logs. NPC (−7,5;−3), carga (−10;3), casa (6;6), geometría funcional y referencias identificados. Primer arnés de captura intentaba salir de un combate activo; se corrigió para completar el encuentro real sin alterar reglas o pruebas existentes. Las capturas de antes muestran una plataforma de hierba, casa D01 y objetos técnicos dispersos.


### E07-P1 verificado técnicamente

Doce prefabs nativos: siete módulos (entramado/piedra/puerta/ventana/cubierta/cerca/muro) y cinco conjuntos (casa 6×8 m abierta sobre física original, posada 5,5×6 m, casa de piedra 4,5×4,5 m, almacén 4,2×5 m y cobertizo 3×3 m). Cubiertas de dos aguas, vuelos/aleros/cumbreras, cerchas, chimeneas, ventanas con jambas/contraventanas y puertas proporcionadas. Mallas se consolidan por material/grupo conservando la separación contextual de cubiertas/muros altos. Son modelos modulares generados por autoría, no edificios comprados o arte final.

Cinco PNG originales 1254² intactos se importan hasta 1024 con mipmaps, repetición y anisotropía 4. Trece materiales E07 URP Lit nuevos comparten esos albedos y microrrelieve procedural D01 de baja intensidad; ese normal aproximado no es un PBR escaneado. Hierro/cuero/lino se reutilizan sin copiar materiales. Variantes clara/oscura y piedra cálida/gris comparten mapas con tintes intencionales. Procedencia/prompts exactos en E07_TEXTURAS_ORIGINALES.json. Generación/compilación salida 0 y prueba dirigida 1/1 aprobada; apariencia integrada se verificará tras P3.
