// ReSharper disable All

#if UNITY_EDITOR
using Atesh.MagicAutoLinker.Examples;
using Atesh.Waitress.Examples;
using Atesh.WeNeedCreatedMessage.Examples;

namespace Xtro
{
    class ResharperInspection
    {
        void Dummy()
        {
            new CreatedMessageExample { Prefab = null };
            new YourScript();
            new YourSimpleScript { AvatarMaterial = null };
            new YourScriptWithSubclass { AvatarMaterial = null, Properties = null };
            new YourScriptWithCollection();
            new YourComplexScript { AvatarMaterial = null };
            new TimerExample();
            new ConditionerExample();
            new FramerExample();
        }
    }
}
#endif