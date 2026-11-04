# Integración de la pila para la alfa

La revisión inicial encontró los PR #1 a #15 abiertos en borrador. Todas las bases comprobadas son ancestros de sus cabezas. La rama estable apuntaba a `5a2e3ce` y la de integración a `742c04f`. La instantánea registra commits, solapamientos, escenas, archivos .meta y el estado que informó GitHub para cada PR. [Instantánea](Validacion/E23/GITHUB_BASE.json)

| PR | Base | Cambio principal | Prueba y acción antes de integrar |
|---|---|---|---|
| #1 | dev | Personajes y presentación inicial | Revisar importación, escala y escena vertical. |
| #2 | #1 | Aldea E07 | Revisar materiales, NavMesh y GUID de escena. |
| #3 | #2 | Modelos articulados | Probar locomoción y control de avatares. |
| #4 | #3 | Combate RPG e IA | Revisar acciones, turnos y prueba táctica. |
| #5 | #4 | Primera demo integrada | Verificar entrada, menú, transición y guardado. |
| #6 | #5 | Expediciones y conquista | Revisar grupo, botín, terreno y estado territorial. |
| #7 | #6 | Arte y UI E16 | Verificar calidad, controles y paneles. |
| #8 | #7 | Candidata E17 | Revisar integración artística y advertencias RTS históricas. |
| #9 | #8 | Progresión E18 | Revisar economía, experiencia y compatibilidad. |
| #10 | #9 | Razas E19 | Revisar identidad, culturas y regresión RTS. GitHub informó estado de conflicto indeterminado en esta consulta. |
| #11 | #10 | UX‑01 | Revisar arranque, controles y diferencias entre pruebas dirigidas y generales. |
| #12 | #11 | UX‑02 | Verificar rutas, oclusión, interacción y preparación. |
| #13 | #12 | Guerra E20 | Revisar órdenes y repertorio RTS. |
| #14 | #13 | Estabilización E20.1 | Repetir la patrulla histórica con cadencias representativas. |
| #15 | #14 | Campaña E21 | Revisar decisiones, ruinas y cierre. |
| [#16](https://github.com/MathiasMerino/RPGRTS/pull/16) | #15 | Mundo libre, borrador | Revisar 230/230 y los recorridos Windows. |
| [#17](https://github.com/MathiasMerino/RPGRTS/pull/17) | #16 | Alfa y consolidación, borrador | Revisar pruebas, ZIP y usabilidad. |

Los solapamientos afectan UI, controladores y escenas. Al aprobar cada antecedente, comprueba su ascendencia en `dev` y el diff efectivo del siguiente PR antes de actualizar su destino. Confirma referencias y GUID en Unity si un cambio toca escenas o prefabs. La rama E23 contiene ya toda la pila por ascendencia y permite probar la combinación. La integración a `dev` y la promoción a `main` requieren su propia aprobación.
