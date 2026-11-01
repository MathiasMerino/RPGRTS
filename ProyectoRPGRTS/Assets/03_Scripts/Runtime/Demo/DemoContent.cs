using System;
using UnityEngine;

namespace Aethernia
{
    [Serializable] public sealed class LineageProfile
    {
        public string Id, Title, Description;
        public LineageProfile(string id,string title,string description){Id=id;Title=title;Description=description;}
    }
    [CreateAssetMenu(menuName="Aethernia/Demo Content")]
    public sealed class DemoContent : ScriptableObject
    {
        public string Subtitle="Un mundo de aventuras, ejércitos y decisiones";
        public string[] Intro={
            "EL SILENCIO DE LOS UMBRALES\n\nEn Aethernia, antiguos pasos arcanos comienzan a apagarse. Los caminos se vuelven inciertos y los reinos buscan respuestas entre las ruinas.",
            "HAS LLEGADO A LA FRONTERA\n\nViajas con provisiones y equipo de entrenamiento. Cerca de una aldea, un merodeador cierra el sendero. Antes de buscar respuestas, tendrás que abrirte paso.",
            "UN CAMINO PROPIO\n\nQuienes sobreviven en la frontera se ayudan para mantener sus hogares. Puedes prestarles tu fuerza, dirigir una patrulla o levantar un campamento; tu destino no exige una corona."
        };
        public LineageProfile[] Lineages={
            new LineageProfile("human","Humano","Una procedencia humana abierta a distintas culturas. Sin bonificaciones raciales en esta demo."),
            new LineageProfile("elf","Elfo","Linaje élfico provisional, inspirado en la diversidad de Aethernia. Sin cultura política impuesta ni bonificaciones."),
            new LineageProfile("dwarf","Enano","Linaje enano provisional. Sus culturas e identidades se definirán después; no altera estadísticas.")
        };
        public string VisualNotice="Linajes provisionales. Los tres reutilizan el modelo humano de clase; no hay apariencia racial final ni bonos.";
        public string NarrativeNotice="Prólogo de demostración: narrativa provisional, revisable y no canónica.";
        public string Credits="CRÓNICAS DE AETHERNIA\nProyecto de Mathias Merino\n\nDesarrollo asistido con Codex.\nMotor Unity 6000.5.8f1 · Universal Render Pipeline.\nEscenario, materiales, personajes Generic, animaciones y efectos originales del proyecto.\nSonidos originales sintetizados: banco E09.\nTipografía Liberation Sans: SIL Open Font License 1.1 (licencia incluida).\n\nArte, reglas nuevas y narrativa de esta demo son provisionales. No se incorporó música comercial ni assets externos sin licencia.";
        public string ClassDescription(string id) => id=="warrior"?"Acero y resistencia. Combate cercano con espada, defensa y una silueta sólida.":id=="monk"?"Disciplina y movilidad. Golpes marciales sin arma visible y bonificación de iniciativa.":"Estudio arcano y distancia. Ataque existente de mayor alcance, con energía sobria y menor resistencia.";
        public static bool KnownLineage(string id)=>id=="human"||id=="elf"||id=="dwarf";
    }
    public enum DemoChapter { Intro=1, FirstCombat=2, RoadToRts=3, FirstRts=4, Settlement=5, FreeExploration=6 }
    [Serializable] public sealed class DemoProgress
    {
        public DemoChapter Chapter=DemoChapter.Intro;
        public bool IntroSeen;
        public int Produced;
    }
}
