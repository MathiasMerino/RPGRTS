# UX-02 — Entrega técnica del mundo inicial

Candidata técnica `c5db7ff` en `feature/ux02-world-exploration`. La revisión humana sigue pendiente. El PR en borrador depende de UX‑01 (PR #11) y E19 (PR #10). [PR #12](https://github.com/MathiasMerino/RPGRTS/pull/12)

## Cambios y alcance real

- Una nueva escena de candidata `Assets/01_Scenes/AetherniaUx02World.unity` sobre UX01 conserva escenas anteriores y reutiliza sus interiores, controladores, personajes y bibliotecas públicas. `Assets/17_UX02` contiene configuración optativa, cintas continuas de camino, dos fondos ajustados localmente y navegación propia; no se regenera la aldea ni la mazmorra.
- Sendero occidental3.8m, bifurcación/bucle3.5m, entrada de ruinas contextual y campamento/puesto de patrulla conectados. Dos árboles heredados se reubican sólo donde invadían las rutas. Geometría física del ramal34×24m; campo militar40×30m. Dimensiones y coordenadas en UX02_MUNDO.md.
- Cámara única UX01, exploración5.4m y transición a RPG/RTS/construcción. Proyección de indicadores posterior a la pose de cámara. RoofCutaway conserva restauración de renderers, materiales y física; la revisión humana de comodidad sigue pendiente.
- E prioriza proximidad/foco con histéresis y línea de visión; no atraviesa paredes ni ventanas modales. Un [E] discreto sustituye iconos permanentes. Diálogos de NPC/objetos reutilizan estilos, fuentes y retratos de clase provisionales.
- Habitante y capitán tienen una sola presentación humanoide compartida. Comerciante, enlace y guardia son tres instancias nuevas con catálogo/rig existentes; el guardia recorre una ruta corta a 1,4 m/s y se detiene cerca del héroe. Los agentes se activan tras registrar la navegación. El rig del guardia se enlaza al agente en runtime y usa la animación Walk existente.
- Descubrimientos únicos de sendero/ruinas/campamento/puesto, alijo8oro, ramas3madera y puerta de provisiones2alimento, todos provisionales y ligados a Facts/recursos reales. La puerta no abre un interior. Almacén informa recursos; comercio y compañero usan reglas/costes existentes. No nueva campaña ni inventario separado.
- Primera patrulla: conversación → preparación sin daño → selección y llegadas reales → orden explícita de ataque. Formación1.2m, rechazo previo de destinos inválidos/NaN/ausencia de selección, aproximación según alcance. Se preservan salud, bajas, daño, habilidades y condiciones reales de victoria. Indicadores compactos anclados a las tropas y confirmación temporal de órdenes.
- RPG del cargamento, recompensa única, construcción, producción, ejército cultural, compañeros, territorios y conquista anteriores conservados. Guía/HUD se ajustan al progreso real; exploración libre deja de pedir edificios ya terminados. La preparación guiada descrita corresponde a la primera patrulla; no se declara un nuevo tutorial de conquista.
- Guardado aditivo en Facts y preparación UX01. Continuar conserva identidades/recursos/formación; salida de mazmorra vuelve al acceso forestal. Carga directa F9 reconoce región interior80 y cierra diálogos del estado anterior.
- Materiales Earth Lit y Alpha/MedievalSurface compartidos, UV métricos, viento existente0.035 y bounds0.12m; URP/versiones intactos. Sin materiales/texturas/shaders/plugins externos nuevos ni recursos Meshy/LocalOnly. Arte y relato siguen provisionales.

## Verificación

Desarrollo: DIRIGIDA_05 aprobó7/7 antes de los ajustes nativos finales. XML01–04 fallidos conservados; se corrigieron arnés de Play Mode/closures, árbol en salida, glifos y propiedad del bloqueo modal. Inspección real de PNG detectó máscara Alpha incompatible con UV métricos y apilado excesivo de barras; se corrigió presentación sin cambiar reglas de combate.

WindowsIntento01 conserva arranque de tres agentes inválidos y ensayo de ratón fallido. Intento02 confirma agentes/patrulla/selección simple/Shift; el punto de ensayo caía sobre UI y su rechazo era correcto. Intento03 alcanza selección/arrastre/movimiento/ataque nativos, RPG, victoria RTS real, construcción/producción/comercio/reclutamiento; falla una expectativa redundante de SetParty(true). Cada caso permanece registrado, sin convertir recorridos parciales en pases completos.

La regresión general de `a2b2b69` aprobó 212/213, incluidas las siete pruebas de UX‑02. El Guerrero volvió a perder el caso histórico de la reserva RTS. Los tres timeouts de UX‑01 aprobaron en este pase completo con los mismos límites. Su causa sigue sin confirmar. Después se ajustó el enlace de locomoción del guardia, acotado a los NPC de UX‑02 y a la comprobación de su recorrido. Las siete pruebas dirigidas aprobaron en `c5db7ff`, incluida la comprobación de Walk en movimiento real. [Regresión general](Validacion/UX02/GENERAL.xml) · [Dirigida final](Validacion/UX02/DIRIGIDA_FINAL.xml)

La build `c5db7ff` aprobó ocho procesos Windows con gráficos. Guerrero humano, Mago enano y Monje elfo completaron el circuito y cargaron su partida en un proceso nuevo. El Mago repitió ambos pasos desde el ZIP extraído. La comprobación usa WASD, E y ratón sintético a través de Input System. Las tres clases ganaron la patrulla inicial al primer intento en esta ronda. El Monje terminó con 1 PV. [Comprobantes Windows](Validacion/UX02/WINDOWS.json)

El ZIP `ProyectoRPGRTS/Builds/Aethernia-UX02-Mundo-Windows.zip` pesa 87 907 373 bytes. CRC, SHA por archivo y coincidencia del ejecutable extraído verificados. Los 21 ZIP anteriores coinciden con la auditoría de entrada. [Manifiesto del ZIP](Validacion/UX02/PORTABLE.json) · [Integridad de fuentes](Validacion/UX02/INTEGRIDAD_FINAL.json)

La evidencia reúne 59 PNG, con cuatro comparativas de UX‑01 y capturas Play Mode a 720p, 1080p y 1440p. Los vídeos usan audio DSP y un paso de captura de 12 Hz. [Capturas](Images/UX02) · [RPG](Videos/UX02/UX02-Combate-RPG.mp4) · [RTS](Videos/UX02/UX02-Patrulla-RTS.mp4)

## Cómo revisar

Extrae el ZIP y ejecuta `Aethernia.exe`. En Unity 6000.5.8f1, abre `Assets/01_Scenes/AetherniaUx02World.unity` y pulsa Play. [Guía de revisión](GUIA_UX02.md)

La evaluación humana sigue pendiente para comodidad de cámara y controles, oclusión en juego libre, ritmo, balance y arte. La captura automática del cruce muestra un control antiguo de interacción parcialmente recortado en el borde inferior. La guía registra cómo comprobar si aparece durante el juego manual. El fallo RTS histórico sigue abierto y el resultado del Monje aconseja incluir esa clase en la revisión de dificultad. [Captura del cruce](Images/UX02/Windows/warrior-W01-ForestFork.png)
