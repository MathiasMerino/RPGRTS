using System;
using UnityEngine;
namespace Aethernia
{
    [Serializable] public sealed class ClassPresentation
    { public string Id,Title,Role;public Sprite Figure,Portrait;public Color Accent; }
    [CreateAssetMenu(menuName="Aethernia/Visual Presentation")]
    public sealed class VisualPresentationProfile : ScriptableObject
    {
        public MedievalUiTheme Theme;
        public Sprite Panel,Paper,Button,Primary,Frame;
        public GameObject ButtonPrefab,CardPrefab;
        public ClassPresentation[] Classes=Array.Empty<ClassPresentation>();
        public Color Text=new Color(.91f,.88f,.80f),Gold=new Color(.78f,.64f,.39f),Ink=new Color(.12f,.10f,.08f);
        public ClassPresentation Find(string id)=>Array.Find(Classes,c=>c.Id==id);
    }
}
