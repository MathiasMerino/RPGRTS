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


UX-01 fue autorizada expresamente después de E19: rama feature/ux01-onboarding-camera-world desde3e53508, base del [PR11draft](https://github.com/MathiasMerino/RPGRTS/pull/11) feature/e19-razas-civilizaciones (PR10). Orden: PR1→…→PR10→UX01; no hay fusiones automáticas ni cambios main/dev. Su comprobación independiente no sustituye la revisión pendiente de los antecedentes.

UX-02 autorizada posteriormente: rama feature/ux02-world-exploration desde d56fa59 (UX01), [PR12draft](https://github.com/MathiasMerino/RPGRTS/pull/12) sobre feature/ux01-onboarding-camera-world. Orden de revisión: PR1→…→PR10/E19→PR11/UX01→PR12/UX02. Sin fusiones ni retarget automático; main/dev se mantienen. La nueva candidata conserva los antecedentes y no convierte sus limitaciones o evaluaciones humanas en aprobaciones.


E20 se apila en [PR #13](https://github.com/MathiasMerino/RPGRTS/pull/13), borrador con cabeza `feature/e20-tactical-warfare` y base `feature/ux02-world-exploration` (PR #12, ca95205). Se verificaron los doce antecedentes abiertos en borrador. Revisión secuencial PR1→…→PR12→PR13. E20 conserva la condición de candidata técnica y requiere revisión humana.


E20.1 está en [PR #14](https://github.com/MathiasMerino/RPGRTS/pull/14), borrador con cabeza `fix/e20-1-alpha-stabilization` y base `feature/e20-tactical-warfare` (PR #13, c89801c). El orden hacia `dev` es PR1→PR2→PR3→PR4→PR5→PR6→PR7→PR8→PR9→PR10/E19→PR11/UX‑01→PR12/UX‑02→PR13/E20→PR14/E20.1. Para cada uno, revisar la diferencia contra su base y sus pruebas. Tras aprobación explícita e integración del antecedente, comprobar su historia en `dev`, ajustar la base del siguiente y repetir la verificación del diff. Si el método de integración aplana commits, preparar un plan de historial con el usuario antes de cambiar la pila. La promoción `dev` → `main` requiere revisión y aprobación propias.


E21 añade el [PR #15](https://github.com/MathiasMerino/RPGRTS/pull/15), borrador desde `feature/e21-first-campaign` sobre `fix/e20-1-alpha-stabilization` (PR #14, 4ab7b4f). La revisión hacia dev continúa en el orden #1→#13→#14→#15, conservando las dependencias de la tabla. Tras aprobar e integrar un antecedente, revisar la historia y el diff del siguiente antes de cambiar su base hacia dev. La aprobación de la campaña y la promoción a main requieren su propia revisión.

## E22 · Verificación de la pila

Consulta de GitHub previa al desarrollo. Los quince PR estaban abiertos en borrador. `dev` apuntaba a `742c04f` y `main` a `5a2e3ce`. E21 (`78695c1`) no era ancestro de `dev`. La cabeza de E22 es `feature/e22-living-world` y su base es `feature/e21-first-campaign`.

| PR | Cabeza | Base | Dependencia |
|---|---|---|---|
| #10 | feature/e19-razas-civilizaciones | feature/e18-progresion-balance | #9 |
| #11 | feature/ux01-onboarding-camera-world | feature/e19-razas-civilizaciones | #10 |
| #12 | feature/ux02-world-exploration | feature/ux01-onboarding-camera-world | #11 |
| #13 | feature/e20-tactical-warfare | feature/ux02-world-exploration | #12 |
| #14 | fix/e20-1-alpha-stabilization | feature/e20-tactical-warfare | #13 |
| #15 | feature/e21-first-campaign | fix/e20-1-alpha-stabilization | #14 |
| E22 | feature/e22-living-world | feature/e21-first-campaign | #15 |

Revisar hacia `dev` en orden #1 → #2 → #3 → #4 → #5 → #6 → #7 → #8 → #9 → #10 → #11 → #12 → #13 → #14 → #15 → E22. Después de cada integración aprobada, comprobar historia y diff efectivo del siguiente. Las escenas y GUID compartidos pueden causar conflictos de Unity, y aplanar commits puede volver a incluir cambios de antecedentes. Revisar tamaño, licencias, rutas privadas y build al avanzar por la pila.

Antes de aprobar E22, revisar regresión de escenas anteriores, arranque Windows, guardados de E21, cobro único, inventario y fondos de comercio, los tres cierres, la patrulla histórica y la técnica RTS. La evaluación humana sigue siendo necesaria para interfaz, ritmo y balance.
