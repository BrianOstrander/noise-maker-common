// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Atesh.Editor
{
    [InitializeOnLoad]
    public static class ResourceInstaller
    {
        const string Assets = "Assets";
        const string Resources = "Resources";

        static readonly List<Action> InstallMethods = new List<Action>();

        static ResourceInstaller()
        {
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            foreach (var InstallMethod in InstallMethods)
            {
                try
                {
                    InstallMethod();
                }
                catch (Exception E)
                {
                    Debug.LogException(E);
                }
            }

            EditorApplication.update -= OnUpdate;
        }

        public static void Register(Action InstallMethod) => InstallMethods.Add(InstallMethod);

        public static void CreateAsset(Object Asset, string AssetFolder, string AssetName)
        {
            if (!Directory.Exists($"{Application.dataPath}/{Resources}")) AssetDatabase.CreateFolder(Assets, Resources);

            var Subfolder = "";
            if (!string.IsNullOrEmpty(AssetFolder))
            {
                if (!Directory.Exists($"{Application.dataPath}/{Resources}/{AssetFolder}")) AssetDatabase.CreateFolder($"{Assets}/{Resources}", AssetFolder);
                Subfolder = $"{AssetFolder}/";
            }

            AssetDatabase.CreateAsset(Asset, $"{Assets}/{Resources}/{Subfolder}{AssetName}.asset");
        }
    }
}