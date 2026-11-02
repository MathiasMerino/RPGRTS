# Revisión humana E18

Candidata provisional de desarrollo. Mantiene modelos/animaciones E08, escenario medieval E07 y presentación E16/E17. No es una versión comercial ni una aprobación humana del balance. Consultar E18_PROGRESION.md, E18_BALANCE.md e informe final para pruebas y limitaciones.

## Ejecutar

Extraer completo el ZIP Aethernia-E18-Progresion-Windows.zip y abrir Aethernia.exe. Nueva partida elige Guerrero, Monje o Mago, nombre y linaje provisional. Respaldar partidas antes de experimentar: crear otra partida respalda la anterior. Los lanzadores Revisar-warrior/monk/mage.cmd abren copias de recorridos E18 reales incluidos para consultar equipo, nivel y asentamiento; guardar modifica esas copias. Para comprobar el ciclo completo utilizar Nueva partida.

Unity: abrir ProyectoRPGRTS con6000.5.8f1, escena Assets/01_Scenes/AetherniaProgressionDemo.unity, Play. URP17.5.0 declarada, sin instalaciones adicionales. No ejecutar generadores sobre assets existentes. Escenas y builds anteriores conservadas.

## Recorrido propuesto

1. Elegir clase y avanzar/saltar prólogo. En primer RPG acercarse, atacar/guardar/recuperar, terminar turno. Ganar entrega12XP/nivel2. Abrir ficha y gastar el punto en Impacto o Disciplina; estudiar sus efectos y el foco de la técnica.
2. Explorar con WASD y E cerca de objetos. Cofre inicial aporta equipo/10oro/4madera. Capitán al norte inicia patrulla RTS; seleccionar héroe/aliados, ordenar movimiento/ataque con clic derecho. Recuperar/guardar/técnica requieren héroe seleccionado; debe sobrevivir. Una carga ciega puede perder y no garantiza victoria.
3. Victoria de patrulla permite asentarse. Administrar campamento: centro15oro/8madera, taller10/5, activar producción, cuartel12/8. Producción consume alimento; pausar taller permite reservarlo. No existe obligación de coronarse ni abandonar exploración.
4. Conversar con habitante y aceptar El cargamento perdido. Recuperar arcón tras combate y volver a cobrar20oro/8XP. El cofre y misión financian compañeros: espada6/luchador5/arco7oro, cada uno2alimento.
5. Grupo y expediciones: incorporar los tres roles, probar reserva/reingreso y seguimiento. Descansar en campamento recupera hasta4PV por miembro/10s. Entrada de ruinas al oeste(4,−11); E junto a guardianías, objetivo y salida. Elegir objetivos, aprovechar alcance del arquero, técnica del héroe y recuperación limitada. El reloj estratégico se pausa en combate.
6. Dos guardianías otorganXP al héroe y4XP a cada compañero participante; nivel2 de compañero a8XP mejora máximos/daño sin curar. Objetivo entrega30oro/15madera/6alimento y botín único mineral/talismán; si inventario está lleno, el botín queda pendiente. Abrir Inventario y Recibir botín cuando haya dos espacios.
7. Volver al campamento: mejorar taller a2 por10oro/5madera; forjar arma propia por8oro/mineral1. Equipar arma/talismán, comparar PV máximos/daño sin curación gratuita. Chaleco8oro/4madera; vender objetos sin equipar o5madera→5oro financia mejoras. No se pueden equipar materiales ni armas exclusivas de otra clase.
8. Construir/activar granja8oro/5madera. Reclutar ejército desde cuartel, hasta6 independientes del grupo3. Entrenar una unidad cuesta8oro/3madera y requiere taller2; mismo soldado/identidad, +2PV máximos/+1daño sin curar. Preparar selección en pestañaEjército.
9. Territorios: marchar al paso, reclamar con E; marchar al puesto y disputar conquistaRTS. Paso/puesto asegurados producen1madera/ciclo cada uno. No conceden nuevos recursos al recargar. Compañeros RPG no se consumen como ejército.
10. Guardar, volver al título, cerrar proceso y Continuar. Revisar nivel/XP/rangos/equipo/inventario/salud/grupo/reservas/edificios/ejército/territorios y recursos. Objetivos únicos, XP y botín no deben repetirse. No hay producción fuera del juego.

## Comprobaciones humanas pendientes

Probar cada clase a720p/1080p/1440p, legibilidad y cierre×/Escape de ficha/inventario/grupo. Revisar anuncios de nivel y elección, comprensión de costes, distinción entre unidades y tácticas, dificultad, tiempos y diversión. Meta de primera sesión25–45min para alguien aprendiendo: no está medida por QA automática, no añadir esperas artificiales. Anotar clase, técnica elegida, encuentros/reintentos, recursos faltantes y tiempo real.

Sin arte final, historias de compañeros, equipo individual de compañeros, gran árbol de talentos ni múltiples dificultades. Sabiduría/Carisma del GDD siguen sin pruebas sociales propias. Incidencia histórica E17-A-RTS documentada: una orden automática de carga/reserva no demuestra victoria universal; consultar XML y trazas. Aprobación E18 humana pendiente y E19 no iniciada.

## Repetir QA

Cerrar el editor antes de Tools/Invoke-UnityValidation.ps1 -Mode Vertical. BuildProgression genera únicamente la build E18. Tools/Invoke-ProgressionSmoke.ps1 -WithGraphics -Record crea slots aislados y recorre Nueva partida→conquista con las tres clases, después abre procesos nuevos paraContinuar. Sin -WithGraphics usa player sin GPU. La grabación usa paso fijo12Hz y audioDSP real; no mideFPS ni duración humana. Consultar informe para resultados efectivamente ejecutados.
