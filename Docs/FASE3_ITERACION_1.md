# Fase 3 — iteración A/B/C1

Fecha: 2026-10-30. Alcance autorizado por el prompt adjunto: auditoría visual, dirección artística y primera mejora funcional de UI. El escenario D, personajes E y ampliación de interfaz C2 se secuencian después. F2-07 conserva estado pendiente.

## Auditoría A

Git inicial limpio, commit cc9691d. Punto de control local: tag `codex/fase3-base-cc9691d`. Se verificaron XML 34/34 y comprobantes de build/player de fase 2 contra ese estado; la nueva implementación tendrá su propia regresión. GDD SHA-256 sin cambios: dba82b1c18eb51bd36ee353cd3380af108aea41fe7ec539da644f08ea76bb661; lectura consultada §§12.1–12.6. Plan, checklist, registro y arquitectura revisados por sus apartados actuales.

Captura antes revisada: Docs/Images/Fase2-Exploracion-1280.png. Problemas: nueve botones de creación/equipo/comercio/partida simultáneos; nombre visible en cualquier actividad; ayuda de exploración permanece en combate; siluetas cápsula indiferenciadas por clase; casa/suelo/decorado de prueba sin lenguaje material realista. Las dos últimas carencias pertenecen a D/E, no se ocultan con un cambio de UI.

Escena: PlayablePrototype, cámara y héroe referencian prefabs existentes; UI con paneles/botones y tema compartidos. Materiales URP simples; referencias métricas y NavMesh ya probados. No hay recursos de personajes finales en Assets/04_Sprites ni un pipeline artístico adicional configurado. Se conserva el escenario funcional.

Herramientas comprobadas: ejecutable Unity 6000.5.8f1 instalado, manifest con URP 17.5.0/uGUI 2.5.0/AI Navigation 2.0.14/Input System 1.20.0/Test Framework 1.7.0; lanzadores locales y ensamblados editor/runtime. No hay herramientas Unity/MCP expuestas por los conectores de esta sesión ni paquete MCP en el manifest. No se afirma su ausencia en otros entornos. El catálogo de skills no ofrece una skill Unity; para modificar uGUI y documentos Markdown bastan las herramientas locales. No se instala ni duplica integración.

## Criterios de esta iteración

- Resumen inicial sin la parrilla de nueve acciones; Personaje, Inventario y Partida reutilizan botones/panel, sin ventanas duplicadas.
- Modos de RPG, construcción y RTS muestran únicamente sus acciones existentes y ayuda específica. No hay botones falsos de habilidades/mapa/formaciones.
- Fuente/paleta/tamaños/espaciado centralizados; límites y superposición medidos en render 1280×720 y 1920×1080.
- Ningún cambio de reglas, escala, colliders, NavMesh, guardado o clases. Referencias y GUID preservados.
- Pruebas funcionales de UI, suite previa, capturas inspeccionadas y nueva build. Resultados pendientes hasta ejecutar.

Dirección y conceptos de las cuatro clases: [DIRECCION_ARTISTICA](../Documentacion/DIRECCION_ARTISTICA.md). Los valores artísticos no confirmados se identifican como propuestas reversibles.
