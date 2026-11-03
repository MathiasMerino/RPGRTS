# Integración pendiente de Pull Requests

Plan revisable, sin ejecutar merges ni modificar ramas estables. Snapshot real: Validacion/E19/GITHUB_BASE.json. Los nueve PR previos están abiertos en borrador; E19 se apila por autorización expresa, no representa aprobación de la cadena.

| PR | Cabeza | Base actual | Dependencia |
|---|---|---|---|
| [#1](https://github.com/MathiasMerino/RPGRTS/pull/1) | feature/e03-e06-personajes-vertical-slice | dev | dev |
| [#2](https://github.com/MathiasMerino/RPGRTS/pull/2) | feature/e07-aldea-medieval | feature/e03-e06-personajes-vertical-slice | PR1 |
| [#3](https://github.com/MathiasMerino/RPGRTS/pull/3) | feature/e08-personajes-animados | feature/e07-aldea-medieval | PR2 |
| [#4](https://github.com/MathiasMerino/RPGRTS/pull/4) | feature/e09-combate-rpg-ia | feature/e08-personajes-animados | PR3 |
| [#5](https://github.com/MathiasMerino/RPGRTS/pull/5) | feature/e10-e13-demo-jugable | feature/e09-combate-rpg-ia | PR4 |
| [#6](https://github.com/MathiasMerino/RPGRTS/pull/6) | feature/e14-e15-expediciones-conquista | feature/e10-e13-demo-jugable | PR5 |
| [#7](https://github.com/MathiasMerino/RPGRTS/pull/7) | feature/e16-pulido-visual-ui | feature/e14-e15-expediciones-conquista | PR6 |
| [#8](https://github.com/MathiasMerino/RPGRTS/pull/8) | feature/e17-art-pipeline-alpha-polish | feature/e16-pulido-visual-ui | PR7 |
| [#9](https://github.com/MathiasMerino/RPGRTS/pull/9) | feature/e18-progresion-balance | feature/e17-art-pipeline-alpha-polish | PR8 |

[E19/PR#10](https://github.com/MathiasMerino/RPGRTS/pull/10) está abierto en borrador sobre feature/e18-progresion-balance. E19 depende de PR9 e incluye además5507425 (organización local autorizada, nunca publicado previamente en PR9). Este alcance se declara en su propio diff. No se reescribe ni publica la rama E18 para esconderlo.

Secuencia propuesta para revisión humana: revisar PR1→2→3→4→5→6→7→8→9→E19 en ese orden. Cada PR necesita inspección de diff, licencias/privados, aceptación jugable y evidencia. Antes de fusionar, actualizar contra su base y ejecutar las pruebas afectadas; si hay cambios/conflictos, verificarlos sin sobrescribir decisiones ajenas. Tras integrar un antecedente con aprobación explícita, retarget del siguiente a dev y nueva revisión de su diff efectivo. No hacer nueve merges ciegos ni squash/rebase destructivo para aplanar la historia.

Bloqueos visibles: aprobación humana pendiente en todas las candidatas; incidencia de estrategia RTS reproducida en E19-A. Las pruebas generales anteriores no se convierten en verdes por aprobar pruebas dirigidas posteriores. Resolver o aceptar expresamente estas limitaciones antes de promover dev→main. Main se reserva para versión aprobada con build identificada, regresión completa revisada, prueba humana y plan de recuperación mediante Git.

No iniciar otra fase o ampliar esta pila automáticamente. Primero revisar este inventario con el docente/usuario y acordar la integración. Este documento no otorga autorización para merges, protecciones, borrar ramas o cambiar main/dev.
