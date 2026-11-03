# E19 — identidad y culturas provisionales

Propuesta jugable ajustable, no canon del GDD ni Biblia Artística aprobada. Catálogo compartido `Assets/15_Civilizations/Civilizations.asset`. Se activa en AetherniaCivilizationsDemo; escenas E01–E18 conservan sus datos. Una cultura puede incorporar cualquier especie; la raza no determina moral, oficio ni lealtad. Todas las12combinaciones Guerrero/Monje/Mago son válidas.

| Especie | Rasgo aplicado al héroe | Proporción de modelo temporal | Clases |
|---|---|---|---|
| Humano | +2 PV máximos |1,00×1,00×1,00|Las tres|
| Elfo |+0,2m/s|0,94×1,04×0,94|Las tres|
| Enano |+1 mitigación al usar guardia|1,12×0,84×1,10|Las tres|
| Orco |+1 daño de técnica, disponible desde nivel2|1,10×1,02×1,08|Las tres|

Se deriva un perfil desde clase/progresión/equipo, sin mutar assets ni sumar bonos guardados. IdentityRevision1 identifica nuevas partidas E19; los legados permanecen en0, conservan identidad y no reciben bonificación ni curación inesperada al cargar. La preferencia cultural se elige en campamento; cambia nuevas obras, no las existentes. Una cultura heredada desconocida se conserva como texto en revisión0, sin efectos; identidades nuevas inválidas se rechazan conservando el archivo. Los acompañantes antiguos sin identidad explícita se muestran como legado, sin inventarles raza.

| Familia de diseño | Cultura provisional | Estado | Arquitectura / paleta | Economía y militar |
|---|---|---|---|---|
| Tradiciones humanas |Frontera|Piloto|Madera/piedra, marrón sobrio|Almacén +20capacidad/nivel; Guardia de frontera|
| Tradiciones élficas |Bosque|Solo datos|Madera/vegetación, verde apagado|Tres roles preparados; no reclutables|
| Tradiciones enanas |Forja|Piloto|Piedra/hierro, gris|Taller +1madera/nivel/ciclo; Choque de forja|
| Tradiciones orcas |Clanes|Solo datos|Madera/cuero/hierro, ocre oscuro|Tres roles preparados; no reclutables|

Las familias indican inspiración de diseño, no restricciones raciales. No sustituyen regiones oficiales ni crean reinos definitivos.

| Unidad piloto |PV|Daño|Alcance|Velocidad|Coste|Requisitos|
|---|---:|---:|---:|---:|---|---|
|Guardia de frontera|16|2|1,6m|2,2m/s|8oro/3alimento|Campamento, cuartel, almacén de Frontera|
|Choque de forja|12|4|1,2m|2m/s|9oro/3alimento|Campamento, cuartel, taller de Forja y puesto conquistado o pacto|

IA/selección/órdenes/daño reutilizan RTS; no son doce controladores nuevos. El guardia prioriza resistencia mediante vida; choque duplica daño base del guardia a cambio de vida/alcance/velocidad. Entrenamiento E18 nivel2 cuesta8oro/3madera con cuartel/taller2: +2PV máximos/+1daño sin curación. La guarnición E19 de Forja despliega espada/choque/arquero/espada desde formación configurable; las patrullas tutoriales y escenas anteriores conservan sus tres roles originales. Tropas no reciben de nuevo rasgos de héroe; su perfil militar ya define estadísticas. Tres acompañantes RPG siguen separados de seis militares RTS.

Obras: mismo coste/huella/navegación E13; CultureId se guarda por edificio. Almacén Frontera pasa100→120capacidad/nivel, base200; taller Forja produce2madera a nivel1 y4 a nivel2 consumiendo1alimento por madera. Una granja nivel1 produce2alimento/ciclo; una forja2 puede consumir reservas, por lo que pausar/gestionar alimento es útil. Producción solo online, ciclo10s pausado en combate. Territorios asegurados mantienen producción existente y propietario persistente.

Política provisional: Compañía multicultural del jugador; Liga de Frontera/A y Guarnición de Forja/B conservan IDs históricos. A aliada inicialmente, B hostil; un pacto en campamento cuesta8oro/3alimento, requiere centro y no se ofrece después de conquistar el puesto. El pacto conserva dueño, bloquea ataque aliado y habilita acceso local; conquistar exige victoria real y abre ese mismo acceso, cambia propietario y conserva consecuencia de reputación E15. Un puesto neutral no se convierte en enemigo por especie.

Arte: modelos Generic E08, materiales/arquitectura E07 y retratos existentes compartidos. Las variaciones escalan solo presentación, sin modificar actor, colisión ni rig. Personaje base≈1,78m; enano visual≈1,50m, elfo≈1,85m, orco≈1,82m. Cápsula de navegación conservadora≈1,8m para todas las variantes: no se promete paso por huecos bajos según anatomía. Edificios conservan huella3×3m; variaciones son dos prefabs derivados con pequeña señal original, no arquitectura cultural definitiva.

Sustitución futura: mantener pivote a suelo, metros, ejes del pipeline E17, separar prefab visual/actor/equipamiento. Entregar modelo y skin en pose de referencia, probar siete estados Generic o retarget Humanoid aprobado; nunca afirmar Avatar Humanoid donde no existe. Reemplazar las referencias del catálogo visual, validar anclajes, envolvente de animación, escala y12combinaciones; conservar fallback público. No se integra Meshy privado, ni nuevas animaciones finales o criaturas.

Validación humana de siluetas, ritmo, legibilidad, diversión y balance pendiente. Las tablas de simulación/QA no equivalen a experiencia humana.
