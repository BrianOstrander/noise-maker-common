// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh.WeNeedCreatedMessage.Extras
{
    [DisallowMultipleComponent]
    public class SetActiveWhenCreated : CreatedMessageReceiver
    {
        #region Inspector
        public bool Value;
        public bool SelfDestroy;
        #endregion

        protected override void Created(bool OnSceneLoad)
        {
            if (!OnSceneLoad)
            {
                Debug.LogWarning(Strings.ComponentIsOnlySupportedForSceneObjects(GetType().Name));
                return;
            }

            gameObject.SetActive(Value);
            if (SelfDestroy) Destroy(this);
        }
    }
}