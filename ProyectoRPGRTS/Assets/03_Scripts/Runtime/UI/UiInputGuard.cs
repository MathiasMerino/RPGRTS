using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aethernia
{
    public static class UiInputGuard
    {
        private static GameObject lastSelection;
        private static InputField input;
        private static TMPro.TMP_InputField sdfInput;
        private static EventSystem lastSystem;
        private static PointerEventData pointer;
        private static readonly List<RaycastResult> hits = new List<RaycastResult>();
        public static bool IsTyping
        {
            get
            {
                var selected=EventSystem.current==null?null:EventSystem.current.currentSelectedGameObject;
                return selected!=null&&((selected.TryGetComponent<InputField>(out var legacy)&&legacy.isFocused)||(selected.TryGetComponent<TMPro.TMP_InputField>(out var sdf)&&sdf.isFocused));
            }
        }
        public static bool BlocksWorldInput
        {
            get
            {
                var system = EventSystem.current;
                if (system == null) return false;
                var selected = system.currentSelectedGameObject;
                if (selected != lastSelection) { lastSelection = selected; input = selected == null ? null : selected.GetComponent<InputField>();sdfInput=selected==null?null:selected.GetComponent<TMPro.TMP_InputField>(); }
                return (input != null && input.isFocused) || (sdfInput!=null&&sdfInput.isFocused) || system.IsPointerOverGameObject();
            }
        }
        public static bool HasUiAt(Vector2 position)
        {
            var system = EventSystem.current;
            if (system == null) return false;
            if (lastSystem != system || pointer == null) { lastSystem = system; pointer = new PointerEventData(system); }
            pointer.Reset(); pointer.position = position; hits.Clear(); system.RaycastAll(pointer, hits);
            return hits.Count > 0;
        }
    }
}
