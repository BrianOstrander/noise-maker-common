// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;

namespace Atesh.Waitress
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine WaitForNextFrame(this MonoBehaviour This, EventHandler<DoneEventArgs> Done = null) => new Framer().Wait(This, Done);
        public static Coroutine WaitForEndOfCurrentFrame(this MonoBehaviour This, EventHandler<DoneEventArgs> Done = null) => new Framer().Wait(This, true, Done);
        public static Coroutine WaitForFixedFrame(this MonoBehaviour This, EventHandler<DoneEventArgs> Done = null) => new FixedFramer().Wait(This, Done);

        // RepeatCount = 0 for infinite repeating
        [ValidateParameters]
        public static Coroutine WaitForTime(this MonoBehaviour This, [GreaterThan(0f)] float Seconds, [NotLessThan(0)] int RepeatCount = 1, bool UnscaledTime = false, EventHandler<DoneEventArgs> Done = null) => new Timer().Wait(This, Seconds, RepeatCount, UnscaledTime, Done);

        // RepeatCount = 0 for infinite repeating
        [ValidateParameters]
        public static Coroutine WaitForCondition(this MonoBehaviour This, [NotNull] Func<bool> Until, EventHandler<ResetAndRepeatEventArgs> ResetAndRepeat = null, [NotLessThan(0)] int RepeatCount = 0, [NotLessThan(0f)] float CheckInterval = 0, bool UnscaledInterval = false, EventHandler<DoneEventArgs> Done = null) => new Conditioner().Wait(This, Until, ResetAndRepeat, RepeatCount, CheckInterval, UnscaledInterval, Done);
    }
}