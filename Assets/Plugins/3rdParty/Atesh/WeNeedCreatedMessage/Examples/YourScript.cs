// ReSharper disable UnusedMember.Local

namespace Atesh.WeNeedCreatedMessage.Examples
{
    sealed class YourScript : CreatedMessageReceiver
    {
        protected override void Created(bool OnSceneLoad)
        {
            print(name + " created. OnSceneLoad = " + OnSceneLoad);
        }

        protected override void CreatedMessageReceiverAwake()
        {
            print(name + " awake");
        }

        void Start()
        {
            print(name + " start");
        }
    }
}