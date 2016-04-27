// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using Atesh.Editor;
using UnityEditor;
using UnityEngine;

namespace Atesh.WindowsAndWidgets.Editor
{
    [InitializeOnLoad]
    class Installer
    {
        static Installer()
        {
            ResourceInstaller.Register(Settings.Install);
            Settings.InstanceCreated += Instance =>
            {
                Debug.LogWarning(Atesh.Strings.CouldntFindResourceData(Settings.AssetName));
                ResourceInstaller.CreateAsset(Instance, Settings.AssetFolder, Settings.AssetName);
            };
        }
    }
} 