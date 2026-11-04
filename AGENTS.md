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
- Piloto activa mando táctico en conquista del puesto, sobre héroe, facciones, ejército y guardado existentes. La patrulla anterior conserva sus reglas y su incidencia abierta. Mago y Monje completaron la ruta Windows y Continuar; Guerrero se detuvo en patrulla heredada. Separar las dirigidas, Windows y general de la revisión humana. Sin E21 ni fusiones.
- No repetir AuthorE20 sobre assets ya guardados. Runner Invoke-ProgressionSmoke.ps1 -Phase E20 con raza por clase. La grabación opcional sólo captura el piloto. El player oculto mide iteraciones de Unity, no FPS jugables si no hay render automático.

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
