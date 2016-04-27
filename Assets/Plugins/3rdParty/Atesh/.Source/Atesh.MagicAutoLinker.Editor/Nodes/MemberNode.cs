// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.Linq;
using System.Collections.Generic;
using Atesh.Editor;
using UnityEditor;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Editor
{
    class MemberNode : MemberContainer
    {
        internal class ObjectValue
        {
            internal Object Object;
            internal string Error;
            internal bool IsWarning;
            internal Rect Rect;
        }

        static int IndentLevel = 1;

        internal string Name;
        internal bool IsCollection;
        internal readonly List<ObjectValue> Values = new List<ObjectValue>();
        internal string Error;
        internal bool IsWarning;

        internal void DrawLabel(bool DrawName = true)
        {
            var OldColor = GUI.color;
            if (!string.IsNullOrEmpty(Error)) GUI.color = IsWarning ? Color.yellow : Color.red;

            if (DrawName) GUILayout.Label(AutoLinkerPreviewWindow.GetIndentBlank(IndentLevel) + Name, GUILayout.ExpandWidth(false));
            GUI.color = OldColor;

            if (IsCollection)
            {
                IndentLevel++;

                if (Members.Count == 0)
                {
                    for (var I = 0; I < Values.Count; I++)
                    {
                        OldColor = GUI.color;
                        if (!string.IsNullOrEmpty(Values[I].Error)) GUI.color = Values[I].IsWarning ? Color.yellow : Color.red;
                        GUILayout.Label($"{AutoLinkerPreviewWindow.GetIndentBlank(IndentLevel)}[{I}]", GUILayout.ExpandWidth(false));
                        GUI.color = OldColor;
                    }
                }
                else
                {
                    for (var I = 0; I < Members.Count; I++)
                    {
                        OldColor = GUI.color;
                        if (!string.IsNullOrEmpty(Members[I].Error)) GUI.color = Members[I].IsWarning ? Color.yellow : Color.red;
                        GUILayout.Label($"{AutoLinkerPreviewWindow.GetIndentBlank(IndentLevel)}[{I}]", GUILayout.ExpandWidth(false));
                        GUI.color = OldColor;

                        Members[I].DrawLabel(false);
                    }
                }

                IndentLevel--;
            }
            else
            {
                IndentLevel++;
                foreach (var Member in Members)
                {
                    Member.DrawLabel();
                }
                IndentLevel--;
            }
        }

        internal void DrawValue()
        {
            if (!string.IsNullOrEmpty(Error))
            {
                var OldColor = GUI.color;
                GUI.color = IsWarning ? Color.yellow : Color.red;
                GUILayout.Label(Error, EditorUtilities.MarginlessLabelStyle, GUILayout.ExpandWidth(false));
                GUI.color = OldColor;
                return;
            }

            if (IsCollection)
            {
                GUILayout.Label(GUIContent.none, EditorUtilities.MarginlessLabelStyle, GUILayout.ExpandWidth(false));

                if (Members.Count == 0)
                {
                    foreach (var Value in Values)
                    {
                        if (!string.IsNullOrEmpty(Value.Error))
                        {
                            var OldColor = GUI.color;
                            GUI.color = Value.IsWarning ? Color.yellow : Color.red;
                            GUILayout.Label(Value.Error, EditorUtilities.MarginlessLabelStyle, GUILayout.ExpandWidth(false));
                            GUI.color = OldColor;
                            continue;
                        }

                        DrawValue(Value);
                    }
                }
                else
                {
                    foreach (var Member in Members)
                    {
                        if (!string.IsNullOrEmpty(Member.Error))
                        {
                            var OldColor = GUI.color;
                            GUI.color = Member.IsWarning ? Color.yellow : Color.red;
                            GUILayout.Label(Member.Error, EditorUtilities.MarginlessLabelStyle, GUILayout.ExpandWidth(false));
                            GUI.color = OldColor;
                            continue;
                        }

                        Member.DrawValue();
                    }
                }
            }
            else if (Members.Count == 0)
            {
                DrawValue(Values.FirstOrDefault());
            }
            else
            {
                GUILayout.Label(GUIContent.none, EditorUtilities.MarginlessLabelStyle, GUILayout.ExpandWidth(false));

                foreach (var Member in Members)
                {
                    Member.DrawValue();
                }
            }
        }

        static void DrawValue(ObjectValue Value)
        {
            if (Value != null && Value.Object)
            {
                if (GUILayout.Button(Value.Object.name, EditorUtilities.MarginlessButtonStyle, GUILayout.ExpandWidth(false))) Selection.activeObject = Value.Object;
                if (Event.current.type == EventType.Repaint) Value.Rect = GUILayoutUtility.GetLastRect();
            }
            else GUILayout.Label(GUIContent.none, EditorUtilities.MarginlessLabelStyle, GUILayout.ExpandWidth(false));
        }

        internal void DrawLink()
        {
            if (IsCollection)
            {
                if (Members.Count == 0)
                {
                    foreach (var Value in Values)
                    {
                        if (string.IsNullOrEmpty(Value.Error))
                        {
                            var Transform = Value.Object is Component ? ((Component)Value.Object).transform : ((GameObject)Value.Object).transform;
                            AutoLinkerPreviewWindow.DrawLink(TransformNode.Root.FindTransform(Transform), Value.Rect, Color.cyan);
                        }
                    }
                }
                else
                {
                    foreach (var Member in Members.Where(Member => string.IsNullOrEmpty(Member.Error)))
                    {
                        Member.DrawLink();
                    }
                }
            }
            else if (Members.Count == 0)
            {
                var Value = Values.FirstOrDefault();
                if (Value != null)
                {
                    var Transform = Value.Object is Component ? ((Component)Value.Object).transform : ((GameObject)Value.Object).transform;
                    AutoLinkerPreviewWindow.DrawLink(TransformNode.Root.FindTransform(Transform), Value.Rect, Color.green);
                }
            }
            else
            {
                foreach (var Member in Members)
                {
                    Member.DrawLink();
                }
            }
        }
    }
}