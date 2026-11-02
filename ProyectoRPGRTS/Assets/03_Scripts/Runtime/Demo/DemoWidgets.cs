using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aethernia
{
    // Construcción visual compartida: ni reglas de juego ni gestión de ventanas.
    public sealed class DemoWidgets
    {
        public readonly TMP_FontAsset Font;
        public readonly Color Ink = new Color(.91f,.87f,.76f);
        public readonly Color Gold = new Color(.76f,.61f,.36f);
        private readonly Sprite button;
        public DemoWidgets(TMP_FontAsset font, Sprite skin) { Font=font;button=skin; }
        public static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
            r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
        }
        public static RectTransform Stretch(Transform parent,string name)
        {
            var r=Rect(parent,name,0,0,0,0);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;return r;
        }
        public static Image Fill(RectTransform r,Color color,bool raycast=true)
        {var i=r.gameObject.AddComponent<Image>();i.color=color;i.raycastTarget=raycast;return i;}
        public TMP_Text Text(Transform parent,string name,string value,float x,float y,float w,float h,float size=20)
        {
            var t=Rect(parent,name,x,y,w,h).gameObject.AddComponent<TextMeshProUGUI>();t.font=Font;t.fontSize=size;t.color=Ink;t.text=value;
            t.raycastTarget=false;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Truncate;t.margin=Vector4.zero;return t;
        }
        public Button Button(Transform parent,string name,string caption,float x,float y,float w,float h,UnityAction action)
        {
            var r=Rect(parent,name,x,y,w,h);var image=Fill(r,new Color(.30f,.26f,.19f));if(button!=null){image.sprite=button;image.type=Image.Type.Sliced;image.color=Color.white;}
            var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;
            var colors=b.colors;colors.highlightedColor=new Color(1,.90f,.71f);colors.selectedColor=new Color(.88f,.77f,.56f);colors.disabledColor=new Color(.42f,.42f,.42f);b.colors=colors;
            var t=Text(r,"Caption",caption,12,4,w-24,h-8,20);t.alignment=TextAlignmentOptions.Midline;
            if(action!=null)b.onClick.AddListener(action);return b;
        }
        public TMP_InputField Input(Transform parent,string name,string value,float x,float y,float width)
        {
            var r=Rect(parent,name,x,y,width,44);Fill(r,new Color(.11f,.12f,.12f));var field=r.gameObject.AddComponent<TMP_InputField>();
            var area=Rect(r,"Text Area",12,5,width-24,34);area.gameObject.AddComponent<RectMask2D>();var text=Text(area,"Text",value,0,0,width-24,34,21);
            field.textViewport=area;field.textComponent=(TextMeshProUGUI)text;field.fontAsset=Font;field.characterLimit=32;field.text=value;return field;
        }
        public Slider Slider(Transform parent,string name,float value,float x,float y,float width,UnityAction<float> changed)
        {
            var r=Rect(parent,name,x,y,width,36);Fill(r,Color.clear);var s=r.gameObject.AddComponent<Slider>();s.minValue=0;s.maxValue=1;
            var track=Rect(r,"Track",0,12,width,10);Fill(track,new Color(.16f,.16f,.14f),false);
            var fill=Stretch(track,"Fill");s.fillRect=fill;s.targetGraphic=Fill(fill,Gold,false);
            var handleArea=Rect(r,"Handle Area",9,3,width-18,30);var handle=Stretch(handleArea,"Handle");handle.pivot=new Vector2(.5f,.5f);handle.sizeDelta=new Vector2(18,0);Fill(handle,Ink);
            s.handleRect=handle;s.direction=UnityEngine.UI.Slider.Direction.LeftToRight;s.value=value;if(changed!=null)s.onValueChanged.AddListener(changed);return s;
        }
    }
}
