// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh.WeNeedCreatedMessage.Extras
{
    public abstract class SingletonBehaviour<InstanceType> : ProtectedSingletonBehaviour<InstanceType> where InstanceType : MonoBehaviour
    {
        public static new InstanceType Instance => ProtectedSingletonBehaviour<InstanceType>.Instance;
    }
}