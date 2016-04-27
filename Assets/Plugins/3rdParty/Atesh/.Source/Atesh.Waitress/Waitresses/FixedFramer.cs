// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;
using UnityEngine;
using System.Collections;

namespace Atesh.Waitress
{
    public class FixedFramer : Waitress
    {
        protected override IEnumerator PerformWaiting()
        {
            yield return new WaitForFixedUpdate();
        }

        protected override bool ShouldRepeat(int RepeatNo, float ElapsedSeconds) => false;

        [ValidateParameters]
        public Coroutine Wait([NotNull] MonoBehaviour Host, EventHandler<DoneEventArgs> Done = null) => StartRunner(Host, Done);

        public Coroutine Wait(EventHandler<DoneEventArgs> Done = null) => StartRunner(null, Done);
    }
}