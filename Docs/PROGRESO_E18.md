# E18 — punto de recuperación

Código, validación y publicación terminados; [PR#9draft](https://github.com/MathiasMerino/RPGRTS/pull/9) abierto sobre E17, sin merge. No iniciar E19 ni subagentes. Informe Docs/ENTREGA_E18.md; guía Docs/GUIA_E18.md y criterios en CHECKLIST.

- Rama feature/e18-progresion-balance desdeE17d7cacd7. Fuente finalae0051b27c6ae946625ccc1d69697e7f4bd33e0e; core de los seis procesos iniciales60d01b8136453b8b1b28b8c19af1ea79807406ec. Ocho commits por bloques y documentación/evidencia posteriores, sin reescribir historial.
- General174/176 en REGRESION_GENERAL.xml; regresión visual custombinding corregida y4/4dirigidas finales PASS. E18RtsScenarioTests.ThreeClassesWithLegalBacklineOrdersAt30 perdióGuerrero; E17-A-RTS sigue abierto. No afirmar regresión totalmente verde ni debilitar/excluir aserción.
- Ocho procesos Windows PASS: seis Nueva partida→conquista/reinicio por tresclases y dos delZIPextraído(finalae0051b). Runs221506 y230250 en Logs/ProgressionQA. Orígenes separados en WINDOWS.json/CICLO_NATIVO.json, sinHP/fondos inyectados.
-49PNG y2MP4 reales con audioDSP exportados; no FPS ni duración humana medidos. Simulación720casos separada de geometría/jugabilidad.
- ZIP Aethernia-E18-Progresion-Windows.zip87.842.896bytes, SHA bc6828e2fe78c0f3f1115aa020ab7a5cc4913cd81b15dcd71aef3a8963d5d72f; CRC/perfileSHA PASS,18ZIPprevios intactos. No repetir empaquetado ni sobrescribir archivos previos.
- Últimos cambios: visualizar solo atributo principal aplicado, descansoE18incluye ejército superviviente para alcanzar máximo entrenado, fallback visual desconocido conserva bindings sin admitir ítems inválidos. Cuatro dirigidas y portable validan estos cambios; no quedó código pendiente de compilar.
- SOURCE_AUDIT/GITHUB_PRE_PUBLICACION verifican GDD/versiones/escenas previas y main/dev/PR1..8; privadosMeshy/LocalOnly ignorados, treshashes originales intactos. No integrar/publicar esos recursos ni hacer merges/force/configglobal.
- RSTUVcerrado como entrega técnica de candidata con incidenciaRTSabierta. PR#9draft publicado; último paso operativo: pushnormal de esta documentación y verificación final limpio/upstream/main/dev/PR1..8. No queda código o artefacto por crear. Humano sigue pendiente.
- Evaluación humana de duración25–45min/diversión/arte/×Escape no realizada. Hoja Docs/Validacion/E18/REVISION_HUMANA.md sin completar.

El detalle histórico de desarrollo y fallos intermedios está conservado en Git, REGISTRO_CAMBIOS.md y XML de Validacion/E18; no rehacer auditorías ni generadores.
