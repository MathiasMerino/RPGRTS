# Crónicas de Aethernia

## Contexto y fuentes
- Responder en español. Raíz de trabajo/Git: esta carpeta; proyecto Unity: `ProyectoRPGRTS/`.
- Unity6000.5.8f1, C#, URP17.5.0. Conservar versiones. Usar EntityId; no introducir GetInstanceID/objectReferenceInstanceIDValue obsoletos.
- GDD original0.2: `ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx`; no sobrescribirlo. Lectura `Docs/GDD_LECTURA.md`, comprobar SHA si cambia el Word.
- Clases Guerrero/Mago/Monje y reglas provisionales aprobadas en `Docs/ESPECIFICACION_PROTOTIPO.md`; hoja futura `Docs/PLAN_EXPANSION.md` sin producción automática.
- Leer resumen CHECKLIST.md y secciones relacionadas del plan/GDD. Diferenciar confirmado, propuesto y pendiente; ejemplos del GDD no son requisitos.
- Candidata actual E19: escena `Assets/01_Scenes/AetherniaCivilizationsDemo.unity`, datos `Assets/15_Civilizations`. Informe `Docs/ENTREGA_E19.md`, guía `Docs/GUIA_E19.md`, razas/culturas y balance en Docs. Rama feature/e19-razas-civilizaciones, PR#10draft sobre E18/PR9, sin E20 ni merges automáticos.
- E19 candidata:21/21dirigidas y6/6procesosWindows(4raw+2portable), general196/197(solo reservaRTSGuerrero falla;21E19PASS); 52PNG/2MP4, ZIPCRC/SHA/19previos yportable verificados. Incidencia RTS histórica reproducida y evaluación humana abiertas. E18 general174/176 con visual corregida después; no sumar pruebas dirigidas como regresión global.
- Mapa actual `Docs/ORGANIZACION_PROYECTO.md`, índice `Docs/README.md`; detalle histórico de fases y límites en `Docs/Historial/CONTEXTO_ETAPAS.md`. No borrar recursos por antigüedad o por faltar en una única escena.

## UX-01 — candidata técnica posterior a E19
- Rama feature/ux01-onboarding-camera-world, PR11draft sobre E19/PR10; código/build5468c38. Nueva escena Assets/01_Scenes/AetherniaUx01Demo.unity y datos Assets/16_UX01; E19 y originales conservados. Informe/guía Docs/ENTREGA_UX01.md y GUIA_UX01.md. No E20 ni merges.
- Dirigidas9/9, refinamiento3/3, Windows6/6(4raw+2portable),37PNG/2MP4; ZIPCRC/SHA/20previosPASS. General202/206: RTS histórico+3timeouts180s. Los3pasan aislados con mismos límites/código; no sustituir la general ni declarar causa/resolución confirmadas. UX-HUM, UX-GEN-TIMEOUT y UX-RTS-HIST abiertos.
- BuildUx01→Builds/Ux01Windows; runner Invoke-ProgressionSmoke.ps1 -Phase UX01 con una raza por clase. No regenerar Ux01Scene. Refine es autoría acotada/idempotente, no arranque.
- Primera misión segura activa RPG; RTS empieza en preparación y exige órdenes/llegadas reales. Guardado aditivo de preparación. Un reintento explícito puede ser necesario, queda registrado; no garantizan victoria las estrategias de prueba.
- Dos fondos Grass E07 invadían visualmente el campo nuevo: copias WesternGround UX aplanadas localmente; materiales compartidos y mallas UV en metros. No modificar los originales para ocultar este problema.

