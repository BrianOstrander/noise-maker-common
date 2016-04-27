// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh
{
    public abstract class ProtectedSingletonScriptable<InstanceType> : ScriptableObject where InstanceType : ScriptableObject
    {
        // ReSharper disable once StaticMemberInGenericType
        protected static string AssetNameToLoad;
        public static event InstanceCreatedEventHandler<InstanceType> InstanceCreated;

        static InstanceType _Instance;
        protected static InstanceType Instance
        {
            get
            {
                if (_Instance == null)
                {
                    // Stupid Unity (or .Net, or Mono I don't know) doesn't trigger the static constructor (e.g. ScriptExecutionOrder) before this property getter call.
                    // So, we trigger it manually. Just stupid!!
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(InstanceType).TypeHandle);

                    if (string.IsNullOrEmpty(AssetNameToLoad)) Debug.LogError(Strings.ClassMustProvideValueForField(typeof(InstanceType).Name, nameof(AssetNameToLoad)));
                    else _Instance = Resources.Load(AssetNameToLoad) as InstanceType;
                }

                if (_Instance == null)
                {
                    CreateInstance();
                    if (Application.isEditor) InstanceCreated?.Invoke(_Instance);
                }

                return _Instance;
            }
        }

        protected static void CreateInstance() => _Instance = CreateInstance<InstanceType>();
    }
}