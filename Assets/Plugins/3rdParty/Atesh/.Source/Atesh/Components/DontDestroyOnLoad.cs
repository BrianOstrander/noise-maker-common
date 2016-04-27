// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        #region Messages
        // ReSharper disable UnusedMember.Local
        void Awake() => DontDestroyOnLoad(this);
        // ReSharper restore UnusedMember.Local
        #endregion
    }
}