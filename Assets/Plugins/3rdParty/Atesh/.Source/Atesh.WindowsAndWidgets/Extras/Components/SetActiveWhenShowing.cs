// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Linq;
using Atesh.WeNeedCreatedMessage;
using UnityEngine;

namespace Atesh.WindowsAndWidgets.Extras
{
    [DisallowMultipleComponent]
    public class SetActiveWhenShowing : CreatedMessageReceiver
    {
        #region Inspector
        public bool Value;
        #endregion

        Window ParentWindow;

        protected override void Created(bool OnSceneLoad)
        {
            ParentWindow = transform. GetComponentsInParent<Window>(true).FirstOrDefault();
            if (!ParentWindow)
            {
                Debug.LogWarning(Strings.CouldntFindWindowAsParent(name));
                return;
            }

            ParentWindow.Showing += ParentWindowShowing;
        }

        void ParentWindowShowing(object Sender, EventArgs E) => gameObject.SetActive(Value);
    }
}