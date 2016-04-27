// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Editor
{
    class TransformNode
    {
        internal static TransformNode Root { get; private set; }

        Transform Original;
        bool Genuine;
        readonly Dictionary<Transform, TransformNode> Children = new Dictionary<Transform, TransformNode>();
        internal Rect Rect;

        static TransformNode()
        {
            ClearRoot();
        }

        internal static void AddTransform(Transform Original)
        {
            var ParentNode = Original.Parents().Aggregate<Transform, TransformNode>(null, (Current, Parent) => NewTransform(Parent, Current));

            var NewNode = NewTransform(Original, ParentNode);
            NewNode.Genuine = true;
        }

        static TransformNode NewTransform(Transform Original, TransformNode ParentNode)
        {
            ParentNode = ParentNode ?? Root;

            TransformNode Child;
            if (ParentNode.Children.TryGetValue(Original, out Child)) return Child;

            var Result = new TransformNode { Original = Original };
            ParentNode.Children.Add(Original, Result);
            return Result;
        }

        // ReSharper disable once ArrangeThisQualifier
        internal TransformNode FindTransform(Transform Original) => this.Original == Original ? this : Children.Select(Child => Child.Value.FindTransform(Original)).FirstOrDefault(X => X != null);

        internal void OnGUI(int IndentLevel = -1)
        {
            if (Root != this)
            {
                if (!Original) return;

                var OldColor = Color.black;

                if (!Genuine)
                {
                    OldColor = GUI.color;
                    GUI.color = Color.gray;
                }

                GUILayout.Label(AutoLinkerPreviewWindow.GetIndentBlank(IndentLevel) + Original.name, GUILayout.ExpandWidth(false)); 
                if (Event.current.type == EventType.Repaint)
                {
                    Rect = GUILayoutUtility.GetLastRect();
                    Rect.min += Vector2.right * 12 * IndentLevel;
                }

                if (!Genuine) GUI.color = OldColor;
            }

            IndentLevel++;
            foreach (var Child in Children)
            {
                Child.Value.OnGUI(IndentLevel);
            }
        }

        internal static void ClearRoot() => Root = new TransformNode();
    }
}