# E19 — candidata de razas y culturas piloto

**En validación final.** Fuente de juego/build1e67333, Unity6000.5.8f1/URP17.5.0. Rama feature/e19-razas-civilizaciones sobre E18/PR9 más organización5507425. Arte, rasgos, nombres culturales y reglas nuevos son propuestas; GDD original0.2 preservado. No se encontró Biblia Artística aprobada accesible. Sin E20 ni merges.

| Entregable solicitado | Estado real / referencia |
|---|---|
|1. Estado E19|Cuatro razas y dos culturas piloto implementadas; cierre general/portable/Git en curso.|
|2. Problema RTS|Investigado y reproducido con Monje en la prueba histórica a30FPS:0PV, recuperación única, arqueros8/11 alcanzan reserva. Guerrero ganó; Mago no llegó a ejecutarse en ese caso abortado. No corrupción/resolución duplicada demostrada. Incidencia abierta, aserciones conservadas. [Diagnóstico](Validacion/E19/A_DIAGNOSTICO.json).|
|3. Razas|Humano+2PV, Elfo+0,2m/s, Enano+1mitigación al defender, Orco+1daño de técnica desde nivel2. Configurables, aplicados solo al perfil derivado del héroe.|
|4. Combinaciones|Las12combinaciones con Guerrero/Monje/Mago; creación real y guardado dirigido de todas. Dos circuitos íntegros: Humano–Guerrero y Enano–Mago. No se afirma recorrido completo de las otras diez.|
|5. Culturas|Frontera y Forja funcionales; Bosque/Clanes solo datos. Inspiración humana/élfica/enana/orca sin exclusividad racial. [Diseño y datos](E19_RAZAS_CULTURAS.md).|
|6. Facciones|Tres IDs históricos: Compañía multicultural, Liga de Frontera/A y Guarnición de Forja/B. Aliado/Neutral/Hostil configurables, pacto persistente y ataque aliado bloqueado. Sin economía autónoma de IA ni diplomacia continental.|
|7. Unidades|Guardia de frontera y Choque de forja reclutables y desplegables; otros diez planes de datos no se anuncian jugables.|
|8. Táctica|Guardia16PV/2daño/1,6m/2,2m/s frente choque12PV/4daño/1,2m/2m/s. Golpes reales2/4verificados; guarnición Forja usa choque en formación configurable. Tutoriales y escenas anteriores conservados.|
|9. Edificios|Dos prefabs derivados E07: almacén Frontera(+20capacidad/nivel), taller Forja(+1madera/nivel/ciclo, consume alimento por madera). Misma huella3×3m y materiales compartidos.|
|10. Reclutamiento|8oro/3alimento y9oro/3alimento, cuartel y obra cultural correspondiente; Forja exige puesto o pacto. Máximo6militares, independiente del grupo3. No depende de raza del héroe.|
|11. Economía|Producción/almacén, venta de madera y costes existentes; forja2 puede consumir más alimento que granja1. Sin recursos inyectados en recorridos nativos ni producción offline.|
|12. Conquista|Victoria real cambia dueño y acceso a Forja, produce madera territorial y consecuencia reputacional configurable. Pacto conserva dueño y abre acceso sin beneficio de conquista.|
|13. Progresión|Mismo héroe, habilidades/foco, nivel/XP y equipo E18 en exploración/RPG/RTS; tres acompañantes con identidad y progresión, ejército separado.|
|14. Guardado|IdentityRevision, especie/cultura por recluta y cultura por edificio; perfiles calculados sin acumular bonificaciones. Cuatro procesos Windows verificaron guardado/reinicio exacto y recompensas únicas.|
|15. Legados|Carga E13–E18 sin inventar culturas de compañeros ni aplicar nuevos bonos/curación. Cultura heredada desconocida en revisión0 conservada; identidad nueva inválida rechazada sin reescribir archivo. E19 requiere su escena/catálogo para volver a cargar.|
|16. Pruebas|21/21dirigidas,3600duelos de reglas, dos PlayMode integrados y4/4procesos nativos. Regresión general y portable pendientes de añadir al cierre. Véase tabla inferior.|
|17. Capturas|Creación12combinaciones, UI cultural720/1080 y capturas reales de dos circuitos Windows; selección, ficha, grupo, unidades, edificios, combate, conquista y control. Manifiesto pendiente de cierre.|
|18. Vídeos|Dos clips de cámara Game Windows y audioDSP original: RPG y última patrulla cultural. Paso fijo12Hz, no FPS medidos. Exportación/decodificación en curso.|
|19. Build|Windowsx64 nueva en Builds/CivilizationsE19Windows; ZIP E19 y extracción verificándose, sin sobrescribir19ZIP previos.|
|20. Rama|feature/e19-razas-civilizaciones; main/dev sin modificaciones autorizadas.|
|21. Commits|1be0be1 auditoría;1b461b6 identidad/pilotos;2eedfff21pruebas/balance; a10829a reclutar Construcción/Forja;805eff9 navegación QA;1e67333 reposición pagada opcional en QA. Historial anterior conservado.|
|22. Pull Request|Pendiente de publicar al terminar verificaciones; base E18/PR9.|
|23. Integración|[Plan PR1..9](INTEGRACION_PRS.md): revisión secuencial, retarget posterior con aprobación; no fusiones automáticas ni ampliación ciega de la pila.|
|24. Checklist|[CHECKLIST](../CHECKLIST.md), historial intacto; E19-HUM separado y pendiente.|
|25. Pendientes|Aprobación humana, estabilidad/expectativa RTS histórica, arte racial/cultural final, dos culturas restantes, economía autónoma de facciones, futuras élites/comandantes. No son funciones terminadas.|
|26. Guía humana|[Guía Unity/Windows](GUIA_E19.md) y [hoja de revisión pendiente](Validacion/E19/REVISION_HUMANA.md).|

