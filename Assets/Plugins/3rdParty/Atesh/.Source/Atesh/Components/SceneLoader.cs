using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atesh
{
    public class SceneLoader : MonoBehaviour
    {
        #region Inspector
        public string Scene;
        public RectTransform ProgressBar;
        public float MaximumProgressBarWidth;
        #endregion

        static IEnumerator Run(string SceneName, Action Done, Action<float> Progress, bool Additive)
        {
            var Loading = SceneManager.LoadSceneAsync(SceneName, Additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            Loading.allowSceneActivation = false;

            while (true)
            {
                Progress?.Invoke(Loading.progress);

                if (Loading.progress < 0.9f) yield return null;

                Progress?.Invoke(1);

                // Wait for a frame to be able to display the result of 100% progress.
                yield return null;

                Loading.allowSceneActivation = true;

                while (!Loading.isDone)
                {
                    yield return null;
                }

                Done?.Invoke();
                break;
            }
        }

        public static void Load(string SceneName, Action Done = null, Action<float> Progress = null, bool Additive = false) => DefaultHost.Instance.StartCoroutine(Run(SceneName, Done, Progress, Additive));

        #region Messages
        // ReSharper disable UnusedMember.Local
        void Start() => Load(Scene, Progress: ProgressBar ? Progress : (Action<float>)null);
        // ReSharper restore UnusedMember.Local
        #endregion

        void Progress(float Value) => ProgressBar.sizeDelta = new Vector2(MaximumProgressBarWidth * Value, ProgressBar.sizeDelta.y);
    }
}