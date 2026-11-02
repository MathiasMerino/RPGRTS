# E18 — progresión y ciclo jugable

Encargo71c1a4f1 autoriza esta etapa; base E17d7cacd7/PR8draft, rama feature/e18-progresion-balance. GDD0.2 originalhashdba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661 conserva reglas conceptuales; números de demo provisionales, no reglas definitivas. Sin subagentes/arte nuevo obligatorio/Meshy/publicación privada/merges/main/E19.

Secuencia: A investigación RTS; BC XP/niveles/atributos; DEF técnicas y elecciones compartidas; GH catálogo/botín; IJK compañeros/economía/ejército; LN balance/IA; MOQ integraciónUI/persistencia; RSTUV QA/build/evidencia/PR. Humano separado.

Auditoría inicial: solo perfilesHP/daño/alcance/velocidad; sin XP/atributos/niveles del héroe. Ataque/guardia/recuperación existentes, no duplicarlos. Inventario8unidades con espada/túnica/ración, equipamiento2slots. Compañeros3/ejército6, mejoras edificiosnivel2 y dos encuentros de ruinas únicos ya existen. Reutilizar y ampliar de forma optativa en escena/rules E18, conservar escenas y parámetros históricos para regresión.

A pendiente: E17reprodujo héroeHP0/aliados5/enemigos0 con derrota correcta; no evento perdido. Carga no garantiza supervivencia y el héroe persigue objetivos automáticamente si no recibe otras órdenes. Verificar también reserva trasera mediante órdenes legales, sin otorgar salud/victoria. Prueba histórica conserva expectativas/estrategia.

Propuesta técnica de demo a concretar tras A: progresión acotada4niveles y curva ajustable; primer combate habilita nivel2. Seis atributos coherentes con GDD, diferenciando bases/clase/equipo/nivel sin sumar al cargar. Una técnica por clase con foco limitado por encuentro, mejoras pequeñas de impacto o disciplina y adaptadores por modo. Catálogo pequeño y botín de primera finalización útil para asentamiento/equipo; no repetir objetivos para XP. Guardado aditivo/migración segura con ledger de recompensas. Nuevas reglas se mantienen separadas de la configuración de prototipos históricos.

Duración humana25–45min es meta de diseño, no medición automática. No añadir esperas para simular duración. Balance/UX/modelos finales/aprobación humana pendientes.

## Parámetros provisionales acotados

Curva acumulada propuesta: niveles1/2/3/4 a0/12/32/60XP, tope4; primer RPG12, patrulla/campamento8, misión cargamento8, encuentros ruinas10cadauno, objetivo12, conquista8. Solo hechos reales y ledger único persistente; ningún XP por UI/esperar/repetir objetivos únicos. Migración puede reconocer una sola vez hechos antiguos válidos como crédito de progresión ganado, claramente documentado, sin fabricar combates ni objetos.

Atributos propuestos: bases10; primaria14(FuerzaGuerrero/DestrezaMonje/InteligenciaMago), secundaria12(ConstituciónGuerrero/SabiduríaMonje/Mago). Primaria+2alnivel3. El perfil de clase ya define salud base y daño; niveles añaden3PVGuerrero/2PVotros por nivel y+1daño alnivel3, primaria determina bonificador de ataque(+2base,+3nivel3); Monje gana1defensa alnivel3. Equipo/guardia se aplican por separado, sin acumulación ni curación gratuita al cambiar/equipar/cargar.

Técnica por clase a nivel2, foco2por encuentro: Golpe poderoso(+3daño), Golpe concentrado(+1daño/+4precisión RPG; bonificación moderada equivalente enRTS), Proyectil arcano(+2daño/alcance8m). Una acciónRPG o intervaloRTS, objetivo hostil vivo, alcance/línea válidos y foco1. Mejoras pequeñas de Impacto(+2daño técnico/rango) o Disciplina(+2PV/+1guardia/rango), hasta2rangos; punto por nivel tras1, requisitos2/3. Sin respec gratis, cambios de clase o equipo durante encuentros. Reutilizar ataque/guardia/recuperación y animación/VFX E08–E09.

Equipo/botín propuestos: mantener espada/tela/ración; añadir arma avanzada por clase(+2daño), chaleco reforzado(+2defensaRPG, mitigaciónRTSacotada), talismán(+2PV), mineral de forja. Recompensa única de ruinas: mineral+talismán, entrega pendiente segura si inventario8está lleno. Forjar arma requiere taller2,mineral1,8oro; chaleco8oro/4madera. Valores sujetos a balance y pruebas; nada definitivo ni cientos de ítems.

Compañeros:3rolesexistentes, experiencia de guardianías hasta nivel2(+2PV/+1daño), salud/identidad/reserva persistentes; no heredar árbol del héroe. Ejército6independiente: entrenamiento pagado de recluta a rango2(+2PV/+1daño), requiere cuartel+taller2,8oro/3madera; sin reemplazar identidad. Usar estadísticas de rol configuradas y revisar costes sin igualar funciones.

