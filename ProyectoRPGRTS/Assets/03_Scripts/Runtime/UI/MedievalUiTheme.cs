using UnityEngine;

namespace Aethernia
{
    [CreateAssetMenu(menuName = "Aethernia/UI Theme")]
    public sealed class MedievalUiTheme : ScriptableObject
    {
        public Color Panel = new Color(0.12f, 0.10f, 0.085f, 0.96f);
        public Color Button = new Color(0.28f, 0.22f, 0.14f);
        public Color Gold = new Color(0.85f, 0.67f, 0.34f);
        public Color Text = new Color(0.94f, 0.89f, 0.76f);
        public Font Font;
    }
}
