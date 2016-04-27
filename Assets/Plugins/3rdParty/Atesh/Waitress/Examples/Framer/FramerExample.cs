// ReSharper disable UnusedMember.Local

using UnityEngine;

namespace Atesh.Waitress.Examples
{
    sealed class FramerExample : MonoBehaviour
    {
        bool WaitressIsActive;

        void OnGUI()
        {
            if (WaitressIsActive)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Framer is active. Please see debug console for events.");
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Start a :");
                if (GUILayout.Button("Framer waiting for the next frame")) FramerButtonClick(false);
                if (GUILayout.Button("Framer waiting for the end of current frame")) FramerButtonClick(true);
                if (GUILayout.Button("FixedFramer waiting for the next fixed frame")) FixedFramerButtonClick();
                GUILayout.EndVertical();
            }
        }

        void FramerButtonClick(bool EndOfCurrentFrame)
        {
            WaitressIsActive = true;

            print("------");
            print("Starting a framer...");
            print("Frame no: " + Time.frameCount);

            // Notice that we can wait for next frame using Waitress classes even in a regular method (non-coroutine method).
            // Unity's waiter classes (such as WaitForSecond and WaitForEndOfFrame) CAN ONLY be used in coroutines.
            new Framer().Wait(EndOfCurrentFrame, (Sender, E) =>
            {
                print("Frame no: " + Time.frameCount);
                print("Framer is done. You can start a new one.");

                WaitressIsActive = false;
            });
        }

        void FixedFramerButtonClick()
        {
            WaitressIsActive = true;

            print("------");
            print("Starting a fixed framer...");
            print("Fixed time: " + Time.fixedTime);

            // Notice that we can wait for next frame using Waitress classes even in a regular method (non-coroutine method).
            // Unity's waiter classes (such as WaitForSecond and WaitForEndOfFrame) CAN ONLY be used in coroutines.
            new FixedFramer().Wait((Sender, E) =>
            {
                print("Fixed time: " + Time.fixedTime);
                print("Fixed framer is done. You can start a new one.");

                WaitressIsActive = false;
            });
        }
    }
}