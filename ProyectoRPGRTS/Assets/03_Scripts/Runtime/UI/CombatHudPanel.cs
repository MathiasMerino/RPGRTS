using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    public sealed class CombatHudPanel : MonoBehaviour
    {
        public GameSession Session;
        public TurnEncounter Encounter;
        public GameObject Panel;
        public Text TargetName,Health,Turn,Result;
        public Image HealthFill;
        public Button SelectTarget;
        private CombatResolution last;
        private void OnEnable(){if(Encounter!=null)Encounter.ActionResolved+=Receive;if(SelectTarget!=null)SelectTarget.onClick.AddListener(Select);}
        private void OnDisable(){if(Encounter!=null)Encounter.ActionResolved-=Receive;if(SelectTarget!=null)SelectTarget.onClick.RemoveListener(Select);}
        private void Select(){if(Encounter.Enemy!=null)Encounter.TrySelect(Encounter.Enemy.GetComponentInChildren<Collider>());}
        private void Receive(CombatFeedback result){last=result.Result;}
        public static string Describe(CombatResolution r)
        {
            var who=r.Actor==CombatSide.Player?"Tú":"Rival";
            if(r.Kind==CombatActionKind.Attack)return who+": "+(r.Hit?(r.Guarded?"bloqueo parcial · ":"impacto · ")+r.Damage+" PV":"fallo")+" · d20="+r.Roll;
            if(r.Kind==CombatActionKind.Guard)return "Guardia preparada: reduce el siguiente daño.";
            if(r.Kind==CombatActionKind.Recover)return "Recuperas "+r.Healing+" PV.";
            if(r.Kind==CombatActionKind.Wait)return "Rival: sin alcance o línea libre; espera.";
            return "Movimiento táctico.";
        }
        private void LateUpdate()
        {
            if(Session==null||Encounter==null)return;var b=Encounter.Battle;var show=!Session.Rules.CoreRefine&&Session.Mode==PrototypeMode.TurnCombat&&b!=null;Panel.SetActive(show);if(!show){last=default;return;}
            TargetName.text="MERODEADOR · "+(Encounter.TargetSelected?"seleccionado":"seleccionar");Health.text=b.EnemyHealth+" / 18 PV";HealthFill.fillAmount=b.EnemyHealth/18f;
            Turn.text=(b.Finished?(b.Won?"VICTORIA":"DERROTA"):b.Phase==TurnPhase.Enemy?"ENEMIGO ACTUANDO":"TU TURNO")+" · Ronda "+b.Round+" · Acción "+(b.ActionAvailable?"1":"0")+" · "+b.MovementRemaining.ToString("0.#")+" m";
            Result.text=last.Accepted?Describe(last):"Iniciativa: tú "+b.HeroInitiative+" / rival "+b.EnemyInitiative;
            SelectTarget.interactable=!b.Finished;
        }
    }
}
