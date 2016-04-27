// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh.WeNeedCreatedMessage
{
    public static class Strings
    {
        public const string DefaultTime = "Default Time";

        public static string InstanceIsNotCreatedYet(string Name) => $"Instance of {Name} is not created yet.";
        public static string OnlyOneInstanceAllowed(string Name) => $"Only one instance is allowed for {Name}.";
        public static string CreatedMessageSenderIsRequired(string ComponentName) => $"{nameof(CreatedMessageSender)} and {nameof(LateCreatedMessageSender)} are required for {ComponentName} component to work properly. You can add the CreatedMessageSender prefab into your scene.";
        public static string ExceptionInCreatedMessage(string ComponentName, Exception E) => $"Exception in created message handler of {ComponentName} component: {E}";
        public static string ComponentIsOnlySupportedForSceneObjects(string Name) => $"{Name} component is only supported for objects loaded with the scene.";
    }
}