## E20 — candidata de guerra táctica
- Rama feature/e20-tactical-warfare, PR13 borrador sobre UX02/PR12 ca95205. Escena Assets/01_Scenes/AetherniaE20Warfare.unity y datos Assets/18_E20. BuildE20 → Builds/E20Windows. Código de build 8989762. Guía Docs/GUIA_E20.md e informe Docs/ENTREGA_E20.md.
- Piloto activa mando táctico en conquista del puesto, sobre héroe, facciones, ejército y guardado existentes. La patrulla anterior conserva sus reglas y su incidencia abierta. Mago y Monje completaron la ruta Windows y Continuar; Guerrero se detuvo en patrulla heredada. General220/221, únicas incidencias de patrulla heredada; 8/8 E20 en general más 1/1 adicional con siete aliados y Guardia de frontera. Portable Continuar PASS, dos inicios fallaron en patrulla. Separar resultados automáticos de la revisión humana. Sin E21 ni fusiones.
- No repetir AuthorE20 sobre assets ya guardados. Runner Invoke-ProgressionSmoke.ps1 -Phase E20 con raza por clase. La grabación opcional sólo captura el piloto. El player oculto mide iteraciones de Unity, no FPS jugables si no hay render automático.

## E20.1 — candidata alfa de estabilización
- Rama fix/e20-1-alpha-stabilization y PR14 borrador sobre E20/PR13 c89801c. Misma escena Assets/01_Scenes/AetherniaE20Warfare.unity. BuildE201 → Builds/E201Windows, fuente f9da1b3. ZIP Aethernia-E201-Alfa-Windows.zip y guía Docs/GUIA_E201.md.
- Histórico a 30 FPS de 0/1 a 1/1 con las mismas aserciones. General223/223, dirigidas17/17, Windows8/8 (2 original y6 desde ZIP). Primera tentativa de patrulla aprobada para las tres clases. Mago 2/18 PV en el primer RPG con Acercarse y semilla de QA; balance humano pendiente. 33 capturas, botón contextual comprobado a 720/1080/1440.
- Runner Invoke-ProgressionSmoke.ps1 -Phase E201 -WithGraphics con raza por clase. La prueba E20.1 parte del menú, usa un slot nuevo y registra balance por etapa. Conservar reportes Docs/ENTREGA_E201.md y Docs/Validacion/E201. Sin fusiones automáticas ni E21.

## E21 — Las rutas de la frontera
- Rama feature/e21-first-campaign, PR15 borrador sobre E20.1/PR14 4ab7b4f. Escena Assets/01_Scenes/AetherniaE21Campaign.unity. BuildE21 → Builds/E21Windows, fuente df03c7a. ZIP Aethernia-E21-Campana-Windows.zip. Guía Docs/GUIA_E21.md y capítulo Docs/CAPITULO_E21.md.
- Las elecciones y el capítulo se guardan como hechos v1. Dossier habilita conquista. El cierre con Tarek permite continuar sin conquistar. General226/226, dirigidas3/3 más inicio de guardianía 1/1 con0–3 acompañantes. Windows8 procesos aprobados, seis desde ZIP, más un intento grabado que ganó el puesto sin ejecutar la técnica. 32PNG, un MP4 y18 filas de perfilado URP. Pacto desde partida nueva, perfilado interactivo y revisión humana pendientes.
- Runner Invoke-ProgressionSmoke.ps1 -Phase E21 -WithGraphics con raza por clase. El Mago enano terminó el primer RPG con2/18 PV aun sin Acercarse. Conservar estadísticas hasta comparar estrategias y semillas. Sin E22 ni fusiones automáticas.

## E22 — Mundo vivo
- Rama `feature/e22-living-world` sobre E21/PR #15 `78695c1`. Escena `Assets/01_Scenes/AetherniaE22LivingWorld.unity`, datos `Assets/20_E22`. BuildE22 → Builds/E22Windows. Guía `Docs/GUIA_E22.md`. No ejecutar AuthorE22 sobre la escena existente.
- Tablón, mojón, cajón y centinela opcional comparten la partida. Elia utiliza la bolsa y reservas existentes. La ranura `demo-e22-v1.json` puede importar `demo-v1.json` cuando aún no existe. Runner `Invoke-ProgressionSmoke.ps1 -Phase E22 -Classes warrior,mage,monk -Races human,dwarf,elf -WithGraphics`. La validación separa resultados automáticos y humanos. Sin E23 ni fusiones.

