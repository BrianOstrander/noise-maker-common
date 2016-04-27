// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;
using UnityEditor;

namespace Xtro
{
    // ReSharper disable once UnusedMember.Global
    class ShortcutsToolbar : EditorWindow
    {
        #region Messages
        // ReSharper disable UnusedMember.Local
        [MenuItem("Window/Shortcuts Toolbar")]
        static void Init()
        {
            var Window = GetWindow<ShortcutsToolbar>(false, "Shortcuts");
            Window.minSize = new Vector2(10, 10);
            Window.Show();
        }

        void OnGUI()
        {
            var Style = new GUIStyle(EditorStyles.toolbarButton)
            {
                fixedHeight = 28,
                imagePosition = ImagePosition.TextOnly
            };

            var Content = new GUIContent();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    Content.text = "";

                    Content.tooltip = "Fast Platform Switch";
                    Content.text = "Platform\nSwitch";
                    if (GUILayout.Button(Content, Style))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Fast Platform Switch");
                    }

                    Content.tooltip = "Advanced PlayerPrefs";
                    Content.text = "Advanced\nPlayerPrefs";
                    if (GUILayout.Button(Content, Style))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Advanced PlayerPrefs Window");
                    }

                    Content.tooltip = "C# Project";
                    Content.text = "C# Project";
                    if (GUILayout.Button(Content, Style))
                    {
                        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    Content.tooltip = "Resource Checker";
                    Content.text = "Resource\nChecker";
                    if (GUILayout.Button(Content, Style))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Resource Checker");
                    }

                    Content.tooltip = "Maintainer";
                    Content.text = "Maintainer";
                    if (GUILayout.Button(Content, Style))
                    {
                        EditorApplication.ExecuteMenuItem("Window/Code Stage/Maintainer");
                    }

                    Content.tooltip = "TextMeshPro Font Creator";
                    Content.text = "Font\nCreator";
                    if (GUILayout.Button(Content, Style))
                    {
                        EditorApplication.ExecuteMenuItem("Window/TextMeshPro - Font Asset Creator");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        // ReSharper restore UnusedMember.Local
        #endregion
    }
}