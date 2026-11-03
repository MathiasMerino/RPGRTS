# Balance E19 — simulación y límites

Propuesta provisional.3600duelos de reglas puras:12combinaciones×3niveles×100semillas1..100. Enemigo18PV/daño3/ataque+2, equipo de entrenamiento+1/+1, distancia1m, recuperación bajo mitad de PV, guardia cada cuarta ronda y técnica si está disponible. No simula navegación, animación, RTS, grupo, tiempo humano ni rendimiento. Los niveles son fixtures de simulación, no partidas inventadas para demostrar progresión. CSV íntegro y prueba dirigida conservados.

|Raza|Clase|Nv1 victorias/100|Nv2|Nv4|
|---|---|---:|---:|---:|
|human|warrior|98|100|100|
|human|monk|97|100|100|
|human|mage|100|100|100|
|elf|warrior|98|100|100|
|elf|monk|99|100|100|
|elf|mage|100|100|100|
|dwarf|warrior|99|100|100|
|dwarf|monk|97|100|100|
|dwarf|mage|100|100|100|
|orc|warrior|98|100|100|
|orc|monk|99|100|100|
|orc|mage|100|100|100|

Los resultados altos reflejan un encuentro tutorial con techo de dificultad, no invulnerabilidad. Las diferencias de clase/progresión E18 siguen siendo mayores que los rasgos raciales pequeños. Velocidad élfica no tiene efecto en un duelo a distancia fija; se verifica en agente/movimiento real y no se le atribuye una ventaja ficticia en esta tabla. Guardia enana solo reduce impactos defendidos; ímpetu orco requiere técnica desbloqueada y foco limitado; humano no se cura al subir/cargar.

Pilotos militares:16PV/2daño/1,6m/2,2m/s/8oro frente12PV/4daño/1,2m/2m/s/9oro; ambos3alimento. La prueba PILOTOS_RTS_UI.xml comprobó golpes con2y4daño mediante el controlador RTS real. No extrapolar estos duelos a balance competitivo,6v6 o todas las composiciones. El guardia soporta más daño por oro, choque aporta más daño próximo por oro; ambos tienen requisitos económicos y comparten entrenamiento E18.

Economía: almacén Frontera +20capacidad/nivel, taller Forja +1madera/nivel/ciclo consumiendo alimento. La forja nivel2 puede agotar una granja nivel1; pausar producción es una decisión útil. Pacto abre acceso sin producir el beneficio territorial de conquistar. No hay reembolso ni multiplicación por cambiar preferencia cultural.

Pendiente: sesión humana prolongada, claridad de elecciones, coste de oportunidad, frecuencia de uso de técnicas y balance de grupos/RTS a distintas temporizaciones. La incidencia histórica RTS de estrategia de reserva se conserva abierta. No se modificaron sus aserciones para acomodar nuevas facciones.
