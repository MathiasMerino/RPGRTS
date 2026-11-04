# E22 · Mundo vivo

E22 continúa la campaña en el mismo mundo. El tablón ofrece dos encargos de exploración y recuperación desde la aldea, y una visita opcional a las ruinas después del cierre. Elia compra y vende con las reservas y el inventario existentes. Los nombres, precios y recompensas siguen siendo provisionales.

| Sistema | Comportamiento |
|---|---|
| Contratos y exploración | Aceptación, objetivo y entrega se registran por separado. El mojón reconoce un hallazgo anterior. El cajón se abre tras aceptar. El centinela exige interacción deliberada y victoria real. |
| Comercio y progresión | Madera, raciones y chaleco. Las ofertas verifican coste y capacidad. La entrega de contratos usa el registro de XP E18 y su límite actual. |
| NPC y asentamiento | Nilo cambia su diálogo tras el encargo de combate. Elia usa una ruta corta tras el cierre, vuelve a su punto y se detiene para interactuar. El taller abre la oferta de chaleco y la granja mantiene la mejora de producción pagada. |
| Grupo, ejército y conquista | Las ruinas usan el héroe y sus compañeros reales. Las recompensas alimentan la misma economía de construcción y reclutamiento. La última guardia queda disponible con cualquiera de los cierres. |
| Guardado | Se usa el formato v1, con hechos y una ranura E22 separada. Se conservan inventario, XP, construcciones, grupo, ejército y territorio. |

## Estabilización de E21

- El código anterior exigía que hubiera vencido el intervalo del ataque básico incluso cuando la técnica ya tenía foco y estaba disponible. En E22, la técnica depende de su propio enfriamiento y puede elegir un rival en alcance cuando el objetivo ordenado no es alcanzable. Conserva el requisito de selección, foco, distancia, línea de visión y combate activo.
- El Mago enano llegó a 2/18 PV en E21 con semilla 42. En E22 terminó el encuentro con 16/18 PV tras cuatro retiradas regladas después de actuar. El perfil se conserva. Queda pendiente comparar otras semillas y estrategias con jugadores.
- Las dirigidas volvieron a ejecutar el inicio de la guardianía con 0, 1, 2 y 3 acompañantes y el cierre independiente con una partida anterior. Windows completó las dos guardianías de campaña con tres acompañantes y venció al centinela opcional con cero, uno y dos. Queda pendiente completar ambas guardianías originales con grupos más pequeños.
- Los tres cierres se completaron desde partidas nuevas. El Guerrero conquistó el puesto y ejecutó la técnica en el intento normal y a 12 Hz durante la grabación. El Mago cerró con un pacto pagado y el Monje por cuenta propia. En esos dos casos, el puesto conservó su propietario.

## Verificación

- 7/7 dirigidas en Unity. Incluyen contratos y comercio en escena, precio y capacidad de bolsa, entrega única y recarga de un guardado anterior. [Resultados](Validacion/E22/Dirigidas.xml)
- Build Windows x64 realizada con Unity 6000.5.8f1. El ZIP superó CRC y SHA-256 por archivo. También se verificaron los 25 ZIP históricos. [Manifiesto](Validacion/E22/PORTABLE.json)
- La configuración local preexistente mantiene antialiasing 2 en calidad media. La compilación usó esa configuración y el cambio local queda fuera de los commits.
- Los recorridos del ZIP aprobaron 6/6 procesos de inicio y Continuar con las tres clases. La grabación añadió 2/2 procesos. La automatización empezó en el menú, usó navegación, encuentros y controladores reales. [Resultados Windows](Validacion/E22/WINDOWS.json)
- La captura automatizada produjo 218 PNG a 1280×720 en los tres recorridos principales. El vídeo muestra el combate RTS del intento grabado con 154 fotogramas y audio de Unity. [Capturas](Images/E22) · [Vídeo](Videos/E22/Puesto-del-Umbral.mp4)
- Regresión general 230/230, sin fallos, omitidas ni inconclusas. La evaluación humana sigue pendiente. [Resultados](Validacion/E22/GENERAL.json)
- El panel del centinela conserva un botón deshabilitado para un segundo objetivo inexistente. Su ajuste pasa a E23.

[Guía de prueba](GUIA_E22.md) · [Contratos](CONTRATOS_E22.md) · [Comercio](COMERCIO_E22.md) · [Pila de PR](INTEGRACION_PRS.md)
