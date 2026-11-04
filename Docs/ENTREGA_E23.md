# E23 · Primera alfa integrada

Escena `Assets/01_Scenes/AetherniaE23IntegratedAlpha.unity`. Build Windows x64 `AETHERNIA_ALPHA_E23_Windows.zip`. La candidata usa la campaña, el mundo libre y los datos de E22 sobre el mismo estado. Se conservaron razas, clases, retratos, modelos, navegación, materiales URP y escenas anteriores. Unity 6000.5.8f1.

## Cambios jugables

- El cartel inferior muestra el verbo del objetivo válido de **E**. Consulta el mismo foco de la entrada de jugador y sólo se muestra al acercarse. Las confirmaciones del cargamento comprueban alcance y línea de visión. Cobrar guarda inmediatamente.
- La guía de exploración libre prioriza entregar los encargos listos y luego orienta a los aceptados. Si el destino está en otra zona, señala la entrada o salida de las ruinas. La campaña conserva la prioridad hasta su cierre.
- El centinela presenta un solo objetivo. La cámara encuadra al héroe y al primer rival de una guardianía con margen para el panel derecho. El panel genérico deja espacio al de turnos durante estos combates.
- El Mago recibe una pista para atacar desde lejos y retirarse antes de terminar el turno. Se mantuvieron estadísticas y reglas. El comercio y los encargos reutilizan sonidos del proyecto.
- Elia conserva la bolsa real, el inventario y los servicios que dependen del taller. Las recompensas siguen alimentando la misma economía de mejora, producción y reclutamiento. RTS conserva despliegue seguro, órdenes, técnica, conquista y producción territorial.

## Evidencia y límites

- E22 cerró 230/230 en regresión y 8/8 procesos Windows, con tres cierres y recarga. El fallo visual del segundo botón pasó a E23. [Informe E22](ENTREGA_E22.md)
- E23 pasó 5/5 dirigidas después de corregir el alcance de la prueba de interfaz. El primer intento obtuvo 4/5 porque el validador del tablón también inspeccionó el menú oculto. Se verificaron los botones y etiquetas del tablón a 1280×720, 1920×1080 y 2560×1440. Esta prueba cubre el tablón. [Prueba dirigida](Validacion/E23/Dirigidas.xml)
- Regresión general, prueba del ZIP y perfilado en curso. La evaluación humana de balance, oclusión, ritmo, nitidez y fluidez presentada queda pendiente.

El guardado v1 de E22 se conserva. La compilación usa la configuración local existente de antialiasing de calidad media, que queda fuera de los commits. No se añadieron dependencias de arte.

[Guía](GUIA_E23.md) · [Notas](NOTAS_E23.md) · [Integración](INTEGRACION_E23.md)
