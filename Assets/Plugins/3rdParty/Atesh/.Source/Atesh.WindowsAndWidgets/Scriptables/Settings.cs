// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh.WindowsAndWidgets
{
    public class Settings : SingletonScriptable<Settings>
    {
        #region Inspector
        [SerializeField]
        WindowFrame _WindowStyle;
        [SerializeField]
        bool _ShowOptionalDebugInfos = true;
        #endregion

        public static readonly string AssetName = $"{nameof(WindowsAndWidgets)}Settings";
        public const string AssetFolder = nameof(Atesh);

        public static WindowFrame WindowStyle
        {
            get
            {
                return Instance._WindowStyle ?? Defaults.WindowStyle;
            }
            set
            {
                Instance._WindowStyle = value;
            }
        }

        public static bool ShowOptionalDebugInfos
        {
            get
            {
                return Instance._ShowOptionalDebugInfos;
            }
            set
            {
                Instance._ShowOptionalDebugInfos = value;
            }
        }

        static Settings()
        {
            AssetNameToLoad = $"{AssetFolder}/{AssetName}";
        }

        public static void Install()
        {
            // We access Instance to trigger it to create its resource asset.
            // ReSharper disable once ConvertMethodToExpressionBody
            Instance.GetInstanceID();
        }
    }
}