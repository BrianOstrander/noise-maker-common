// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;
using UnityEditor;

namespace Atesh.MagicAutoLinker.Editor
{
    [CustomEditor(typeof(AutoLinker))]
    class AutoLinkerEditor : UnityEditor.Editor
    {
        const string PreviewButton = "Preview";
        const string PreviewAllButton = "Preview All";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool Clicked;

            // ReSharper disable AssignmentInConditionalExpression
            if (Clicked = GUILayout.Button(PreviewButton)) AutoLinkerPreviewWindow.Target = (AutoLinker)target;
            else if (Clicked = GUILayout.Button(PreviewAllButton)) AutoLinkerPreviewWindow.Target = null;
            // ReSharper restore AssignmentInConditionalExpression

            if (Clicked) AutoLinkerPreviewWindow.Show();
        }
    }
}