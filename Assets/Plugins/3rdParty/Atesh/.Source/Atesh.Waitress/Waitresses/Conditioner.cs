// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;
using System.Collections;

namespace Atesh.Waitress
{
    public class Conditioner : Waitress
    {
        Func<bool> Until;
        EventHandler<ResetAndRepeatEventArgs> ResetAndRepeat;
        int RepeatCount;
        float CheckInterval;
        bool UnscaledInterval;

        readonly Timer CheckIntervalTimer = new Timer();

        protected override IEnumerator PerformWaiting()
        {
            while (!Until())
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (CheckInterval == 0) yield return null;
                else yield return CheckIntervalTimer.Wait(Host, CheckInterval, UnscaledTime: UnscaledInterval);
            }
        }

        protected override bool ShouldRepeat(int RepeatNo, float ElapsedSeconds)
        {
            bool FinishRepeating;

            if (ResetAndRepeat == null) FinishRepeating = true;
            else
            {
                var Args = new ResetAndRepeatEventArgs
                {
                    RepeatNo = RepeatNo,
                    ElapsedSeconds = ElapsedSeconds
                };

                ResetAndRepeat(this, Args);
                FinishRepeating = Args.Finish;
            }

            return !FinishRepeating && (RepeatCount == 0 || RepeatNo < RepeatCount);
        }

        Coroutine InternalWait(MonoBehaviour Host, Func<bool> Until, EventHandler<ResetAndRepeatEventArgs> ResetAndRepeat, int RepeatCount, float CheckInterval, bool UnscaledInterval, EventHandler<DoneEventArgs> Done)
        {
            this.Until = Until;
            this.ResetAndRepeat = ResetAndRepeat;
            this.RepeatCount = RepeatCount;
            this.CheckInterval = CheckInterval;
            this.UnscaledInterval = UnscaledInterval;

            return StartRunner(Host, Done);
        }

        // RepeatCount = 0 for infinite repeating
        [ValidateParameters]
        public Coroutine Wait([NotNull] MonoBehaviour Host, [NotNull] Func<bool> Until, EventHandler<ResetAndRepeatEventArgs> ResetAndRepeat = null, [NotLessThan(0)] int RepeatCount = 0, [NotLessThan(0f)] float CheckInterval = 0, bool UnscaledInterval = false, EventHandler<DoneEventArgs> Done = null) => InternalWait(Host, Until, ResetAndRepeat, RepeatCount, CheckInterval, UnscaledInterval, Done);

        // RepeatCount = 0 for infinite repeating
        [ValidateParameters]
        public Coroutine Wait([NotNull] Func<bool> Until, EventHandler<ResetAndRepeatEventArgs> ResetAndRepeat = null, [NotLessThan(0)] int RepeatCount = 0, [NotLessThan(0f)] float CheckInterval = 0, bool UnscaledInterval = false, EventHandler<DoneEventArgs> Done = null) => InternalWait(null, Until, ResetAndRepeat, RepeatCount, CheckInterval, UnscaledInterval, Done);
    }
}