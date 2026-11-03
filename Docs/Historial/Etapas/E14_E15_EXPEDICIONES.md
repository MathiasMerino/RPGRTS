# E14–E15: expediciones y conquista

Base c33e34d/PR5 draft sin fusionar. Rama feature/e14-e15-expediciones-conquista; main/dev y PR1..5 intactos. GDD0.2/hash dba82... coincide; §§3,6,8 y distinciones conceptuales consultadas. Encargo actual autoriza implementar secuencialmente, no requiere volver a pedir aprobación del plan técnico. No E16. No subagentes.

## Decisiones provisionales

- Mismo héroe/estado/catálogo/perfiles. Tres compañeros únicos (espadachín/arquero/luchador) con RecruitRecord reutilizado, identidad propia y bandera de participación; hasta3activos y héroe (4). Retirar deja al compañero en reserva, no lo borra. No se convierten en soldados RTS ni se duplican con ejército.
- Reclutar compañeros en campamento por coste E13 existente5oro/2alimento; sin exigir nuevo recurso. Recuperación en campamento mediante descanso10s de tiempojugado, hasta4PV por miembro; caminar cancela. Es una regla provisional explícita para evitar muerte permanente/softlock; no restaura salud al viajar ni durante las pruebas de combate. Héroe conserva raciones/reglas actuales.
- Combate grupal como ampliación de TurnBattle/TurnEncounter; perfil adaptado de roles existentes, misma tirada d20+2 vs12 y mismos ayudantes daño/guardia/recuperación. Orden individual por iniciativa; jugador controla aliados y enemigos usan IA compatible. Una acción y movimiento4m por turno; enemigos se aproximan3m. Sin bonos raciales nuevos ni cifras alteradas para ganar pruebas.
- Ruinas del Umbral, nombre no canónico: interior compacto en región existente de mazmorra, acceso/salida por interacción y NavMesh; dos encuentros de dos guardianes, objetivo y alijo opcional. Recompensa única propuesta30oro/15madera/6alimento al completar objetivo y5oro opcionales; fact existente comparte inventario/recursos. Sin recompensa por entrar/reiniciar/cargar. La vida no se restaura al viajar; reagrupamiento valida posiciones.
- Facciones: catálogo liga organización/relaciones a reputación A/B existente, sin inferir raza/cultura. Compañía del jugador provisional y voluntaria, cultura/reino vacíos; no coronación. Territorios campamento, paso neutral y puesto enemigo. IDs/propietarios persistentes; propietario cambia solo tras victoria real.
- Conquista usa RtsBattleController con héroe y reclutas E13 elegidos, sin seis aliados gratuitos de patrulla. Ejército mantiene capacidad E13(6), independiente de3compañeros. Puesto con3defensores de perfiles existentes. Recompensa territorial: +1madera por ciclo10s online al controlar paso/puesto (máximo dos), con capacidad E13; sin producción offline ni reinicio de ciclo al cargar. Paso neutral se reclama pacíficamente cerca si tiene acceso; puesto por batalla real.
- Derrota conserva puesto enemigo; bajas RTS siguen regla E13; no borrado de partidas ni reino. Retirada segura al exterior con salud real; recuperación mediante recursos existentes en campamento. Datos nuevos aditivosv1 con normalización explícita, sin inventar objetos nulos por JsonUtility.

## Secuencia

E14-A estado/validación/reclutamiento; E14-B seguimiento y combate grupal; E14-C mazmorra/recompensa/UI e integración; E15-A facciones/territorios/producción; E15-B ejército seleccionado y conquista; QA focales/regresión/nativeWindows/reinicio/capturas/build/ZIP/PRdraftapiladohaciaE10–E13.

## Auditoría inicial (histórico)

Se comprobaron PR1..5 open/draft y main/dev sin cambios; snapshot Logs/E14/github-before.json. El usuario autorizó restaurar las tres texturas E07 eliminadas localmente: restauradas exactamente desdeGit. Meshy local preservado, sin integrar ni publicar. Se avanzó por datos, grupo, mazmorra, territorio y validación; los fallos intermedios se conservan como evidencia. Resultados finales y límites en ENTREGA_E14_E15.md, sin equivaler a aprobación humana.

## Integración observada

- E14: prueba Play Mode de dos guardianías aprobada; mismos objetos de héroe/grupo, IDs/HP delegados al estado, sin almacén de salud paralelo. El seguimiento solo opera al explorar y se suspende en combate; RTS oculta compañeros. Regroup valida NavMesh antes de transición. Reservas persisten sin objetos duplicados.
- Territorios: campamento conectado al paso, paso al puesto. El reclamo neutral requiere interacción cercana; conquistar requiere acceso por el paso y ejército preparado. RtsBattleController conserva motor/acciones, elimina aliados gratuitos de patrulla para esta conquista y despliega exclusivamente IDs seleccionados. La patrulla E12 permanece diferenciada por ContextId.
- Economía: recompensa única → construcción/reclutamiento E13 → ejército → conquista → producción WorldOperations.Advance. El ciclo persistente impide duplicación/offline. Salud, equipo, oro, madera, alimento y reputación pertenecen a PrototypeState.
- Escala: héroe1.8m, cápsula/radio existentes; ruinas24×16m, paredes3m y vanos centrales4m; columnas0.6×3m. Recorte frontal visual0.65m, collider3m conservado; suelo UV cada2m con material compartido. Cámara fija isométrica URP; ninguna textura/material externo nuevo. Ruinas, nombres y cifras siguen siendo provisionales.
- Datos: formato aditivov1, listas ausentes normalizadas; HasDemoProgress conserva migración anterior. IDs únicos32hex, máximos3compañeros/6soldados/3territorios, roles/propietarios conocidos y pertenencia de selección validados. Guardar/cargar durante combate rechazado. Modelos y efectos comparten catálogo/presentación y no deciden daño.
- Fallos conservados como evidencia: navegación detenida por destinos ocupados y primer timeout de grupo; corregidos antes del pase E14_INTEGRACION.xml. No se modificó salud/daño para lograr victorias. Fixtures unitarias y de derrota están separadas de recorridos Windows con partidas E13 auténticas.
