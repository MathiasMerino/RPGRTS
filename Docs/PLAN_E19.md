# Plan E19 â€” propuesta tÃ©cnica autorizada

Base: E18/PR9 mÃ¡s organizaciÃ³n5507425. GDD0.2 verificado por SHA en Validacion/E19/BASELINE.json; original preservado. No se encontrÃ³ una Biblia ArtÃ­stica aprobada accesible. DIRECCION_ARTISTICA.md rige el arte provisional. El GDD propone linajes y culturas sin fijar estas cuatro razas ni rasgos; la lista procede del encargo E19. Cultura, especie y facciÃ³n son ejes independientes.

Secuencia: A investigar RTS sin debilitar pruebas; BCD catÃ¡logo racial, UI y guardado; EFGIJ dos pilotos culturales; HKLMN integraciÃ³n con sistemas existentes; OPQR pruebas y balance; ST recorridos/build; U PR e integraciÃ³n. Tareas/aceptaciÃ³n en CHECKLIST.md. Todos los cambios de juego nuevos se activan con catÃ¡logo E19; las escenas anteriores conservan su configuraciÃ³n.

DiseÃ±o provisional: humano +2 vida mÃ¡xima; elfo +0,2m/s; enano +1 reducciÃ³n al defender; orco +1 daÃ±o de tÃ©cnica. Sin restricciones de clase. Se calculan como perfil derivado una vez, sin incrementar valores guardados. Los legados no reciben identidad inventada ni curaciÃ³n al cargar. Modelos E08 compartidos con variaciones proporcionales de presentaciÃ³n, sin nuevos rigs finales.

Cuatro familias: frontera, bosque, forja y clanes. Pilotos frontera y forja: guardia defensiva de frontera y combatiente de choque de forja, con costes/vida/daÃ±o/alcance/velocidad y requisitos distintos. Otros diez perfiles son datos preparados y no se anuncian jugables. AlmacÃ©n de frontera aporta capacidad extra; taller de forja aporta producciÃ³n extra con coste alimentario. Cultura de cada edificio/recluta se conserva aunque cambie la preferencia del campamento. El acceso militar depende de edificios/control/relaciÃ³n y recursos, nunca de raza del hÃ©roe.

Se reutilizan grupo RPG3, ejÃ©rcito RTS6, progresiÃ³n/habilidades E18, tres facciones/territorios E15 y ventanas existentes. Relaciones simples y conquista afectan acceso local, sin diplomacia continental. Los valores se validarÃ¡n con transacciones, combate y dos recorridos Ã­ntegros de combinaciones distintas; simulaciones no sustituyen experiencia humana.

Se preservan QualitySettings.asset preexistente, nueve escenas previas, GDD, recursos privados y19ZIP. Sin merges/main/dev/force push, sin E20. El informe distinguirÃ¡ pruebas dirigidas, regresiÃ³n general, procesos nativos, capturas y evaluaciÃ³n humana pendiente.

## A — resultado dirigido
La prueba histórica de reserva a30FPS volvió a fallar con Monje (Guerrero ganó). El registro termina con héroe0PV, recuperación aceptada una vez y derrota real; los dos tiradores restantes pueden alcanzar al héroe retirado. La orden se renueva cada0,6s y bloquea persecución por OrderedUntil: no hay evidencia de que ignore movimiento. La posición de reserva no garantiza victoria. No se debilita ni elimina la aserción histórica y no se fuerza un cambio de balance para pintarla verde. Regresión visual histórica pasa. Incidencia abierta, separada de identidad cultural; nuevas pruebas validarán resultados y consecuencias reales. Evidencia A_BASELINE.xml/A_DIAGNOSTICO.json.
