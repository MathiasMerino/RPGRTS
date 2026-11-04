# E21 — Entrega de Las rutas de la frontera

La candidata conecta el cargamento, una decisión de abastecimiento, la patrulla, dos guardianías, el hallazgo del Umbral y la resolución del puesto. El cierre con Tarek permite seguir explorando por cuenta propia. El capítulo y sus nombres siguen siendo provisionales.

- **Decisiones.** Elia ofrece informar a la Liga por alimento y reputación, o financiar la reparación con oro a cambio de madera. Ambas usan la misma partida y quedan registradas una sola vez.
- **Mundo.** Nilo y Elia reconocen la decisión. Tarek recibe el hallazgo de la mazmorra. La conquista conserva las reglas militares y activa propiedad y producción mediante el sistema territorial existente. El cierre independiente conserva al propietario real y permite volver después al puesto.
- **Presentación.** Se reutilizan cámara, arte, navegación y geometría de E20. El HUD señala el objetivo actual. El Registro muestra el acto, el cargamento, la elección y el estado del puesto. Los retratos son provisionales.
- **Persistencia.** Guardado v1 con hechos aditivos y validación de exclusividad. El capítulo se deriva del estado del mundo. Una partida anterior puede continuar desde su hito y conserva sus recompensas.

## Pruebas y build

- Build Windows x64 de `AetherniaE21Campaign.unity`, fuente `df03c7a`. Guerrero completó los cinco actos y Continuar desde la build original, 2/2 procesos. Guerrero, Monje y Mago hicieron lo mismo desde el ZIP, 6/6 procesos sin grabación.
- Se conserva una ejecución adicional fallida del Guerrero a 12 Hz. Conquistó el puesto pero no usó la técnica y la sonda se detuvo. El recorrido sin grabación sí ejecutó la técnica. El vídeo muestra la batalla real del intento grabado.
- CRC, SHA‑256 por archivo y 24 ZIP anteriores verificados. La extracción contiene el ejecutable y la guía. 32 PNG a 1280×720 y un MP4 de 157 fotogramas de la cámara de juego. [Resultados Windows](Validacion/E21/WINDOWS.json) · [Capturas](Images/E21/Portable) · [Vídeo](Videos/E21/Puesto-del-Umbral.mp4)
- Regresión general **226/226** con cero fallos, inconclusas u omitidas. Pasó el caso histórico del Guerrero. [Resultado general](Validacion/E21/GENERAL.json)
- Dirigidas **3/3**. Cubren dependencias, decisión y coste real, y cierre independiente con un guardado anterior y el puesto aún rival.
- Dirigida adicional **1/1** para iniciar la guardianía con 0, 1, 2 y 3 acompañantes. Las muestras parten del mismo campamento. Dos intentos del fixture anterior se detuvieron al cancelar un combate activo y por un timeout de navegación en sus bucles. Se conservan sus resultados. [Prueba de composiciones](Validacion/E21/PartySizes.xml)
- El recorrido QA usa la navegación y los controladores reales, con entradas sintéticas para controles clave y una semilla de combate repetible. Se mantiene separada la revisión humana.

## Balance y cobertura

| Clase y raza en el ZIP | Acuerdo | Tras primer RPG | Tras patrulla | Tras conquista |
|---|---|---:|---:|---:|
| Guerrero humano | Informe | 17/29 | 23/31 | 29/37 |
| Monje elfo | Informe | 11/22 | 22/24 | 18/28 |
| Mago enano | Reparación | 2/18 | 18/20 | 24/24 |

El Mago volvió a quedar con margen estrecho aun sin usar Acercarse. En el escenario inicial, el enemigo aparece a unos 3 m y puede avanzar y atacar en su turno. La sonda atacó desde su posición inicial, sin probar retirada táctica. Se conservaron estadísticas y habilidades. Las rutas completas usaron tres compañeros. Los recorridos completos con grupos más pequeños siguen pendientes. El cierre independiente se probó con un estado de partida anterior preparado para el diálogo posterior a las guardianías. El pacto ya existe y el capítulo reconoce ese hecho. La ruta del pacto queda pendiente de una prueba nueva completa en E21. [Datos de balance](Validacion/E21/BALANCE.json) · [Hechos y propiedad final](Validacion/E21/CHAPTER.json)

## Perfilado

La build extraída completó 18 combinaciones de calidad y resolución, con 7 216 envíos de cámara URP fuera de pantalla. Se cargó la partida final de E21 del Guerrero. La referencia «E13» en el JSON es el rótulo histórico de la sonda.

| Alto 1080p | Media | P95 |
|---|---:|---:|
| Aldea en pausa | 6,43 ms | 8,96 ms |
| Menú con vista previa | 10,64 ms | 14,43 ms |
| Preparación RTS en pausa | 10,07 ms | 14,18 ms |

El resultado cubre estas cargas y esta partida. FPS presentados, navegación y combate activo quedan para perfilado interactivo. [Mediciones completas](Validacion/E21/PERFORMANCE.json)

## Revisión humana y E22

Para E22, prioriza una revisión humana de las opciones, la orientación al salir de las ruinas, el pacto completo y el primer RPG del Mago con retirada táctica. Con esos resultados se puede decidir qué actividades secundarias merece ampliar y comparar composiciones pequeñas en las ruinas.

[Diseño, misiones y consecuencias](CAPITULO_E21.md) · [Guía](GUIA_E21.md) · [Checklist](../CHECKLIST.md) · [PR #15](https://github.com/MathiasMerino/RPGRTS/pull/15)
