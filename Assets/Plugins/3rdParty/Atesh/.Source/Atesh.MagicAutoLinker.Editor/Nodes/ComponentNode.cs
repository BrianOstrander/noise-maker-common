// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using Atesh.Editor;
using UnityEditor;
using UnityEngine;

namespace Atesh.MagicAutoLinker.Editor
{
    class ComponentNode : MemberContainer
    {
        internal Component Original;

        internal void OnGUI()
        {
            if (Members.Count == 0) return;

            var Icon = AssetPreview.GetMiniTypeThumbnail(Original.GetType()) ?? EditorGUIUtility.IconContent("cs Script Icon").image;

            GUILayout.Box(GUIContent.none, GUILayout.Height(1), GUILayout.Width(150));
            GUILayout.BeginVertical(EditorUtilities.NoStretchStyle);
            {
                var Content = new GUIContent(Original.GetType().Name, Icon);
                GUILayout.Label(Content, EditorStyles.boldLabel, GUILayout.ExpandWidth(false), GUILayout.Height(19));

                #region Members
                GUILayout.BeginHorizontal(EditorUtilities.NoStretchStyle);
                {
                    GUILayout.BeginVertical(EditorUtilities.NoStretchStyle);
                    {
                        foreach (var Member in Members)
                        {
                            Member.DrawLabel();
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(EditorUtilities.NoStretchStyle);
                    {
                        foreach (var Member in Members)
                        {
                            Member.DrawValue();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                #endregion
            }
            GUILayout.EndVertical();
        }

        internal void DrawLink()
        {
            foreach (var Member in Members)
            {
                Member.DrawLink();
            }
        }
    }
}