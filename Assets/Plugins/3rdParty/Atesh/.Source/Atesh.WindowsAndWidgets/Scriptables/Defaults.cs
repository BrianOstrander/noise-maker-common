// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh.WindowsAndWidgets
{
    public class Defaults : SingletonScriptable<Defaults>
    {
        #region Inspector
        [SerializeField]
#pragma warning disable 649
        WindowFrame _WindowStyle;
#pragma warning restore 649
        #endregion

        public static WindowFrame WindowStyle => Instance._WindowStyle;

        static Defaults()
        {
            AssetNameToLoad = $"{nameof(WindowsAndWidgets)}Defaults";
        }
    }
}