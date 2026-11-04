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

La primera pasada y la final dieron 4/4 pruebas dirigidas. La escena conserva las referencias de NavMesh, la firma de colisiones, los puntos de interacción y las reglas de E23. Tras inspeccionar las capturas se aclaró la cubierta, se corrigió la orientación de las hojas y se ajustó la portada. Hay siete pares de capturas de cámara equivalente en escenas jugables y una captura del estudio cultural. [Dirigidas](Validacion/ARTENV01/Dirigidas.xml)

## Build y mediciones

- Windows x64 generado con Unity 6000.5.8f1 desde la copia independiente. ZIP con CRC y SHA por archivo comprobados. Se conservaron 27 ZIP anteriores. [Manifiesto](Validacion/ARTENV01/PORTABLE.json)
- Biblioteca de 21 prefabs y 18 mallas originales, con 39.566 triángulos únicos entre estas mallas. La suma excluye las instancias en escena y los recursos heredados. [Mallas](Validacion/ARTENV01/MALLAS.csv)
- 18 cargas fuera de pantalla, 4.332 envíos URP, RTX 3050 Laptop GPU. En Alto a 1080p el P95 fue 12,54 ms en aldea pausada, 19,81 ms en menú y 25,56 ms en preparación RTS. El contador medio SetPass subió de 21 a 30 en aldea y preparación frente al informe histórico de E23. Las dos sesiones usaron archivos de guardado distintos, por lo que el aumento de tiempo no aísla el coste del arte. Quedan pendientes el perfilado comparativo controlado y los FPS presentados. [Mediciones](Validacion/ARTENV01/PERFORMANCE.json)
- Regresión general y recorrido Windows en curso. La evaluación artística humana queda pendiente.

[Guía](GUIA_ART_ENV_01.md) · [Capturas comparativas](Images/ARTENV01) · [PR #18](https://github.com/MathiasMerino/RPGRTS/pull/18)
