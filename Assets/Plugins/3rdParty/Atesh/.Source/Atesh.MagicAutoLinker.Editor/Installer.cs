// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;
using UnityEditor;
using Atesh.WeNeedCreatedMessage;

namespace Atesh.MagicAutoLinker.Editor
{
    [InitializeOnLoad]
    class Installer
    {
        static Installer()
        {
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            InjectExecutionOrder();

            EditorApplication.update -= OnUpdate;
        }

        static void InjectExecutionOrder()
        {
            var FullName = typeof(AutoLinker).FullName;
            var Scripts = ScriptExecutionOrder.Instance.Scripts;

            var Order = Scripts.IndexOf(FullName);
            if (Order < 0) Scripts.Insert(0, FullName);
            else if (Order != 0) Debug.LogWarning(Strings.AutoLinkerMustBeFirstInExecutionOrder);
        }
    }
}