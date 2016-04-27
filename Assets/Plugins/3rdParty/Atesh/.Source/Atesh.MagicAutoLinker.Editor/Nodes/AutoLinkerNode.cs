// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;
using Atesh.Editor;
using UnityEditor;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Editor
{
    class AutoLinkerNode
    {
        internal static AutoLinkerNode Root { get; private set; }

        AutoLinker Original;
        readonly List<ComponentNode> Components = new List<ComponentNode>();
        readonly Dictionary<AutoLinker, AutoLinkerNode> Children = new Dictionary<AutoLinker, AutoLinkerNode>();
        Rect Rect;

        static AutoLinkerNode()
        {
            ClearRoot();
        }

        internal AutoLinkerNode NewAutoLinker(AutoLinker Original)
        {
            var Result = new AutoLinkerNode { Original = Original };
            Children.Add(Original, Result);
            return Result;
        }

        internal ComponentNode NewComponent(Component Component)
        {
            var Result = new ComponentNode { Original = Component };
            Components.Add(Result);
            return Result;
        }

        internal void OnGUI()
        {
            if (Root != this)
            {
                if (!Original) return;

                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal(EditorUtilities.NoStretchStyle);
                {
                    GUILayout.Label("AutoLinker on ", GUILayout.ExpandWidth(false));
                    if (GUILayout.Button(Original.name, EditorUtilities.MarginlessButtonStyle, GUILayout.ExpandWidth(false))) Selection.activeGameObject = Original.gameObject;
                    if (Event.current.type == EventType.Repaint) Rect = GUILayoutUtility.GetLastRect();
                }
                GUILayout.EndHorizontal();

                foreach (var Component in Components)
                {
                    Component.OnGUI();
                }
            }

            foreach (var Child in Children)
            {
                Child.Value.OnGUI();
            }

            if (Root != this) GUILayout.EndVertical();
        }

        internal static void ClearRoot() => Root = new AutoLinkerNode();

        internal void DrawLink()
        {
            if (Root != this)
            {
                AutoLinkerPreviewWindow.DrawLink(TransformNode.Root.FindTransform(Original.transform), Rect, Color.white);

                foreach (var Component in Components)
                {
                    Component.DrawLink();
                }
            }

            foreach (var Child in Children)
            {
                Child.Value.DrawLink();
            }
        }
    }
}