// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;
using System.Collections;

namespace Atesh.Waitress
{
    public class Timer : Waitress
    {
        float Seconds;
        int RepeatCount;
        bool UnscaledTime;
        float EndTime;

        protected override IEnumerator PerformWaiting()
        {
            if (UnscaledTime)
            {
                while (Time.unscaledTime < EndTime)
                {
                    yield return null;
                }
            }
            else yield return new WaitForSeconds(Seconds);
        }

        protected override bool ShouldRepeat(int RepeatNo, float ElapsedSeconds) => RepeatCount == 0 || RepeatNo < RepeatCount;

        Coroutine InternalWait(MonoBehaviour Host, float Seconds, int RepeatCount, bool UnscaledTime, EventHandler<DoneEventArgs> Done)
        {
            this.Seconds = Seconds;
            this.RepeatCount = RepeatCount;
            this.UnscaledTime = UnscaledTime;

            if (UnscaledTime) EndTime = Time.unscaledTime + Seconds;

            return StartRunner(Host, Done);
        }

        // RepeatCount = 0 for infinite repeating
        [ValidateParameters]
        public Coroutine Wait([NotNull] MonoBehaviour Host, [GreaterThan(0f)] float Seconds, [NotLessThan(0)] int RepeatCount = 1, bool UnscaledTime = false, EventHandler<DoneEventArgs> Done = null) => InternalWait(Host, Seconds, RepeatCount, UnscaledTime, Done);

        // RepeatCount = 0 for infinite repeating
        [ValidateParameters]
        public Coroutine Wait([GreaterThan(0f)] float Seconds, [NotLessThan(0)] int RepeatCount = 1, bool UnscaledTime = false, EventHandler<DoneEventArgs> Done = null) => InternalWait(null, Seconds, RepeatCount, UnscaledTime, Done);
    }
}