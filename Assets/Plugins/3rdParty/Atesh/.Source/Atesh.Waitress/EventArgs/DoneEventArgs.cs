// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh.Waitress
{
    public class DoneEventArgs : EventArgs
    {
        public bool Canceled;
        public int RepeatNo = -1;
        public bool WillRepeat;
        public float ElapsedSeconds;
    }
}