// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using Atesh.Editor;
using UnityEditor;
using UnityEngine;

namespace Atesh.WeNeedCreatedMessage.Editor
{
    [InitializeOnLoad]
    class Installer
    {
        static Installer()
        {
            ResourceInstaller.Register(ScriptExecutionOrder.Install);
            ScriptExecutionOrder.InstanceCreated += Instance =>
            {
                Debug.LogWarning(Atesh.Strings.CouldntFindResourceData(ScriptExecutionOrder.AssetName));
                ResourceInstaller.CreateAsset(Instance, ScriptExecutionOrder.AssetFolder, ScriptExecutionOrder.AssetName);
            };

            InjectExecutionOrder();
        }

        static void InjectExecutionOrder()
        {
            var CreatedMessageSenderType = typeof(CreatedMessageSender);
            var LateCreatedMessageSenderType = typeof(LateCreatedMessageSender);

            foreach (var MonoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (MonoScript.GetClass() == CreatedMessageSenderType && MonoImporter.GetExecutionOrder(MonoScript) == 0) MonoImporter.SetExecutionOrder(MonoScript, -32000);
                else if (MonoScript.GetClass() == LateCreatedMessageSenderType && MonoImporter.GetExecutionOrder(MonoScript) == 0) MonoImporter.SetExecutionOrder(MonoScript, 32000);
            }
        }
    }
}