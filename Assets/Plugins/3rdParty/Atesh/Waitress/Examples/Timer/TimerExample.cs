// ReSharper disable UnusedMember.Local

using System.Collections;
using UnityEngine;

namespace Atesh.Waitress.Examples
{
    sealed class TimerExample : MonoBehaviour
    {
        const int Duration = 3;

        bool WaitressIsActive;
        Timer Timer;

        void OnGUI()
        {
            if (WaitressIsActive)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Timer is active. Please see debug console for events.");
                if (Timer != null && GUILayout.Button("Cancel timer")) CancelButtonClick();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Start a timer:");
                if (GUILayout.Button("For " + Duration + " seconds")) TimerButtonClick();
                if (GUILayout.Button("On a coroutine")) StartCoroutine(CoroutineButtonClick());
                if (GUILayout.Button("Using unscaled time")) UnscaledTimeButtonClick();
                if (GUILayout.Button("For " + Duration + " seconds and repeating constantly")) RepeatingTimerButtonClick();
                GUILayout.EndVertical();
            }
        }

        void TimerButtonClick()
        {
            WaitressIsActive = true;

            print("------");
            print("Starting a timer for " + Duration + " seconds...");
            print("Time: " + Time.time);

            Timer = new Timer();
            Timer.Wait(Duration, Done: (Sender, E) =>
            {
                print(E.Canceled ? "Timer canceled!" : "Timer is done. You can start a new one.");
                print("Time: " + Time.time);

                Timer = null;
                WaitressIsActive = false;
            });
        }

        void RepeatingTimerButtonClick()
        {
            WaitressIsActive = true;

            print("------");
            print("Starting a timer for " + Duration + " seconds and repeating constantly...");
            print("Time: " + Time.time);

            Timer = new Timer();
            Timer.Wait(Duration, 0, Done: (Sender, E) =>
            {
                if (E.Canceled)
                {
                    print("Timer canceled!");
                    print("Time: " + Time.time);

                    Timer = null;
                    WaitressIsActive = false;
                }
                else
                {
                    print("Repeat time: " + Time.time);
                    print("Repeat no: " + E.RepeatNo);
                }

                print("Will repeat: " + E.WillRepeat);
            });
        }

        IEnumerator CoroutineButtonClick()
        {
            WaitressIsActive = true;

            print("------");
            print("Coroutine started");
            print("Starting a timer for " + Duration + " seconds...");

            print("Coroutine paused");
            print("Time: " + Time.time);
            yield return new Timer().Wait(Duration);
            print("Coroutine resumed");
            print("Time: " + Time.time);

            print("Timer is done. You can start a new one.");

            WaitressIsActive = false;
        }

        void UnscaledTimeButtonClick()
        {
            WaitressIsActive = true;

            print("------");

            Time.timeScale = 0.5f;
            print("Game time is slowed to 1/2 speed.");

            var BeginTime = Time.time;
            var UnscaledBeginTime = Time.unscaledTime;

            print("Starting a timer for " + Duration + " seconds IN UNSCALED TIME...");
            print("Game time: " + BeginTime);
            print("Real time: " + UnscaledBeginTime);

            Timer = new Timer();
            Timer.Wait(Duration, UnscaledTime: true, Done: (Sender, E) =>
            {
                var EndTime = Time.time;
                var UnscaledEndTime = Time.unscaledTime;

                print(E.Canceled ? "Timer canceled!" : "Timer is done. You can start a new one.");
                print("Game time: " + EndTime + " (PASSED: " + (EndTime - BeginTime) + " seconds)");
                print("Real time: " + UnscaledEndTime + " (PASSED: " + (UnscaledEndTime - UnscaledBeginTime) + " seconds)");

                Time.timeScale = 1;
                print("Game time is reset to normal.");

                Timer = null;
                WaitressIsActive = false;
            });
        }

        void CancelButtonClick()
        {
            Timer.Cancel();
        }
    }
}