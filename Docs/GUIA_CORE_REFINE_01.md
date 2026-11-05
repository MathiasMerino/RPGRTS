# CORE-REFINE-01 · Guía de revisión

En Windows, extrae toda la carpeta y abre **Aethernia.exe**. En Unity **6000.5.8f1**, abre **Assets/01_Scenes/AetherniaCoreRefine01.unity** y pulsa **Play**. En ambos casos se abre el menú normal.

## Recorrido sugerido

1. Elige clase y raza en **Nueva partida**. Usa un perfil de prueba y respalda tu guardado antes de elegir **Continuar** si quieres conservar una partida previa. CORE reutiliza la ranura E22/E23. **F5** guarda cuando la partida permite guardar.
2. Cierra la introducción, camina con **WASD** y usa la rueda para ajustar la cámara. Habla con Mara mediante **E**, acepta el encargo y sigue la guía hasta el cargamento.
3. En combate, selecciona un rival en el terreno o en la barra de iniciativa. **A** indica aliado, **R** rival e **I** iniciativa. **Esc** cancela la selección. Usa el clic derecho en el suelo o **Acercarse** para moverte por la superficie navegable. La barra indica los metros y si queda una acción. **Fin de turno** cede el control.
4. Prueba **Atacar**, **Guardia** y **Recuperar**. Abre **Registro** y compara la tirada y el modificador contra la defensa. Al desbloquearla, pulsa **Técnica**, selecciona un objetivo marcado con **+** y pulsa **Confirmar**. La barra indica su alcance y coste de foco. Si un obstáculo tapa la línea de ataque, busca otra posición.
5. Abre **Grupo**, incorpora acompañantes y entra en la expedición. Identifica quién actúa en la barra y prueba el alcance de cada aliado. El flanqueo da +2 a ataques básicos de cuerpo a cuerpo con un aliado vivo al otro lado del rival, en el mismo nivel, a corta distancia y con línea libre.
6. Tras el encargo, sigue la campaña y habla con el capitán. En la preparación RTS, selecciona tropas, emite órdenes y espera su llegada al objetivo. Tras volver a explorar, abre **Menú → Construir** cuando esté disponible. Guarda, vuelve al título y prueba **Continuar**.

## Escenas de Unity

- **AetherniaCoreRefine01** es la escena jugable de esta candidata.
- **AetherniaArtEnv01** mantiene la candidata de arte previa. **AetherniaArtEnv01Study** es su escena de estudio.
- **AetherniaE23IntegratedAlpha**, las escenas UX anteriores y **PlayablePrototype** sirven para regresión.

En los resultados de revisión, anota clase, raza, resolución, tipo de combate y paso exacto. Si aparece una interfaz duplicada, captura el cuadro completo y registra qué ventana estaba abierta.
