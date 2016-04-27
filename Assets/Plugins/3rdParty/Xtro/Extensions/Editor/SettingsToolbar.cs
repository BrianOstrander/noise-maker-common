// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;
using UnityEditor;

namespace Xtro
{
    // ReSharper disable once UnusedMember.Global
    class SettingsToolbar : EditorWindow
    {
        #region Messages
        // ReSharper disable UnusedMember.Local
        [MenuItem("Window/Settings Toolbar")]
        static void Init()
        {
            var Window = GetWindow<SettingsToolbar>(false, "Settings");
            Window.minSize = new Vector2(10, 10);
            Window.Show();
        }

        void OnGUI()
        {
            var Style = new GUIStyle(EditorStyles.toolbarButton)
            {
                fixedWidth = 30,
                fixedHeight = 28,
                imagePosition = ImagePosition.ImageOnly
            };

            var Content = new GUIContent();

            EditorGUILayout.BeginHorizontal();

            Content.text = "";

            Content.tooltip = "Input";
            Content.image = EditorGUIUtility.Load("icons/d_movetool.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Input");
            }

            Content.tooltip = "Audio";
            Content.image = EditorGUIUtility.Load("icons/d_SceneViewAudio.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Audio");
            }

            Content.tooltip = "Time";
            Content.image = EditorGUIUtility.Load("icons/d_UnityEditor.AnimationWindow.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Time");
            }

            Content.tooltip = "Player";
            Content.image = EditorGUIUtility.Load("icons/d_unityeditor.gameview.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
            }

            Content.tooltip = "Physics";
            Content.image = AssetPreview.GetMiniTypeThumbnail(typeof(PhysicMaterial));
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics");
            }

            Content.tooltip = "Physics2D";
            Content.image = AssetPreview.GetMiniTypeThumbnail(typeof(PhysicsMaterial2D));
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics 2D");
            }

            Content.tooltip = "Quality";
            Content.image = EditorGUIUtility.Load("icons/d_viewtoolorbit.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Quality");
            }

            Content.tooltip = "Graphics";
            Content.image = AssetPreview.GetMiniTypeThumbnail(typeof(Shader));
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
            }

            Content.tooltip = "Network";
            Content.image = AssetPreview.GetMiniTypeThumbnail(typeof(NetworkView));
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Network");
            }

            Content.tooltip = "Editor";
            Content.image = EditorGUIUtility.Load("icons/d_SettingsIcon.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
            }

            Content.tooltip = "Script Execution Order";
            Content.image = EditorGUIUtility.Load("icons/UnityEditor.ConsoleWindow.png") as Texture2D;
            if (GUILayout.Button(Content, Style))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Script Execution Order");
            }

            EditorGUILayout.EndHorizontal();

        }
        // ReSharper restore UnusedMember.Local
        #endregion
    }
}