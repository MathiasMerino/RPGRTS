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
        private readonly VisualPresentationProfile style;
        public DemoWidgets(TMP_FontAsset font, Sprite skin,VisualPresentationProfile presentation=null) { Font=font;style=presentation;button=style==null?skin:style.Button;if(style!=null){Ink=style.Text;Gold=style.Gold;} }
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
            if(style!=null&&style.ButtonPrefab!=null)
            {
                var node=Object.Instantiate(style.ButtonPrefab,parent,false);node.name=name;var rect=node.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);
                var label=node.GetComponentInChildren<TMP_Text>();var lr=label.rectTransform;lr.anchorMin=Vector2.zero;lr.anchorMax=Vector2.one;lr.offsetMin=new Vector2(12,4);lr.offsetMax=new Vector2(-12,-4);label.horizontalAlignment=HorizontalAlignmentOptions.Center;label.verticalAlignment=VerticalAlignmentOptions.Middle;label.text=caption;var result=node.GetComponent<Button>();result.onClick.RemoveAllListeners();if(action!=null)result.onClick.AddListener(action);return result;
            }
            var r=Rect(parent,name,x,y,w,h);var image=Fill(r,new Color(.30f,.26f,.19f));if(button!=null){image.sprite=button;image.type=Image.Type.Sliced;image.color=Color.white;}
            var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;
            var colors=b.colors;colors.highlightedColor=new Color(1,.90f,.71f);colors.selectedColor=new Color(.88f,.77f,.56f);colors.disabledColor=new Color(.42f,.42f,.42f);b.colors=colors;
            var t=Text(r,"Caption",caption,12,4,w-24,h-8,style==null?20:18);t.alignment=TextAlignmentOptions.Midline;
            if(action!=null)b.onClick.AddListener(action);return b;
        }
        public Image Surface(RectTransform rect,Color fallback,bool paper=false)
        {var image=Fill(rect,style==null?fallback:Color.white);if(style!=null){image.sprite=paper?style.Paper:style.Panel;image.type=Image.Type.Sliced;}return image;}
        public void Primary(Button control)
        {if(style==null)return;control.GetComponent<Image>().sprite=style.Primary;control.GetComponentInChildren<TMP_Text>().color=style.Ink;}
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
