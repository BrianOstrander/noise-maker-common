- What is WeNeedCreatedMessage -

WeNeedCreatedMessage is a system that can send "Created" event messages to objects(only the ones you wish) in your scenes even if they are inactive. Unfortunately, Unity doesn't have such feature but it's highly needed for game developers who are using Unity.

WeNeedCreatedMessage tries to fulfill this missing feature of Unity. Unity team should hear our voice and add this feature to the next versions of Unity so it can be a better game engine :)

- How to use -

Installation:

1) Just add the CreatedMessageSender prefab into your scene.
2) Don't add it into additive scenes.
Ex: Scene A loads scene B additively.
Scene A must have the CreatedMessageSender prefab in it.
Scene B shouldn't have it.
3) (iOS only) Set your project’s API compatibility level to full .NET. Don’t use the subset level.

Implementation:

1) Inherit your scripts (which you want to receive the "created" messages) from CreatedMessageReceiver class.
2) Override the Created method as shown below...

public sealed class YourClass : CreatedMessageReceiver
{
    protected override void Created(bool OnSceneLoad)
    {
        // You created message code here...
    }

    void Awake()
    {
        // ...
    }

    void Start()
    {
        // ...
    }
}

Script Execution Order (Optional):

You can set the created message execution order for your scripts just like Unity's built-in script execution ordering. Just select the "CreatedMessageScriptExecutionOrder" object in Assets/Resources folder in your project and enter the script full* names along with the execution order number.
You can move the CreatedMessageScriptExecutionOrder object to any other folder in your project. Just make sure that it stays in a "Resource" folder.
*: Full script name must include the namespace and class name. Ex: YourNameSpace.YourScript

- How does it work -

CreatedMessageSender prefab sends the created message to all the receivers (the scripts inherited from CreatedMessageReceiver) when...
 A) Before all the Awake messages in the beginning of the first frame after the scene was loaded. (OnSceneLoad parameter is true)
 B) After all the "yield WaitForEndOfFrame" coroutines at the very end of every frame if any receiver is instantiated in this frame. (OnSceneLoad parameter is false)

- Known Issues - 

1) Unfortunately, for the new receivers which were instantiated as active in runtime(not including additive scene objects), the created message runs after the awake and start messages. You may want to check the value of OnSceneLoad parameter to know if the created message was sent before awake and start messages.

If you want the created message to be run before the awake message, instantiate them inactively then activate them in next frame. This rule doesn't apply to the scene objects which were instantiated automatically when the scene was loaded. Their created message runs before awake message all the time.

- Troubleshooting -

1) PostSharp Serialization error on iOS runtime: You may encounter this runtime error on iOS platform if your project’s API compatibility level is .NET subset. Please use full .NET level as your compatibility level. Also note that this rule only applies to iOS platform.