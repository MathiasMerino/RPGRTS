using UnityEngine;

namespace Aethernia
{
    public sealed class CombatTransient : MonoBehaviour
    {
        private float start,duration;
        private Vector3 origin,destination;
        private TextMesh label;
        private Transform view;
        private bool travel;
        public void Configure(float life,TextMesh text=null,Transform camera=null)
        {start=Time.time;duration=life;origin=transform.position;label=text;view=camera;}
        public void Projectile(Vector3 target,float life)
        {Configure(life);destination=target;travel=true;}
        private void Update()
        {
            var age=Time.time-start;var t=Mathf.Clamp01(age/duration);
            if(travel)transform.position=Vector3.Lerp(origin,destination,t);
            else if(label!=null){transform.position=origin+Vector3.up*t*.35f;if(view!=null)transform.rotation=view.rotation;var c=label.color;c.a=1-t;label.color=c;}
            if(age>=duration)Destroy(gameObject);
        }
    }
}
