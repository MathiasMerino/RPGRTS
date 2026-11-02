using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    // Estudio visual aislado: no crea agentes, sesión, daño ni estado de partida.
    public sealed class RuntimeClassPreview:MonoBehaviour
    {
        private CharacterVisualCatalog catalog;private RawImage image;private Image fallback;private DemoFrontEnd owner;private Canvas canvas;
        private GameObject studio;private Camera view;private RenderTexture target;private string current;
        public CharacterVisualModel Model {get;private set;}
        public bool IsShowing=>Model!=null&&studio!=null&&studio.activeSelf&&view.enabled;
        public void Configure(CharacterVisualCatalog value,string id,RawImage output,Image backup)
        {
            catalog=value;image=output;fallback=backup;owner=GetComponentInParent<DemoFrontEnd>();canvas=GetComponentInParent<Canvas>();
            studio=new GameObject("E17 isolated class preview");studio.transform.position=new Vector3(0,-10000,0);
            var camera=new GameObject("Class preview camera");camera.transform.SetParent(studio.transform,false);view=camera.AddComponent<Camera>();view.orthographic=true;view.orthographicSize=1.08f;view.nearClipPlane=.01f;view.farClipPlane=15;view.cullingMask=1<<31;view.clearFlags=CameraClearFlags.SolidColor;view.backgroundColor=new Color(.045f,.055f,.063f);camera.transform.localPosition=new Vector3(2.2f,1.45f,4.5f);camera.transform.LookAt(studio.transform.position+Vector3.up*.92f);
            Light("Key",new Vector3(2,2.5f,3),5,new Color(1,.89f,.73f));Light("Fill",new Vector3(-2,1.7f,1),2.5f,new Color(.67f,.77f,1));ShowClass(id);EnsureTarget();
        }
        private void Light(string name,Vector3 at,float intensity,Color color){var go=new GameObject(name);go.transform.SetParent(studio.transform,false);go.transform.localPosition=at;var light=go.AddComponent<Light>();light.type=LightType.Point;light.range=7;light.intensity=intensity;light.color=color;light.shadows=LightShadows.None;light.cullingMask=1<<31;}
        public void ShowClass(string id)
        {
            if(studio==null||current==id&&Model!=null)return;
            if(Model!=null){Model.gameObject.SetActive(false);Destroy(Model.gameObject);Model=null;}
            var success=CharacterVisualFactory.TryCreate(catalog,id,studio.transform,out var model,out _,out _);Model=model;current=id;
            if(success){SetLayer(model.transform);model.BindActor(null);model.ApplyEquipment(new PrototypeState{ClassId=id,Health=1,Weapon=id=="warrior"?"training-sword":"",Armour=id=="warrior"?"cloth":""});}
            image.enabled=success;if(fallback!=null)fallback.enabled=!success;
        }
        private static void SetLayer(Transform node){node.gameObject.layer=31;foreach(Transform child in node)SetLayer(child);}
        private void EnsureTarget()
        {
            if(image==null||view==null)return;var scale=canvas==null?1:canvas.scaleFactor;var h=Mathf.Clamp(Mathf.RoundToInt(image.rectTransform.rect.height*scale),256,1536);var w=Mathf.Clamp(Mathf.RoundToInt(h*Mathf.Max(.1f,image.rectTransform.rect.width/Mathf.Max(1,image.rectTransform.rect.height))),256,2048);
            if(target!=null&&target.width==w&&target.height==h)return;
            Release();target=new RenderTexture(w,h,24,RenderTextureFormat.ARGB32){name="E17 class preview",useMipMap=false,antiAliasing=1};target.Create();view.targetTexture=target;image.texture=target;
        }
        private void LateUpdate(){if(studio==null)return;var visible=owner!=null&&owner.Screen!=DemoScreen.None&&canvas!=null&&canvas.enabled&&Model!=null;if(studio.activeSelf!=visible)studio.SetActive(visible);view.enabled=visible;if(visible)EnsureTarget();}
        private void OnEnable(){if(studio!=null)studio.SetActive(true);}
        private void OnDisable(){if(studio!=null)studio.SetActive(false);}
        private void Release(){if(target==null)return;if(view!=null)view.targetTexture=null;if(image!=null)image.texture=null;target.Release();Destroy(target);target=null;}
        private void OnDestroy(){Release();if(studio!=null)Destroy(studio);}
    }
}