## Evidencia y límites

| Ejecución | Resultado / alcance |
|---|---|
|A baseline|1/2: visual histórica PASS; victoria RTS de reserva falla con Monje. XML/diagnóstico conservados.|
|Inicial identidad/UI|17/18: glifo no soportado; corregido sin ignorar aviso. Repetición18/18.|
|Pilotos reales|1/1PlayMode: perfiles pagados,2/4daño real, UI720/1080. Fixture controlado de campamento, no campaña desde cero.|
|Dirigidas finales|21/21:19casos de reglas/archivos/assets y2casos PlayMode. Incluyen12combinaciones y3600duelos semillados; no sumarlos como3600pruebas NUnit distintas. [XML posterior](Validacion/E19/DIRIGIDAS_POST_RECLUTAMIENTO_21.xml).|
|Windows final1e67333|4/4: dos partidas desde cero y dos reinicios. [Comprobantes](Validacion/E19/CICLO_NATIVO.json). Ambos pilotos golpearon en ambos ejércitos y sobrevivieron en estos recorridos finales.|
|Regresión completa|EN EJECUCIÓN, sin filtro. No se declara verde con resultados anteriores.|
|Portable|PENDIENTE de ejecutar desde extracción CRC/SHA verificada.|
|Humana|PENDIENTE; no hay medición de sesión humana/diversión ni aprobación artística.|

Intentos anteriores se conservan: CS1061 en llamada del test de simulación, corregida a UseTechnique; Windows rechazó reclutar desde Construcción (corregido y comprobado); runner intentó compañeros desde pestaña Culturas (se añadió navegación real); una victoria cultural perdió Forja y el runner suponía supervivencia (se añadió regreso/reposición pagada sin resucitar IDs). No se usan esas partidas parciales como procesos completos aprobados. La rama de reposición no se ejecutó en los dos recorridos finales porque sus pilotos sobrevivieron; el comportamiento de bajas sí quedó observado en el intento conservado.

No se perfiló FPS/GPU/GC de E19. El consumo máximo de memoria muestreado y las duraciones de automatización son contexto de QA, no una prueba de rendimiento ni tiempo humano. [Balance](E19_BALANCE.md) separa simulación de combate real.

Modelos/rigs Generic E08 compartidos, proporciones visuales temporales y dos variantes de prefabs. Actor/colisión/navegación permanecen con medidas conservadoras; enano visual≈1,50m y cápsula≈1,8m. No hay nuevos modelos raciales finales, animaciones definitivas, Avatar Humanoid o recursos Meshy publicados. [Pipeline de sustitución](E17_PIPELINE_PERSONAJES.md).

El ajuste de QualitySettings.asset existía antes de E19 y se conserva sin incluirlo en commits; la build usa ese ajuste local, identificado por SHA en BASELINE/PRESERVACION. No se presenta como árbol Git completamente limpio. Registros y cachés originales se conservan localmente y la evidencia seleccionada se publica por separado.

Siguiente actividad recomendada: revisión humana de esta candidata, análisis de la expectativa histórica RTS y aprobación/integración ordenada de la pila de PR. No iniciar E20 automáticamente.
