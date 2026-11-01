using System;
using System.IO;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class E09CombatBuilder
    {
        public const string Root="Assets/09_Feedback/CombatE09";
        public const string BankPath=Root+"/CombatAudio.asset";
        public const string ImpactPath=Root+"/Impact.prefab";
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var panel=Object.FindAnyObjectByType<CombatHudPanel>();var sprite=hud.CompactView.Theme.ButtonSprite;
            // Sin sprite uGUI dibuja un quad simple aunque fillAmount cambie.
            hud.CompactView.HealthFill.sprite=sprite;panel.HealthFill.sprite=sprite;panel.HealthFill.fillMethod=Image.FillMethod.Horizontal;panel.HealthFill.type=Image.Type.Filled;
            panel.Turn.rectTransform.anchoredPosition=new Vector2(24,-51);panel.Result.rectTransform.anchoredPosition=new Vector2(24,-78);
            panel.SelectTarget.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,32);
            panel.TargetName.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,32);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);Debug.Log("E09_REFINED: barras visibles proporcionales con sprite compartido, sin rediseñar C02.");
        }
        public static void Create()
        {
            if(AssetDatabase.LoadAssetAtPath<CombatAudioBank>(BankPath)!=null)throw new InvalidOperationException("E09 ya existe; no regenerar recursos/GUID.");
            foreach(var folder in new[]{Root,Root+"/Materials",Root+"/Textures"})EnvironmentMaterials.Folder(folder);
            var bank=ScriptableObject.CreateInstance<CombatAudioBank>();bank.Clips=new AudioClip[Enum.GetNames(typeof(FeedbackSound)).Length];
            foreach(FeedbackSound sound in Enum.GetValues(typeof(FeedbackSound)))
            {
                var path=Root+"/Audio/"+sound+".wav";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
                var importer=(AudioImporter)AssetImporter.GetAtPath(path);if(importer==null)throw new InvalidOperationException("PCM original ausente: "+sound);
                importer.forceToMono=true;importer.loadInBackground=false;var settings=importer.defaultSampleSettings;settings.preloadAudioData=true;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;importer.defaultSampleSettings=settings;importer.SaveAndReimport();
                bank.Clips[(int)sound]=AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            }
            AssetDatabase.CreateAsset(bank,BankPath);var material=Material();var impact=Impact(material);
            var scene=EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);var physics=EnvironmentPhysicsSnapshot.Capture(scene);
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var session=d.Session;
            var root=new GameObject("E09 Combat Feedback");var audio=root.AddComponent<CombatAudioPlayer>();audio.Configure(bank);
            var effects=root.AddComponent<CombatFeedbackPresenter>();effects.Configure(d.Encounter,d.Hero.GetComponent<CharacterVisualPresenter>(),d.Hero.GetComponent<CharacterCombatAnimationPresenter>(),audio,impact,material,hud.CompactView.Theme.Font);
            root.AddComponent<EnemyCombatVisual>().Configure(d.Encounter,effects,material,d.Hero.transform);
            root.AddComponent<UiAudioFeedback>().Configure(audio,hud,Object.FindAnyObjectByType<LostCargoView>(),session);
            d.Encounter.ConfigureTactics(true,AssetDatabase.LoadAssetAtPath<GameObject>(AnimatedCharacterBuilder.Prefabs+"/warrior_Animated.prefab"));
            Hud(hud,d.Encounter,session);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics)throw new InvalidOperationException("E09 alteró física del escenario.");
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Refine();
            Debug.Log("E09_CREATED: staged legal turns, shared enemy rig, contextual HUD, bounded URP feedback and 14 original PCM clips. Human/audio quality review pending.");
        }
        private static Material Material()
        {
            var texture=new Texture2D(32,32,TextureFormat.RGBA32,false);var pixels=new Color[1024];
            for(var y=0;y<32;y++)for(var x=0;x<32;x++){var r=new Vector2((x-15.5f)/15.5f,(y-15.5f)/15.5f).magnitude;pixels[y*32+x]=new Color(1,1,1,Mathf.Pow(Mathf.Clamp01(1-r),1.5f));}
            texture.SetPixels(pixels);texture.Apply();var path=Root+"/Textures/SoftDot.png";File.WriteAllBytes(path,texture.EncodeToPNG());Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.SaveAndReimport();
            var shader=Shader.Find("Universal Render Pipeline/Particles/Unlit");if(shader==null)throw new InvalidOperationException("Shader de partículas URP ausente.");
            var material=new Material(shader){name="E09 Shared Soft Feedback",renderQueue=3000};material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(path));material.SetFloat("_Surface",1);material.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);material.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);material.SetFloat("_ZWrite",0);material.SetFloat("_Cull",0);material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.SetOverrideTag("RenderType","Transparent");
            AssetDatabase.CreateAsset(material,Root+"/Materials/SharedFeedback.mat");return material;
        }
        private static GameObject Impact(Material material)
        {
            var node=new GameObject("E09 Impact · bounded");
            try
            {
                var ps=node.AddComponent<ParticleSystem>();var main=ps.main;main.loop=false;main.playOnAwake=false;main.duration=.5f;main.startLifetime=new ParticleSystem.MinMaxCurve(.18f,.35f);main.startSpeed=new ParticleSystem.MinMaxCurve(.35f,1.2f);main.startSize=new ParticleSystem.MinMaxCurve(.035f,.075f);main.maxParticles=24;main.simulationSpace=ParticleSystemSimulationSpace.World;
                var emission=ps.emission;emission.rateOverTime=0;emission.SetBursts(new[]{new ParticleSystem.Burst(0,12)});var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=.06f;
                var color=ps.colorOverLifetime;color.enabled=true;var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},new[]{new GradientAlphaKey(1,0),new GradientAlphaKey(0,1)});color.color=gradient;
                var renderer=node.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
                return PrefabUtility.SaveAsPrefabAsset(node,ImpactPath);
            }
            finally{Object.DestroyImmediate(node);}
        }
        private static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var t=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();t.SetParent(parent,false);t.anchorMin=t.anchorMax=t.pivot=new Vector2(0,1);t.anchoredPosition=new Vector2(x,-y);t.sizeDelta=new Vector2(w,h);return t;
        }
        private static Text Text(Transform parent,string name,float x,float y,float w,float h,MedievalUiTheme theme)
        {var t=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Text>();t.font=theme.Font;t.fontSize=15;t.color=theme.Text;t.raycastTarget=false;t.alignment=TextAnchor.MiddleLeft;return t;}
        private static void Hud(VerticalSliceHud hud,TurnEncounter encounter,GameSession session)
        {
            var theme=hud.CompactView.Theme;var panel=Rect(hud.transform,"E09 Combat Context",456,98,570,112);var image=panel.gameObject.AddComponent<Image>();image.sprite=theme.InsetSprite;image.type=Image.Type.Sliced;image.color=Color.white;
            var view=hud.gameObject.AddComponent<CombatHudPanel>();view.Session=session;view.Encounter=encounter;view.Panel=panel.gameObject;
            var target=Rect(panel,"Select real enemy",12,7,350,32);target.gameObject.AddComponent<Image>().sprite=theme.ButtonSprite;view.SelectTarget=target.gameObject.AddComponent<Button>();view.TargetName=Text(target,"Enemy name",8,0,334,32,theme);view.TargetName.fontStyle=FontStyle.Bold;
            view.Health=Text(panel,"Enemy health",410,7,145,26,theme);var track=Rect(panel,"Enemy health track",14,39,540,9).gameObject.AddComponent<Image>();track.color=new Color(.10f,.06f,.04f);
            var fill=Rect(track.transform,"Enemy health fill",0,0,540,9).gameObject.AddComponent<Image>();fill.color=new Color(.58f,.19f,.13f);fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;view.HealthFill=fill;
            view.Turn=Text(panel,"Actual turn",14,51,540,24,theme);view.Turn.color=theme.Gold;view.Result=Text(panel,"Actual result",14,78,540,27,theme);view.Result.fontSize=14;
        }
    }
}