Reglas nuevas optativas mediante configuración E18, con snapshots derivados inmutables en combate. GuardadoVersion1 conserva compatibilidad estructural con revisión aditiva de progresión; ausencias antiguas reciben valores seguros. Validar nivel/XP/ledger/rangos/equipo/salud y transacciones sin duplicación. Revisar partidas antiguas explícitamente.

## A — investigación ejecutada

Siete escenarios6/7, XMLRTS_A_6_7.xml. La expectativa histórica de victoria se conserva; contratos de resolución/selección/inicialización/reintento siguen correctos. La táctica nueva de reserva también puede perder (Monje), por lo que reservar al héroe no se presenta como garantía ni corrección. Trazas terminales y estados reales en RTS_A_TRACES.json. No se identificó carrera/evento perdido para corregir; dependencia de geometría/temporización/órdenes y balance sigue abierta. El escenario exploratorio conserva su aserción y fallo: no esconderlo ni declarar regresión completa si vuelve a fallar. Se continúa BC como autoriza E18, sin cambiar reglas para forzar victoria.

## BC — base verificada

Escena AetherniaProgressionDemo y RootAssets/14_Progression, reglas compartidas optativas. Curva/fuentes/derivados descritos ya integrados, migración con crédito único por hechos válidos y ledger deXP. Datos guardadosVersion1aditivos, no reset ni curación al cargar.7/7progresión PASS tras arreglar carpeta de captura (18/19primer pasecon12reglas tácticas legacy PASS). Victoria real primer RPG/level2/aprenderDisciplina/save/Continue por tres clases, física igualE17y sin privados. Técnica tiene datos pero aún no ejecución/UI: DEF/MOQ pendientes.

## DEF — técnicas verificadas

Técnicas: power-strike/Golpe poderoso, focused-strike/Golpe concentrado, arcane-bolt/Proyectil arcano. Datos/costes/alcances descritos integrados; foco2repuesto por encuentro, coste1yunaacciónRPG o cooldown3sRTS con intervalo de ataque. MonjeRTS añade2daño en lugar de precisión de tirada, porque RTS no usa d20. Guardias/acciones existentes conservadas. Impacto y Disciplina dos alternativas hasta2rangos con1punto por niveltras1. Estado compartido y elecciones guardadas, sin respec gratuito.

Ficha en ventanaPersonaje existente, requisitos/costes/estado/efectos/atributos/XP/puntos; mejora con botón real, sin segunda interfaz global. HeaderNv, técnica en pie RPGy panelRTS.4pruebas técnicas+12legacy en lote, controlador1/1tresclasesRPG/RTS/persistencia yUI1/1tresclasesa720/1080. Se conservan fallos de auxiliares (referencia preContinue y validador de menú usado en HUD) corregidos sin modificar efectos/aserciones. Capturas reales en Logs/E18/Captures/Skill-*.

## GH — equipo y botín verificados

Catálogo9tipos: tres antiguos, tres armas por clase(+2), chaleco(+2defRPG,1mitigaciónRTScondañomínimo1antesguardia), talismán(+2PVsinheal), mineral. Equipamiento/clase/categoría/bonos/guardado validados, referencias visuales anteriores reutilizadas (no modelo definitivo). Recompensa ruinas registra botín pendiente; dos espacios requeridos, entrega total o conserva pendiente, una sola vez incluso trasload. Migración reconoce primera finalización E17 para ese botín nuevo y créditosXP, no repiteoro/objetivos antiguos.

Forja requieretaller2,8oro,mineral1; chaleco requieretaller,8oro/4madera. Venta de objetos sin equipar y venta5madera→5oro en campamento; comprar2maderacuesta5oro, por lo que no hay arbitraje de compra/venta. Fuente de riqueza territorial/producción ahora permite financiar equipo.14/14core,1/1comercio y1/1UIintegrada por tresclases(6PNG720/1080), fases de fondos y caption acortada conservadas en XML. PruebasUI usan partidas completas E17 reales conSHA/origen: no son una nueva mazmorra E18 ni fondos sintéticos. El ciclo E18real se hará en validación final.

## IJK — implementación en validación

Roles E18 compartidos por RPG/RTS en datos separados de los históricos: espada12PV/2daño/1,5m/2,5m·s/6oro; luchador10/3/1,3/3,2/5oro; arquero10/2/6/2,3/7oro. Todos2alimento. Compañeros4XP por guardianía en la que realmente participaron,8XP→nivel2(+2PVmáximos/+1daño), sin curación ni crédito retroactivo de guardianías E17. Ledger por encuentro e identidad impide repetir. Ejército entrenado con8oro/3madera, cuartel+taller2; no gana XP de compañeros, conserva identidad/salud. Tres acompañantes y seis reclutas independientes. Guardado añade Level/Experience con normalización segura de ausencias antiguas; efectos derivados en grupo/RTS/descanso y validación de HP/XP. UI de entrenamiento en pestañaEjército, costes por rol en reclutamiento. No equipo individual para compañeros: usan el arquetipo compartido, no el inventario del héroe.
