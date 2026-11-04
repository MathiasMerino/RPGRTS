# UX-02 — Guía de revisión humana

Candidata técnica provisional; evaluación humana pendiente. Rama `feature/ux02-world-exploration`, PR12 sobre UX01/PR11; para revisar desde GitHub hay que usar esa rama, sin fusionar la cadena automáticamente. Abre `ProyectoRPGRTS/Assets/01_Scenes/AetherniaUx02World.unity` en Unity6000.5.8f1 y pulsa Play, o ejecuta `Builds/Ux02Windows/Aethernia.exe`. Inicia en el menú, con Guerrero/Mago/Monje y las cuatro razas existentes. No uses los generadores de autoría para arrancar el juego.

## Controles

WASD mueve al héroe durante exploración; rueda ajusta zoom acotado. Flechas/botón central desplazan la cámara; tras una pausa corta vuelve a seguir al héroe. E usa el único objetivo próximo con indicador discreto; paredes bloquean la interacción. Esc cierra el diálogo; Ayuda y Menú conservan las ventanas previas. F5/F9 guardan/cargan en estados compatibles. P pausa. La partida personal usa su ubicación habitual; las pruebas automáticas emplean slots aislados.

En RTS: clic izquierdo selecciona, Shift añade y arrastrar selecciona un área. Clic sobre suelo vacío deselecciona. Clic derecho ordena mover en formación o atacar a un enemigo válido. Ctrl+1/2/3 memoriza grupos;1/2/3 los recupera. Selecciona al héroe para Guardia, Recuperar y técnica cuando esté desbloqueada. La preparación no causa daño; la primera orden de ataque válida tras los hitos activa al enemigo.

## Recorrido para revisar

1. Nueva partida, raza y clase, introducción omisible. Camina por la aldea segura; revisa tamaño/animación del héroe, suavidad y zoom.
2. Habla con el habitante para aceptar El cargamento perdido. El objetivo y su marcador cambian al arcón existente. Recuperarlo inicia el RPG por turnos; selecciona rival, acércate y usa las acciones legales. Tras vencer, regresa a entregar la carga; recompensa única.
3. Recorre el camino occidental, observa al guardia y lee la señal del cruce. Prueba el desvío al alijo, la madera y la entrada de las ruinas. Los hallazgos persisten; caminar no inicia batallas. Las ruinas mantienen el requisito de campamento asegurado.
4. Habla con el capitán al oeste y elige Preparar patrulla. Selecciona un aliado, luego varios; ordénales llegar a ambos hitos. Puedes colocar tropas y observar el puesto sin recibir daño. Guardar/volver al título/Continuar conserva formación e hitos.
5. Da clic derecho sobre un rival para atacar. Comprueba separación, alcances, indicadores, acciones del héroe y resultado real. Puede haber derrota; Reintentar es explícito. No se garantiza victoria con una estrategia de reserva.
6. Tras la victoria, construye centro y taller en espacio libre, activa producción y recluta mediante el cuartel existente. La economía comparte la bolsa con comercio y exploración.
7. Visita al comerciante, consulta el almacén y pide las provisiones en su puerta. Esa puerta representa una entrega, no un interior jugable. El enlace del campamento incorpora un compañero mediante costes y límites existentes; actívalo en el panel de grupo.
8. Vuelve por el sendero a las ruinas. E entra en la mazmorra anterior; los encuentros requieren interacción. Usa su salida para regresar al acceso del bosque. Salud/grupo/inventario se conservan. Compañeros, culturas, conquista y mundo anteriores siguen disponibles.
9. Guarda, vuelve al título y continúa. Comprueba posición, misión, hallazgos, recursos, edificios, ejército y compañeros sin duplicar premios.
10. Repite ventanas y diálogos a1280×720,1920×1080 y2560×1440. Revisa techos/árboles/muros, comodidad de ratón/arrastre/Shift y legibilidad de terreno/órdenes.

## Registrar después

Indica clase/raza, paso y resultado observado. Señala si la ruta se entiende sin leer ayudas, si el héroe se mantiene visible, si el RTS responde como esperas y si la dificultad resulta razonable. Las comprobaciones automatizadas y capturas no sustituyen esta revisión. Consulta ENTREGA_UX02.md para resultados, build exacta y problemas conservados.

Durante la revisión visual, comprueba el borde inferior al cambiar de objetivo o modalidad. La captura automática del cruce muestra un control antiguo de interacción parcialmente recortado. Registra si aparece durante el juego manual.
