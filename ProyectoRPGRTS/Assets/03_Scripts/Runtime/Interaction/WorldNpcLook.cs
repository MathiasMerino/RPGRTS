using UnityEngine;
namespace Aethernia
{
    public sealed class WorldNpcLook : MonoBehaviour
    {
        [SerializeField] private DemoFlow flow;
        [SerializeField] private Transform visual;
        private Quaternion rest;
        public void Configure(DemoFlow f,Transform model){flow=f;visual=model;}
        private void Start(){rest=visual.localRotation;}
        private void Update()
        {
            if(flow==null||visual==null)return;
            var near=flow.IsPlaying&&flow.Session.Mode==PrototypeMode.Exploration&&Vector3.Distance(transform.position,flow.Director.Hero.transform.position)<3;
            var direction=flow.Director.Hero.transform.position-visual.position;direction.y=0;
            var target=near&&direction.sqrMagnitude>.01f?Quaternion.Inverse(visual.parent.rotation)*Quaternion.LookRotation(direction):rest;
            visual.localRotation=Quaternion.Slerp(visual.localRotation,target,5*Time.deltaTime);
        }
    }
}
