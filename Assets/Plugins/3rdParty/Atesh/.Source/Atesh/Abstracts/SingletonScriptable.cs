// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public abstract class SingletonScriptable<InstanceType> : ProtectedSingletonScriptable<InstanceType> where InstanceType : ScriptableObject
    {
        public static new InstanceType Instance => ProtectedSingletonScriptable<InstanceType>.Instance;
    }
}