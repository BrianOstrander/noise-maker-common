// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using Atesh.MagicAutoLinker;

namespace Atesh.WindowsAndWidgets
{
    public static class Strings
    {
        internal const string WindowMustHaveClientArea = "Window must have ClientArea as a child object.";
        internal const string OptionalInfoLogsCanBeDisabled = "(You can disable optional info logs in Widgets settings)";
        internal static readonly string AutoLinkerSelfDestroyIsGoodForTemplate = $"It's good for performance to set {nameof(AutoLinker)}.{nameof(AutoLinker.SelfDestroy)} to true on template windows";

        internal static string WindowMustBeInactiveWhenCreated(string Name) => $"Window '{Name}' must be inactive when it is created.";
        internal static string MissingComponent(string WindowName, string ComponentName) => $"Window '{WindowName}' must have " + ComponentName + " component.";
        internal static string CouldntFindWindowAsParent(string GameObjectName) => $"Couldn't find a Window component on any parent of game object '{GameObjectName}'.";
    }
}