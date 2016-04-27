// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using Atesh.WeNeedCreatedMessage.Extras;
using UnityEngine;

namespace Atesh.WeNeedCreatedMessage
{
    public class LateCreatedMessageSender : ProtectedSingletonBehaviour<LateCreatedMessageSender>
    {
        internal static bool Present => Instance;

        #region Messages
        // ReSharper disable UnusedMember.Local
        void LateUpdate()
        {
#pragma warning disable 618
            // todo: isLoadingLevel obsolete olduğu için buraya yeni çözüm lazım.
            if (Application.isLoadingLevel) return;
#pragma warning restore 618

            CreatedMessageSender.SendMessages(false);
        }
        // ReSharper restore UnusedMember.Local
        #endregion
    }
}