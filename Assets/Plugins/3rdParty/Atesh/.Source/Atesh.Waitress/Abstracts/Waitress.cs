// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;
using System.Collections;

namespace Atesh.Waitress
{
    public abstract class Waitress
    {
        protected MonoBehaviour Host { get; private set; }
        IEnumerator Runner;
        EventHandler<DoneEventArgs> Done;
        float BeginTime;

        protected void OnDone(DoneEventArgs Args) => Done?.Invoke(this, Args);

        protected abstract IEnumerator PerformWaiting();
        protected abstract bool ShouldRepeat(int RepeatNo, float ElapsedSeconds);

        public bool Active => Runner != null;

        IEnumerator Run()
        {
            var RepeatNo = 1;
            BeginTime = Time.time;

            try
            {
                while (true)
                {
                    yield return Host.StartCoroutine(PerformWaiting());

                    if (Runner == null) yield break;

                    var ElapsedSeconds = Time.time - BeginTime;
                    var WillRepeat = ShouldRepeat(RepeatNo, ElapsedSeconds);

                    // Reset runner so waitress can be started again in done callback.
                    if (!WillRepeat) Runner = null;
                    OnDone(new DoneEventArgs { RepeatNo = RepeatNo, WillRepeat = WillRepeat, ElapsedSeconds = ElapsedSeconds });

                    if (!WillRepeat) break;

                    RepeatNo++;
                }
            }
            finally
            {
                // Make sure we don't have a runner outside of this cycle.
                Runner = null;
            }
        }

        protected Coroutine StartRunner(MonoBehaviour Host, EventHandler<DoneEventArgs> Done)
        {
            if (Runner != null) throw new InvalidOperationException(Atesh.Strings.AlreadyActive($"{nameof(Waitress)} instance"));

            if (!Host) Host = DefaultHost.Instance;

            this.Host = Host;
            this.Done = Done;

            Runner = Run();
            return Host.StartCoroutine(Runner);
        }

        public void Cancel()
        {
            if (Runner == null) throw new InvalidOperationException(Atesh.Strings.NotActive($"{nameof(Waitress)} instance"));

            if (!Host || !Host.isActiveAndEnabled) return;

            Host.StopCoroutine(Runner);
            Runner = null;

            OnDone(new DoneEventArgs { Canceled = true, ElapsedSeconds = Time.time - BeginTime });
        }
    }
}