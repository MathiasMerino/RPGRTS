# Crónicas de Aethernia

## Contexto y fuentes
- Responder en español. Raíz de trabajo/Git: esta carpeta; proyecto Unity: `ProyectoRPGRTS/`.
- Unity6000.5.8f1, C#, URP17.5.0. Conservar versiones. Usar EntityId; no introducir GetInstanceID/objectReferenceInstanceIDValue obsoletos.
- GDD original0.2: `ProyectoRPGRTS/Assets/05_Docs/GDD_GAMERPGTS.docx`; no sobrescribirlo. Lectura `Docs/GDD_LECTURA.md`, comprobar SHA si cambia el Word.
- Clases Guerrero/Mago/Monje y reglas provisionales aprobadas en `Docs/ESPECIFICACION_PROTOTIPO.md`; hoja futura `Docs/PLAN_EXPANSION.md` sin producción automática.
- Leer resumen CHECKLIST.md y secciones relacionadas del plan/GDD. Diferenciar confirmado, propuesto y pendiente; ejemplos del GDD no son requisitos.
- Entrega actual E18: escena `Assets/01_Scenes/AetherniaProgressionDemo.unity`, datos `Assets/14_Progression`. Informe `Docs/ENTREGA_E18.md`, guía `Docs/GUIA_E18.md`, balance/progresión en Docs. PR#9draft sobre E17/PR8; no E19 ni merges automáticos.
- E18: general174/176, fallo visual corregido4/4dirigidas posteriores;8procesosWindows,49PNG/2MP4 yZIPportable verificados. Incidencia E17-A-RTS y evaluación humana siguen pendientes: no declarar regresión completamente verde.
- Mapa actual `Docs/ORGANIZACION_PROYECTO.md`, índice `Docs/README.md`; detalle histórico de fases y límites en `Docs/Historial/CONTEXTO_ETAPAS.md`. No borrar recursos por antigüedad o por faltar en una única escena.

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
- E18: `-Mode BuildProgression` → `Builds/ProgressionE18Windows`; `Tools/Invoke-ProgressionSmoke.ps1` usa slots/preferencias aislados. Regresión `-Mode Vertical`, filtro `-TestFilter` para casos pertinentes.
- PlayablePrototype y escenas E03–E17 se conservan para regresión. Guías antiguas en Docs/Historial y guías de compilaciones reutilizables en Docs.
- No repetir generadores sobre assets existentes: ProgressionScene/Create, AlphaPipeline/Create, AlphaScene/Create, FirstDemo/Create, Village/Animated/Humanoid/MedievalHud/autorías de arte. Son autoría, no arranque.
- Visual Studio2026/Tools for Unity y paquete IDE Unity2.0.26 ya instalados. Instalar herramientas solo por necesidad concreta compatible.

## Git y contexto
- Origin `https://github.com/MathiasMerino/RPGRTS.git`; público. Revisar credenciales/archivos grandes antes de publicar.
- main estable/predeterminada; dev integración; ramas temporales feature/*,fix/*,test/* desde dev o base apilada autorizada cuando la entrega anterior siga sin fusionar. Conservar commits/historial y documentar dependencia de PRs.
- No publicar remotos en tareas locales sin autorización de la sesión; no merge, force push, reinicializar, borrar referencias, cambiar configuración global/protecciones o publicar recursos privados automáticamente.
- PR→dev normalmente; dev→main solo con aprobación y evidencia. Autenticación GCM sin tokens en archivos/URLs/commits; si falta, pedir intervención.
- Usar rg por sistema y .rgignore; no recorrer Library/Temp/Logs/obj/.vs/paquetes completos salvo diagnóstico dirigido. El historial/medios/evidencias se abren por ruta o con rg --no-ignore cuando hagan falta. Reutilizar lectura del GDD; cargar solo skills necesarias.
- No delegar ni crear subagentes salvo petición expresa del usuario.
