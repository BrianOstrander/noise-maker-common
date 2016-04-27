// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh.Editor
{
    public static class EditorUtilities
    {
        public static readonly GUIStyle NoStretchStyle = new GUIStyle { stretchWidth = false };
        public static readonly GUIStyle MarginlessButtonStyle = new GUIStyle("Button") { margin = new RectOffset() };
        public static readonly GUIStyle MarginlessLabelStyle = new GUIStyle("Button") { margin = new RectOffset(), normal = new GUIStyleState { textColor = MarginlessButtonStyle.normal.textColor } };
    }
}