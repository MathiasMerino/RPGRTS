# E17 — reemplazo de personajes

El catálogo `Assets/13_AlphaCandidate/Characters/ClassVisuals.asset` reutiliza los prefabs/rigs E08 y retratos existentes. No duplica mallas, materiales, controladores de movimiento o reglas. Cada Entry permite `ReplacementPrefab`, `UseReplacement` y `ReplacementRequiresAnimation`. Mantener desactivado hasta revisar procedencia, licencia, escala y contrato.

Cadena: reemplazo revisado → HumanoidPrefab E08 utilizable → Prefab provisional → PreviousPrefab → representación técnica. La fábrica valida malla/material, raíz a escala1, ausencia de agentes/colliders/controladores de gameplay, rig/Avatar y clips enlazados cuando se exige animación. El presentador conserva el modelo anterior hasta crear correctamente el siguiente; un cambio visual no modifica estado/equipo/guardado. Héroes y unidades de apoyo usan la misma fábrica.

## Organización sin mover originales

- `07_Characters/ProductionProvisional/Source`: hojas/retratos originales; sus materiales/prefabs siguen en la biblioteca existente.
- `07_Characters/AnimatedE08`: rigs, clips, perfiles y prefabs provisionales reutilizados.
- `07_Characters/Humanoid`: infraestructura de candidatos/Avatar Humanoid existente.
- `13_AlphaCandidate/Characters`: registro para recursos revisados; registrar referencias, sin copiar equivalentes.
- `07_Characters/MonkMeshy` y `Assets/LocalOnly`: experimentales privados excluidos de Git. Modelos fuente/FBX/texturas/materiales externos se mantienen juntos por recurso; publicar únicamente después de autorización. No mover o renombrar `.meta` a mano.

## Preparar un modelo final

Raíz de prefab a escala1, 1unidad=1m, piesY0, frente+Z; piloto humano≈1.78–1.80m. Normalizar solo el hijo de representación o la exportación desde Blender; conservar controlador/cápsula/agente del héroe. Texturas y materiales URP compartidos, UV0 revisados, normales/tangentes correctas. Prefab con CharacterVisualModel, anclajes de arma/equipo y bindings que reflejen inventario real. Sin lógica de daño, navegación o guardado dentro del visual.

Para Humanoid: T/Apose, esqueleto y Avatar válidos, retarget de clips comprobado. Para Generic: perfiles/clips compatibles con la jerarquía concreta; no asumir que clips Generic E08 animan otro FBX. Vincular HumanoidVisualRig y HumanoidAnimationDriver al perfil/controlador real. Probar reposo, movimiento, ataque, guardia, impacto, derrota, equipo y tres resoluciones antes de activar el reemplazo. El sistema es extensible por ClassId, sin implementar ahora nuevas razas/clases.

## Monje Meshy observado

Original conservado; se analizó una copia local con Read/Write activado. 1malla, **99.466triángulos/222.717vértices**, 1submesh/1material, **UV0ausente**, 0texturas embebidas, 0SkinnedMeshRenderer/0huesos/0Animator y sin Avatar funcional. Importador Generic no implica rig. Bounds importados≈1.261×1.899×0.474m con raízEulerX270°. Estudio privado normaliza un hijo a1.78m con factor≈0.93753, piesY0; frente/estética pendientes de revisión humana.

Es estático y experimental: no usa automáticamente animaciones actuales. Un material único no permite colorear prendas distintas añadiendo slots vacíos. Requiere Blender: revisar/soldar duplicados con criterio, UVunwrap, pintura/atlas y separación por caras si se desea; después retopología/rig/pesos y exportación compatibles. No se generó LOD ni retopología destructiva. Escena y captura de inspección quedan locales, sin referencias en el catálogo de runtime ni publicación de derivados. Licencia/autorización de publicación pendientes.
