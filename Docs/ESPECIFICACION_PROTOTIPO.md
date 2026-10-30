# Especificación del prototipo técnico

Fecha: 2026-10-29. Alcance: demostrar los sistemas del GDD v0.2 con contenido provisional y datos ajustables. Estas cifras y nombres técnicos no fijan las reglas definitivas, la historia ni el equilibrio comercial.

Estado: aprobado por el usuario mediante «Aprobar como reglas provisionales». La aprobación cubre este prototipo técnico; las decisiones definitivas de producción siguen abiertas.

## Decisiones confirmadas

El usuario autorizó completar el trabajo en segundo plano y eligió Guerrero, Mago y Monje como tres clases iniciales. Se conserva Unity 6000.5.8f1, URP 17.5.0, 1 unidad = 1 metro, entrada de PC y uGUI reutilizable. Las escenas originales se conservan; el prototipo se desarrolla en PlayablePrototype.unity.

## Parámetros propuestos para probar

| Clase | Salud | Daño base | Alcance | Velocidad |
| --- | --- | --- | --- | --- |
| Guerrero | 24 | 6 | 1,8 m | 3,2 m/s |
| Mago | 16 | 8 | 6 m | 3,2 m/s |
| Monje | 20 | 5 | 1,8 m | 4 m/s |

Creación básica: elegir clase y nombre; representación provisional a escala. Catálogo de tres clases, sin inventar razas, subclases ni personajes de origen definitivos.

Combate RPG de ensayo: un héroe y un enemigo, turnos alternos, una acción y hasta 4 m de movimiento por turno. Ataque con d20 + 2 frente a defensa 12; daño según configuración. Guardia reduce el siguiente daño a la mitad. Una recuperación de 4 puntos por encuentro permite comprobar recursos/estado. Enemigo de prueba con 18 de salud y 3 de daño. Semilla controlable para pruebas, aleatoria durante el juego. No reproduce reglas completas de D&D.

Inventario mínimo: espada de entrenamiento (+1 daño), túnica de prueba (+1 defensa) y ración (+4 salud). Capacidad 8 unidades. Equipamiento limitado a arma y armadura. El cofre de ensayo entrega espada y túnica una sola vez, junto con 10 oro y 4 madera. Las transacciones y el guardado no deben duplicar objetos.

La iniciativa se tira con d20 para ambos participantes; Monje recibe +2. Si el rival gana la iniciativa, realiza su primera acción antes del turno del jugador. Los siguientes turnos alternan movimiento y acción. Se mantiene el carácter de datos de ensayo aprobados, sin establecer una progresión RPG definitiva.

Economía de ensayo: recursos iniciales 50 oro, 20 madera y 10 alimentos. Una operación comercial intercambia 5 oro por 2 madera; un ciclo de producción de 10 segundos simulados consume 1 alimento y produce 1 madera por taller. Sin precios continentales ni diplomacia avanzada.

Construcción: un taller de prueba, huella 3 × 3 m, coste 10 oro y 5 madera. Colocación dentro del área de ensayo, en terreno libre, alejada al menos 2 m del héroe y sin solapar estructuras/obstáculos. Collider y obstáculo de navegación tallado. Construcciones persistentes; no hay demolición ni reembolso automático.

Tiempo: el reloj simulado avanza solo durante exploración/construcción. Se pausa durante combate RPG y RTS en este prototipo; el resultado de las batallas se integra como un hecho persistente. Esta política es una hipótesis explícita para probar coherencia, no la sincronización definitiva del GDD.

## Contenido acotado y consecuencias

Una región técnica con un asentamiento representado por la casa de referencia y un recinto de mazmorra. Dos facciones llamadas Facción A y Facción B hasta aprobar identidades. Un objetivo de prueba, sin trama: activar el hito de la mazmorra. Su resultado incrementa reputación con A, modifica relación con B y queda registrado al guardar/cargar. Es contenido de verificación, sin actos ni narrativa final.

Guerra RTS: una batalla de 6 unidades contra 6, salud y daño militares simplificados, selección individual y de grupo, órdenes de movimiento y ataque automático cercano. El resultado modifica un hecho territorial. Se medirán errores y comportamiento en esta escala; no se promete rendimiento para ejércitos multitudinarios. Formaciones, moral, asedio y abastecimiento completos corresponden a expansión.

Transición experimental: un botón de escalada permite comprobar que un resultado RPG se convierte en una ventaja de la batalla RTS. Prototipo aislado y opcional, sin imponer transiciones a todos los encuentros. Su evaluación final de diversión necesita pruebas humanas.

## Persistencia y aceptación

Guardado local JSON con versión, nombre/clase, posición, inventario/equipo, recursos, tiempo, construcciones, reputación y hechos. Carga de datos ausentes/incompatibles debe fallar sin alterar la partida actual. El archivo de pruebas se guarda en Logs; la partida del jugador usa una ruta propia en persistentDataPath y solo se escribe al guardar explícitamente.

La interfaz muestra el modo activo, selección, recursos, instrucciones, acciones y mensajes. Los paneles y botones reutilizan prefabs y un tema común. Se comprobarán resoluciones de 1280 × 720 y 1920 × 1080. El hardware de prueba es este PC Windows; no se fija hardware mínimo comercial sin medición.

Aceptación técnica: compilar en el editor exacto, verificar cada regla aislada, comprobar el recorrido integrado, guardar/cargar sin pérdidas, revisar render y arrancar una build local sin controlar ventanas. No declarar los sistemas finales ni el juego completo a partir del prototipo.
