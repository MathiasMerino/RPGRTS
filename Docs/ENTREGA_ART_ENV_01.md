# ART-ENV-01 · Arte ambiental de la frontera

La candidata amplía la escena jugable de E23 en `Assets/01_Scenes/AetherniaArtEnv01.unity`. Conserva el menú, la partida, los personajes provisionales y los controladores de juego. La dirección toma de la Biblia Artística 1.0 el entramado humano, la madera orgánica élfica, la masa de piedra enana y los refugios de piel orcos. Son pilotos visuales.

| Zona | Cambio |
|---|---|
| Aldea | Tejas escalonadas de arcilla y pizarra, bordes de cubierta, diagonales en hastiales, ventanas y rótulo de posada. |
| Mercado | Lona curva a franjas, postes, cajas de tablillas y mercancía; luz cálida localizada. |
| Caminos y bosque | Adoquines biselados en tramos usados y bordes de helechos y carex modelados en malla. |
| Ruinas | Portada de dovelas, pilares irregulares y luz de acceso, con recorte contextual. |
| Campamento y puesto | Tramos de empalizada, puestos de guardia y suministros fuera de las zonas de órdenes. |
| Asentamiento | Variantes visuales para surcos de granja y banco de taller, con reglas y física heredadas. |
| Biblioteca cultural | Arco élfico, pórtico enano y refugio orco en `AetherniaArtEnv01Study.unity`. La arquitectura humana aparece en la campaña. |

Los recursos se guardan como prefabs y mallas de Unity en `Assets/06_Environment/ArtEnv01`, en metros. El generador C# consolida las caras por material. Cuatro mapas originales de arcilla y lona se generan con Python. Se reutilizan URP Lit, piedra, madera y el recorte de E23. Blender se evaluó como herramienta de modelado y no estaba instalado. La escena de estudio queda disponible en el Editor.

La primera comprobación dio 4/4 pruebas. Tras inspeccionar las capturas se aclaró la cubierta, se corrigió la orientación de las hojas y se ajustó la portada. La verificación final está en ejecución. La evaluación artística humana queda pendiente.

[Guía](GUIA_ART_ENV_01.md)
