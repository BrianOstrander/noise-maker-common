// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Atesh.WeNeedCreatedMessage.Extras
{
    public abstract class ProtectedSingletonBehaviour<InstanceType> : CreatedMessageReceiver where InstanceType : MonoBehaviour
    {
        static InstanceType _Instance;
        protected static InstanceType Instance
        {
            get
            {
                if (!_Instance) throw new InvalidOperationException(Strings.InstanceIsNotCreatedYet(typeof(InstanceType).Name));

                return _Instance;
            }
        }

        //todo: Bu protected ise SingletonCreated niye var? SingletonCreated hiç kullanılmıyosa silinmeli mi?
        protected override void Created(bool OnSceneLoad)
        {
            if (_Instance) Debug.LogError(Strings.OnlyOneInstanceAllowed(typeof(InstanceType).Name));
            else _Instance = GetComponent<InstanceType>();

            SingletonCreated(OnSceneLoad);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "VirtualMemberNeverOverriden.Global")]
        protected virtual void SingletonCreated(bool OnSceneLoad) { }
    }
}