using UnityEngine;

namespace Atesh
{
    public class DefaultHost : MonoBehaviour
    {
        static DefaultHost _Instance;

        public static DefaultHost Instance
        {
            get
            {
                if (_Instance) return _Instance;

                _Instance = new GameObject().AddComponent<DefaultHost>();
                _Instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(_Instance.gameObject);

                return _Instance;
            }
        }
    }
}