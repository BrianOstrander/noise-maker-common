// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System;

namespace Atesh.Waitress
{
    public class ResetAndRepeatEventArgs : EventArgs
    {
        public bool Finish;
        public int RepeatNo;
        public float ElapsedSeconds;
    }
}