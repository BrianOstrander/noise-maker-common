// Author : Onur 'Xtro' Er / Atesh Entertainment
// Email : onurer@gmail.com

using System.ComponentModel;

namespace Atesh.WindowsAndWidgets
{
    public class WindowClosingEventArgs : CancelEventArgs
    {
        public bool Destroy;
        //todo: CloseReason CloseReason { get; }
    }
}