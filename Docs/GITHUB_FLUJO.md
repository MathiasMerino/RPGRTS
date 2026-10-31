# Conexión de GitHub y flujo de trabajo

Fecha: 2026-10-31, America/La_Paz. Repositorio: https://github.com/MathiasMerino/RPGRTS. Raíz Git local: `C:/Users/Mathy/Desktop/PROYECTO GAME`; Unity permanece en `ProyectoRPGRTS/`.

El encargo autoriza publicar la base actual `cacad54` y esta documentación en `main` y `dev`. Los 14 commits existentes forman una sola cadena y se conservan con sus identificadores; no se reinicializa, reescribe o fuerza Git. Las ramas históricas `master`/`codex/*` y los checkpoints se mantienen localmente; se publican únicamente las ramas necesarias `main`/`dev`.

## Revisión previa

- Estado inicial limpio. Sin remoto configurado; `git ls-remote --symref` del destino devuelve cero referencias y GitHub confirma tamaño 0. Remoto público, cuenta autenticada con permiso de escritura; no se cambió su visibilidad.
- Revisados los 757 blobs alcanzables por todo el historial: 682 versiones de texto examinadas con patrones de claves/tokens, claves privadas y credenciales literales. Sin candidatos detectados. Es una revisión por patrones, no una garantía absoluta de ausencia de secretos.
- Sin blobs de 10 MiB o más; el mayor tiene 3.003.405 bytes. No se necesita Git LFS ni migración del historial en esta conexión.
- `.gitignore` excluye cachés, builds, logs, IDE, `.env` y certificados; se añaden extensiones de claves/keystores. No hay rutas generadas o sensibles detectadas entre los archivos versionados. Assets, `.meta`, Packages y ProjectSettings se conservan.
- Git Credential Manager proporciona autenticación existente, sin escribir ni mostrar tokens. No se instala otra herramienta ni se cambia configuración global.

## Ramas y PR

`feature/*`, `fix/*`, `test/*` → PR a `dev` → PR de versión aprobada a `main`.

`main` es la rama estable y predeterminada de GitHub; `dev` queda activa localmente. Las tareas futuras parten de `dev` y crean solo una rama temporal cuando haga falta. Las PR describen cambios, validación real y pendientes; los merges conservan commits. La publicación inicial autorizada no declara aprobados el arte final o la evaluación humana F2-07.

Ejemplo para una tarea futura, con árbol limpio:

```powershell
git switch dev
git pull --ff-only
git switch -c feature/descripcion
# Implementar, validar y registrar cambios.
git push -u origin feature/descripcion
# Abrir PR hacia dev; una versión aprobada se propone mediante dev → main.
```

Antes de incorporar una versión a `main`, obtener aprobación del usuario. La norma se documenta en AGENTS.md; esta tarea no añade reglas de protección ni cambia permisos de GitHub. No usar push forzado, squash/rebase sobre el historial publicado o borrados destructivos como procedimiento de sincronización. Si el avance no admite fast-forward, revisar la divergencia y usar un merge normal cuando corresponda.

## Comprobación de la conexión

La entrega exige que `main`, `dev`, `origin/main` y `origin/dev` apunten al mismo commit; que los 14 identificadores originales sean antecesores de ambas ramas; que `HEAD` remoto sea `main`; y que `dev` termine activa, con upstream `origin/dev` y árbol limpio. Comprobar también los archivos publicados mediante la API de GitHub y una descarga independiente. Evidencia local en `ProyectoRPGRTS/Logs/GitHub-*`.

La primera publicación de `cacad54` fue comprobada: GitHub devolvió las mismas 615 rutas/blob SHA del árbol local, `main` y `dev` idénticas y `main` predeterminada. Una clonación independiente desde GitHub conserva los 14 identificadores originales y supera `git fsck --full --no-reflogs`. Esta documentación y el ajuste de ignore se añaden mediante un commit normal, con avance de `main` por fast-forward desde `dev`; la comprobación final de ambas ramas/árbol y estado limpio se registra en GitHub-final-verification.json. No se publica ninguna rama histórica adicional.

No se modifica jugabilidad, escenas, modelos, paquetes o ajustes Unity. No se repiten las 52 pruebas/build de C02: su resultado es histórico; la validación nueva corresponde a Git, archivos y publicación.
