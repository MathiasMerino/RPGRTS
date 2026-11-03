using System.Linq;
using UnityEngine;

namespace Aethernia
{
    // Extensión del controlador existente: mismo ejército, salud, selección y resolución.
    public sealed partial class RtsBattleController
    {
        public Vector3 ArenaOffset=>session!=null&&session.Rules.UxExperience?new Vector3(-34,0,-10):Vector3.zero;
        public bool Preparing { get; private set; }
        public int PreparationStep=>session.State.Demo?.UxPreparationStep??0;
        public Vector3 PreparationTarget=>ArenaOffset+(PreparationStep<2?new Vector3(-8,0,6):new Vector3(-5,0,12));
        public bool BeginPreparation(ExplorationUnit hero)
        {
            if(!BeginWithHero(hero))return false;
            Preparing=true;
            foreach(var s in soldiers)if(s.Team==1)s.Unit.gameObject.SetActive(false);
            var progress=session.State.Demo;
            var allies=soldiers.Where(s=>s.Team==0).ToArray();
            if(progress.UxFormation!=null&&progress.UxFormation.Length==allies.Length)
                for(var i=0;i<allies.Length;i++)
                {allies[i].Unit.Warp(progress.UxFormation[i]);allies[i].Unit.Select(progress.UxSelected!=null&&i<progress.UxSelected.Length&&progress.UxSelected[i]);}
            // La primera selección se enseña con las unidades sin seleccionar.
            else foreach(var s in allies)s.Unit.Select(false);
            if(progress.UxPreparationStep>=3)RevealPreparedEnemies();
            session.Say("Patrulla en preparación: selecciona un aliado con clic izquierdo. No hay ataques activos.");
            return true;
        }
        private void TickPreparation()
        {
            nextTick=Time.time+.15f;
            var p=session.State.Demo;if(p==null)return;
            if(p.UxPreparationStep==0&&SelectedAllies==1)p.UxPreparationStep=1;
            if(p.UxPreparationStep==1&&SelectedAllies>=2&&ArrivedAt(PreparationTarget))p.UxPreparationStep=2;
            if(p.UxPreparationStep==2&&SelectedAllies>=2&&ArrivedAt(PreparationTarget)){p.UxPreparationStep=3;RevealPreparedEnemies();session.Say("Patrulla reagrupada. Enemigos al este: coloca tus tropas y da clic derecho sobre un rival cuando estés listo.");}
        }
        private bool ArrivedAt(Vector3 target)
        {
            var arrived=0;foreach(var s in soldiers)
                if(s.Team==0&&s.Unit.IsSelected&&s.MoveOrder&&!s.Unit.IsMoving&&Vector3.Distance(s.Unit.transform.position,target)<3)arrived++;
            return arrived>=2;
        }
        private void RevealPreparedEnemies(){foreach(var s in soldiers)if(s.Team==1)s.Unit.gameObject.SetActive(true);}
        private bool EngagePrepared()
        {
            if(PreparationStep<3){session.Say("Primero mueve varias unidades al hito y reagrúpalas en el segundo.");return false;}
            Preparing=false;session.State.Demo.UxFormation=null;session.State.Demo.UxSelected=null;
            session.Say("Ataque confirmado. Mantén la formación y utiliza las acciones del héroe.");return true;
        }
        public void CapturePreparation()
        {
            if(!Preparing||session.State.Demo==null)return;
            var allies=soldiers.Where(s=>s.Team==0).ToArray();
            session.State.Demo.UxFormation=allies.Select(s=>s.Unit.transform.position).ToArray();
            session.State.Demo.UxSelected=allies.Select(s=>s.Unit.IsSelected).ToArray();
        }
    }
}
