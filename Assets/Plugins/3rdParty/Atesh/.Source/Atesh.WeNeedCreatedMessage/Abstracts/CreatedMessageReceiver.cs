// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using UnityEngine;

namespace Atesh.WeNeedCreatedMessage
{
    public abstract class CreatedMessageReceiver : MonoBehaviour
    {
        static bool ErrorShown;

        protected CreatedMessageReceiver()
        {
            CreatedMessageSender.RegisterReceiver(GetInstanceID(), new CreatedMessageSender.Receiver { ScriptFullName = GetType().FullName, Created = CallCreatedIfNotPrefab });
        }

        void CallCreatedIfNotPrefab(bool OnSceneLoad)
        {
            // "this" check in the if block is for making sure that we still have the reference for the created object. It may cause an exception if it's destroyed right after it's created in the same frame.
            if (this && !transform.IsPrefab()) Created(OnSceneLoad);
        }

        /// <summary>
        /// This method must be overriden to get notified for Created message
        /// </summary>
        /// <param name="OnSceneLoad">If true, it indicates that the method is called for the object which is instantiated automatically when the scene was loaded. If false, it indicates that the method is called for the object which is instantiated in runtime.</param>
        protected abstract void Created(bool OnSceneLoad);

        protected void Awake()
        {
            CheckIfSenderPresent();
            CreatedMessageReceiverAwake();
        }

        /// <summary>
        /// This method must be overriden to get notified for Awake message
        /// </summary>
        protected virtual void CreatedMessageReceiverAwake() { }

        void CheckIfSenderPresent()
        {
            if (!ErrorShown)
            {
                if (this is CreatedMessageSender || this is LateCreatedMessageSender) return;

                if (!CreatedMessageSender.Present || !LateCreatedMessageSender.Present)
                {
                    Debug.LogError(Strings.CreatedMessageSenderIsRequired(GetType().Name));
                    ErrorShown = true;
                }
            }
        }
    }
}