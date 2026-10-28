# Registro de cambios de Crónicas de Aethernia

Conservar entradas anteriores; añadir fecha, tareas, alcance, archivos y validación al terminar cada actividad.

## 2026-10-28 — Diagnóstico y preparación inicial

Tareas: PRE-01, PRE-02 y PRE-03. Alcance autorizado: analizar y preparar el proyecto, sin programar mecánicas.

Se localizó el proyecto real `ProyectoRPGRTS/` y se leyó íntegramente el GDD 0.2. Se revisaron escena, assets, scripts, prefabs, paquetes, ajustes e instalación del IDE. Resultado: plantilla URP organizada, una escena y ninguna mecánica propia implementada. Se distinguieron definiciones conceptuales, propuestas experimentales y decisiones pendientes.

Se crearon `AGENTS.md`, `CHECKLIST.md`, `PLAN_DESARROLLO.md` y este registro en la raíz de trabajo, fuera de Assets para no generar importaciones documentales. Se añadieron `.gitignore` y `.rgignore` para excluir cachés/archivos locales y acotar búsquedas. Se prepara `Docs/GDD_LECTURA.md` como copia textual consultable, conservando el Word original y las tablas.

Se verificaron Visual Studio Community 2026, la carga de trabajo ManagedGame, Visual Studio Tools for Unity y el paquete de integración Unity 2.0.26. No se instalaron extensiones adicionales: la integración necesaria ya está presente. No se modificó configuración global del equipo ni se añadieron servicios externos.

Validación realizada: inspección estática de archivos; versiones cotejadas con manifest/lock; lectura de todo el contenido del GDD y sus partes complementarias; revisión de escena y ausencia de código propio. Se capturaron hashes SHA-256 de los 60 archivos originales relevantes para comprobar que no se alteren. La búsqueda de errores C# en el log histórico no encontró coincidencias; hay avisos históricos de licencia/puerto cuya vigencia no está confirmada.

Validación documental completada: 303 párrafos con texto y dos tablas cubiertos en la copia de lectura; hashes SHA-256 de los 60 archivos originales idénticos; 26 tareas con todos sus campos, estados y dependencias válidos; documentos UTF-8 y exclusiones comprobados. PRE-03 completada. No se abrió Unity, no se compiló ni se probó Play Mode/build; no se declara verificada ninguna funcionalidad jugable.

Git local inicializado en esta raíz con identidad existente y sin cambiar configuración global; PRE-04 en curso hasta registrar y comprobar el commit inicial. Siguiente paso: completar Git y presentar el plan para su aprobación antes de BAS-01 y de cualquier implementación.

## 2026-10-28 — Cierre de preparación y verificación Git

Tarea PRE-04 completada: commit base `7e009d8` con 64 archivos fuente/documentales. Se verificaron exclusiones de Library, Logs, UserSettings y proyectos C# generados; estado limpio después del commit y ningún remoto configurado. Se utilizó la identidad Git existente. La escritura del índice y del commit requirió autorización del entorno, concedida para las operaciones locales.

La revisión de whitespace del estado inicial señala espacios ya existentes en YAML de Unity; se conservaron para respetar los originales. La revisión limitada a los siete documentos/filtros nuevos no detectó problemas. La copia textual del GDD incluye 303 párrafos con texto y dos tablas; los 60 archivos originales inventariados conservan sus hashes.

Se actualiza el checklist a 4 tareas verificadas y 22 pendientes. Se registrará este cierre en un segundo commit documental. No se cambió ningún asset, escena, paquete, script ni configuración Unity, y no se añadieron extensiones. Compilación, Play Mode y build siguen pendientes.

Siguiente actividad: PRE-05, aprobación del plan por el usuario según su prompt; después BAS-01. Ninguna mecánica está autorizada todavía.

## 2026-10-28 — Aprobación del plan e inicio de validación Unity

El usuario indicó «continua con el prompt» tras recibir el diagnóstico y el plan; PRE-05 queda aprobada. Se inicia BAS-01: importación, compilación, revisión de paquetes/referencias y entrada/salida de Play Mode con Unity 6000.5.8f1. La aprobación permite avanzar de forma incremental con la base técnica; no fija las reglas RPG, las tres clases ni otros diseños pendientes del GDD.

Archivos actualizados: checklist, plan y este registro. Resultado de ejecución Unity pendiente; no se marca BAS-01 como completada todavía.

## 2026-10-28 — BAS-01 verificada sin control de ventana

El usuario solicitó priorizar pruebas automáticas en segundo plano y autorizó continuar. Se incorporaron un lanzador PowerShell y un conjunto de pruebas de editor con EnterPlayMode/ExitPlayMode. El lanzador comprueba la versión instalada, el bloqueo real del proyecto, el código de salida y un XML nuevo con pruebas ejecutadas; no considera una ejecución sin resultados como válida.

Resultado: 3/3 pruebas aprobadas, sin omisiones, con Unity 6000.5.8f1 y salida 0. Se verificaron paquetes declarados frente a registrados, URP configurado, ausencia de scripts/referencias serializadas perdidos en SampleScene y entrada/salida real de Play Mode. Evidencia local: `ProyectoRPGRTS/Logs/Baseline-results.xml` y `Baseline.log`. No se utilizó la ventana de Unity en esta ejecución.

Durante la preparación se corrigieron referencias duplicadas en el asmdef, un nombre PackageInfo ambiguo y una API InstanceID retirada en Unity 6.5. Se adoptó EntityId y se repitió hasta compilar y pasar las pruebas. No se modificaron los paquetes para resolverlo.

Se inicia BAS-02 y la preparación de BAS-03 con una guía métrica, un generador de prefabs/escena que rechaza sobrescrituras y pruebas de dimensiones, colisiones y render. Todavía pendientes de generar y verificar. La escena original se conserva.

## 2026-10-28 — BAS-02 y BAS-03 completadas y verificadas

Unity generó seis prefabs con raíces a escala 1: cubo de 1 m, humano de 1,80 m, puerta con hueco 1,20 × 2,20 m, ventana con hueco 1 × 1,20 m, casa de 6 × 8 m y suelo de 40 × 40 m. Se crearon siete materiales URP compartidos y la escena independiente TechnicalSandbox, con calle marcada, obstáculo, paso estrecho, iluminación y cámara fija de inspección. Todos los GUID y `.meta` fueron generados por Unity. Se conserva también SceneTemplateSettings.json, añadido automáticamente por el editor al crear la escena.

El conjunto final aprobó 12/12 pruebas con salida 0 y ninguna omisión: regresión del entorno original, tamaños importados, colisiones de paso, referencias, materiales, suelo y Play Mode con render GPU. Se corrigió el SetUp de la prueba gráfica para soportar su reinvocación durante Play Mode antes de repetir el conjunto completo. La captura 1280×720 se revisó visualmente y se conservó en `Docs/Images/TechnicalSandbox.png`.

Documentación: `Docs/ESCALA_REFERENCIA.md` y `Docs/VALIDACION_BASE_TECNICA.md`, además del plan y checklist actualizados. Se registró en AGENTS.md la preferencia de validación en segundo plano y el requisito EntityId de Unity 6.5. La comparación Git no detectó cambios de contenido en SampleScene, el GDD, los paquetes ni los ajustes preexistentes.

Resultado acumulado: 8/26 tareas verificadas y 18 pendientes. La escena es un entorno técnico; no incorpora controles de cámara, selección/movimiento, navegación horneada, interacción, UI ni sistemas RPG/RTS completos. La build de integración se realizará en BAS-09. Siguiente tarea autorizada: BAS-04, cámara isométrica con desplazamiento y zoom.
