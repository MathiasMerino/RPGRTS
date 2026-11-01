using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    // Representación independiente del agente: pasos tácticos existentes por su ruta real, sin aplicar daño.
    public sealed class EnemyCombatVisual : MonoBehaviour
    {
        [SerializeField]private TurnEncounter encounter;
        [SerializeField]private CombatFeedbackPresenter feedback;
        [SerializeField]private Material ringMaterial;
        [SerializeField]private Transform hero;
        private ExplorationUnit unit;
        private GameObject visual;
        private Animator animator;
        private HumanoidAnimationProfile profile;
        private LineRenderer ring;
        private bool defeated,preparing;
        private bool pendingAttack;
        private float deathUntil,stepStart;
        private Vector3[] path;
        private float pathLength;
        private float inactiveSince;
        public GameObject Visual=>visual;
        public void Configure(TurnEncounter combat,CombatFeedbackPresenter effects,Material material,Transform target){encounter=combat;feedback=effects;ringMaterial=material;hero=target;}
        private void OnEnable()
        {
            if(encounter!=null){encounter.EnemySpawned+=Spawn;encounter.EnemyMoved+=Step;encounter.EnemyPreparingAttack+=Prepare;encounter.Presentation+=Cue;}
            if(feedback!=null)feedback.ImpactPresented+=Impact;
        }
        private void OnDisable()
        {
            if(encounter!=null){encounter.EnemySpawned-=Spawn;encounter.EnemyMoved-=Step;encounter.EnemyPreparingAttack-=Prepare;encounter.Presentation-=Cue;}
            if(feedback!=null)feedback.ImpactPresented-=Impact;Clear();
        }
        private void Clear(){if(unit!=null)foreach(var renderer in unit.GetComponentsInChildren<Renderer>())renderer.enabled=true;if(visual!=null)Destroy(visual);visual=null;animator=null;unit=null;path=null;defeated=false;preparing=false;inactiveSince=0;pendingAttack=false;}
        private void Cue(EncounterPresentation cue){if(cue.Cue==EncounterCue.Reset)Clear();}
        private void Spawn(ExplorationUnit actor,GameObject prefab)
        {
            Clear();unit=actor;if(prefab==null)return;visual=Instantiate(prefab,actor.transform.position,actor.transform.rotation);visual.name="E09 Merodeador · visual provisional";
            var rig=visual.GetComponent<HumanoidVisualRig>();var driver=visual.GetComponent<HumanoidAnimationDriver>();
            if(rig==null||driver==null||!rig.IsUsable||driver.Profile==null){Clear();return;}profile=driver.Profile;driver.enabled=false;animator=rig.Animator;animator.enabled=true;animator.applyRootMotion=false;animator.fireEvents=false;
            foreach(var renderer in actor.GetComponentsInChildren<Renderer>())renderer.enabled=false;
            var model=visual.GetComponent<CharacterVisualModel>();if(model.WeaponAnchor!=null&&model.WeaponAnchor.childCount>0)model.WeaponAnchor.GetChild(0).gameObject.SetActive(true);
            var node=new GameObject("Enemy selection ring");node.transform.SetParent(visual.transform,false);ring=node.AddComponent<LineRenderer>();ring.sharedMaterial=ringMaterial;ring.useWorldSpace=false;ring.loop=true;ring.positionCount=32;ring.startWidth=ring.endWidth=.025f;ring.startColor=ring.endColor=new Color(.70f,.31f,.17f);
            for(var i=0;i<32;i++){var a=i*Mathf.PI*2/32;ring.SetPosition(i,new Vector3(Mathf.Cos(a)*.40f,.035f,Mathf.Sin(a)*.40f));}
        }
        private void Step(Vector3 from,Vector3 to)
        {
            if(visual==null)return;var nav=new NavMeshPath();path=NavMesh.CalculatePath(from,to,NavMesh.AllAreas,nav)&&nav.status==NavMeshPathStatus.PathComplete?nav.corners:new[]{from,to};pathLength=0;
            for(var i=1;i<path.Length;i++)pathLength+=Vector3.Distance(path[i-1],path[i]);stepStart=Time.time;
        }
        private void Attack(){if(animator!=null&&animator.isInitialized)animator.SetTrigger("Attack");else pendingAttack=true;}
        private void Prepare(){preparing=true;Attack();}
        private void Impact(CombatFeedback data)
        {
            if(animator==null||!data.Result.Accepted||data.Result.Kind!=CombatActionKind.Attack)return;
            if(data.Result.Actor==CombatSide.Enemy){if(!preparing)Attack();preparing=false;return;}
            if(!data.Result.Hit)return;
            if(data.Result.TargetHealthAfter<=0){defeated=true;deathUntil=Time.time+1.4f;animator.SetBool("Defeated",true);if(ring!=null)ring.enabled=false;}
            else if(animator.isInitialized)animator.SetTrigger("Hit");
        }
        private void Update()
        {
            if(visual==null||animator==null)return;
            if(pendingAttack&&animator.isInitialized){animator.SetTrigger("Attack");pendingAttack=false;}
            if(defeated){if(Time.time>=deathUntil)Clear();return;}
            if(unit!=null&&unit.gameObject.activeInHierarchy)
            {
                var moving=path!=null&&Time.time-stepStart<.35f;
                if(moving)
                {
                    var left=pathLength*Mathf.Clamp01((Time.time-stepStart)/.35f);var point=path[0];
                    for(var i=1;i<path.Length;i++){var length=Vector3.Distance(path[i-1],path[i]);if(left<=length){point=Vector3.MoveTowards(path[i-1],path[i],left);break;}left-=length;point=path[i];}visual.transform.position=point;
                }
                else{visual.transform.position=unit.transform.position;path=null;}
                var delta=encounter.EnemyPosition-visual.transform.position;
                if(!moving&&hero!=null)delta=hero.position-visual.transform.position;
                delta.y=0;if(delta.sqrMagnitude>.001f)visual.transform.rotation=Quaternion.LookRotation(delta);
                animator.SetInteger("Locomotion",moving?1:0);if(profile.UseGaitRate)animator.SetFloat("GaitRate",1);
                if(ring!=null)ring.enabled=encounter.TargetSelected;
            }
            else{if(inactiveSince<=0)inactiveSince=Time.time;if(Time.time-inactiveSince>3.2f)Clear();}
        }
    }
}
