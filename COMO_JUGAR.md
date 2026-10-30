# Crónicas de Aethernia Prototipo técnico

Este prototipo implementa el recorrido jugable y los sistemas mínimos acordados para comprobar la visión del GDD. Las reglas y los recursos artísticos son provisionales; la hoja de ruta de expansión está en `Docs/PLAN_EXPANSION.md`.

## Abrir el juego

Ejecutar `ProyectoRPGRTS/Builds/Windows/Aethernia.exe`. Mantener junto al ejecutable todos los archivos de la carpeta Windows. Para editarlo, abrir `ProyectoRPGRTS/` en Unity 6000.5.8f1 y cargar `Assets/01_Scenes/PlayablePrototype.unity`.

También se entrega `ProyectoRPGRTS/Builds/Aethernia-Prototipo-Windows.zip`: descomprimir todo y abrir Aethernia.exe. Al iniciar se crea una partida de ensayo nueva; cargar una partida previa requiere F9 o Cargar. No hay guardado automático.

## Controles

| Acción | Control |
| --- | --- |
| Seleccionar héroe, unidad RTS u objeto | Clic izquierdo |
| Caminar, ordenar un grupo o colocar un taller | Clic derecho en el terreno |
| Desplazar la cámara | WASD, flechas o arrastrar con botón central |
| Acercar/alejar | Rueda del ratón |
| Activar un objeto seleccionado cercano | E o botón Interactuar |
| Guardar/cargar fuera de un encuentro | F5/F9 o botones correspondientes |

Los paneles y la escritura del nombre bloquean los controles del mundo. El nombre y la clase se eligen en la interfaz; cambiar clase durante esta prueba restablece la salud. Es una facilidad del prototipo, no una regla definitiva de progresión.

## Recorrido recomendado

1. Elegir Guerrero, Mago o Monje y escribir un nombre. Seleccionar el personaje azul y caminar con clic derecho.
2. Seleccionar el cofre marrón cercano, acercarse y pulsar E. Entrega equipo y recursos una sola vez. Equipar espada y túnica desde el panel.
3. Probar Comercio: 5 oro por 2 madera. El mercado de la Facción A también permite esa operación al interactuar cerca.
4. Entrar en Construir y hacer clic derecho en terreno despejado. Cada taller cuesta 10 oro y 5 madera; su huella es 3 × 3 m. No se permite solapar objetos ni colocar sobre el héroe. Consume alimento y produce madera cada 10 segundos simulados.
5. Volver a Explorar. Buscar el portal gris a la izquierda del asentamiento, seleccionarlo y activarlo cerca. En la mazmorra, activar el hito marrón: la Facción A gana +10 de reputación y B cambia -5. El portal verde permite regresar.
6. Guardar, cambiar una condición de prueba y cargar para comprobar que se restauran personaje, recursos, equipo, construcciones y consecuencias.
7. Probar Combate RPG: la iniciativa determina quién actúa primero. Usar Acercarse o clic derecho para el movimiento limitado, luego Atacar, Guardia o Recuperar y finalizar turno. Mago tiene alcance mayor; Monje obtiene bonificación de iniciativa. Recuperar se usa una vez por encuentro. Si hay derrota, volver a Explorar y elegir clase para reiniciar la prueba.
8. Tras una victoria RPG, Escalar a RTS prueba una transición opcional: los seis aliados empiezan con 14 de salud en lugar de 10. Batalla RTS desde la barra superior inicia una prueba independiente de seis contra seis.
9. En RTS, seleccionar una unidad o Todo el grupo y dar órdenes con clic derecho. Avanzar ordena una formación corta; los soldados atacan automáticamente a los enemigos próximos. Al concluir, volver a Explorar y guardar la consecuencia territorial.

El calendario económico se pausa durante RPG y RTS, según la política provisional aprobada. Los encuentros deben terminar antes de cambiar de actividad o guardar; la interacción de exploración se desactiva durante combate.

## Repetir la validación en segundo plano

Con Unity cerrado sobre este proyecto, ejecutar desde PowerShell en esta carpeta:

```powershell
.\Tools\Invoke-UnityValidation.ps1 -Mode Vertical
.\Tools\Invoke-UnityValidation.ps1 -Mode Build
.\Tools\Invoke-PrototypeSmoke.ps1
```

El primer comando prueba editor, geometría, UI, navegación, persistencia y sistemas integrados; también genera capturas con GPU en modo batch. El segundo compila la build de Windows. El tercero inicia esa build sin ventana, recorre los sistemas automáticamente y escribe un comprobante en Logs. Las pruebas no utilizan la partida guardada del jugador.

No repetir GenerateSandbox, GeneratePlayable o UpgradeVertical sobre assets ya generados: las herramientas rechazan sobrescrituras. Las escenas de muestra y referencia se conservan como evidencia de las etapas anteriores.

## Alcance y siguientes fases

El asentamiento, mazmorra, unidades, facciones y edificios usan geometría de prueba y nombres técnicos. No incluyen el continente completo, narrativa final, todas las clases, compañeros, diplomacia avanzada, asedios, arte final ni una edición comercial terminada. La evaluación humana de diversión, accesibilidad y equilibrio sigue siendo necesaria antes de convertir las hipótesis del prototipo en decisiones finales.

Consultar `CHECKLIST.md`, `Docs/ESPECIFICACION_PROTOTIPO.md`, `Docs/VALIDACION_PROTOTIPO.md` y `REGISTRO_CAMBIOS.md` para alcance aprobado, resultados e historial.
