# E20 — Guerra táctica y continuidad del mundo

## Candidata

**La defensa del paso fronterizo** es un piloto provisional en `AetherniaE20Warfare.unity`. Se llega desde el campamento al Paso del bosque y al Puesto del Umbral en la misma partida. El capitán orienta al jugador tras el prólogo. Los reclutas salen del cuartel y del ejército persistente. La campaña usa sus costes, cultura, perfiles y bajas. El puesto requiere acceso asegurado, proximidad y facción hostil.

| Área | Cambio |
|---|---|
| Mando | Clic, Mayús, arrastre y deselección. Tres grupos con Ctrl+1–3, recuperables con 1–3. Los grupos depuran bajas y se reinician al salir. |
| Órdenes | Mover, atacar objetivo, detener, mantener, reagrupar, defender y avance ofensivo. El clic en un objetivo vivo hostil o el botón Iniciar confirma la ofensiva tras una llegada real. |
| Formaciones | Línea, columna y compacta. Separación base 1,45 m y pequeña separación adicional entre corto y largo alcance. La orden se rechaza si algún destino no tiene ruta completa o solapa otro. |
| IA | Los defensores parten de sus puestos y reaccionan con alcance y línea de visión. El radio máximo de persecución automática del puesto es 9 m. Las posturas aliadas varían la reacción local. En este piloto la línea de visión deja pasar unidades del mismo bando y sigue respetando muros y cuerpos rivales. |
| Héroe | El actor RPG conserva identidad, modelo, PV, arma, armadura y nivel. Recuperación limitada, Guardia y técnicas E18 con coste, alcance y enfriamiento. |
| Terreno y cámara | Cuatro módulos de piedra E07 con colisión y navegación propia. Paso central de unos 8 m entre muros y dos flancos en terreno llano, 1 unidad = 1 m. HUD contextual, marcador de orden y F para centrar la selección. Exploración recupera la cámara de seguimiento. |
| Consecuencias | La victoria real actualiza el propietario, XP y producción del territorio. La derrota y retirada conservan el propietario rival. Las bajas y los cambios del mundo se guardan al regresar. Las órdenes de combate son temporales. |

La patrulla de UX‑02 se conserva y precede al piloto en una partida nueva. La fuerza se recluta mediante la economía existente, incluida la opción de activar una granja si escasea el alimento. Se mantienen los modelos, materiales y animaciones provisionales. El piloto acota la escala al ejército que admite el cuartel existente, con hasta seis reclutas y un héroe. Las plazas de formación se verifican matemáticamente para 5–10 unidades y las rutas se comprueban dentro de la escena durante el recorrido.

## Incidencia RTS heredada

El registro de UX‑02 muestra que `ThreeClassesWithLegalBacklineOrdersAt30` termina con el Guerrero a 0 PV y en `FirstRts`. Su última recuperación aceptada fue la única. El último arquero seguía a unos 5,75 m y aplicó 2 puntos de daño en el momento de la derrota. El registro también muestra 43 rechazos por enfriamiento. Son observaciones del recorrido y la causa raíz del patrón de victorias por clase sigue abierta. E20 mantiene la prueba histórica, sus aserciones y su escena.

## Pruebas y evidencias

- Dirigidas 8/8 en Unity 6000.5.8f1. El Mago entra con partida UX‑02, despliega, usa técnica y conquista. El Guerrero herido verifica derrota por daño de los defensores. Otras pruebas cubren la retirada, las tres geometrías, rutas central y laterales, línea de visión y dependencias públicas de la escena.
- Windows x64, fuente `8989762`. Mago/enano y Monje/elfo completaron partida nueva y Continuar, 4 procesos aprobados. Guerrero/humano se detuvo en la patrulla heredada, antes del piloto. En un intento quedaron 2 aliados, 0 rivales y el héroe a 0 PV, lo que cuenta como derrota. El Monje superó la patrulla en su segundo intento explícito.
- ZIP con CRC y SHA‑256 por archivo aprobados. Se verificó la integridad de los 22 ZIP previos. Dos partidas nuevas desde el ZIP terminaron en derrota en la patrulla heredada. **Continuar desde el ZIP pasó** con una copia de la partida E20 del Mago, con propietario, habilidad, equipo y ejército verificados.
- El vídeo se generó con 142 fotogramas continuos de la cámara de la build y audio DSP de Unity. La tasa fija de captura fue de 12 Hz. El muestreo de 120 iteraciones ocultas con 4 aliados y 4 rivales en RTX 3050 Laptop a 1280×720 registró 0 renderizados automáticos de cámara. Se conservan tiempos del bucle y memoria del proceso. FPS jugables, coste de GPU y GC quedan pendientes de medición interactiva.
- La pestaña **Territorios** muestra el resultado y el propietario. El aviso de guardado puede reemplazar el mensaje de victoria o derrota al volver.

[Resultados y verificación de integridad](Validacion/E20) · [Capturas Windows](Images/E20/Windows) · [Vídeo](Videos/E20/Paso-Fronterizo-Windows.mp4)

## Revisión humana

Queda pendiente validar dificultad, ritmo económico, lectura a distintas resoluciones y utilidad de las posiciones. La guía explica la ruta del campamento y los controles. Las próximas comprobaciones técnicas deben comparar rutas de escape del Guerrero en la patrulla histórica a 30 FPS y repetir el piloto con más composiciones antes de elevar el límite militar.

- [Guía de revisión](GUIA_E20.md)
- [PR #13 en borrador](https://github.com/MathiasMerino/RPGRTS/pull/13)
