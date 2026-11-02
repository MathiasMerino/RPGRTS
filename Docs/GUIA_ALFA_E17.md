# Revisión de E17

Candidata de desarrollo provisional. No es v1.0; aprobación artística, diversión y balance pendientes. Consultar E17_ALFA.md para resultados y limitaciones reales de la entrega.

## Abrir en Unity

Unity Hub → Add project from disk → ProyectoRPGRTS. Usar Unity6000.5.8f1, URP17.5.0 ya declarada. Abrir Assets/01_Scenes/AetherniaAlphaCandidate.unity y pulsar Play. No ejecutar generadores sobre los assets existentes. Las escenas anteriores se conservan.

## Ejecutable Windows

Extraer el ZIP E17 completo y abrir Aethernia.exe. Nueva partida permite escoger Guerrero, Monje o Mago, nombre y linaje provisional. Continuar recupera la partida local; los lanzadores Revisar-warrior/monk/mage.cmd utilizan copias E13 reales incluidas para revisar grupo, ruinas y conquista sin repetir el prólogo. Guardar modifica esas copias: respaldarlas para repetir el recorrido.

## Controles

- WASD: caminar en exploración; E: interacción cercana; rueda/flechas/botón central: cámara. P: opciones/pausa.
- RPG: acercarse, ataque/habilidad/guardia/recuperación, fin de turno. El reloj estratégico se pausa durante combate.
- RTS: clic o arrastre para seleccionar, Shift para añadir; clic derecho para mover/atacar. Seleccionar al héroe habilita sus acciones. Ctrl+1/2/3 guarda grupos;1/2/3 los selecciona. El héroe debe sobrevivir.
- Campamento: administrar, escoger plano, clic derecho sobre suelo libre; no bloquear recorridos. Producción/recursos compartidos con ejército y expedición.
- Grupo y expediciones: incorporar compañeros o reservarlos, preparar ejército, territorios. Entrada de ruinas al oeste del campamento(4,−11); E cerca de encuentros, objetivo y salida. Descansar en campamento consume tiempo y conserva las reglas existentes.
- Ficha/inventario/sesión/grupo se cierran con × o Escape. Ayuda aparta paneles de construcción y mando; vuelve al contexto al cerrarse.

## Revisión visual sugerida

Comparar menú y selección a720p/1080p/1440p;4K disponible para revisar UI. Los retratos/conceptos son referencias existentes; los cuerpos animados siguen siendo provisionales. Revisar siluetas de las tres clases, caminos, hojas, techos recortables, ruinas, marcas de viaje y círculos RTS.

En Opciones probar Bajo/Medio/Alto y aplicar. Bajo reduce escala interna3D a80%, sombras20m/512, MSAA1 y elimina AO/post; Medio mantiene100%, sombras40m/1024/MSAA2; Alto100%,60m/2048/MSAA4 y sombras suaves. La UI conserva resolución nativa. Las preferencias históricas de índice1 pasan a Medio; no cambia el formato de partida.

## Pendientes explícitos

Evaluación humana de ×/ESC/controles/diversión, aprobación artística y balance; modelos E08 no finales. Meshy experimental no incluido ni animable automáticamente (sin rig/UV válidas en el recurso inspeccionado). Una carga RTS automática puede perder al héroe aunque sobrevivan aliados: no se garantiza victoria ni se ajustó salud para ocultarlo. E17-A-RTS sigue abierto con trazas.

## Repetir comprobaciones técnicas

Cerrar el editor antes de ejecutar Tools/Invoke-UnityValidation.ps1 -Mode Vertical. BuildAlpha compila únicamente la escena E17; no sustituye las builds anteriores. Invoke-DemoSmoke.ps1 e Invoke-ExpeditionSmoke.ps1 aceptan -WithGraphics -Executable y usan slots aislados. Invoke-AlphaBenchmark.ps1 mide explícitamente aldea pausada, menú3D y RTS pausado en una build sin editor ni grabación; no interpreta los12Hz de vídeo como FPS.
