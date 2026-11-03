# Parámetros de la demo E18

Reglas provisionales y reversibles, exclusivamente AetherniaProgressionDemo. Unidades Unity = metros; velocidades m/s. No altera configuraciones de escenas históricas.

| Rol compartido RPG/RTS | PV | Daño | Alcance | Velocidad | Reclutar |
|---|---:|---:|---:|---:|---|
| Espadachín |12|2|1,5|2,5|6oro/2alimento|
| Luchador |10|3|1,3|3,2|5oro/2alimento|
| Arquero |10|2|6|2,3|7oro/2alimento|

Espada resiste más; luchador paga exposición a distancia corta por movilidad/daño; arquero controla espacio, tiene menor movilidad. Compañeros ganan4XP por guardianía participada;8XP nivel2 +2PV/+1daño, sin restaurar salud. No experiencia retroactiva de partidas antiguas porque no registraban participantes. Ejército nivel2 cuesta8oro/3madera con taller2 y cuartel; mismos bonos, sin XP de compañeros. Entrenar no cura; descanso existente en campamento recupera hasta4PV/10s también al ejército superviviente en E18, por lo que el máximo ampliado puede alcanzarse. No resucita bajas militares eliminadas. No inventario individual: equipo visual del arquetipo.

Héroes base: Guerrero24PV/6daño, Monje20/5, Mago16/8. Nivel1..4 requiere0/12/32/60XP. Salud crece3/2/2 por nivel, primaria14→16 en nivel3 añade precisión+2→+3 y daño+1. Destreza16 del Monje añade1defensa. Armadura, arma, guardia y talismán se aplican por separado y se recalculan al cargar. La salud crece según parámetros de clase; no se presenta Constitución como bonificador calculado. La ficha muestra únicamente el atributo principal que determina ataque/daño y los valores derivados efectivos. Los demás nombres del GDD se conservan como referencias de diseño pendientes de otras aplicaciones, sin números decorativos en ficha. No se implementa un reglamento completo de mesa.

Técnicas nivel2: Guerrero+3daño, Monje+1/+4precisión RPG(+2daño RTS en lugar de tirada), Mago+2/8m; foco2, coste1/acción, recargaRTS3s e intervalo de ataque. Impacto+2daño/rango y Disciplina+2PV/+1guardia/rango permiten elegir daño o supervivencia;2rangos máximo,1punto/nivel tras1. Nada se acumula por equipar/cargar.

Guardianías: perfiles de roles con+4PV/+1daño, para que la técnica no elimine todos los objetivos de un solo impacto. IA E18 puede gastar su acción en recuperar hasta4PV una sola vez por actor al caer a mitad de salud; si no alcanza un ataque válido puede gastar guardia. Usa acciones normales, sin curación secreta ni bonificaciones ocultas. Conquista propuesta4defensores frente a ejército preparado hasta6 más héroe. La patrulla histórica mantiene sus expectativas y su incidencia abierta; no se modifica para ocultar el resultado.

Economía: centro15oro/8madera, taller10/5, granja8/5, cuartel12/8, almacén10/5. Nivel2 productivo/almacén10/5 duplica producción/capacidad de ese edificio. Tallerconsume1alimento por madera/ciclo; granja2alimento por ciclo; ciclo10s de mundo, sin progreso fuera del juego ni en combate. Almacén+100capacidad por nivel; base200. Paso y puesto aportan1madera/ciclo cada uno. Vender5madera→5oro financia equipo y entrenamiento; comprar2madera cuesta5oro, sin arbitraje. Forja8oro+mineral1+taller2; chaleco8oro/4madera+taller.

Misiones y expedición financian la elección de mejoras: cargamento20oro, cofre10oro/4madera, ruinas30oro/15madera/6alimento y botín único mineral/talismán. No todos los gastos militares/equipo caben inmediatamente sin producción/territorios/comercio. Inventario8unidades, botín pendiente si no caben ambas piezas. Guardar/cargar no concede ciclos extra ni recompensa repetida.

La sesión humana objetivo25–45min requiere evaluación humana; las pruebas automáticas no la miden. Los tiempos de acción/descanso/producción son reglas anteriores, sin esperas nuevas para inflar duración. Falta completar mediciones de escenarios E18 y build nativa antes de cerrar balance técnico.

## Simulación ejecutada

720casos,120semillas×3clases×con/sin grupo, solo reglas sin geometría ni navegación. Preparados360/360victorias; solosGuerrero89/120,Monje86/120,Mago93/120. Rondas medias preparados5,20/5,46/4,53 respectivamente; solos10,42/11,13/8,35. DatosCSV/BALANCE_SIMULADO.json en Validacion/E18; recuperación enemiga y coste de acción también verificados. Esto no es una sesión humana ni prueba de trayectoria física ni garantiza victoria RTS.
