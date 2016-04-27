// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;
using System.Collections;

namespace Atesh.Waitress
{
    public class Framer : Waitress
    {
        bool EndOfCurrentFrame;

        protected override IEnumerator PerformWaiting()
        {
            yield return EndOfCurrentFrame ? new WaitForEndOfFrame() : null;
        }

        protected override bool ShouldRepeat(int RepeatNo, float ElapsedSeconds) => false;

        Coroutine InternalWait(MonoBehaviour Host, bool EndOfCurrentFrame, EventHandler<DoneEventArgs> Done)
        {
            this.EndOfCurrentFrame = EndOfCurrentFrame;

            return StartRunner(Host, Done);
        }

        [ValidateParameters]
        public Coroutine Wait([NotNull] MonoBehaviour Host, bool EndOfCurrentFrame, EventHandler<DoneEventArgs> Done = null) => InternalWait(Host, EndOfCurrentFrame, Done);

        // Instead of these methods, we could define a default value of false for EndOfCurrentFrame parameter in Wait method with the Host parameter.
        // But we can't do it since MonoBehaviour is castable to bool.
        // vvvvv
        [ValidateParameters]
        public Coroutine Wait([NotNull] MonoBehaviour Host) => Wait(Host, false);

        [ValidateParameters]
        public Coroutine Wait([NotNull] MonoBehaviour Host, EventHandler<DoneEventArgs> Done) => Wait(Host, false, Done);
        // ^^^^^

        public Coroutine Wait(bool EndOfCurrentFrame = false, EventHandler<DoneEventArgs> Done = null) => InternalWait(null, EndOfCurrentFrame, Done);
    }
}