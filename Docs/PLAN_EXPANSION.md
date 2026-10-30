# Hoja de ruta posterior al prototipo

Fecha: 2026-10-29. Entregable FUT-01: planificación de la expansión, no implementación del continente completo. Dependencia: cerrar VER-10 con build, resultados de pruebas, capturas y limitaciones del prototipo. Las prioridades siguientes se revisarán con pruebas humanas y mediciones.

Estado: hoja de ruta aprobada por el usuario mediante «Aprobar la hoja de ruta». No autoriza el inicio automático de producción de las fases futuras.

| Orden | Trabajo propuesto | Dependencias | Criterio de salida |
| --- | --- | --- | --- |
| 1 | Evaluar la base con jugadores: controles, lectura de escenarios, combate, UI y transición opcional | VER-10, EXP-01 | Registrar problemas reproducibles y decidir conservar/modificar/descartar cada hipótesis |
| 2 | Aprobar reglas RPG propias: razas, atributos, niveles, magia, estados y clases; ampliar Guerrero/Mago/Monje primero | Evaluación inicial | Especificación aprobada con ejemplos, balance de referencia y pruebas de reglas; sin copiar reglamentos completos |
| 3 | Diseñar inventario, equipo, fabricación y progresión definitivos | Reglas RPG | Datos versionados, límites y transacciones comprobados; migración desde el prototipo definida |
| 4 | Definir compañeros, creación personalizada/de origen y narrativa | Reglas RPG, aprobación de personajes y trama | Contenido aprobado, contratos de diálogos/misiones y pruebas de consecuencias; no escribir ahora historia definitiva |
| 5 | Diseñar mapa continental, regiones, viajes y calendario compartido | Resultados de tiempo/guardado del prototipo | Política de pausas y simulación de regiones inactivas aprobada; prototipo medido de carga y persistencia |
| 6 | Ampliar facciones, diplomacia, negocios, caravanas, mercados y propiedad | Mundo/tiempo, decisiones de economía | Reglas de oferta/demanda y gobierno aprobadas, sin recursos duplicados ni simulación individual innecesaria |
| 7 | Ampliar construcción, trabajadores y producción | Economía y propiedad | Catálogo aprobado, colocación y navegación dinámicas verificadas, costes medidos |
| 8 | Ampliar RTS: reclutamiento, formaciones, tipos de unidad, comandantes, moral, suministro y asedios | Prototipo RTS y mediciones de multitudes | Volumen objetivo y presupuesto de CPU/memoria aprobados; pruebas comparables antes de optimizar |
| 9 | Sustituir geometría provisional por arte/audio propios, mantener escala y accesibilidad | Dirección artística aprobada y presupuestos de plataforma | Lectura, rendimiento, licencias de recursos y UX comprobados en hardware objetivo |
| 10 | Preparar producción y distribución | Sistemas y contenido anteriores | Guardados migrables, QA completo, empaquetado, plataformas y alcance comercial aprobados |

## Límites de la planificación

No hay calendario ni presupuesto comercial aprobado. La evidencia técnica del prototipo se obtiene en este PC Windows y no define requisitos mínimos de otros equipos. Antes de cada fase se crearán tareas más pequeñas con los mismos campos del checklist: ID, sistema, prioridad, dependencias, aceptación, archivos, pruebas y resultado.

La transición RPG–RTS seguirá siendo opcional y experimental hasta evaluar su jugabilidad con personas. Su funcionamiento técnico, el transporte de un resultado y la pausa del calendario pueden probarse automáticamente; la diversión, claridad subjetiva y equilibrio requieren evaluación humana.

Las propuestas de regiones, culturas, trama, personajes y relaciones del GDD conservan su estado provisional. Este plan no las convierte en decisiones finales. No se inicia la expansión como consecuencia de aprobar esta hoja de ruta; primero se revisan los resultados del prototipo y se elige una fase concreta.