## E23 — Alfa integrada
- Rama `feature/e23-integrated-alpha` sobre E22 `df1b719` y PR #16. Escena `Assets/01_Scenes/AetherniaE23IntegratedAlpha.unity`. `BuildE23` → `Builds/E23Windows`. Guía `Docs/GUIA_E23.md` e integración `Docs/INTEGRACION_E23.md`.
- Reutiliza el guardado de E22. Pruebas de layout del tablón a 720p/1080p/1440p. Runner `Invoke-ProgressionSmoke.ps1 -Phase E23 -Classes warrior,mage,monk -Races human,dwarf,elf -WithGraphics`. Candidata técnica y evaluación humana separadas. Sin E24 ni fusiones.

## Alcance y método
- Diagnosticar y planificar antes de implementar. Un encargo explícito de implementación autoriza su alcance; no inventar fases, reglas completas o historia sin aprobación.
- Trabajar tareas acotadas, preservar trabajo existente y registrar ID/estado/aceptación/archivos/pruebas en CHECKLIST y REGISTRO después de cada actividad; conservar historial.
- Estados: Pendiente, En desarrollo, Bloqueado, Completado y verificado. Código que compila no demuestra aceptación ni revisión humana.
- No sobrescribir cambios ajenos. Mantener separados datos compartidos, estado mutable, lógica y presentación; SOLID con criterio, interfaces/eventos útiles y suscripciones liberadas.
- Escala1unidad=1metro; uGUI/estilos/prefabs compartidos, URP y modelos provisionales existentes. Sin nuevas dependencias o optimización compleja sin necesidad/mediciones.
- Conservar `.meta`/GUID. Mover desde Unity o junto con `.meta`; no editar cachés/proyectos C# generados. No copiar recursos/interfaces protegidos.
- Meshy y `Assets/LocalOnly` son experimentales privados: conservar/ignorar, no integrar ni publicar originales/derivados sin autorización.

## Validación y herramientas
- Antes de cada tarea: git status, dependencias/aceptación y análisis; al finalizar revisar diff y registrar cambios importantes.
- Preferencia: segundo plano con `Tools/Invoke-UnityValidation.ps1`, editor exacto y una sola instancia por proyecto. Revisar Console/referencias/Play Mode cuando corresponda.
- E19: `-Mode BuildCivilizations` → `Builds/CivilizationsE19Windows`; `Tools/Invoke-ProgressionSmoke.ps1 -Phase E19 -Classes warrior,mage -Races human,dwarf -WithGraphics` usa slots/preferencias aislados. E18 conserva BuildProgression y valor predeterminado del runner. Regresión `-Mode Vertical`, filtro dirigido opcional.
- PlayablePrototype y escenas E03–E17 se conservan para regresión. Guías antiguas en Docs/Historial y guías de compilaciones reutilizables en Docs.
- No repetir generadores sobre assets existentes: Civilizations/Create, ProgressionScene/Create, AlphaPipeline/Create, AlphaScene/Create, FirstDemo/Create, Village/Animated/Humanoid/MedievalHud/autorías de arte. Son autoría, no arranque.
- Visual Studio2026/Tools for Unity y paquete IDE Unity2.0.26 ya instalados. Instalar herramientas solo por necesidad concreta compatible.

## Git y contexto
- Origin `https://github.com/MathiasMerino/RPGRTS.git`; público. Revisar credenciales/archivos grandes antes de publicar.
- main estable/predeterminada; dev integración; ramas temporales feature/*,fix/*,test/* desde dev o base apilada autorizada cuando la entrega anterior siga sin fusionar. Conservar commits/historial y documentar dependencia de PRs.
- No publicar remotos en tareas locales sin autorización de la sesión; no merge, force push, reinicializar, borrar referencias, cambiar configuración global/protecciones o publicar recursos privados automáticamente.
- PR→dev normalmente; dev→main solo con aprobación y evidencia. Autenticación GCM sin tokens en archivos/URLs/commits; si falta, pedir intervención.
- Usar rg por sistema y .rgignore; no recorrer Library/Temp/Logs/obj/.vs/paquetes completos salvo diagnóstico dirigido. El historial/medios/evidencias se abren por ruta o con rg --no-ignore cuando hagan falta. Reutilizar lectura del GDD; cargar solo skills necesarias.
- No delegar ni crear subagentes salvo petición expresa del usuario.
