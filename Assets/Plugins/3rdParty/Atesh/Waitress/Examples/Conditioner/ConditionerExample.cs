// ReSharper disable UnusedMember.Local

using System.Collections;
using UnityEngine;

namespace Atesh.Waitress.Examples
{
    sealed class ConditionerExample : MonoBehaviour
    {
        bool SampleCondition;

        bool WaitressIsActive;
        Conditioner Conditioner;
        const int RepeatCount = 5;

        void OnGUI()
        {
            if (WaitressIsActive)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Conditioner is active. Please see debug console for events.");
                if (GUILayout.Button("Set the condition")) SetConditionButtonClick();
                if (Conditioner != null && GUILayout.Button("Cancel conditioner")) CancelButtonClick();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Start a conditioner:");
                if (GUILayout.Button("Until the condition is set")) ConditionerButtonClick();
                if (GUILayout.Button("On a coroutine")) StartCoroutine(CoroutineButtonClick());
                if (GUILayout.Button("Until the condition is set and repeating itself " + RepeatCount + " times by reseting the condition")) RepeatingConditionerButtonClick();
                GUILayout.EndVertical();
            }
        }

        void SetConditionButtonClick()
        {
            SampleCondition = true;
            print("The condition is " + SampleCondition);
        }

        void ConditionerButtonClick()
        {
            WaitressIsActive = true;

            print("------");

            SampleCondition = false;
            print("The condition is " + SampleCondition);

            print("Starting a conditioner until the condition is set...");

            Conditioner = new Conditioner();
            Conditioner.Wait(() => SampleCondition, Done: (Sender, E) =>
            {
                print(E.Canceled ? "Conditioner canceled!" : "Conditioner is done. You can start a new one.");

                Conditioner = null;
                WaitressIsActive = false;
            });
        }

        void RepeatingConditionerButtonClick()
        {
            WaitressIsActive = true;

            print("------");
            SampleCondition = false;
            print("The condition is " + SampleCondition);

            print("Starting a conditioner until the condition is set and repeating " + RepeatCount + " times by resetting the condition...");

            Conditioner = new Conditioner();
            Conditioner.Wait(() => SampleCondition, (Sender, E) =>
            {
                if (E.RepeatNo < RepeatCount)
                {
                    print("Resetting the condition for repeat number " + E.RepeatNo);
                    SampleCondition = false;
                }
                else E.Finish = true;

            }, Done: (Sender, E) =>
            {
                if (E.Canceled)
                {
                    print("Conditioner canceled!");

                    Conditioner = null;
                    WaitressIsActive = false;
                }
                else
                {
                    print("Repeat no: " + E.RepeatNo);

                    if (!E.WillRepeat)
                    {
                        print("Conditioner is done. You can start a new one.");

                        Conditioner = null;
                        WaitressIsActive = false;
                    }
                }

                print("Will repeat: " + E.WillRepeat);

            });
        }

        IEnumerator CoroutineButtonClick()
        {
            WaitressIsActive = true;

            print("------");
            print("Coroutine started");

            SampleCondition = false;
            print("The condition is " + SampleCondition);

            print("Starting a conditioner until the condition is set...");

            print("Coroutine paused");
            yield return new Conditioner().Wait(() => SampleCondition);
            print("Coroutine resumed");

            print("Conditioner is done. You can start a new one.");

            WaitressIsActive = false;
        }

        void CancelButtonClick()
        {
            Conditioner.Cancel();
        }
    }
}